using System;
using System.Collections.Generic;
using System.Text;

namespace ProductShop.DTOs.Category
{
    using Newtonsoft.Json;
    public class Export_CategoriesyProductsCount
    {
        [JsonProperty("Garden")]
        public string Category { get; set; }

        [JsonProperty("productsCount")]
        public int ProductCount { get; set; }

        [JsonProperty("averagePrice")]
        public string AveragePrice { get; set; }

        [JsonProperty("totalRevenue")]
        public string TotalRevenue { get; set; }
    }
}
