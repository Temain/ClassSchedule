using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Teacher
{
    public class TeacherDisciplineAuditorium
    {
        public int AuditoriumId { get; set; }
        public string AuditoriumNumber { get; set; }
        public IEnumerable<string> Groups { get; set; }
    }
}