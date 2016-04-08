using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan
{
    public class DisciplineSemesterPlan
    {
        /// <summary>
        /// Номер семестра
        /// </summary>
        [XmlAttribute("Ном")]
        public int SemesterNumber { get; set; }

        /// <summary>
        /// Лекции, час.
        /// </summary>
        [XmlAttribute("Лек")]
        public int HoursOfLectures { get; set; }

        /// <summary>
        /// Практики, час.
        /// </summary>
        [XmlAttribute("Пр")]
        public int HoursOfPractice { get; set; }

        /// <summary>
        /// Лабораторные, час.
        /// </summary>
        [XmlAttribute("Лаб")]
        public int HoursOfLaboratory { get; set; }

        /// <summary>
        /// Лекций в неделю
        /// </summary>
        [XmlAttribute("ПроектЛекВНед")]
        public float LecturesPerWeek { get; set; }

        /// <summary>
        /// Практик в неделю
        /// </summary>
        [XmlAttribute("ПроектПрВНед")]
        public float PracticePerWeek { get; set; }

        /// <summary>
        /// Лабораторных в неделю
        /// </summary>
        [XmlAttribute("ПроектЛабВНед")]
        public float LaboratoryPerWeek { get; set; }

        /// <summary>
        /// Экзамен
        /// </summary>
        [XmlAttribute("Экз")]
        public int Exam { get; set; }

        /// <summary>
        /// Зачёт
        /// </summary>
        [XmlAttribute("Зач")]
        public int Check { get; set; }
    }
}
