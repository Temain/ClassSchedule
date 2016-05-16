using System.Collections.Generic;
using ClassSchedule.Web.Models.Teacher;

namespace ClassSchedule.Web.Models.Auditorium
{
    public class AuditoriumDiscipline
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public bool IsLection { get; set; }

        public IEnumerable<AuditoriumDisciplineTeacher> Teachers { get; set; } 
    }
}