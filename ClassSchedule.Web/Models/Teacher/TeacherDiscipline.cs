using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Teacher
{
    public class TeacherDiscipline
    {
        public int DisciplineId { get; set; }
        public string DisciplineName { get; set; }

        public bool IsLection { get; set; }

        public IEnumerable<TeacherDisciplineAuditorium> Auditoriums { get; set; } 
    }
}