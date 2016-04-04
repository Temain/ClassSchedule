using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Schedule
{
    public class TeacherViewModel
    {
        public int TeacherId { get; set; }
        public string TeacherFullName { get; set; }
        public string Employment { get; set; }
    }
}