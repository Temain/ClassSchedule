using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Business.Models.Schedule;
using ClassSchedule.Domain.Context;

namespace ClassSchedule.Business.Services
{
    public class LessonService : ILessonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGroupService _groupService;
        private readonly IJobService _jobService;

        public LessonService(ApplicationDbContext context, IGroupService groupService, IJobService jobService)
        {
            _context = context;
            _groupService = groupService;
            _jobService = jobService;
        }

        public List<ScheduleViewModel> GetScheduleForGroups(int[] groupsIds, int educationYearId, int weekNumber
            , int? dayNumber = null, int? classNumber = null, bool checkDowntimes = false)
        {
            var schedule = _context.Schedule
                .Include(x => x.EducationYear)
                .Include(x => x.Group)
                .Include(x => x.Lessons.Select(ls => ls.LessonType))
                .Include(x => x.Lessons.Select(ls => ls.Discipline.Chair))
                .Include(x => x.Lessons.Select(ls => ls.Discipline.DisciplineName))
                .Include(x => x.Lessons.Select(ls => ls.LessonDetails.Select(ld => ld.Auditorium.Housing)))
                .Include(x => x.Lessons.Select(ls => ls.LessonDetails.Select(ld => ld.PlannedChairJob.Job.Employee.Person)))
                .Where(x => groupsIds.Contains(x.GroupId) && x.EducationYearId == educationYearId
                    && x.WeekNumber == weekNumber && x.DeletedAt == null);

            if (dayNumber != null)
            {
                schedule = schedule.Where(x => x.DayNumber == dayNumber);
            }

            if (classNumber != null)
            {
                schedule = schedule.Where(x => x.ClassNumber == classNumber);
            }

            var scheduleViewModel = schedule
                .Select(x => new ScheduleViewModel
                {
                    ScheduleId = x.ScheduleId,
                    EducationYearId = x.EducationYearId,
                    EducationYear = x.EducationYear.EducationYearName,
                    WeekNumber = x.WeekNumber,
                    DayNumber = x.DayNumber,
                    ClassNumber = x.ClassNumber,
                    GroupId = x.GroupId,
                    GroupName = x.Group.GroupName,
                    ClassDate = x.ClassDate,
                    IsNotActive = x.IsNotActive,
                    Lessons = x.Lessons.Where(ls => ls.DeletedAt == null)
                        .Select(ls => new LessonViewModel
                        {
                            LessonId = ls.LessonId,
                            ScheduleId = ls.ScheduleId,
                            LessonTypeId = ls.LessonTypeId ?? 0,
                            DisciplineId = ls.DisciplineId,
                            DisciplineName = ls.Discipline != null && ls.Discipline.DisciplineName != null ? ls.Discipline.DisciplineName.Name : "",
                            ChairId = ls.Discipline != null ? ls.Discipline.ChairId : 0,
                            ChairName = ls.Discipline != null && ls.Discipline.Chair != null ? ls.Discipline.Chair.DivisionName : "",
                            Order = ls.Order,
                            LessonDetails = ls.LessonDetails.Where(ld => ld.DeletedAt == null)
                                .Select(ld => new LessonDetailViewModel
                                {
                                    LessonDetailId = ld.LessonDetailId,
                                    LessonId = ld.LessonId,
                                    AuditoriumId = ld.AuditoriumId,
                                    AuditoriumName = ld.Auditorium != null && ld.Auditorium.Housing != null ? ld.Auditorium.AuditoriumNumber + ld.Auditorium.Housing.Abbreviation + "." : "",
                                    HousingId = ld.Auditorium != null ? ld.Auditorium.HousingId : 0,
                                    PlannedChairJobId = ld.PlannedChairJobId ?? 0,
                                    TeacherLastName = ld.PlannedChairJob != null
                                            && ld.PlannedChairJob.Job != null
                                            && ld.PlannedChairJob.Job.Employee != null
                                            && ld.PlannedChairJob.Job.Employee.Person != null
                                        ? ld.PlannedChairJob.Job.Employee.Person.LastName : "",
                                    TeacherFirstName = ld.PlannedChairJob != null
                                            && ld.PlannedChairJob.Job != null
                                            && ld.PlannedChairJob.Job.Employee != null
                                            && ld.PlannedChairJob.Job.Employee.Person != null
                                        ? ld.PlannedChairJob.Job.Employee.Person.FirstName : "",
                                    TeacherMiddleName = ld.PlannedChairJob != null
                                            && ld.PlannedChairJob.Job != null
                                            && ld.PlannedChairJob.Job.Employee != null
                                            && ld.PlannedChairJob.Job.Employee.Person != null
                                        ? ld.PlannedChairJob.Job.Employee.Person.MiddleName : "",
                                    Order = ld.Order
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            if (checkDowntimes)
            {
                var downtimes = _jobService.TeachersDowntime(weekNumber, maxDiff: 2);
                foreach (var downtime in downtimes)
                {
                    scheduleViewModel.Where(x => x.GroupId == downtime.GroupId && x.DayNumber == downtime.DayNumber && x.ClassNumber == downtime.ClassNumber)
                        .SelectMany(g => g.Lessons.SelectMany(d => d.LessonDetails))
                        .Where(p => p.PlannedChairJobId == downtime.PlannedChairJobId)
                        .All(c => { c.TeacherHasDowntime = true; return true; });
                }
            }

            var groups = _context.Groups
                .Where(x => groupsIds.Contains(x.GroupId) && x.DeletedAt == null)
                .ToList();
            foreach (var group in groups)
            {
                var contains = scheduleViewModel.Any(s => s.GroupId == group.GroupId);
                if (!contains)
                {
                    scheduleViewModel.Add(new ScheduleViewModel
                    {
                        GroupId = group.GroupId,
                        GroupName = group.GroupName,
                        EducationYearId = educationYearId,
                        WeekNumber = weekNumber,
                        DayNumber = dayNumber ?? 0,
                        ClassNumber = classNumber ?? 0
                    });
                }
            }

            return scheduleViewModel;
        }
    }
}
