using System;
using System.Linq;
using System.Web.Mvc;
using ClassSchedule.Business.Models.Auditorium;
using ClassSchedule.Domain.Context;

namespace ClassSchedule.Web.Controllers
{
    public class AuditoriumController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public AuditoriumController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Переписать
        [HttpPost]
        public ActionResult Schedule(int auditoriumId, int[] weekNumbers)
        {
            var schedule = _context.LessonDetails
                .Where(x => x.AuditoriumId == auditoriumId && weekNumbers.Contains(x.Lesson.Schedule.WeekNumber) && x.DeletedAt == null)
                .OrderBy(n => n.Lesson.Schedule.WeekNumber)
                .ThenBy(n => n.Lesson.Schedule.DayNumber)
                .ThenBy(n => n.Lesson.Schedule.ClassNumber)
                .GroupBy(g => new { g.Lesson.Schedule.WeekNumber, g.Lesson.Schedule.DayNumber, g.Lesson.Schedule.ClassNumber })
                .Select(x => new AuditoriumLessonViewModel
                {
                    WeekNumber = x.Key.WeekNumber,
                    DayNumber = x.Key.DayNumber,
                    ClassNumber = x.Key.ClassNumber,
                    Disciplines = x.GroupBy(g => new { g.Lesson.DisciplineId, g.Lesson.Discipline.DisciplineName.Name, g.Lesson.LessonTypeId })
                        .Select(d => new AuditoriumDisciplineViewModel
                        {
                            DisciplineId = d.Key.DisciplineId,
                            DisciplineName = d.Key.Name,
                            IsLection = d.Key.LessonTypeId == (int)LessonTypes.Lection,
                            Teachers = d
                                .GroupBy(g => new
                                {
                                    g.PlannedChairJob.Job.Employee.PersonId,
                                    TeacherLastName = g.PlannedChairJob.Job.Employee.Person.LastName,
                                    TeacherFirstName = g.PlannedChairJob.Job.Employee.Person.FirstName,
                                    TeacherMiddleName = g.PlannedChairJob.Job.Employee.Person.MiddleName,
                                })
                                .Select(a => new AuditoriumDisciplineTeacherViewModel
                                {
                                    PersonId = a.Key.PersonId,
                                    TeacherLastName = a.Key.TeacherLastName,
                                    TeacherFirstName = a.Key.TeacherFirstName,
                                    TeacherMiddleName = a.Key.TeacherMiddleName,
                                    Groups = a.Select(y => y.Lesson.Schedule.Group.GroupName)
                                })
                        })
                }
                )
                .ToList();

            ViewBag.WeekNumbers = weekNumbers.OrderBy(x => x);

            return PartialView("_AuditoriumWeekSchedule", schedule);
        }

        [HttpPost]
        public ActionResult Available(DateTime classDate, int classNumber)
        {
            var availableAuditoriums = _context.Auditoriums
                .Where(x => x.DeletedAt == null
                    && !x.LessonDetails.Any(l => l.DeletedAt == null && l.Lesson.Schedule.ClassDate == classDate && l.Lesson.Schedule.ClassNumber == classNumber))
                .ToList() // Вынужденная мера
                .GroupBy(g => new { g.HousingId, g.Housing.HousingName })
                .Select(x => new
                {
                    HousingId = x.Key.HousingId,
                    HousingName = x.Key.HousingName,
                    Auditoriums = x.GroupBy(f => f.AuditoriumNumber[0])
                        .Select(y => new
                        {
                            y.Key,
                            Floors = String.Join(", ", y.Select(z => z.AuditoriumNumber).OrderBy(n => n))
                        })
                        .OrderBy(f => f.Key)
                })
                .OrderBy(n => n.HousingId)
                .ToList();

            return Json(availableAuditoriums);
        }
    }
}