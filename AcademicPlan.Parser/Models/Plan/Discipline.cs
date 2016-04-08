using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan
{
    public class Discipline
    {
        /// <summary>
        /// Наименование дисциплины
        /// </summary>
        [XmlAttribute("Дис")]
        public string DisciplineName { get; set; }

        /// <summary>
        /// Код кафедры в УП ВПО
        /// </summary>
        [XmlAttribute("Кафедра")]
        public int ChairCode { get; set; }

        /// <summary>
        /// Детализация плана по дисциплине по семестрам
        /// </summary>
        [XmlElement("Сем")]
        public List<DisciplineSemesterPlan> DisciplineSemesterPlans { get; set; }
    }
}
