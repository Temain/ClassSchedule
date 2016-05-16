using System.Collections.Generic;

namespace ClassSchedule.Web.Models.Auditorium
{
    public class AuditoriumDisciplineTeacher
    {
        public int PersonId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherLastName { get;set; }
        public string TeacherFirstName { get; set; }
        public string TeacherMiddleName { get; set; }
        public string TeacherShortName { get; set; }
        public IEnumerable<string> Groups { get; set; }
    }
}