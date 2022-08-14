using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    [XmlType("User")]
    public class ExportUserDTO
    {
        [XmlAttribute("username")]
        public string Username { get; set; }

        [XmlArray("Purchases")]
        public ExortPurchaseDTO[] Purchases { get; set; }

        [XmlElement("TotalSpent")]
        public decimal TotalSpent { get; set; }
    }
}
