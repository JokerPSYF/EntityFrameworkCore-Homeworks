using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportPrisonerDTO
    {


        [MaxLength(20)]
        [Required]
        [MinLength(3)]
        [JsonProperty("FullName")]
        public string FullName { get; set; }

        [Required]
        [RegularExpression(@"The [A-Z][a-z]+")]
        [JsonProperty("Nickname")]
        public string Nickname { get; set; }

        [Range(18,65)]
        [Required]
        [JsonProperty("Age")]
        public int Age { get; set; }

        [Required]
        [JsonProperty("IncarcerationDate")]

        public string IncarcerationDate { get; set; }

        [JsonProperty("ReleaseDate")]
        public string ReleaseDate { get; set; }

        [JsonProperty("Bail")]
        public decimal? Bail { get; set; }

        [JsonProperty("CellId")]
        public int? CellId { get; set; }

        [JsonProperty("Mails")]
        public ImportMailsDTO[] Mails { get; set; }
    }
}
