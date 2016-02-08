using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Детализация графика учебного процесса по курсам
    /// </summary>
    [Table("CourseSchedule", Schema = "acpl")]
    public class CourseSchedule
    {
        public int CourseScheduleId { get; set; }

        /// <summary>
        /// Номер курса
        /// </summary>
        public int CourseNumber { get; set; }

        /// <summary>
        /// Максимальная нагрузка 1
        /// </summary>
        public float FirstMaxLoad { get; set; }

        /// <summary>
        /// Максимальная нагрузка 2
        /// </summary>
        public float SecondMaxLoad { get; set; }

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
        /// Учебный план
        /// </summary>
        public int AcademicPlanId { get; set; }
        public virtual AcademicPlan AcademicPlan { get; set; }


        /// <summary>
        /// Детализация графика учебного процесса
        /// для конкретного курса по семестрам
        /// </summary>
        public List<SemesterSchedule> SemesterSchedules { get; set; } 
    }
}
