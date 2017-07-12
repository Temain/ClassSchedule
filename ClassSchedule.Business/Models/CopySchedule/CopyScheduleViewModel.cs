using System.Collections.Generic;

namespace ClassSchedule.Business.Models.CopySchedule
{
    public class CopyScheduleViewModel
    {
        public int EditedWeek { get; set; }
        public string EditedWeekStartDate { get; set; }
        public string EditedWeekEndDate { get; set; }

        public int CurrentWeek { get; set; }
        public string CurrentWeekStartDate { get; set; }
        public string CurrentWeekEndDate { get; set; }
        public string Today { get; set; }

        public int[] SelectedWeeks { get; set; }
        public List<WeekViewModel> Weeks { get; set; }

        public int[] SelectedDays { get; set; }
        public List<DayViewModel> Days { get; set; }

        public int[] SelectedGroups { get; set; }
        public List<GroupViewModel> Groups { get; set; }
    }
}