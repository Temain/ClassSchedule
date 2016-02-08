using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models
{
    /// <summary>
    /// Учебный план
    /// </summary>
    [Table("AcademicPlan", Schema = "plan")]
    public class AcademicPlan
    {
        public int AcademicPlanId { get; set; }

        public string AcademicPlanName { get; set; }

        /// <summary>
        /// Кафедра
        /// </summary>
        public int ChairId { get; set; }
        public virtual Chair Chair { get; set; }

        /// <summary>
        /// Количество семестров
        /// </summary>
        public string NumberOfSemesters { get; set; }

        /// <summary>
        /// Программа обучения
        /// </summary>
        public int ProgramOfEducationId { get; set; }
        public virtual ProgramOfEducation ProgramOfEducation { get; set; }

        public List<CourseSchedule> CourseSchedules { get; set; }
    }
}
