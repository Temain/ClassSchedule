using System.Collections.Generic;

namespace ClassSchedule.Web.Models.Schedule
{
    public class GroupLessonsViewModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public List<LessonViewModel> Lessons { get; set; } 
    }
}