using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Cars
{
    [JsonObject]
    public class GetToyotaCarsDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Make")]
        public string Make { get; set; }

        [JsonProperty("Model")]
        public string Model { get; set; }

        [JsonProperty("TravelledDistance")]
        public long TravelledDistance { get; set; }
    }
}
