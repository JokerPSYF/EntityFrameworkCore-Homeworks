using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        [Key]

        public int CourseId { get; set; }

        [Required]
        [Column(TypeName = "nvarchar(80)")]

        public string Name { get; set; }

        [Column(TypeName = "nvarchar(max)")]

        public string Description { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }

        public decimal Price { get; set; }

    }
}
