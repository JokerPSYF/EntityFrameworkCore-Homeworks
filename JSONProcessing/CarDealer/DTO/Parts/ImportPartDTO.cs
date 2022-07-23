using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

using Newtonsoft.Json;

namespace CarDealer.DTO.Parts
{
    [JsonObject]
    public class ImportPartDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("quantity")]
        public int Quantity { get; set; }

        [JsonProperty("supplierId")]
        [Required]
        public int SupplierId { get; set; }
    }
}