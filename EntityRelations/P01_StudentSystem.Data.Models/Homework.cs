using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace P01_StudentSystem.Data.Models
{
    public class Homework
    {
        [Key]

        public int HomeworkId { get; set; }

        [Column(TypeName = "varchar(max)")]

        public string MyProperty { get; set; }

        public DateTime SubmissionTime { get; set; }

        [ForeignKey(nameof(Student))]

        public int StudentId { get; set; }

        public virtual Student Student { get; set; }

        [ForeignKey(nameof(Course))]

        public int CourseId { get; set; }

        public virtual Course Course{ get; set; }
    }
}
