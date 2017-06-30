using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Teacher
{
    public class TeacherDiscipline
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public bool IsLection { get; set; }

        public IEnumerable<TeacherDisciplineAuditorium> Auditoriums { get; set; } 
    }
}