using CarDealer.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Cars
{
    using Newtonsoft.Json;

    [JsonObject]
    public class ImportCarDTO
    {
        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("travelledDistance")]
        public long TravelledDistance { get; set; }

        [JsonProperty("partsId")]
        public int[] PartId { get; set; }
    }
}
