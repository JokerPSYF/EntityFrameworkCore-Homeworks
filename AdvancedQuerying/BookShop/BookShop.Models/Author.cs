using System;
using System.ComponentModel.DataAnnotations;

namespace BookShop.Models
{
    public class Author
    {
        [Key]
        public int AuthorId { get; set; }

        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }
    }
}
