using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.SessionState;
using System.Web.UI;
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
            var groups = GetEditableGroups();
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

        /// <summary>
        /// Редактирование занятия
        /// </summary>
        // TODO: Убрать из параметров weekNumber, он есть в профиле пользователя 
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

            var lessons = GetLessonViewModel(groupId, weekNumber, dayNumber, classNumber, checkDowntimes: false);

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

        /// <summary>
        /// Копирование занятия
        /// </summary>
        // TODO: Убрать из параметров weekNumber, он есть в профиле пользователя 
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

        /// <summary>
        /// Удаление занятия
        /// </summary>
        // TODO: Убрать из параметров weekNumber, он есть в профиле пользователя 
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

        /// <summary>
        /// Возвращает данные для обновления занятий преподавателей
        /// Например, для того чтобы подсветилась подсказка об окнах в расписании преподавателей
        /// </summary>
        [HttpPost]
        public ActionResult RefreshLesson(int groupId, int dayNumber, int classNumber, int[] teacherIds)
        {
            if (!Request.IsAjaxRequest() || teacherIds == null) return null;

            var editableGroups = GetEditableGroups()
                .Select(x => x.GroupId);

            var teachersPersonIds = UnitOfWork.Repository<Job>()
                .GetQ(t => teacherIds.Contains(t.JobId))
                .Select(x => x.Employee.PersonId);

            var changedLessons = UnitOfWork.Repository<Lesson>()
               .GetQ(x => teachersPersonIds.Contains(x.Job.Employee.PersonId) && editableGroups.Contains(x.GroupId)
                   && x.WeekNumber == UserProfile.WeekNumber && x.DayNumber == dayNumber 
                   && x.ClassNumber != classNumber && x.DeletedAt == null)
               .ToList();

            var lessonCellsForRefresh = new List<RefreshCellViewModel>();
            foreach (var lesson in changedLessons)
            {
                var targetGroupId = lesson.GroupId;
                var targetClassNumber = lesson.ClassNumber;
                var lessonViewModel = GetLessonViewModel(targetGroupId, UserProfile.WeekNumber, dayNumber, targetClassNumber);

                var lessonCellContent = RenderPartialToString(this, "_LessonCell", lessonViewModel, ViewData, TempData); 
                var lessonCellForRefresh = new RefreshCellViewModel
                {
                    DayNumber = dayNumber,
                    ClassNumber = targetClassNumber,
                    GroupId = targetGroupId,
                    Content = lessonCellContent
                };
                lessonCellsForRefresh.Add(lessonCellForRefresh);
            }

            return Json(lessonCellsForRefresh);
        }

        /// <summary>
        /// Выбор потока / курса / группы
        /// </summary>
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

        /// <summary>
        /// Возвращает список групп, для которых редактируется расписание
        /// </summary>
        public IQueryable<Group> GetEditableGroups()
        {
            var groups = UnitOfWork.Repository<Group>()
                .GetQ(g => g.IsDeleted != true, orderBy: o => o.OrderBy(n => n.DivisionName));

            if (UserProfile.GroupId != null)
            {
                groups = groups.Where(g => g.GroupId == UserProfile.GroupId);
            }
            else if (UserProfile.CourseId != null)
            {
                groups = groups.Where(g => g.CourseId == UserProfile.CourseId);             
            }

            return groups;
        } 

        // TODO: Убрать из параметров weekNumber, он есть в профиле пользователя
        public List<LessonViewModel> GetLessonViewModel(int groupId, int weekNumber, int dayNumber, int classNumber, bool checkDowntimes = true)
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
            if (checkDowntimes)
            {
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
            }        

            return lessons;
        }

        public static string RenderPartialToString(Controller controller, string partialViewName, object model, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName);

            if (result.View != null)
            {
                controller.ViewData.Model = model;
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(controller.ControllerContext, result.View, viewData, tempData, output);
                        result.View.Render(viewContext, output);
                    }
                }

                return sb.ToString();
            }

            return String.Empty;
        }

        #endregion
    }
}