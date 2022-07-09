﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Writer
    {
        public Writer()
        {
            this.Songs = new HashSet<Song>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Common.GlobalConstants.WriterNameLength)]
        public string Name { get; set; }

        [MaxLength(Common.GlobalConstants.WriterNameLength)]
        public string Pseudonym { get; set; }

        public virtual ICollection<Song> Songs { get; set; }
    }
}
