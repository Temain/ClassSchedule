using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Детализация графика учебного процесса 
    /// для конкретного курса на определенный семестр
    /// </summary>
    [Table("SemesterSchedule", Schema = "plan")]
    public class SemesterSchedule
    {
        public int SemesterScheduleId { get; set; }

        /// <summary>
        /// Номер семестра
        /// </summary>
        public int SemesterNumber { get; set; }

        /// <summary>
        /// Номер первой недели
        /// </summary>
        public int NumberOfFirstWeek { get; set; }

        /// <summary>
        /// Теоретическое обучение, недель
        /// </summary>
        public int TheoreticalTrainingWeeks { get; set; }

        /// <summary>
        /// Экзаменационные сессии, недель
        /// </summary>
        public int ExamSessionWeeks { get; set; }

        /// <summary>
        /// Учебные практики, недель
        /// </summary>
        public int StudyTrainingWeeks { get; set; }

        /// <summary>
        /// Производственные практики, недель
        /// </summary>
        public int PracticalTrainingWeeks { get; set; }

        /// <summary>
        /// Выпускная квалификационная работа, недель
        /// </summary>
        public int FinalQualifyingWorkWeeks { get; set; }

        /// <summary>
        /// Гос. экзамены и/или защита ВКР, недель
        /// </summary>
        public int StateExamsWeeks { get; set; }

        /// <summary>
        /// Каникулы, недель
        /// </summary>
        public int WeeksOfHolidays { get; set; }

        /// <summary>
        /// Научно-исследовательская работа, недель
        /// </summary>
        public int ResearchWorkWeeks { get; set; }

        /// <summary>
        /// График учебного процесса
        /// </summary>
        public string Schedule { get; set; }

        public int NumberOfLastWeek
        {
            get { return NumberOfFirstWeek + Schedule.Length - 1; }
        }


        public int CourseScheduleId { get; set; }
        public virtual CourseSchedule CourseSchedule { get; set; }

        /// <summary>
        /// План на семестр по каждой дисциплине
        /// </summary>
        public virtual ICollection<DisciplineSemesterPlan> DisciplineSemesterPlans { get; set; } 
    }
}
