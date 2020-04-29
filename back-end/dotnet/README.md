## Criando Web API no dotnet core
1. Instalar [dotnet core](https://dotnet.microsoft.com/download)

2. Criar TodosAPI
```bash
dotnet new TodosAPI -o TodosAPI
```

3. Entrar na pasta
```
cd TodosAPI
```

4. Adicionar pacote Microsoft.EntityFrameworkCore.InMemory
```bash
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

5. Criar classe Models.Todo
```csharp
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodosAPI.Models
{
    public class Todo
    {
        public enum PriorityEnum
        {
            LOW,
            MEDIUM,
            HIGH    
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Title { get; set; }

        public DateTime CreationDate { get; set; }

        public bool Done { get; set; }

        public PriorityEnum Priority { get; set; }
    }
}
```

6. Criar classe Models.TodoList
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace TodosAPI.Models
{
    public class TodoList
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public string Title { get; set; }

        public ICollection<Todo> Todos { get; set; }
    }
}
```

7. Criar Data.ApiContext:
```csharp

using Microsoft.EntityFrameworkCore;
using TodosAPI.Models;

namespace TodosAPI.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
          : base(options)
        {}
        
        public DbSet<Todo> Todos { get; set; }

        public DbSet<TodoList> TodoLists { get; set; }
    }
}
```

8. Incluir novo serviço em Startup.ConfigureServices, antes de services.AddControllers()
```csharp
services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase(databaseName: "todo.db"));
```

9. Alterar também o Startup.cs para incluir CORS:
```csharp
app.UseCors(c => c.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
```

10. Adicionar code generator
```bash
dotnet tool install --global dotnet-aspnet-codegenerator
dotnet add package Microsoft.VisualStudio.Web.CodeGeneration.Design
dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Design
```

11. Adicionar Todo Controller:
```bash
dotnet-aspnet-codegenerator -p TodosAPI.csproj controller -name TodoController -api -m TodosAPI.Models.Todo -dc TodosAPI.Data.ApiContext -outDir Controllers -namespace TodosAPI.Controllers
```

12. Adicionar TodoList Controller?
```bash
dotnet-aspnet-codegenerator -p TodosAPI.csproj controller -name TodoListController -api -m TodosAPI.Models.TodoList -dc TodosAPI.Data.ApiContext -outDir Controllers -namespace TodosAPI.Controllers
```

13. Iniciar a aplicação
```bash
dotnet run
```

14. Usar Postman para testar
```js
HTTP POST https://localhost:5001/api/Todo 
{
	"id": 0,
	"Title": "Teste",
	"CreationDate": "2020-04-06T17:16:40",
	"Done": false,
	"Priority": 0
}

HTTP GET https://localhost:5001/api/Todo 

HTTP PUT https://localhost:5001/api/Todo 
{
	"id": 1,
	"Title": "Teste Novo",
	"CreationDate": "2020-04-06T17:16:40",
	"Done": false,
	"Priority": 1
}

HTTP DELETE https://localhost:5001/api/Todo/1
```

15. Adicionar pacote Microsoft.EntityFrameworkCore.Sqlite
```bash
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
```

16. Incluir em Startup.ConfigureServices() a configuração do SqLite
```bash
services.AddDbContext<ApiContext>(opt => opt.UseSqlite("Data Source=todo.db"));
```

17. Adicionar Migrations:
```bash
dotnet ef migrations add InitialCreate
```

18. Aplicar mudanças no banco de dados
```bash
dotnet ef database update
```

19. Executar e testar de novo
```bash
dotnet run
```

## Incluindo Autenticação/Autorização

1. Adicionar pacotes de autenticação e autorização
```bash
dotnet add package Microsoft.AspNetCore.Authentication
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
```

2. Criar modelo para Usuário:
```csharp
using System.ComponentModel.DataAnnotations.Schema;

namespace TodosAPI.Models
{
    public class User
    {
        public enum RoleEnum { admin, user };

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }        
        public string Role { get; set; }
    }
}
```

3. Criar arquivo Helpers/AuthenticationHelper.cs para computar o Hash da senha:
```csharp
using System.Text;

namespace TodosAPI.Helpers 
{
    public static class AuthenticationHelper
    {
        public static string ComputeHash(string input)
        {
            var sha1 = System.Security.Cryptography.SHA1.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha1.ComputeHash(bytes);
            return System.Convert.ToBase64String(hash);
        }
    }
}
```

4. Alterar arquivo "Data\ApiContext.cs" para adicionar DbSet para usuários para armazenar o usuário no DbContext. Incluir também o método OnModelCreating(ModelBuilder), para inserir primeiro usuário admin:
```csharp
public DbSet<User> Users { get; set; }
        
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    // initial user. password must be changed later.
    modelBuilder.Entity<User>()
      .HasData(new User { Id = 1, Username = "admin", Password = AuthenticationHelper.ComputeHash("123"), Role = User.RoleEnum.admin.ToString() });

    // set uUsername as Unique
    modelBuilder.Entity<User>()
      .HasIndex(u => u.Username)
      .IsUnique();
}
```

5. Adicionar migration para incluir tabela usuário:
```bash
dotnet ef migrations add Authentication
dotnet ef database update
```

5. Incluir UserController.cs usando gerador de código
```csharp
dotnet-aspnet-codegenerator -p TodosAPI.csproj controller -name UserController -api -m TodosAPI.Models.User -dc TodosAPI.Data.ApiContext -outDir Controllers -namespace TodosAPI.Controllers
```

6. Criar em UserController.cs o método MapUser(User):
```csharp
private static dynamic MapUser(User user) 
{
    return new
    {
        Id = user.Id,
        Username = user.Username   
    };
}
```

7. Para os métodos que retornam o usuário, chamar este método para não divulgar a senha e papel do usuário:
```csharp
// GET: api/User
[HttpGet]
public async Task<ActionResult<IEnumerable<dynamic>>> GetUsers()
{
    return await _context.Users.Select(u => MapUser(u)).ToListAsync();
}

// GET: api/User/5
[HttpGet("{id}")]
public async Task<ActionResult<dynamic>> GetUser(int id)
{
    var user = await _context.Users.FindAsync(id);

    if (user == null)
    {
        return NotFound();
    }

    return MapUser(user);
}

[HttpPost]
public async Task<ActionResult<User>> PostUser(User user)
{
    user.Password = AuthenticationHelper.ComputeHash(user.Password);

    _context.Users.Add(user);
    await _context.SaveChangesAsync();

    return CreatedAtAction("GetUser", new { id = user.Id }, MapUser(user));
}

// DELETE: api/User/5
[HttpDelete("{id}")]
public async Task<IActionResult> DeleteUser(int id)
{
    var user = await _context.Users.FindAsync(id);
    if (user == null)
    {
        return NotFound();
    }

    _context.Users.Remove(user);
    await _context.SaveChangesAsync();

    return NoContent();
}
```

8. Iniciar a aplicação
```bash
dotnet run
```

9. Usar Postman para testar
```js
HTTP POST https://localhost:5001/api/User 
{
	"id": 0,
	"Username": "Teste",
	"Password": "123",
	"role": "admin"
}

HTTP GET https://localhost:5001/api/User 

HTTP PUT https://localhost:5001/api/User 
{
	"id": 2,
	"Username": "TesteNovo",
	"Password": "456",
	"role": "admin"
}

HTTP DELETE https://localhost:5001/api/User/2
```

11. Parar a aplicação (CTRL+C). Agora é hora de incluir a autenticação e autorização.

12. Adicionar configuração para chave secreta do JWT. Este Token será usado para gerar o Token JWT e deve ser conhecido somente pelo servidor. Incluir a seguinte configuração no appsettings.json (e appsettings.Development.json):
```json
{
  "Security": {
    "Secret": "fedaf7d8863b48e197b9287d492b708e"
  },
  ...
}
```

13. Criar arquivo "Helpers\SecuritySettings.cs" que representa a configuração criada acima:
```csharp
namespace TodosAPI.Helpers
{
    public class SecuritySettings
    {
        public string Secret { get; set; }
    }
}
```

14. Criar Models\Login.cs que irá ter os dados de usuário e senha:
```csharp
namespace TodosAPI.Models
{
    public class Login
    {
        public string Username { get; set; }
        public string Password { get; set; }        
    }
}
```

15. Criar serviço de usuário "Services\LoginService.cs" que irá ser responsável pela autenticação e login:
```csharp
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

        public (User user, string token) Authenticate(Login login)
        {
            var user = _dbContext.Users.SingleOrDefault(u => u.Username == login.Username && u.Password == AuthenticationHelper.ComputeHash(login.Password));
            
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
```
15. Criar Controllers/LoginController.cs para receber dados de usuário e senha:
```csharp
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
```

16. Alterar o método ConfigureServices(IServiceCollection) do Startup.cs para adicionar Middleware de Autenticação. Incluir também as cláusulas using requeridas pelo código:
```csharp
public void ConfigureServices(IServiceCollection services)
{
    // services.AddDbContext<ApiContext>(opt => opt.UseInMemoryDatabase(databaseName: "TodoDB"));
    services.AddDbContext<ApiContext>(opt => opt.UseSqlite("Data Source=todo.db"));

    services.AddCors();
    services.AddControllers();

    // Add functionality to inject IOptions<T>
    services.AddOptions();

    // Add security settings to be injected
    services.Configure<SecuritySettings>(Configuration.GetSection("Security"));            

    // Add scoed
    services.AddScoped<ILoginService, LoginService>();

    services.AddAuthentication(x =>
    {
        x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(x =>
    {
        var securitySettings = new SecuritySettings();
        Configuration.GetSection("Security").Bind(securitySettings);
        var key = Encoding.ASCII.GetBytes(securitySettings.Secret);            
    
        x.RequireHttpsMetadata = false;
        x.SaveToken = true;
        x.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });
}
```

17. Alterar o método Configure(IApplicationBuilder, IWebHostEnvironment), para incluir os Middlewares de autenticação e autorização:
```csharp
app.UseAuthentication();            
app.UseAuthorization();
```

18. Alterar Controllers para incluir autorizaçao:
- UserController:
```csharp
[Authorize(Roles = "admin")]
public class UserController : ControllerBase {...}
```

- TodoController:
```csharp
[Authorize]
public class TodoController : ControllerBase { ... }
```

- TodoListController:
```csharp
[Authorize]
public class TodoListController : ControllerBase { ... }
```

19. Iniciar a aplicação
```bash
dotnet run
```

20. Usar Postman para testar
```js
HTTP POST https://localhost:5001/api/Login 
{
	"Username": "admin",
	"Password": "123"
}

HTTP GET https://localhost:5001/api/User
Authorization: Bearer TOKEN

HTTP POST https://localhost:5001/api/User 
{
	"id": 0,
	"Username": "Teste",
	"Password": "456",
	"role": "user"
}
Authorization: Bearer TOKEN

HTTP POST https://localhost:5001/api/Login 
{
	"Username": "Teste",
	"Password": "456"
}

HTTP GET https://localhost:5001/api/User
Authorization: Bearer TOKEN


HTTP GET https://localhost:5001/api/Todo
Authorization: Bearer TOKEN
```
