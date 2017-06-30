using System.Collections.Generic;

namespace ClassSchedule.Business.Models.GroupInfo
{
    public class ChartSeriesViewModel
    {
        public string name { get; set; }

        public string color { get; set; }

        public int? pointWidth { get; set; }

        public int? pointRange { get; set; }

        public List<ChartIntervalViewModel> data { get; set; } 
    }
}