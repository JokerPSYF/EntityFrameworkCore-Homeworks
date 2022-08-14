using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace VaporStore.Data.Models
{
    public class Game
    {
        public Game()
        {
            this.Purchases = new List<Purchase>();
            this.GameTags = new List<GameTag>();
        }

        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        [Required]
        public DateTime ReleaseDate  { get; set; }

        [ForeignKey(nameof(Developer))]
        public int DeveloperId { get; set; }
        public virtual Developer Developer { get; set; }

        [ForeignKey(nameof(Genre))]
        public int GenreId { get; set; }
        public virtual Genre Genre { get; set; }

        public virtual ICollection<Purchase> Purchases { get; set; }

        public virtual ICollection<GameTag> GameTags { get; set; }
    }

}

//•	GameTags - collection of type GameTag. Each game must have at least one tag.
