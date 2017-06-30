using System;
using System.Collections.Generic;
using System.Linq;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Business.Models.Schedule;
using ClassSchedule.Domain.Context;

namespace ClassSchedule.Business.Services
{
    public class LessonService : ILessonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGroupService _groupService;

        public LessonService(ApplicationDbContext context, IGroupService groupService)
        {
            _context = context;
            _groupService = groupService;
        }

        public List<GroupLessonsViewModel> GetScheduleForGroups(string userId)
        {
            var groups = _groupService.GetEditableGroups(userId);
            var groupLessons = groups
                .Select(x => new GroupLessonsViewModel
                {
                    GroupId = x.GroupId,
                    GroupName = x.GroupName,
                    NumberOfStudents = x.NumberOfStudents,
                    Lessons = x.Schedule
                        .SelectMany(g => g.Lessons)
                        .GroupBy(g => new { g.Schedule.DayNumber, g.Schedule.ClassNumber, g.DisciplineId, g.Discipline.DisciplineName.Name, g.LessonTypeId })
                        .Select(s => new LessonViewModel
                        {
                            DayNumber = s.Key.DayNumber,
                            ClassNumber = s.Key.ClassNumber,
                            DisciplineId = s.Key.DisciplineId,
                            DisciplineName = s.Key.Name,
                            LessonTypeId = s.Key.LessonTypeId ?? 0,
                            LessonDetails = s.SelectMany(d => d.LessonDetails)
                                .Select(y => new LessonDetailViewModel
                                {
                                    LessonId = y.LessonId,
                                    AuditoriumId = y.AuditoriumId,
                                    AuditoriumName = y.Auditorium.AuditoriumNumber + y.Auditorium.Housing.Abbreviation + ".",
                                    PlannedChairJobId = y.PlannedChairJobId ?? 0,
                                    TeacherLastName = y.PlannedChairJob != null 
                                            && y.PlannedChairJob.Job != null
                                            && y.PlannedChairJob.Job.Employee != null 
                                            && y.PlannedChairJob.Job.Employee.Person != null
                                        ? y.PlannedChairJob.Job.Employee.Person.LastName : "",
                                    TeacherFirstName = y.PlannedChairJob != null 
                                            && y.PlannedChairJob.Job != null
                                            && y.PlannedChairJob.Job.Employee != null 
                                            && y.PlannedChairJob.Job.Employee.Person != null 
                                        ? y.PlannedChairJob.Job.Employee.Person.FirstName : "",
                                    TeacherMiddleName = y.PlannedChairJob != null 
                                            && y.PlannedChairJob.Job != null
                                            && y.PlannedChairJob.Job.Employee != null 
                                            && y.PlannedChairJob.Job.Employee.Person != null 
                                        ? y.PlannedChairJob.Job.Employee.Person.MiddleName : "",
                                })
                        })
                })
                .ToList();

            return groupLessons;
        }
    }
}
