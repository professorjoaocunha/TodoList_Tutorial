
using Microsoft.EntityFrameworkCore;
using TodosAPI.Models;
using TodosAPI.Services;

namespace TodosAPI.Data
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options)
          : base(options)
        {}
        
        public DbSet<Todo> Todos { get; set; }

        public DbSet<TodoList> TodoLists { get; set; }

        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
          modelBuilder.Entity<User>().HasData(new User { Id = 1, Username = "admin", Password = LoginService.HashPassword("123"), Role = User.RoleEnum.admin.ToString() });
        }
    }
}