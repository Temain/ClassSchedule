using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.GroupInfo
{
    public class DisciplineViewModel
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public int ChairId { get; set; }
        public string ChairName { get; set; }

        public List<DisciplineSemesterPlanViewModel> DisciplineSemesterPlans { get; set; } 
    }
}