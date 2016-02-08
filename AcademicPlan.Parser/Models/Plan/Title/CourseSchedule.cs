using System.Collections.Generic;
using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan.Title
{
    /// <summary>
    /// Детализация графика учебного процесса по курсам
    /// </summary>
    public class CourseSchedule
    {
        /// <summary>
        /// Номер курса
        /// </summary>
        [XmlAttribute("Ном")]
        public int CourseNumber { get; set; }

        /// <summary>
        /// Максимальная нагрузка 1
        /// </summary>
        [XmlAttribute("МаксНагр1")]
        public float FirstMaxLoad { get; set; }

        /// <summary>
        /// Максимальная нагрузка 2
        /// </summary>
        [XmlAttribute("МаксНагр2")]
        public float SecondMaxLoad { get; set; }

        /// <summary>
        /// Теоретическое обучение, недель
        /// </summary>
        [XmlAttribute("СтрНедТО")]
        public int TheoreticalTrainingWeeks { get; set; }

        /// <summary>
        /// Каникулы, недель
        /// </summary>
        [XmlAttribute("КаникулНед")]
        public string WeeksOfHolidays { get; set; }

        /// <summary>
        /// Расписание
        /// Т - Теоретическое обучение
        /// Э - Экзаменационные сессии
        /// К - Каникулы
        /// Н - Научно-исследовательская работа
        /// П - Производственная практика
        /// Д - Выпускная квалификационная работа
        /// Г - Гос. Экзамены и/или защита ВКР
        /// = - Неделя отсутствует
        /// </summary>
        [XmlAttribute("График")]
        public string Schedule { get; set; }

        /// <summary>
        /// Детализация графика учебного процесса
        /// для конкретного курса по семестрам
        /// </summary>
        [XmlElement("Семестр")]
        public List<SemesterSchedule> SemesterSchedules { get; set; } 
    }
}
