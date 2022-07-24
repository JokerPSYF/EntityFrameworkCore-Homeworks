using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("User")]
    public class ExportSoldProductsDTO
    {
        [XmlElement("firstName")]
        public string FirstName{ get; set; }

        [XmlElement("lastName")]
        public string LastName { get; set; }

        [XmlArray("soldProducts")]
        public ExportUserPartDTO[] SoldProducts { get; set; }
    }
}
