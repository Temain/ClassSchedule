namespace ClassSchedule.Web.Models.Schedule
{
    public class AuditoriumViewModel
    {
        public int AuditoriumId { get; set; }
        public string AuditoriumNumber { get; set; }
        public string AuditoriumTypeName { get; set; }
        public int HousingId { get; set; }
        public string HousingAbbreviation { get; set; }
        public int? ChairId { get; set; }
        public int Places { get; set; }
        public string Comment { get; set; }
        public string Employment { get; set; }
    }
}