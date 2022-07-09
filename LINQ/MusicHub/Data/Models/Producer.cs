using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MusicHub.Data.Models
{
    public class Producer
    {
        public Producer()
        {
            this.Albums = new HashSet<Album>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MaxLength(Common.GlobalConstants.ProducerNameLength)]
        public string Name { get; set; }

        [MaxLength(Common.GlobalConstants.ProducerNameLength)]
        public string Pseudonym { get; set; }

        [MaxLength(Common.GlobalConstants.SongNameLength)]
        public string PhoneNumber { get; set; }

        public virtual ICollection<Album> Albums { get; set; }
    }
}
