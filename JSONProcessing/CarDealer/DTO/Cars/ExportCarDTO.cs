using System;
using System.Collections.Generic;
using System.Text;
using CarDealer.DTO.Parts;
using Newtonsoft.Json;

namespace CarDealer.DTO.Cars
{
    [JsonObject("car")]
    public class ExportCarDTO
    {
        [JsonProperty("Make")]
        public string Make { get; set; }

        [JsonProperty("Model")]
        public string Model { get; set; }

        [JsonProperty("TravelledDistance")]
        public long TravelledDistance { get; set; }

        [JsonProperty("parts")]
        public GetPartsDTO[] Parts { get; set; }
    }
}
