using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Business.Models.Schedule
{
    public class ShowScheduleViewModel
    {
        public int WeekNumber { get; set; }
        public DateTime FirstDayOfWeek { get; set; }
        public DateTime LastDayOfWeek { get; set; }

        public int NumberOfGroups { get; set; }
        public List<ScheduleViewModel> Schedule { get; set; }
    }
}
