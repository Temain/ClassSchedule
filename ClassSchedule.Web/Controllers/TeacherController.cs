using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.DataAccess.Repositories;
using ClassSchedule.Domain.Models;
using ClassSchedule.Web.Models.Schedule;
using ClassSchedule.Web.Models.Teacher;

namespace ClassSchedule.Web.Controllers
{
    public class TeacherController : BaseController
    {
        public TeacherController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public ActionResult Schedule(int teacherId, int dayNumber)
        {
            var teacher = UnitOfWork.Repository<Job>()
                .GetQ(x => x.JobId == teacherId)
                .SingleOrDefault();
            if (teacher == null)
            {
                return Content("Error");
            }

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var personId = teacher.Employee.PersonId;
            var dailySchedule = UnitOfWork.Repository<Lesson>()
                .GetQ(x => x.Job.Employee.PersonId == personId
                           && x.WeekNumber == UserProfile.WeekNumber && x.DayNumber == dayNumber
                           && x.DeletedAt == null,
                    orderBy: o => o.OrderBy(n => n.ClassNumber))
                .GroupBy(g => new {g.ClassNumber})
                .Select(x => new TeacherLesson
                    {
                        DayNumber = dayNumber,
                        ClassNumber = x.Key.ClassNumber,
                        Disciplines = x.GroupBy(g => new { g.DisciplineId, g.Discipline.DisciplineName, g.LessonTypeId })                            
                            .Select(d => new TeacherDiscipline
                                {
                                    DisciplineId = d.Key.DisciplineId,
                                    DisciplineName = d.Key.DisciplineName,
                                    IsLection = d.Key.LessonTypeId == (int) LessonTypes.Lection,
                                    Auditoriums = d.GroupBy(g => new { g.AuditoriumId, AuditoriumNumber = g.Auditorium.AuditoriumNumber + g.Auditorium.Housing.Abbreviation })
                                        .Select(a => 
                                            new TeacherDisciplineAuditorium
                                            {
                                                AuditoriumId = a.Key.AuditoriumId,
                                                AuditoriumNumber = a.Key.AuditoriumNumber,
                                                Groups = a.Select(y => y.Group.DivisionName)
                                            })
                                })
                    }
                )
                .ToList();

            // Окна в расписании преподавателя
            var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
            if (jobRepository != null)
            {
                var teacherDowntime = jobRepository.TeachersDowntime(UserProfile.WeekNumber, teacherId, maxDiff: 2)
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
                        var downtimeLesson = new TeacherLesson
                        {
                            DayNumber = teacherDowntime[i].DayNumber,
                            ClassNumber = j,
                            IsDowntime = true
                        };

                        dailySchedule.Add(downtimeLesson);
                    }
                }
            }    

            var t = stopWatch.ElapsedMilliseconds;

            return PartialView("_TeacherDailySchedule", dailySchedule);
        }

        [HttpPost]
        public ActionResult Schedule(int teacherId, int[] weekNumbers)
        {
            var teacher = UnitOfWork.Repository<Job>()
                .GetQ(x => x.JobId == teacherId)
                .SingleOrDefault();
            if (teacher == null)
            {
                return Content("Error");
            }

            // Stopwatch stopWatch = new Stopwatch();
            // stopWatch.Start();

            var personId = teacher.Employee.PersonId;
            var schedule = UnitOfWork.Repository<Lesson>()
                .GetQ(x => x.Job.Employee.PersonId == personId
                        && weekNumbers.Contains(x.WeekNumber) && x.DeletedAt == null,
                    orderBy: o => o.OrderBy(n => n.WeekNumber)
                        .ThenBy(n => n.DayNumber)
                        .ThenBy(n => n.ClassNumber))
                .GroupBy(g => new { g.WeekNumber, g.DayNumber, g.ClassNumber })
                .Select(x => new TeacherLesson
                    {
                        WeekNumber = x.Key.WeekNumber,
                        DayNumber = x.Key.DayNumber,
                        ClassNumber = x.Key.ClassNumber,
                        Disciplines = x.GroupBy(g => new { g.DisciplineId, g.Discipline.DisciplineName, g.LessonTypeId })
                            .Select(d => new TeacherDiscipline
                                {
                                    DisciplineId = d.Key.DisciplineId,
                                    DisciplineName = d.Key.DisciplineName,
                                    IsLection = d.Key.LessonTypeId == (int) LessonTypes.Lection,
                                    Auditoriums = d
                                        .GroupBy(g => new
                                        {
                                            g.AuditoriumId, 
                                            AuditoriumNumber = g.Auditorium.AuditoriumNumber + g.Auditorium.Housing.Abbreviation
                                        })
                                        .Select(a => new TeacherDisciplineAuditorium
                                            {
                                                AuditoriumId = a.Key.AuditoriumId,
                                                AuditoriumNumber = a.Key.AuditoriumNumber,
                                                Groups = a.Select(y => y.Group.DivisionName)
                                            })
                                })
                    }
                )
                .ToList();

            // Окна в расписании преподавателя
            var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
            if (jobRepository != null)
            {
                var teacherDowntimeAll = jobRepository.TeachersDowntime(weekNumbers, teacherId, maxDiff: 2)
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
                            var downtimeLesson = new TeacherLesson
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
               
            }

            ViewBag.WeekNumbers = weekNumbers.OrderBy(x => x);

            // var t = stopWatch.ElapsedMilliseconds;

            return PartialView("_TeacherWeekSchedule", schedule);
        }
    }
}