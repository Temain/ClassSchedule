namespace ClassSchedule.Business.Models.ChangeWeek
{
    public class WeekViewModel
    {
        public int WeekNumber { get; set; }
        public string WeekStartDate { get; set; }
        public string WeekEndDate { get; set; }

        public string ScheduleTypeName { get; set; }
        public string ScheduleTypeColor { get; set; }
    }
}