using System.Xml.Serialization;

namespace ProductShop.DTos.Export
{

    [XmlType("Product")]
    public class ExportSoldProductShortInfoDto
    {
        [XmlElement("name")]
        public string Name { get; set; }

        [XmlElement("price")]
        public decimal Price { get; set; }
    }
}
