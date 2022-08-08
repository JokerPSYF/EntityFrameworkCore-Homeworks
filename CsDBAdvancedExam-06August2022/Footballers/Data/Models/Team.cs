using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Footballers.Data.Models
{
    public class Team
    {
        public Team()
        {
            this.TeamsFootballers = new List<TeamFootballer>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression(@"[a-zA-Z0-9 \.\-]+")]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        public string Nationality { get; set; }

        [Required]
        public int Trophies { get; set; }

        public virtual ICollection<TeamFootballer> TeamsFootballers { get; set; }
    }
}

//•	Name – text with length [3, 40].
//May contain letters (lower and upper case), digits, spaces, a dot sign ('.') and a dash ('-'). (required)

//•	TeamsFootballers – collection of type TeamFootballer
