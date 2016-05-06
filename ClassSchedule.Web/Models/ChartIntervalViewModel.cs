using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models
{
    public class ChartIntervalViewModel
    {
        public int X { get; set; }
        public DateTime Low { get; set; }
        public DateTime High { get; set; }
    }
}