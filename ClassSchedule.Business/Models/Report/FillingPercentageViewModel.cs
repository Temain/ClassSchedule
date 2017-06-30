using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Report
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