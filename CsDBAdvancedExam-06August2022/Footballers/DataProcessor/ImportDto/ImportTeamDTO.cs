using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace Footballers.DataProcessor.ImportDto
{
    public class ImportTeamDTO
    {
        [Required]
        [MinLength(3)]
        [MaxLength(40)]
        [RegularExpression(@"[a-zA-Z0-9 \.\-]+")]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [MinLength(2)]
        [MaxLength(40)]
        [JsonProperty("Nationality")]
        public string Nationality { get; set; }

        [Required]
        [JsonProperty("Trophies")]
        public string Trophies { get; set; }

        //[JsonProperty("Footballers")]
        //public ImportFootballerIdDTO[] Footballers { get; set; }

        [JsonProperty("Footballers")]
        public int[] Footballers { get; set; }
    }
}
