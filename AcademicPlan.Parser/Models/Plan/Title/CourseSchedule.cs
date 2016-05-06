using System.Collections.Generic;
using System.Linq;
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
        /// Расписание
        /// Т - Теоретическое обучение
        /// Э - Экзаменационные сессии
        /// К - Каникулы
        /// Н - Научно-исследовательская работа
        /// У - Учебная практика
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

        /// <summary>
        /// Теоретическое обучение, недель
        /// </summary>
        //[XmlAttribute("СтрНедТО")]
        public int TheoreticalTrainingWeeks
        {
            get
            {
                return Schedule.Count(x => x == (char)ScheduleAbbreviations.TheoreticalTraining);
            }
        }

        /// <summary>
        /// Экзаменационные сессии, недель
        /// </summary>
        public int ExamSessionWeeks
        {
            get
            {
                return Schedule.Count(x => x == (char)ScheduleAbbreviations.ExamSession);
            }
        }

        /// <summary>
        /// Учебные практики, недель
        /// </summary>
        public int StudyTrainingWeeks
        {
            get
            {
                return Schedule.Count(x => x == (char)ScheduleAbbreviations.StudyTraining);
            }
        }

        /// <summary>
        /// Производственные практики, недель
        /// </summary>
        public int PracticalTrainingWeeks
        {
            get
            {
                return Schedule.Count(x => x == (char)ScheduleAbbreviations.PracticalTraining);
            }
        }

        /// <summary>
        /// Выпускная квалификационная работа, недель
        /// </summary>
        public int FinalQualifyingWorkWeeks
        {
            get
            {
                return Schedule.Count(x => x == (char)ScheduleAbbreviations.FinalQualifyingWork);
            }
        }

        /// <summary>
        /// Гос. экзамены и/или защита ВКР, недель
        /// </summary>
        public int StateExamsWeeks
        {
            get
            {
                return Schedule.Count(x => x == (char)ScheduleAbbreviations.StateExams);
            }
        }

        /// <summary>
        /// Каникулы, недель
        /// </summary>
        public int WeeksOfHolidays
        {
            get
            {
                return Schedule.Count(x => x == (char)ScheduleAbbreviations.Holidays);
            }
        }

        /// <summary>
        /// Научно-исследовательская работа, недель
        /// </summary>
        public int ResearchWorkWeeks
        {
            get
            {
                return Schedule.Count(x => x == (char)ScheduleAbbreviations.ResearchWork);
            }
        }
    }

    public enum ScheduleAbbreviations
    {
        /// <summary>
        /// Теоретическое обучение
        /// </summary>
        TheoreticalTraining = 'Т',

        /// <summary>
        /// Экзаменационная сессия
        /// </summary>
        ExamSession = 'Э',

        /// <summary>
        /// Учебная практика
        /// </summary>
        StudyTraining = 'У',

        /// <summary>
        /// Произодственная практика
        /// </summary>
        PracticalTraining = 'П',

        /// <summary>
        /// Выпускная квалификационная работа
        /// </summary>
        FinalQualifyingWork = 'Д',

        /// <summary>
        /// Гос. экзамены и/или защита ВКР
        /// </summary>
        StateExams = 'Г',

        /// <summary>
        /// Каникулы
        /// </summary>
        Holidays = 'К',

        /// <summary>
        /// Научно-исследовательская работа
        /// </summary>
        ResearchWork = 'Н'
    }
}
