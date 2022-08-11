using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.Data.Models
{
    public class Project
    {
        public Project()
        {
            this.Tasks = new List<Task>();
        }

        [Key]
        public int Id { get; set; }

        [MinLength(2)]
        [MaxLength(40)]
        [Required]
        public string Name { get; set; }

        [Required]
        public DateTime OpenDate { get; set; }

        public DateTime? DueDate { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }

//•	Tasks - collection of type Task

}
