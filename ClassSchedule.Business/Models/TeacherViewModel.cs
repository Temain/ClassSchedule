namespace ClassSchedule.Business.Models
{
    public class TeacherViewModel
    {
        public int PersonId { get; set; }
        public int JobId { get; set; }
        public int PlannedChairJobId { get; set; }
        public string TeacherFullName { get; set; }
        public string Employment { get; set; }
    }
}