using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;

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