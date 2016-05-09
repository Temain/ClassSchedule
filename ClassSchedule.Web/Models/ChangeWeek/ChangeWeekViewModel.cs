using System.Collections.Generic;

namespace ClassSchedule.Web.Models.ChangeWeek
{
    public class ChangeWeekViewModel
    {
        public int EditedWeek { get; set; }
        public string EditedWeekStartDate { get; set; }
        public string EditedWeekEndDate { get; set; }

        public int CurrentWeek { get; set; }
        public string CurrentWeekStartDate { get; set; }
        public string CurrentWeekEndDate { get; set; }
        public string Today { get; set; }

        public List<WeekViewModel> Weeks { get; set; }
    }
}