using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Theatre.DataProcessor.ImportDto
{
    public class ImportTheatreDTO
    {
        [MinLength(4)]
        [MaxLength(30)]
        [Required]
        [JsonProperty("Name")]
        public string Name { get; set; }

        [Required]
        [Range(1, 10)]
        [JsonProperty("NumberOfHalls")]
        public sbyte NumberOfHalls { get; set; }

        [Required]
        [MinLength(4)]
        [MaxLength(30)]
        [JsonProperty("Director")]
        public string Director { get; set; }

        [JsonProperty("Tickets")]
        public ImportTicketDTO[] Tickets { get; set; }
    }
}
