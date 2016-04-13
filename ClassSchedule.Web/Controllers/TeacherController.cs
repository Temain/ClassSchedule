using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess.Interfaces;
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
                return Content("Ошибка");
            }

            var personId = teacher.Employee.PersonId;
            //var dailySchedule = UnitOfWork.Repository<Lesson>()
            //    .GetQ(x => x.Job.Employee.PersonId == personId 
            //        && x.WeekNumber == UserProfile.WeekNumber && x.DayNumber == dayNumber
            //        && x.DeletedAt == null,
            //        orderBy: o => o.OrderBy(n => n.ClassNumber))
            //    .Select(x => new TeacherLesson
            //    {
            //        DayNumber = x.DayNumber,
            //        ClassNumber = x.ClassNumber,
            //        Discipline = x.Discipline.DisciplineName,
            //        IsLection = x.LessonTypeId == (int) LessonTypes.Lection,
            //        Auditorium = x.Auditorium.AuditoriumNumber + x.Auditorium.Housing.Abbreviation,
            //        Group = x.Group.DivisionName
            //    })
            //    .ToList();

            //.Select(x => new TeacherLesson
            //{
            //    DayNumber = x.DayNumber,
            //    ClassNumber = x.ClassNumber,
            //    Discipline = x.Discipline.DisciplineName,
            //    IsLection = x.LessonTypeId == (int)LessonTypes.Lection,
            //    Auditorium = x.Auditorium.AuditoriumNumber + x.Auditorium.Housing.Abbreviation,
            //    Group = x.Group.DivisionName
            //})
            //.ToList();

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            var dailySchedule = UnitOfWork.Repository<Lesson>()
                .GetQ(x => x.Job.Employee.PersonId == personId
                           && x.WeekNumber == UserProfile.WeekNumber && x.DayNumber == dayNumber
                           && x.DeletedAt == null,
                    orderBy: o => o.OrderBy(n => n.ClassNumber))
                .GroupBy(g => new {g.ClassNumber})
                .Select(
                    x => 
                        new TeacherLesson
                        {
                            ClassNumber = x.Key.ClassNumber,
                            Disciplines = x.GroupBy(g => new { g.DisciplineId, g.Discipline.DisciplineName })                            
                                .Select(
                                    d => 
                                        new TeacherDiscipline
                                        {
                                            DisciplineId = d.Key.DisciplineId,
                                            DisciplineName = d.Key.DisciplineName,
                                            Auditoriums = d.GroupBy(g => new { g.AuditoriumId, AuditoriumNumber = g.Auditorium.AuditoriumNumber + g.Auditorium.Housing.Abbreviation })
                                                .Select(
                                                    a => 
                                                        new TeacherDisciplineAuditorium
                                                        {
                                                            AuditoriumId = a.Key.AuditoriumId,
                                                            AuditoriumNumber = a.Key.AuditoriumNumber,//a.FirstOrDefault().Auditorium.AuditoriumNumber + a.FirstOrDefault().Auditorium.Housing.Abbreviation,
                                                            Groups = a.Select(y => y.Group.DivisionName)
                                                        })
                                        })
                        }
                )
                .ToList();

            var t = stopWatch.ElapsedMilliseconds;

            return PartialView("_TeacherDailySchedule", dailySchedule);
        }
    }
}