using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Models;
using ClassSchedule.Web.Models.Auditorium;
using ClassSchedule.Web.Models.Teacher;

namespace ClassSchedule.Web.Controllers
{
    public class AuditoriumController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AuditoriumController(ApplicationDbContext context)
        {
            _context = context;
        }

        //[HttpPost]
        //public ActionResult Schedule(int auditoriumId, int[] weekNumbers)
        //{
        //    var schedule = UnitOfWork.Repository<Lesson>()
        //        .GetQ(x => x.AuditoriumId == auditoriumId
        //                && weekNumbers.Contains(x.WeekNumber) && x.DeletedAt == null,
        //            orderBy: o => o.OrderBy(n => n.WeekNumber)
        //                .ThenBy(n => n.DayNumber)
        //                .ThenBy(n => n.ClassNumber))
        //        .GroupBy(g => new { g.WeekNumber, g.DayNumber, g.ClassNumber })
        //        .Select(x => new AuditoriumLesson
        //        {
        //            WeekNumber = x.Key.WeekNumber,
        //            DayNumber = x.Key.DayNumber,
        //            ClassNumber = x.Key.ClassNumber,
        //            Disciplines = x.GroupBy(g => new { g.DisciplineId, g.Discipline.DisciplineName, g.LessonTypeId })
        //                .Select(d => new AuditoriumDiscipline
        //                {
        //                    DisciplineId = d.Key.DisciplineId,
        //                    DisciplineName = d.Key.DisciplineName,
        //                    IsLection = d.Key.LessonTypeId == (int)LessonTypes.Lection,
        //                    Teachers = d
        //                        .GroupBy(g => new
        //                        {
        //                            g.Job.Employee.PersonId,
        //                            TeacherLastName = g.Job.Employee.Person.LastName,
        //                            TeacherFirstName = g.Job.Employee.Person.FirstName,
        //                            TeacherMiddleName = g.Job.Employee.Person.MiddleName,
        //                        })
        //                        .Select(a => new AuditoriumDisciplineTeacher
        //                        {
        //                            PersonId = a.Key.PersonId,
        //                            TeacherLastName = a.Key.TeacherLastName,
        //                            TeacherFirstName = a.Key.TeacherFirstName,
        //                            TeacherMiddleName = a.Key.TeacherMiddleName,
        //                            Groups = a.Select(y => y.Group.DivisionName)
        //                        })
        //                })
        //        }
        //        )
        //        .ToList();

        //    ViewBag.WeekNumbers = weekNumbers.OrderBy(x => x);

        //    return PartialView("_AuditoriumWeekSchedule", schedule);
        //}

        //[HttpPost]
        //public ActionResult Available(DateTime classDate, int classNumber)
        //{
        //    var availableAuditoriums = UnitOfWork.Repository<Auditorium>()
        //        .GetQ(x => x.IsDeleted != true
        //            && !x.Lessons.Any(l => l.DeletedAt == null && l.ClassDate == classDate && l.ClassNumber == classNumber))
        //        .ToList() // Вынужденная мера
        //        .GroupBy(g => new { g.HousingId, g.Housing.HousingName })
        //        .Select(x => new
        //        {
        //            HousingId = x.Key.HousingId,
        //            HousingName = x.Key.HousingName,
        //            Auditoriums = x.GroupBy(f => f.AuditoriumNumber[0])
        //                .Select(y => new
        //                {
        //                    y.Key,
        //                    Floors = String.Join(", ", y.Select(z => z.AuditoriumNumber).OrderBy(n => n))
        //                })
        //                .OrderBy(f => f.Key)
        //        })
        //        .OrderBy(n => n.HousingId)
        //        .ToList();

        //    return Json(availableAuditoriums);
        //}
    }
}