﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.Data.Models
{
    public class Department
    {
        public Department()
        {
            this.Cells = new HashSet<Cell>();
        }

        [Key]
        public int Id { get; set; }

        [MaxLength(25)]
        [Required]
        public string Name { get; set; }

        public virtual ICollection<Cell> Cells { get; set; }
    }
}
