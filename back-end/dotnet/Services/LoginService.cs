using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TodosAPI.Data;
using TodosAPI.Helpers;
using TodosAPI.Models;

namespace TodosAPI.Services
{
    public interface ILoginService
    {
        (User user, string token) Authenticate(Login login);
    }

    public sealed class LoginService : ILoginService
    {
        private readonly ApiContext _dbContext;
        private readonly SecuritySettings _securitySettings;

        public LoginService(ApiContext dbContext, IOptions<SecuritySettings> securitySettings)
        {
            this._dbContext = dbContext;
            this._securitySettings = securitySettings.Value;
        }

        public static string HashPassword(string password)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha1.ComputeHash(bytes);
            return System.Convert.ToBase64String(hash);
        }

        public (User user, string token) Authenticate(Login login)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Username == login.Username && u.Password == HashPassword(login.Password));
            
            // return null if user not found
            if (user == null)
                return (null, null);

            // discard password 
            user.Password = string.Empty;

            // authentication successful so generate jwt token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_securitySettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] 
                {
                    new Claim(ClaimTypes.Name, user.Id.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            
            return (user, tokenHandler.WriteToken(token));
        }
    }
}