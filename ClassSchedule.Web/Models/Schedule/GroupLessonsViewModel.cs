using System.Collections.Generic;

namespace ClassSchedule.Web.Models.Schedule
{
    public class GroupLessonsViewModel
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; }

        public IEnumerable<ShowLessonViewModel> Lessons { get; set; } 
    }
}