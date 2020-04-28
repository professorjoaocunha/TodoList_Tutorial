using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodosAPI.Data;
using TodosAPI.Models;
using TodosAPI.Services;

namespace TodosAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ApiContext _context;

        private readonly ILoginService _loginService;

        public LoginController(ApiContext context, ILoginService loginService)
        {
            _context = context;
            _loginService = loginService;
        }

        // GET: api/Todo
        [HttpPost]
        public ActionResult<dynamic> Authenticate([FromBody]Login login)
        {
            var result = _loginService.Authenticate(login);            
            
            if (string.IsNullOrEmpty(login.Username) || string.IsNullOrEmpty(login.Username)) {
                return BadRequest();
            }

            // check if user was found
            if (result.user == null)
                return NotFound();

            // check if token was defined (login success)
            if (result.token == null)
                return Forbid();            
                        
            return new
            {
                user = result.user,
                token = result.token
            };
        }
    }
}
