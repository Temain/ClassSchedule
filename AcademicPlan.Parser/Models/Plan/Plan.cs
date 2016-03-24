using System.Collections.Generic;
using System.Xml.Serialization;
using AcademicPlan.Parser.Models.Plan.Title;

namespace AcademicPlan.Parser.Models.Plan
{
    public class Plan
    {
        [XmlAttribute("ФормаОбучения")]
        public string EducationForm { get; set; }

        [XmlAttribute("УровеньОбразования")]
        public string EducationLevel { get; set; }

        [XmlElement("Титул")]
        public PlanTitle PlanTitle { get; set; }

        /// <summary>
        /// План по каждой дисциплине
        /// </summary>
        [XmlArray("СтрокиПлана"), XmlArrayItem("Строка")]
        public List<Discipline> Disciplines { get; set; }
    }
}
