using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// План по определенной дисциплине на определенную неделю 
    /// определенного семестра определенного курса
    /// </summary>
    [Table("DisciplineWeekPlan", Schema = "plan")]
    public class DisciplineWeekPlan
    {
        public int DisciplineWeekPlanId { get; set; }

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
        /// План по определенной дисциплине на семестр определнного курса
        /// </summary>
        public int DisciplineSemesterPlanId { get; set; }
        public virtual DisciplineSemesterPlan DisciplineSemesterPlan { get; set; }
    }
}
