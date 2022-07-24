using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class ExportUserAndProducts
    {
        [XmlElement("firstName")]
        public string FirstName { get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlElement("age")]
        public int? Age { get; set; }

        [XmlIgnore]
        public bool AgeSpecified { get { return this.Age != null; } }

        [XmlElement("SoldProducts")]
        public ExportSoldProductCount SoldProducts { get; set; }
    }
}
