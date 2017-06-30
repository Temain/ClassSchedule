using System.Collections.Generic;
using ClassSchedule.Business.Models.Schedule;

namespace ClassSchedule.Business.Interfaces
{
    public interface ILessonService
    {
        List<GroupLessonsViewModel> GetScheduleForGroups(string userId);
    }
}
