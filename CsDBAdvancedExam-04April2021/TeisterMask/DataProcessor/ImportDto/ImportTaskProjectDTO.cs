using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Xml.Serialization;
using TeisterMask.Data.Models.Enums;

namespace TeisterMask.DataProcessor.ImportDto
{
    [XmlType("Task")]
    public class ImportTaskProjectDTO
    {
        [MinLength(2)]
        [MaxLength(40)]
        [Required]
        [XmlElement("Name")]
        public string Name { get; set; }

        [XmlElement("OpenDate")]
        public string OpenDate { get; set; }

        [XmlElement("DueDate")]
        public string DueDate { get; set; }

        [Required]
        [XmlElement("ExecutionType")]
        [EnumDataType(typeof(ExecutionType))]
        public int ExecutionType { get; set; }

        [Required]
        [EnumDataType(typeof(LabelType))]
        [XmlElement("LabelType")]
        public int LabelType { get; set; }
    }
}
