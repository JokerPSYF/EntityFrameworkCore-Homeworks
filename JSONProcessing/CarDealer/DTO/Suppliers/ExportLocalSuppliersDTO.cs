﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace CarDealer.DTO.Suppliers
{
    [JsonObject]
    public class ExportLocalSuppliersDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }

        [JsonProperty("Name")]
        public string Name { get; set; }

        [JsonProperty("PartsCount")]
        public int PartsCount { get; set; }
    }
}
