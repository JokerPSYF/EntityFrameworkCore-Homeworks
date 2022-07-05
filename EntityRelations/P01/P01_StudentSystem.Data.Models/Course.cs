using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace P01_StudentSystem.Data.Models
{
    public class Course
    {
        public Course()
        {
            this.StudentsEnrolled = new HashSet<StudentCourse>();

            this.Resources = new HashSet<Resource>();

            this.HomeworkSubmissions = new HashSet<Homework>();
        }

        [Key]
        public int CourseId { get; set; } //

        [Required]
        [Column(TypeName = "nvarchar(80)")]
        public string Name { get; set; } //

        [Column(TypeName = "nvarchar(max)")]
        public string Description { get; set; } //

        [Column(TypeName = "datetime2")]
        public DateTime StartDate { get; set; } //

        [Column(TypeName = "datetime2")]
        public DateTime EndDate { get; set; } //

        public decimal Price { get; set; } //

        public virtual ICollection<StudentCourse> StudentsEnrolled { get; set; }

        public virtual ICollection<Resource> Resources { get; set; }

        public virtual ICollection<Homework> HomeworkSubmissions { get; set; }
    }
}
