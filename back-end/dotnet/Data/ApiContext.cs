
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