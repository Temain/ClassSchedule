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
using WebGrease.Css.Extensions;

namespace ClassSchedule.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        public HomeController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        /// <summary>
        /// Показ расписания на неделю
        /// </summary>
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
                    NumberOfStudents = x.NumberOfStudents,
                    Lessons = x.Lessons
                        .Where(g => g.WeekNumber == UserProfile.WeekNumber && g.DeletedAt == null)
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
                                // IsNotActive = y.IsNotActive
                            })
                        }) 
                })
                .ToList();

            // Окна между занятиями у преподавателей
            var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
            if (jobRepository != null)
            {
                var downtimes = jobRepository.TeachersDowntime(UserProfile.WeekNumber, maxDiff: 2);
                foreach (var downtime in downtimes)
                {
                    var groupId = downtime.GroupId;
                    var dayNumber = downtime.DayNumber;
                    var classNumber = downtime.ClassNumber;
                    var jobId = downtime.JobId;

                    groupLessons.Where(g => g.GroupId == groupId)
                        .SelectMany(g => g.Lessons)
                        .Where(x => x.DayNumber == dayNumber && x.ClassNumber == classNumber)
                        .SelectMany(x => x.LessonParts)
                        .Where(p => p.TeacherId == jobId)
                        .All(c => { c.TeacherHasDowntime = true; return true; });
                }
            }

            viewModel.GroupLessons = groupLessons;
            viewModel.WeekNumber = UserProfile.WeekNumber;

            // Вычисление первой и последней даты редактируемой недели 
            DateTime yearStartDate = UserProfile.EducationYear.DateStart;
            int delta = DayOfWeek.Monday - yearStartDate.DayOfWeek;
            DateTime firstMonday = yearStartDate.AddDays(delta);
            viewModel.FirstDayOfWeek = firstMonday.AddDays(UserProfile.WeekNumber * 7);
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

            var lessons = GetLessonViewModel(groupId, weekNumber, dayNumber, classNumber);

            // Типы занятий
            viewModel.LessonTypes = UnitOfWork.Repository<LessonType>()
                .GetQ(orderBy: o => o.OrderBy(n => n.Order))
                .Select(
                    x =>
                        new LessonTypeViewModel
                        {
                            LessonTypeId = x.LessonTypeId,
                            LessonTypeName = x.LessonTypeName
                        })
                .ToList();

            // Корпуса
            var housingRepository = UnitOfWork.Repository<Housing>() as HousingRepository;
            if (housingRepository != null)
            {
                var housings = housingRepository.HousingEqualLength();
                viewModel.Housings = housings
                    .Select(
                        x =>
                            new HousingViewModel
                            {
                                HousingId = x.HousingId,
                                HousingName = x.HousingName,
                                Abbreviation = x.Abbreviation
                            })
                    .ToList();
            }      

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

                    var auditoriumRepository = UnitOfWork.Repository<Auditorium>() as AuditoriumRepository;
                    if (auditoriumRepository != null)
                    {
                        var auditoriums = auditoriumRepository.AuditoriumWithEmployment(chairId, housingId, weekNumber, dayNumber, classNumber, groupId);
                        lessonPart.Auditoriums = auditoriums
                            .Select(
                                x =>
                                    new AuditoriumViewModel
                                    {
                                        AuditoriumId = x.AuditoriumId,
                                        AuditoriumNumber = x.AuditoriumNumber,
                                        AuditoriumTypeName = x.AuditoriumTypeName,
                                        ChairId = x.ChairId,
                                        Places = x.Places,
                                        Comment = x.Comment,
                                        Employment = x.Employment
                                    })
                                    .ToList();
                    }
                }
                    
            }

            viewModel.Lessons = lessons;

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
                foreach (ModelError error in allErrors)
                {
                    var message = error.ErrorMessage;
                }

                return null;
            }

            // Удаление занятия(ий)
            var viewModelLessonIds = viewModel.Lessons.SelectMany(x => x.LessonParts).Select(p => p.LessonId);
            var lessonsForDelete = UnitOfWork.Repository<Lesson>()
                .Get(x => x.GroupId == viewModel.GroupId && x.WeekNumber == viewModel.WeekNumber
                        && x.DayNumber == viewModel.DayNumber && x.ClassNumber == viewModel.ClassNumber
                        && x.DeletedAt == null)
                .Where(x => !viewModelLessonIds.Contains(x.LessonId));
            foreach (var lesson in lessonsForDelete)
            {
                lesson.DeletedAt = DateTime.Now;
                UnitOfWork.Repository<Lesson>().Update(lesson);
                UnitOfWork.Save();

                Logger.Info("Занятие помечено как удалённое : LessonId=" + lesson.LessonId);
            }

            // Создание и обновление занятий
            foreach (var lessonViewModel in viewModel.Lessons)
            {
                foreach (var lessonPartViewModel in lessonViewModel.LessonParts)
                {             
                    var lessonId = lessonPartViewModel.LessonId;
                    var lesson = UnitOfWork.Repository<Lesson>()
                        .Get(x => x.LessonId == lessonId)
                        .SingleOrDefault();

                    // Обновление занятия
                    if (lesson != null)
                    {
                        lesson.LessonTypeId = lessonViewModel.LessonTypeId;
                        lesson.DisciplineId = lessonViewModel.DisciplineId;
                        lesson.AuditoriumId = lessonPartViewModel.AuditoriumId;
                        lesson.JobId = lessonPartViewModel.TeacherId;
                        // lesson.IsNotActive = lessonPartViewModel.IsNotActive;
                        lesson.UpdatedAt = DateTime.Now;

                        UnitOfWork.Repository<Lesson>().Update(lesson);
                        UnitOfWork.Save();

                        Logger.Info("Обновлено занятие : LessonId=" + lesson.LessonId);
                    }
                    // Создание нового занятия
                    else
                    {
                        var classDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart,
                            viewModel.WeekNumber, viewModel.DayNumber);
                        lesson = new Lesson
                        {
                            LessonGuid = Guid.NewGuid(),                           
                            WeekNumber = viewModel.WeekNumber,
                            DayNumber = viewModel.DayNumber,
                            ClassNumber = viewModel.ClassNumber,
                            ClassDate = classDate,
                            GroupId = viewModel.GroupId,
                            LessonTypeId = lessonViewModel.LessonTypeId,
                            DisciplineId = lessonViewModel.DisciplineId,
                            AuditoriumId = lessonPartViewModel.AuditoriumId,
                            JobId = lessonPartViewModel.TeacherId,
                            // IsNotActive = lessonPartViewModel.IsNotActive,
                            CreatedAt = DateTime.Now,
                        };

                        UnitOfWork.Repository<Lesson>().Insert(lesson);
                        UnitOfWork.Save();

                        Logger.Info("Создано новое занятие : LessonId=" + lesson.LessonId);
                    }    
                }
            }

            var lessonCell = GetLessonViewModel(viewModel.GroupId, viewModel.WeekNumber, viewModel.DayNumber,
                viewModel.ClassNumber);
            return PartialView("_LessonCell", lessonCell);
        }

        [HttpPost]
        public ActionResult CopyLesson(int weekNumber, int sourceGroupId, int sourceDayNumber, int sourceClassNumber,
            int targetGroupId, int targetDayNumber, int targetClassNumber)
        {
            var targetLessons = UnitOfWork.Repository<Lesson>()
                .Get(x => x.GroupId == targetGroupId && x.WeekNumber == weekNumber
                    && x.DayNumber == targetDayNumber && x.ClassNumber == targetClassNumber
                    && x.DeletedAt == null);
            foreach (var targetLesson in targetLessons)
            {
                targetLesson.DeletedAt = DateTime.Now;
                UnitOfWork.Repository<Lesson>().Update(targetLesson);
                UnitOfWork.Save();
            }

            var sourceLessons = UnitOfWork.Repository<Lesson>()
                .Get(x => x.GroupId == sourceGroupId && x.WeekNumber == weekNumber
                    && x.DayNumber == sourceDayNumber && x.ClassNumber == sourceClassNumber
                    && x.DeletedAt == null);
            foreach (var sourceLesson in sourceLessons)
            {
                var targetClassDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart,
                    weekNumber, targetDayNumber);
                var targetLesson = new Lesson
                {
                    AuditoriumId = sourceLesson.AuditoriumId,
                    DisciplineId = sourceLesson.DisciplineId,
                    GroupId = targetGroupId,
                    JobId = sourceLesson.JobId,
                    LessonTypeId = sourceLesson.LessonTypeId,
                    WeekNumber = weekNumber,
                    DayNumber = targetDayNumber,
                    ClassNumber = targetClassNumber,
                    ClassDate = targetClassDate,
                    CreatedAt = DateTime.Now,
                    LessonGuid = Guid.NewGuid()
                };

                UnitOfWork.Repository<Lesson>().Insert(targetLesson);
                UnitOfWork.Save();

                Logger.Info("Скопировано занятие : SourceLessonId={0}, TargetLessonId={1}", sourceLesson.LessonId, targetLesson.LessonId);
            }

            var lessonCell = GetLessonViewModel(targetGroupId, weekNumber, targetDayNumber, targetClassNumber);
            return PartialView("_LessonCell", lessonCell);
        }

        [HttpPost]
        public ActionResult RemoveLesson(int groupId, int weekNumber, int dayNumber, int classNumber)
        {
            var lessons = UnitOfWork.Repository<Lesson>()
                .Get(x => x.GroupId == groupId && x.WeekNumber == weekNumber
                    && x.DayNumber == dayNumber && x.ClassNumber == classNumber
                    && x.DeletedAt == null);
            foreach (var lesson in lessons)
            {
                lesson.DeletedAt = DateTime.Now;
                UnitOfWork.Repository<Lesson>().Update(lesson);
                UnitOfWork.Save();

                Logger.Info("Занятие помечено как удалённое : LessonId=" + lesson.LessonId);
            }

            return null;
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

        #region Вспомогательные методы

        public List<LessonViewModel> GetLessonViewModel(int groupId, int weekNumber, int dayNumber, int classNumber)
        {
            var lessons = UnitOfWork.Repository<Lesson>()
                .GetQ(x => x.GroupId == groupId && x.WeekNumber == weekNumber
                    && x.DayNumber == dayNumber && x.ClassNumber == classNumber
                    && x.DeletedAt == null)
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
                            HousingId = y.Auditorium.HousingId,
                            AuditoriumId = y.AuditoriumId,
                            TeacherId = y.JobId,
                            AuditoriumName = y.Auditorium.AuditoriumNumber + y.Auditorium.Housing.Abbreviation + ".",
                            TeacherLastName = y.Job.Employee.Person.LastName,
                            TeacherFirstName = y.Job.Employee.Person.FirstName,
                            TeacherMiddleName = y.Job.Employee.Person.MiddleName,
                            // IsNotActive = y.IsNotActive
                        })
                })
                .ToList();

            // Проверка на окна у преподавателей
            var lessonTeacherIds = lessons
                .SelectMany(x => x.LessonParts)
                .Select(p => p.TeacherId)
                .Distinct();

            var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
            if (jobRepository != null)
            {
                foreach (var teacherId in lessonTeacherIds)
                {
                    var teacherDowntime = jobRepository.TeachersDowntime(UserProfile.WeekNumber, teacherId, maxDiff: 2)
                        .Where(x => x.DayNumber == dayNumber && x.ClassNumber == classNumber)
                        .Distinct();

                    if (teacherDowntime.Any())
                    {
                        lessons.SelectMany(x => x.LessonParts)
                           .Where(p => p.TeacherId == teacherId)
                           .All(c => { c.TeacherHasDowntime = true; return true; });   
                    }
                }
            }           

            return lessons;
        }

        #endregion
    }
}