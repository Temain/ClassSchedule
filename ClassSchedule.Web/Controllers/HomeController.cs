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

        [HttpGet]
        public ActionResult EditLesson(int groupId, int weekNumber, int dayNumber, int classNumber)
        {
            if (!Request.IsAjaxRequest()) return null;
            
            var viewModel = new EditLessonViewModel
            {
                GroupId = groupId,
                WeekNumber = weekNumber,
                DayNumber = dayNumber,
                ClassNumber = classNumber
            };

            var lessons = UnitOfWork.Repository<Lesson>()
                .GetQ(x => x.GroupId == groupId && x.WeekNumber == weekNumber
                           && x.DayNumber == dayNumber && x.ClassNumber == classNumber)
                .GroupBy(x => new
                {
                    x.DisciplineId,
                    x.Discipline.DisciplineName,
                    x.Discipline.ChairId,
                    x.Discipline.Chair.DivisionName,
                    x.LessonTypeId
                })
                .Select(x => new LessonViewModel
                {
                    DisciplineId = x.Key.DisciplineId,
                    DisciplineName = x.Key.DisciplineName,
                    ChairId = x.Key.ChairId,
                    ChairName = x.Key.DivisionName,
                    LessonTypeId = x.Key.LessonTypeId ?? 0,
                    LessonParts = x
                        .Select(y => new LessonPartViewModel
                        {
                            LessonId = y.LessonId,
                            // LessonTypeId = y.LessonTypeId ?? 0,
                            // DisciplineId = y.DisciplineId,
                            // DisciplineName = y.Discipline.DisciplineName,
                            // ChairId = y.Discipline.ChairId,
                            // ChairName = y.Discipline.Chair.DivisionName,
                            HousingId = y.Auditorium.HousingId,
                            AuditoriumId = y.AuditoriumId,
                            TeacherId = y.JobId,
                            IsNotActive = y.IsNotActive
                        })
                })
                .ToList();

            // var watch = System.Diagnostics.Stopwatch.StartNew();

            // Типы занятий
            viewModel.LessonTypes = UnitOfWork.Repository<LessonType>()
                .GetQ()
                .Select(
                    x =>
                        new LessonTypeViewModel
                        {
                            LessonTypeId = x.LessonTypeId,
                            LessonTypeName = x.LessonTypeName
                        })
                .OrderBy(n => n.LessonTypeName)
                .ToList();

            // Корпуса
            viewModel.Housings = UnitOfWork.Repository<Housing>()
                .GetQ()
                .Select(x => new HousingViewModel
                {
                    HousingId = x.HousingId,
                    HousingName = x.Abbreviation
                })
                .ToList();

            foreach (var lesson in lessons)
            {                   
                // Преподаватели
                var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
                if (jobRepository != null)
                {
                    var chairTeachers = jobRepository.ActualTeachers(UserProfile.EducationYear, lesson.ChairId);
                    lesson.ChairTeachers = chairTeachers
                        .Select(
                            x =>
                                new TeacherViewModel
                                {
                                    TeacherId = x.Key,
                                    TeacherFullName = x.Value
                                })
                        .ToList();
                }                   

                // Аудитории
                foreach (var lessonPart in lesson.LessonParts)
                {
                    var chairId = lesson.ChairId;
                    var housingId = lessonPart.HousingId;
                    lessonPart.Auditoriums = UnitOfWork.Repository<Auditorium>()
                        .GetQ(filter: x => (x.ChairId == chairId || x.ChairId == null) && x.HousingId == housingId,
                            orderBy: o => o.OrderByDescending(n => n.ChairId)
                                .ThenBy(n => n.AuditoriumNumber))
                        .Select(x => new AuditoriumViewModel
                        {
                            AuditoriumId = x.AuditoriumId,
                            AuditoriumNumber = x.AuditoriumNumber,
                            AuditoriumTypeName = x.AuditoriumType.AuditoriumTypeName,
                            Places = x.Places ?? 0
                        }).ToList();
                }
                    
            }

            viewModel.Lessons = lessons;

            // watch.Stop();
            // var elapsedMs = watch.ElapsedMilliseconds;

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditLesson(EditLessonViewModel viewModel)
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
                foreach (var lessonPartViewModel in lessonViewModel.LessonParts)
                {
                    // Обновление занятия
                    if (lessonPartViewModel.LessonId != 0 && lessonPartViewModel.LessonId != null)
                    {
                        var lessonId = lessonPartViewModel.LessonId;
                        var lesson = UnitOfWork.Repository<Lesson>()
                            .Get(x => x.LessonId == lessonId)
                            .SingleOrDefault();

                        if (lesson != null)
                        {
                            lesson.LessonTypeId = lessonPartViewModel.LessonTypeId;
                            lesson.DisciplineId = lessonPartViewModel.DisciplineId;
                            lesson.AuditoriumId = lessonPartViewModel.AuditoriumId;
                            lesson.JobId = lessonPartViewModel.TeacherId;
                            lesson.IsNotActive = lessonPartViewModel.IsNotActive;
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
                            LessonTypeId = lessonPartViewModel.LessonTypeId,
                            WeekNumber = viewModel.WeekNumber,
                            DayNumber = viewModel.DayNumber,
                            ClassNumber = viewModel.ClassNumber,
                            ClassDate = classDate,
                            GroupId = viewModel.GroupId,
                            DisciplineId = lessonViewModel.DisciplineId,
                            AuditoriumId = lessonPartViewModel.AuditoriumId,
                            JobId = lessonPartViewModel.TeacherId,
                            IsNotActive = lessonPartViewModel.IsNotActive,
                            CreatedAt = DateTime.Now,
                        };

                        UnitOfWork.Repository<Lesson>().Insert(lesson);
                        UnitOfWork.Save();
                    }    
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