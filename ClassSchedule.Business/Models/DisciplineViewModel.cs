using System.Collections.Generic;

namespace ClassSchedule.Business.Models
{
    public class DisciplineViewModel
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public int ChairId { get; set; }
        public string ChairName { get; set; }

        // public List<DisciplineSemesterPlanViewModel> DisciplineSemesterPlans { get; set; } 
    }
}