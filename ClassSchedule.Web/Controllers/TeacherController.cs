using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Models;
using ClassSchedule.Web.Models.Schedule;

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
            var dailySchedule = UnitOfWork.Repository<Lesson>()
                .GetQ(x => x.Job.Employee.PersonId == personId 
                    && x.WeekNumber == UserProfile.WeekNumber && x.DayNumber == dayNumber
                    && x.DeletedAt == null,
                    orderBy: o => o.OrderBy(n => n.ClassNumber))
                .Select(x => new TeacherLesson
                {
                    DayNumber = x.DayNumber,
                    ClassNumber = x.ClassNumber,
                    Discipline = x.Discipline.DisciplineName,
                    IsLection = x.LessonTypeId == (int) LessonTypes.Lection,
                    Auditorium = x.Auditorium.AuditoriumNumber + x.Auditorium.Housing.Abbreviation,
                    Group = x.Group.DivisionName
                })
                .ToList();

            return PartialView("_TeacherDailySchedule", dailySchedule);
        }
    }
}