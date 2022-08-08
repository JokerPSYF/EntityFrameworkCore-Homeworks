using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Footballers.DataProcessor.ImportDto
{
    [JsonArray]
    public class ImportFootballerIdDTO
    {
        [JsonProperty]
        public int Id { get; set; }
    }
}
