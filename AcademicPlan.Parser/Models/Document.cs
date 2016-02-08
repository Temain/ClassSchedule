using System;
using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models
{
    [Serializable()]
    [XmlRoot("Документ")]
    public class Document
    {
        /// <summary>
        /// Предыдущее наименование файла
        /// </summary>
        [XmlAttribute("PrevName")]
        public string PreviousFileName { get; set; }

        [XmlElement("План")]
        public Plan.Plan Plan { get; set; }
    }
}
