using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using Newtonsoft.Json;

namespace VaporStore.DataProcessor.Dto.Import
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
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public string ReleaseDate { get; set; }

        [Required]
        [JsonProperty("Developer")]
        public string Developer { get; set; }

        [JsonProperty("Genre")]
        [Required]
        public string Genre { get; set; }

        [JsonProperty("Tags")]
        public string[] Tags { get; set; }

    }
}
