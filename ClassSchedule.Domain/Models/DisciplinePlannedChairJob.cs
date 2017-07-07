using System;
namespace ClassSchedule.Domain.Models
{
    public class DisciplinePlannedChairJob
    {
        public int DisciplinePlannedChairJobId { get; set; }

        public int DisciplineId { get; set; }
        public Discipline Discipline { get; set; }

        public int PlannedChairJobId { get; set; }
        public PlannedChairJob PlannedChairJob { get; set; }
    }
}
