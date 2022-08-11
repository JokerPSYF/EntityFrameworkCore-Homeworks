using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace TeisterMask.Data.Models
{
    public class Employee
    {
        public Employee()
        {
            this.EmployeesTasks = new List<EmployeeTask>();
        }

        [Key]
        public int Id { get; set; }

        [MinLength(3)]
        [MaxLength(40)]
        [Required]
        [RegularExpression(@"[a-z0-9]|[A-Z0-9]+")]
        public string Username  { get; set; }

        [Required]
        [EmailAddress]
        public string Email  { get; set; }

        [Required]
        [RegularExpression(@"\d{3}-\d{3}-\d{4}")]
        public string Phone { get; set; }

        public virtual ICollection<EmployeeTask> EmployeesTasks { get; set; }

        //•	EmployeesTasks - collection of type EmployeeTask
    }
}
