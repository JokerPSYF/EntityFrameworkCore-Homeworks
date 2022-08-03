using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Newtonsoft.Json;

namespace SoftJail.DataProcessor.ImportDto
{
    public class ImportDepartmentCellDto
    {
        [JsonProperty("Name")]
        [MinLength(3)]
        [MaxLength(25)]
        public string Name { get; set; }

        [JsonProperty("Cells")]
        public ImportCellDTO[] Cells { get; set; }
    }
}
