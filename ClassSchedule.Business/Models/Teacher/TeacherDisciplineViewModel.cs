using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Teacher
{
    public class TeacherDisciplineViewModel
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public bool IsLection { get; set; }

        public IEnumerable<TeacherDisciplineAuditoriumViewModel> Auditoriums { get; set; } 
    }
}