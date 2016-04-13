using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Teacher
{
    public class TeacherDisciplineAuditorium
    {
        public int AuditoriumId { get; set; }
        public string AuditoriumNumber { get; set; }
        public IEnumerable<string> Groups { get; set; }
    }
}