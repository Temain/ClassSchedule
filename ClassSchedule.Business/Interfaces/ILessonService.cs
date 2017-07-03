using System.Collections.Generic;
using ClassSchedule.Business.Models.Schedule;

namespace ClassSchedule.Business.Interfaces
{
    public interface ILessonService
    {
        List<ScheduleViewModel> GetScheduleForGroups(int[] groupsIds, int educationYearId, int weekNumber, int? dayNumber = null, int? classNumber = null, bool checkDowntimes = false);
    }
}
