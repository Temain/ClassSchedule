using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ClassSchedule.Web.Models
{
    public class ChartSeriesViewModel
    {
        public string Name { get; set; }

        public int? PointWidth { get; set; }

        public List<ChartIntervalViewModel> Data { get; set; } 
    }
}