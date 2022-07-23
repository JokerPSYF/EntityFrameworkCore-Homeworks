using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Sales
{
    [JsonObject]
    public class ImportSaleDTO
    {
        [JsonProperty("carId")]
        public int CarId { get; set; }

        [JsonProperty("customerId")]
        public int CustomerId { get; set; }

        [JsonProperty("discount")]
        public decimal Discount { get; set; }
    }
}
