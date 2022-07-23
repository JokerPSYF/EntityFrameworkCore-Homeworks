using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Parts
{
    [JsonObject]
    public class GetPartsDTO
    {
        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("Price")]
        public string Price { get; set; }
    }
}
