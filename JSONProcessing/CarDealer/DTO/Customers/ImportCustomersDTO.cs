using System;
using System.Collections.Generic;
using System.Text;

using Newtonsoft.Json;

namespace CarDealer.DTO.Customers
{
    [JsonObject]
    public class ImportCustomersDTO
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("birthDate")]
        public DateTime BirthDate { get; set; }

        [JsonProperty("isYoungDriver")]
        public bool IsYoungDriver { get; set; }
    }
}
