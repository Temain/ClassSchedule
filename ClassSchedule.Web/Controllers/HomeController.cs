using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Models;
using ClassSchedule.Web.Helpers;
using ClassSchedule.Web.Models;
using ClassSchedule.Web.Models.Schedule;
using Microsoft.AspNet.Identity;

namespace ClassSchedule.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        public ActionResult Index()
        {
            var viewModel = new ScheduleViewModel();
            var groups = UnitOfWork.Repository<Group>()
                .GetQ(g => g.IsDeleted != true, orderBy: o => o.OrderBy(n => n.DivisionName));

            if (UserProfile.GroupId != null)
            {
                groups = groups.Where(g => g.GroupId == UserProfile.GroupId);                
            }
            else if (UserProfile.CourseId != null)
            {
                groups = groups.Where(g => g.CourseId == UserProfile.CourseId);
                viewModel.FacultyName = UserProfile.Course.Faculty.DivisionName;
            }           

            var groupLessons = groups
                .Select(x => new GroupLessonsViewModel
                {
                    GroupId = x.GroupId,
                    GroupName = x.DivisionName,
                    Lessons = x.Lessons
                        .Select(y => new LessonViewModel
                        {
                            LessonId = y.LessonId,
                            LessonTypeId = y.LessonTypeId ?? 0,
                            LessonTypeName = y.LessonType.LessonTypeName,
                            DisciplineId = y.DisciplineId,
                            DisciplineName = y.Discipline.DisciplineName,
                            ChairId = y.Discipline.ChairId,
                            ChairName = y.Discipline.Chair.DivisionName,
                            AuditoriumId = y.AuditoriumId,
                            AuditoriumName = y.Auditorium.Housing.HousingName + y.Auditorium.AuditoriumNumber,
                            TeacherId = y.JobId,
                            TeacherFullName = y.Job.Employee.Person.LastName,
                            IsNotActive = y.IsNotActive
                        })
                })
                .ToList();

            viewModel.GroupLessons = groupLessons;
            viewModel.WeekNumber = UserProfile.WeekNumber;

            // Вычисление первой и последней даты редактируемой недели 
            DateTime yearStartDate = UserProfile.EducationYear.DateStart;
            int delta = DayOfWeek.Monday - yearStartDate.DayOfWeek;
            DateTime firstMonday = yearStartDate.AddDays(delta);
            viewModel.FirstDayOfWeek = firstMonday.AddDays(UserProfile.WeekNumber*7);
            viewModel.LastDayOfWeek = viewModel.FirstDayOfWeek.AddDays(6);

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult GetLesson(int groupId, int weekNumber, int dayNumber, int classNumber)
        {
            if (Request.IsAjaxRequest())
            {
                var lessons = UnitOfWork.Repository<Lesson>()
                    .GetQ(x => x.GroupId == groupId && x.WeekNumber == weekNumber
                        && x.DayNumber == dayNumber && x.ClassNumber == classNumber)
                    .Select(x => new LessonViewModel
                    {
                        LessonId = x.LessonId,
                        LessonTypeId = 1,
                        DisciplineId = x.DisciplineId,
                        DisciplineName = x.Discipline.DisciplineName,
                        ChairName = x.Discipline.Chair.DivisionName,
                        AuditoriumId = x.AuditoriumId,
                        TeacherId = x.JobId,
                        IsNotActive = x.IsNotActive
                    });

                return Json(lessons);
            }

            return null;
        }

        [HttpPost]
        public ActionResult Edit(EditLessonViewModel viewModel)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(404);
            }

            if (!ModelState.IsValid)
            {
                var allErrors = ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors).ToList();

                return null;
            }

            foreach (var lessonViewModel in viewModel.Lessons)
            {
                // Обновление занятия
                if (lessonViewModel.LessonId != 0 && lessonViewModel.LessonId != null)
                {
                    var lessonId = lessonViewModel.LessonId;
                    var lesson = UnitOfWork.Repository<Lesson>()
                        .Get(x => x.LessonId == lessonId)
                        .SingleOrDefault();

                    if (lesson != null)
                    {
                        lesson.LessonTypeId = lessonViewModel.LessonTypeId;
                        lesson.DisciplineId = lessonViewModel.DisciplineId;
                        lesson.AuditoriumId = lessonViewModel.AuditoriumId;
                        lesson.JobId = lessonViewModel.TeacherId;
                        lesson.IsNotActive = lessonViewModel.IsNotActive;
                        lesson.UpdatedAt = DateTime.Now;

                        UnitOfWork.Repository<Lesson>().Update(lesson);
                        UnitOfWork.Save();
                    }                   
                }
                // Создание нового занятия
                else
                {
                    var classDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart, 
                        viewModel.WeekNumber, viewModel.DayNumber);
                    var lesson = new Lesson
                    {
                        LessonGuid = Guid.NewGuid(),
                        LessonTypeId = lessonViewModel.LessonTypeId,
                        WeekNumber = viewModel.WeekNumber,
                        DayNumber = viewModel.DayNumber,
                        ClassNumber = viewModel.ClassNumber,
                        ClassDate = classDate,
                        GroupId = viewModel.GroupId,
                        DisciplineId = lessonViewModel.DisciplineId,
                        AuditoriumId = lessonViewModel.AuditoriumId,
                        JobId = lessonViewModel.TeacherId,
                        IsNotActive = lessonViewModel.IsNotActive,
                        CreatedAt = DateTime.Now,
                    };

                    UnitOfWork.Repository<Lesson>().Insert(lesson);
                    UnitOfWork.Save();
                }
            }

            return Json("Success");
        }

        [HttpGet]
        public ActionResult SelectFlow()
        {
            var faculties = UnitOfWork.Repository<Faculty>()
                .GetQ(f => f.IsDeleted != true, orderBy: o => o.OrderBy(n => n.DivisionName))
                .Select(x => new {x.FacultyId, FacultyName = x.DivisionName});
            ViewBag.Faculties = new SelectList(faculties, "FacultyId", "FacultyName");

            return View();
        }

        [HttpPost]
        public ActionResult SelectFlow(SelectFlowViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            if (viewModel.CourseId != null || viewModel.GroupId != null || viewModel.FlowId != null)
            {
                UserProfile.CourseId = viewModel.CourseId;
                UserProfile.GroupId = viewModel.GroupId;

                UserManager.Update(UserProfile);
            }

            return RedirectToAction("Index");
        }

    }
}