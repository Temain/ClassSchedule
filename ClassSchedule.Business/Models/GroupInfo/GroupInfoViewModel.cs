using System.Collections.Generic;

namespace ClassSchedule.Business.Models.GroupInfo
{
    public class GroupInfoViewModel
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }

        public int ProgramOfEducationId { get; set; }

        public int? NumberOfStudents { get; set; }

        public string Profile { get; set; }

        public string EducationForm { get; set; }

        public string EducationLevel { get; set; }

        public int NumberOfSemesters { get; set; }

        /// <summary>
        /// График учебного процесса на каждый семестр учебного года
        /// </summary>
        public List<SemesterScheduleViewModel> SemesterSchedules { get; set; } 

        /// <summary>
        /// План по каждой дисциплине на каждый семестр
        /// </summary>
        public List<DisciplineViewModel> Disciplines { get; set; }
    }
}