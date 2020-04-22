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