﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Schedule
{
    public class LessonDetailViewModel
    {
        public int LessonDetailId { get; set; }

        public int? LessonId { get; set; }

        public int PlannedChairJobId { get; set; }
        public string TeacherLastName { get; set; }
        public string TeacherFirstName { get; set; }
        public string TeacherMiddleName { get; set; }
        public bool TeacherHasDowntime { get; set; }

        public int HousingId { get; set; }
        public int AuditoriumId { get; set; }
        public string AuditoriumName { get; set; }

        public List<AuditoriumViewModel> Auditoriums { get; set; }
    }
}