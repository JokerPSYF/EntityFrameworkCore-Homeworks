using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace ProductShop.Dtos.Export
{
    [XmlType("SoldProducts")]
    public class ExportSoldProductCount
    {
        [XmlElement("count")]
        public int ProductsCount { get; set; }

        [XmlArray("products")]
        public List<ExportProduct> Products { get; set; }
    }
}
