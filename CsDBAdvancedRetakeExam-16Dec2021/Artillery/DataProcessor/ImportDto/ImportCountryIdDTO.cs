using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Artillery.DataProcessor.ImportDto
{
    public class ImportCountryIdDTO
    {
        [JsonProperty("Id")]
        public int Id { get; set; }
    }
}
