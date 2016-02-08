using System.Collections.Generic;
using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan.Title
{
    /// <summary>
    /// Общая информация о плане
    /// </summary>
    public class PlanTitle
    {
        [XmlAttribute("ПолноеИмяПлана")]
        public string PlanFullName { get; set; }

        [XmlAttribute("ИмяПлана")]
        public string PlanName { get; set; }

        [XmlAttribute("Факультет")]
        public string Faculty { get; set; }

        [XmlAttribute("КодКафедры")]
        public string ChairCode { get; set; }

        /// <summary>
        /// Код направления без резделителей (точек)
        /// </summary>
        [XmlAttribute("ПоследнийШифр")]
        public string DirectionCode { get; set; }

        [XmlAttribute("ГодНачалаПодготовки")]
        public string YearStart { get; set; }

        [XmlAttribute("СеместровНаКурсе")]
        public string NumberOfSemesters { get; set; }

        [XmlElement("Утверждение")]
        public Approval Approval { get; set; }

        /// <summary>
        /// Хранит направление и основную образовательную программу подготовки
        /// </summary>
        [XmlArray("Специальности"), XmlArrayItem("Специальность")]
        public List<Speciality> Specialities { get; set; }

        [XmlArray("Квалификации"), XmlArrayItem("Квалификация")]
        public List<Qualification> Qualifications { get; set; }

        [XmlArray("Разработчики"), XmlArrayItem("Разработчик")]
        public List<Developer> Developers { get; set; }

        /// <summary>
        /// Детализация графика учебного процесса по курсам
        /// </summary>
        [XmlArray("ГрафикУчПроцесса"), XmlArrayItem("Курс")]
        public List<CourseSchedule> CourseSchedules { get; set; }
    }
}
