using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using Trucks.Data.Models.Enums;

namespace Trucks.DataProcessor.ImportDto
{
    [XmlType("Truck")]
    public class ImportTruckDTO
    {
        [MinLength(8)]
        [MaxLength(8)]
        [RegularExpression(@"([A-Z]{2}\d{4}[A-Z]{2})")]
        [XmlElement("RegistrationNumber")]
        public string RegistrationNumber { get; set; }

        [MinLength(17)]
        [MaxLength(17)]
        [Required]
        [XmlElement("VinNumber")]
        public string VinNumber { get; set; }

        [Range(950, 1420)]
        [XmlElement("TankCapacity")]
        public int TankCapacity { get; set; }


        [Range(5000, 29000)]
        [XmlElement("CargoCapacity")]
        public int CargoCapacity { get; set; }

        [Required]
        [EnumDataType(typeof(CategoryType))]
        [XmlElement("CategoryType")]
        public int CategoryType { get; set; }

        [Required]
        [EnumDataType(typeof(MakeType))]
        [XmlElement("MakeType")]
        public int MakeType { get; set; }
    }
}
