using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// План по определенной дисциплине на определенный семестр определенного курса
    /// </summary>
    [Table("DisciplineSemesterPlan", Schema = "plan")]
    public class DisciplineSemesterPlan
    {
        public int DisciplineSemesterPlanId { get; set; }

        /// <summary>
        /// Дисциплина
        /// </summary>
        public int DisciplineId { get; set; }
        public virtual Discipline Discipline { get; set; }

        /// <summary>
        /// Кафедра
        /// </summary>
        public int ChairId { get; set; }
        public virtual Chair Chair { get; set; }

        /// <summary>
        /// Часов лекций
        /// </summary>
        public int? HoursOfLectures { get; set; }

        /// <summary>
        /// Часов лабораторных
        /// </summary>
        public int? HoursOfLaboratory { get; set; }

        /// <summary>
        /// Часов практик
        /// </summary>
        public int? HoursOfPractice { get; set; }

        /// <summary>
        /// График на семестр
        /// </summary>
        public int SemesterScheduleId { get; set; }
        public virtual SemesterSchedule SemesterSchedule { get; set; }

        /// <summary>
        /// Тип контроля (экзамен, зачет и т.д.)
        /// </summary>
        public int SessionControlTypeId { get; set; }
        public virtual SessionControlType SessionControlType { get; set; }

        /// <summary>
        /// Понедельно расписанное количество часов этой дисциплины
        /// </summary>
        public ICollection<DisciplineWeekPlan> DisciplineWeekPlans { get; set; }

        /// <summary>
        /// Возможные преподаватели
        /// </summary>
        public ICollection<Job> Jobs { get; set; } 
    }
}
