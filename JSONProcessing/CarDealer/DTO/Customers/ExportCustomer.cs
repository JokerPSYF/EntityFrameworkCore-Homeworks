using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Customers
{
    [JsonObject]
    public class ExportCustomer
    {
        [JsonProperty("fullName")]
        public string Name { get; set; }

        [JsonProperty("boughtCars")]
        public int BoughtCars { get; set; }

        [JsonProperty("spentMoney")]
        public decimal SpendMoney { get; set; }


    }
}
