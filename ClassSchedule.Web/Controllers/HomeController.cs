using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.DataAccess.Repositories;
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
                        .Where(g => g.WeekNumber == UserProfile.WeekNumber)
                        .GroupBy(g => new { g.DayNumber, g.ClassNumber, 
                            g.DisciplineId, g.Discipline.DisciplineName, g.LessonTypeId })
                        .Select(s => new LessonViewModel
                        {
                            DisciplineId = s.Key.DisciplineId,
                            DisciplineName = s.Key.DisciplineName,
                            LessonTypeId = s.Key.LessonTypeId ?? 0,
                            DayNumber = s.Key.DayNumber,
                            ClassNumber = s.Key.ClassNumber,
                            LessonParts = s.Select(y => new LessonPartViewModel
                            {
                                LessonId = y.LessonId,
                                LessonTypeId = y.LessonTypeId ?? 0,
                                LessonTypeName = y.LessonType.LessonTypeName,
                                DayNumber = y.DayNumber,
                                ClassNumber = y.ClassNumber,
                                DisciplineId = y.DisciplineId,
                                DisciplineName = y.Discipline.DisciplineName,
                                ChairId = y.Discipline.ChairId,
                                ChairName = y.Discipline.Chair.DivisionName,
                                AuditoriumId = y.AuditoriumId,
                                AuditoriumName = y.Auditorium.AuditoriumNumber + y.Auditorium.Housing.Abbreviation + ".",
                                TeacherId = y.JobId,
                                TeacherLastName = y.Job.Employee.Person.LastName,
                                TeacherFirstName = y.Job.Employee.Person.FirstName,
                                TeacherMiddleName = y.Job.Employee.Person.MiddleName,
                                IsNotActive = y.IsNotActive
                            })
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
                // var watch = System.Diagnostics.Stopwatch.StartNew();

                var lessons = UnitOfWork.Repository<Lesson>()
                    .GetQ(x => x.GroupId == groupId && x.WeekNumber == weekNumber
                               && x.DayNumber == dayNumber && x.ClassNumber == classNumber)
                    .GroupBy(x => new
                    {
                        x.DisciplineId,
                        x.Discipline.DisciplineName,
                        x.Discipline.ChairId,
                        x.Discipline.Chair.DivisionName
                    })
                    .Select(x => new LessonViewModel
                    {
                        DisciplineId = x.Key.DisciplineId,
                        DisciplineName = x.Key.DisciplineName,
                        ChairId = x.Key.ChairId,
                        ChairName = x.Key.DivisionName,
                        LessonParts = x
                            .Select(y => new LessonPartViewModel
                            {
                                LessonId = y.LessonId,
                                LessonTypeId = y.LessonTypeId ?? 0,
                                DisciplineId = y.DisciplineId,
                                DisciplineName = y.Discipline.DisciplineName,
                                ChairId = y.Discipline.ChairId,
                                ChairName = y.Discipline.Chair.DivisionName,
                                AuditoriumId = y.AuditoriumId,
                                TeacherId = y.JobId,
                                IsNotActive = y.IsNotActive
                            })
                    })
                    .ToList();

                //foreach (var lesson in lessons)
                //{
                    //var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
                    //if (jobRepository != null)
                    //{
                    //    var chairTeachers = jobRepository.ActualTeachers(UserProfile.EducationYear, lesson.ChairId);
                    //    lesson.ChairTeachers = chairTeachers
                    //        .Select(
                    //            x =>
                    //                new TeacherViewModel
                    //                {
                    //                    TeacherId = x.JobId,
                    //                    TeacherFullName = x.Employee.Person.FullName
                    //                }).ToList();
                    //}

                    //lesson.ChairTeachers = UnitOfWork.Repository<Job>()
                    //    .Get(x => x.ChairId == lesson.ChairId)
                    //    .Select(x =>
                    //        new TeacherViewModel
                    //        {
                    //            TeacherId = x.JobId,
                    //            TeacherFullName = x.Employee.Person.LastName
                    //        })
                    //    .ToList();

                //}

                // watch.Stop();
                // var elapsedMs = watch.ElapsedMilliseconds;

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

            foreach (var lessonViewModel in viewModel.LessonParts)
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