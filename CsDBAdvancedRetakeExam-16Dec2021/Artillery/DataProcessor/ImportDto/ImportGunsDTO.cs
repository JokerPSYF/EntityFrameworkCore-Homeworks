using Artillery.Data.Models.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Artillery.DataProcessor.ImportDto
{
    public class ImportGunsDTO
    {
        [Required]
        [JsonProperty("ManufacturerId")]
        public int ManufacturerId { get; set; }

        [Required]
        [Range(100, 1350000)]
        [JsonProperty("GunWeight")]
        public int GunWeight { get; set; }

        [Required]
        [Range(2.00, 35.00)]
        [JsonProperty("BarrelLength")]
        public double BarrelLength { get; set; }

        [JsonProperty("NumberBuild")]
        public int? NumberBuild { get; set; }

        [Required]
        [Range(1, 100000)]
        [JsonProperty("Range")]
        public int Range { get; set; }

        [Required]
        [JsonProperty("GunType")]
        [EnumDataType(typeof(GunType))]
        public string GunType { get; set; }

        [Required]
        [JsonProperty("ShellId")]
        public int ShellId { get; set; }

        [JsonProperty("Countries")]
        public ImportCountryIdDTO[] Countries { get; set; }
    }
}
