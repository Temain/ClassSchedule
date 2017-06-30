using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Auditorium
{
    public class AuditoriumDisciplineViewModel
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public bool IsLection { get; set; }

        public IEnumerable<AuditoriumDisciplineTeacherViewModel> Teachers { get; set; } 
    }
}