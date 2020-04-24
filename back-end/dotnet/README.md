## Criando Web API no dotnet core
1. Instalar [dotnet core](https://dotnet.microsoft.com/download)

2. Criar WebAPI
```bash
dotnet new webapi -o TodosAPI
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
