namespace ClassSchedule.Business.Models.Report
{
    public class FacultyPercentageViewModel
    {
        public int FacultyId { get; set; }
        public string FacultyName { get; set; }
        public int Uploaded { get; set; }
        public int MustBeUploaded { get; set; }
        public int? SemesterNumber { get; set; }
        public decimal FilledPercent { get; set; }
        public decimal? Efficiency { get; set; }
    }
}