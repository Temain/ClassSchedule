using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassSchedule.Domain.Models.QueryResults
{
    public class TeacherDowntimeQueryResult
    {
        public int PersonId { get; set; }
        public int PlannedChairJobId { get; set; }
        public int JobId { get; set; }
        public int GroupId { get; set; }
        public int WeekNumber { get; set; }
        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }

        public int ClassDiff { get; set; }
    }
}
