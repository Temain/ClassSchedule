using System.Collections.Generic;

namespace ClassSchedule.Business.Models.Schedule
{
    public class EditLessonViewModel
    {
        public int GroupId { get; set; }
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }

        public List<HousingViewModel> Housings { get; set; } 
        public List<LessonTypeViewModel> LessonTypes { get; set; }  
        public List<LessonViewModel> Lessons { get; set; } 
    }
}