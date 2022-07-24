using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("Users")]
    public class ExportAllUsersAndCountDTO
    {
        [XmlElement("count")]
        public int CountOfUsers { get; set; }

        [XmlArray("users")]
        public List<ExportUserAndProducts> Users { get; set; }
    }
}
