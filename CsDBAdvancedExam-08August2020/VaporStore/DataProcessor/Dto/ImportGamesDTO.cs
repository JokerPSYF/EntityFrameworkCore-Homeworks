using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace VaporStore.DataProcessor.Dto
{
    public class ImportGamesDTO
    {
        [Required]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Range(0, double.MaxValue)]
        [Required]
        [JsonProperty("Price")]
        public decimal Price { get; set; }

        [Required]
        [JsonProperty("ReleaseDate")]
        public DateTime ReleaseDate { get; set; }

        [Required]
        [JsonProperty("Developer")]
        public string Developer { get; set; }

        [JsonProperty("Developer")]
        public string Genre { get; set; }

        [JsonProperty("Tags")]
        public string[] Tags { get; set; }

    }
}
