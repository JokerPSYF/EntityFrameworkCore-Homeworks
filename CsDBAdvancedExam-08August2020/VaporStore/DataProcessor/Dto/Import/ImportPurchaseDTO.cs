using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using VaporStore.Data.Models.Enums;

namespace VaporStore.DataProcessor.Dto.Import
{
    [XmlType("Purchase")]
    public class ImportPurchaseDTO
    {
        [Required]
        [XmlAttribute("title")]
        public string Title { get; set; }

        [Required]
        [EnumDataType(typeof(PurchaseType))]
        [XmlElement("Type")]
        public string Type { get; set; }

        [Required]
        [RegularExpression(@"([A-Z0-9]{4}-[A-Z0-9]{4}-[A-Z0-9]{4})")]
        [XmlElement("Key")]
        public string Key { get; set; }

        [Required]
        [RegularExpression(@"(\d{4} \d{4} \d{4} \d{4})")]
        [XmlElement("Card")]
        public string Card { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy HH:mm}", ApplyFormatInEditMode = true)]
        [XmlElement("Date")]
        public string Date { get; set; }
    }
}
