using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Business.Models.Teacher;
using ClassSchedule.Domain.Context;

namespace ClassSchedule.Web.Controllers
{
    public class TeacherController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly IJobService _jobService;

        public TeacherController(ApplicationDbContext context, IJobService jobService)
        {
            _context = context;
            _jobService = jobService;
        }

        // Переписать
        public ActionResult Schedule(int teacherId, int dayNumber)
        {
            var teacher = _context.Jobs
                .Where(x => x.JobId == teacherId)
                .SingleOrDefault();
            if (teacher == null)
            {
                return Content("Error");
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var personId = teacher.Employee.PersonId;
            var dailySchedule = _context.Lessons
                .Where(x => x.LessonDetails.Any(d => d.PlannedChairJob.Job.Employee.PersonId == personId)
                    && x.Schedule.WeekNumber == UserProfile.WeekNumber && x.Schedule.DayNumber == dayNumber
                    && x.DeletedAt == null)
                .OrderBy(n => n.Schedule.ClassNumber)
                .GroupBy(g => new { g.Schedule.ClassNumber })
                .Select(x => new TeacherLessonViewModel
                {
                    DayNumber = dayNumber,
                    ClassNumber = x.Key.ClassNumber,
                    Disciplines = x.GroupBy(g => new { g.DisciplineId, g.Discipline.DisciplineName.Name, g.LessonTypeId })
                        .Select(d => new TeacherDisciplineViewModel
                        {
                            DisciplineId = d.Key.DisciplineId,
                            DisciplineName = d.Key.Name,
                            IsLection = d.Key.LessonTypeId == (int)LessonTypes.Lection,
                            Auditoriums = d.SelectMany(s => s.LessonDetails) /*d.GroupBy(g => new { g. AuditoriumId, AuditoriumNumber = g.Auditorium.AuditoriumNumber + g.Auditorium.Housing.Abbreviation })*/
                                .Select(a =>
                                    new TeacherDisciplineAuditoriumViewModel
                                    {
                                        AuditoriumId = a.AuditoriumId,
                                        AuditoriumNumber = a.Auditorium.AuditoriumNumber + a.Auditorium.Housing.Abbreviation,
                                        Groups = x.Select(gr => gr.Schedule.Group.GroupName)
                                    })
                        })
                }
                )
                .ToList();

            // Окна в расписании преподавателя
            var teacherDowntime = _jobService.TeachersDowntime(UserProfile.WeekNumber, teacherId, maxDiff: 2)
                .Select(x => new
                {
                    x.DayNumber,
                    x.ClassNumber,
                    x.ClassDiff
                })
                .Distinct()
                .OrderBy(n => n.ClassNumber)
                .ToList();

            // Перебираем занятия между которыми окна
            for (int i = 0; i < teacherDowntime.Count() - 1; i++)
            {
                var classNumber = teacherDowntime[i].ClassNumber;
                var classDiff = teacherDowntime[i].ClassDiff;

                // Формируем пустые занятия-окна
                for (int j = classNumber + 1; j <= classNumber + classDiff; j++)
                {
                    var downtimeLesson = new TeacherLessonViewModel
                    {
                        DayNumber = teacherDowntime[i].DayNumber,
                        ClassNumber = j,
                        IsDowntime = true
                    };

                    dailySchedule.Add(downtimeLesson);
                }
            }

            var t = stopWatch.ElapsedMilliseconds;

            return PartialView("_TeacherDailySchedule", dailySchedule);
        }

        [HttpPost]
        public ActionResult Schedule(int teacherId, int[] weekNumbers)
        {
            var teacher = _context.Jobs
                .Where(x => x.JobId == teacherId)
                .SingleOrDefault();
            if (teacher == null)
            {
                return Content("Error");
            }

            // Stopwatch stopWatch = new Stopwatch();
            // stopWatch.Start();

            var personId = teacher.Employee.PersonId;
            var schedule = _context.LessonDetails
                .Where(x => x.PlannedChairJob.Job.Employee.PersonId == personId
                    && weekNumbers.Contains(x.Lesson.Schedule.WeekNumber) && x.DeletedAt == null)
                .OrderBy(n => n.Lesson.Schedule.WeekNumber)
                .ThenBy(n => n.Lesson.Schedule.DayNumber)
                .ThenBy(n => n.Lesson.Schedule.ClassNumber)
                .GroupBy(g => new { g.Lesson.Schedule.WeekNumber, g.Lesson.Schedule.DayNumber, g.Lesson.Schedule.ClassNumber })
                .Select(x => new TeacherLessonViewModel
                {
                    WeekNumber = x.Key.WeekNumber,
                    DayNumber = x.Key.DayNumber,
                    ClassNumber = x.Key.ClassNumber,
                    Disciplines = x.GroupBy(g => new { g.Lesson.DisciplineId, g.Lesson.Discipline.DisciplineName.Name, g.Lesson.LessonTypeId })
                            .Select(d => new TeacherDisciplineViewModel
                            {
                                DisciplineId = d.Key.DisciplineId,
                                DisciplineName = d.Key.Name,
                                IsLection = d.Key.LessonTypeId == (int) LessonTypes.Lection,
                                Auditoriums = d
                                    .GroupBy(g => new
                                    {
                                        g.AuditoriumId,
                                        AuditoriumNumber = g.Auditorium.AuditoriumNumber + g.Auditorium.Housing.Abbreviation
                                    })
                                    .Select(a => new TeacherDisciplineAuditoriumViewModel
                                    {
                                        AuditoriumId = a.Key.AuditoriumId,
                                        AuditoriumNumber = a.Key.AuditoriumNumber,
                                        Groups = x.Select(gr => gr.Lesson.Schedule.Group.GroupName)
                                    })
                            })
                }
                )
                .ToList();

            // Окна в расписании преподавателя
            var teacherDowntimeAll = _jobService.TeachersDowntime(weekNumbers, teacherId, maxDiff: 2)
                .Select(x => new
                {
                    x.WeekNumber,
                    x.DayNumber,
                    x.ClassNumber,
                    x.ClassDiff
                })
                .Distinct()
                .ToList();

            // Для каждой недели
            foreach (var weekNumber in weekNumbers)
            {
                var teacherDowntimeWeek = teacherDowntimeAll.Where(x => x.WeekNumber == weekNumber)
                    .OrderBy(n => n.ClassNumber)
                    .ToList();

                // Перебираем занятия между которыми окна
                for (int i = 0; i < teacherDowntimeWeek.Count() - 1; i++)
                {
                    var classNumber = teacherDowntimeWeek[i].ClassNumber;
                    var classDiff = teacherDowntimeWeek[i].ClassDiff;

                    // Формируем пустые занятия-окна
                    for (int j = classNumber + 1; j <= classNumber + classDiff; j++)
                    {
                        var downtimeLesson = new TeacherLessonViewModel
                        {
                            WeekNumber = weekNumber,
                            DayNumber = teacherDowntimeWeek[i].DayNumber,
                            ClassNumber = j,
                            IsDowntime = true
                        };

                        schedule.Add(downtimeLesson);
                    }
                }
            }

            ViewBag.WeekNumbers = weekNumbers.OrderBy(x => x);

            // var t = stopWatch.ElapsedMilliseconds;

            return PartialView("_TeacherWeekSchedule", schedule);
        }
    }
}