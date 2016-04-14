using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models.Schedule
{
    public class RefreshCellViewModel
    {
        public int DayNumber { get; set; }
        public int ClassNumber { get; set; }
        public int GroupId { get; set; }
        public string Content { get; set; }
    }
}