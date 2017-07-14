using System.Collections.Generic;
using System.Data.Entity;
using ClassSchedule.Business.Models.CopySchedule;
using ClassSchedule.Business.Models.Schedule;
using ClassSchedule.Domain.Models;

namespace ClassSchedule.Business.Interfaces
{
    public interface ILessonService
    {
        List<ScheduleViewModel> GetScheduleForGroups(int[] groupsIds, int educationYearId, int weekNumber, int? dayNumber = null, int? classNumber = null, bool checkDowntimes = false);
        EntityState SaveLesson(EditLessonViewModel viewModel, ApplicationUser user, ref string changeLog);
        void RemoveLesson(int groupId, int educationYearId, int weekNumber, int dayNumber, int classNumber, ref string changeLog);
        void CopyLesson(int sourceGroupId, int sourceDayNumber, int sourceClassNumber, int targetGroupId, int targetDayNumber, int targetClassNumber, ApplicationUser user, ref string changeLog);
        void CopySchedule(CopyScheduleViewModel viewmodel, ApplicationUser user, ref string changeLog);
    }
}
