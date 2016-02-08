using System.Linq;
using System.Xml.Serialization;

namespace AcademicPlan.Parser.Models.Plan.Title
{
    /// <summary>
    /// Детализация графика учебного процесса 
    /// для конкретного курса на определенный семестр
    /// </summary>
    public class SemesterSchedule
    {
        /// <summary>
        /// Номер семестра
        /// </summary>
        [XmlAttribute("Ном")]
        public int SemesterNumber { get; set; }

        [XmlAttribute("НомерПервойНедели")]
        public int NumberOfFirstWeek { get; set; }

        [XmlAttribute("НомерПервогоЭлемента")]
        public int NumberOfFirstElement { get; set; }

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
        /// Теоретическое обучение, недель
        /// </summary>
        [XmlAttribute("СтрНедТО")]
        public int TheoreticalTrainingWeeks { get; set; }

        /// <summary>
        /// Экзаменационные сессии, недель
        /// </summary>
        public int ExamSessionWeeks 
        {
            get
            {
                return Schedule.Count(x => x == (char) ScheduleAbbreviations.ExamSession);          
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
            Holidays = 'К'
        }
    }
}