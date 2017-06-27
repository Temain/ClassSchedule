namespace ClassSchedule.Web.Models.GroupInfo
{
    public class ChartIntervalViewModel
    {
        public int x { get; set; }
        public long low { get; set; }
        public long high { get; set; }

        public int lowWeek { get; set; }
        public int highWeek { get; set; }
    }
}