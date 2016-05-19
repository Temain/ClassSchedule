using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Report
{
    public class FillingPercentageViewModel
    {
        /// <summary>
        /// Процент заполнения расписания
        /// </summary>
        public decimal TotalFilledPercent { get; set; }

        /// <summary>
        /// Загружено учебных планов
        /// </summary>
        public int AcademicPlanUploaded { get; set; }

        /// <summary>
        /// Должно быть загружено учебных планов
        /// </summary>
        public int AcademicPlanMustBeUploaded { get; set; }

        public virtual ICollection<FacultyPercentageViewModel> Faculties { get; set; } 
    }
}