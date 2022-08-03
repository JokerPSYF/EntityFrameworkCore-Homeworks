using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace SoftJail.DataProcessor.ImportDto
{
    [JsonObject("Cell")]
    public class ImportCellDTO
    {
        [JsonProperty("CellNumber")]
        [Range(1,1000)] 
        public int CeilNumber { get; set; }

        [JsonProperty("HasWindow")]
        public bool HasWindow { get; set; }
    }
}
