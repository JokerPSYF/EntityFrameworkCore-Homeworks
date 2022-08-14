using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace VaporStore.DataProcessor.Dto.Export
{
    public class ExportGameDTO
    {
        [XmlAttribute("title")]
        public string Title { get; set; }

        [XmlElement("Genre")]
        public string Genre { get; set; }

        [XmlElement("Price")]
        public decimal Price { get; set; }
    }
}
