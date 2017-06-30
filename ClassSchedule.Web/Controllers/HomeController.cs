using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.UI;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.DataAccess.Repositories;
using ClassSchedule.Domain.Helpers;
using ClassSchedule.Domain.Models;
using ClassSchedule.Web.Helpers;
using ClassSchedule.Web.Models;
using ClassSchedule.Web.Models.ChangeWeek;
using ClassSchedule.Web.Models.Schedule;
using ClassSchedule.Web.Models.SelectFlow;
using Microsoft.AspNet.Identity;
using WebGrease.Css.Extensions;

namespace ClassSchedule.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Показ расписания на неделю
        /// </summary>
        public ActionResult Index()
        {
            try
            {
                var viewModel = new ScheduleViewModel();
                var groups = GetEditableGroups();
                var groupLessons = groups
                    .Select(x => new GroupLessonsViewModel
                    {
                        GroupId = x.GroupId,
                        GroupName = x.GroupName,
                        NumberOfStudents = x.NumberOfStudents,
                        Lessons = x.Schedule
                            .SelectMany(g => g.Lessons)
                            .GroupBy(g => new { g.Schedule.DayNumber, g.Schedule.ClassNumber, g.DisciplineId, g.Discipline.DisciplineName.Name, g.LessonTypeId })
                            .Select(s => new LessonViewModel
                            {
                                DayNumber = s.Key.DayNumber,
                                ClassNumber = s.Key.ClassNumber,
                                DisciplineId = s.Key.DisciplineId,
                                DisciplineName = s.Key.Name,
                                LessonTypeId = s.Key.LessonTypeId ?? 0,
                                LessonDetails = s.SelectMany(d => d.LessonDetails)
                                    .Select(y => new LessonDetailViewModel
                                    {
                                        LessonId = y.LessonId,
                                        AuditoriumId = y.AuditoriumId,
                                        AuditoriumName = y.Auditorium.AuditoriumNumber + y.Auditorium.Housing.Abbreviation + ".",
                                        PlannedChairJobId = y.PlannedChairJobId ?? 0,
                                        TeacherLastName = y.PlannedChairJob != null && y.PlannedChairJob.Job != null
                                                && y.PlannedChairJob.Job.Employee != null && y.PlannedChairJob.Job.Employee.Person != null
                                            ? y.PlannedChairJob.Job.Employee.Person.LastName : "",
                                        TeacherFirstName = y.PlannedChairJob != null && y.PlannedChairJob.Job != null
                                                && y.PlannedChairJob.Job.Employee != null && y.PlannedChairJob.Job.Employee.Person != null ? y.PlannedChairJob.Job.Employee.Person.FirstName : "",
                                        TeacherMiddleName = y.PlannedChairJob != null && y.PlannedChairJob.Job != null
                                                && y.PlannedChairJob.Job.Employee != null && y.PlannedChairJob.Job.Employee.Person != null ? y.PlannedChairJob.Job.Employee.Person.MiddleName : "",
                                    })
                            })
                    })
                    .ToList();


                //// Окна между занятиями у преподавателей
                //var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
                //if (jobRepository != null)
                //{
                //    var downtimes = jobRepository.TeachersDowntime(UserProfile.WeekNumber, maxDiff: 2);
                //    foreach (var downtime in downtimes)
                //    {
                //        var groupId = downtime.GroupId;
                //        var dayNumber = downtime.DayNumber;
                //        var classNumber = downtime.ClassNumber;
                //        var jobId = downtime.JobId;

                //        groupLessons.Where(g => g.GroupId == groupId)
                //            .SelectMany(g => g.Lessons)
                //            .Where(x => x.DayNumber == dayNumber && x.ClassNumber == classNumber)
                //            .SelectMany(x => x.LessonParts)
                //            .Where(p => p.TeacherId == jobId)
                //            .All(c => { c.TeacherHasDowntime = true; return true; });
                //    }
                //}

                viewModel.GroupLessons = groupLessons;
                viewModel.WeekNumber = UserProfile.WeekNumber;

                // Вычисление первой и последней даты редактируемой недели 
                DateTime yearStartDate = UserProfile.EducationYear.DateStart;
                int delta = DayOfWeek.Monday - yearStartDate.DayOfWeek;
                DateTime firstMonday = yearStartDate.AddDays(delta);
                viewModel.FirstDayOfWeek = firstMonday.AddDays((UserProfile.WeekNumber - 1) * 7);
                viewModel.LastDayOfWeek = viewModel.FirstDayOfWeek.AddDays(6);

                return View(viewModel);
            }
            catch(Exception ex)
            {
                int r = 0;
            }

            return null;
        }

        ///// <summary>
        ///// Редактирование занятия
        ///// </summary>
        //// TODO: Убрать из параметров weekNumber, он есть в профиле пользователя 
        //[HttpGet]
        //public ActionResult EditLesson(int groupId, int weekNumber, int dayNumber, int classNumber)
        //{
        //    if (!Request.IsAjaxRequest()) return null;

        //    var viewModel = new EditLessonViewModel
        //    {
        //        GroupId = groupId,
        //        WeekNumber = weekNumber,
        //        DayNumber = dayNumber,
        //        ClassNumber = classNumber
        //    };

        //    var lessons = GetLessonViewModel(groupId, weekNumber, dayNumber, classNumber, checkDowntimes: false);

        //    // Типы занятий
        //    viewModel.LessonTypes = UnitOfWork.Repository<LessonType>()
        //        .GetQ(orderBy: o => o.OrderBy(n => n.Order))
        //        .Select(
        //            x =>
        //                new LessonTypeViewModel
        //                {
        //                    LessonTypeId = x.LessonTypeId,
        //                    LessonTypeName = x.LessonTypeName
        //                })
        //        .ToList();

        //    // Корпуса
        //    var housingRepository = UnitOfWork.Repository<Housing>() as HousingRepository;
        //    if (housingRepository != null)
        //    {
        //        var housings = housingRepository.HousingEqualLength();
        //        viewModel.Housings = housings
        //            .Select(
        //                x =>
        //                    new HousingViewModel
        //                    {
        //                        HousingId = x.HousingId,
        //                        HousingName = x.HousingName,
        //                        Abbreviation = x.Abbreviation
        //                    })
        //            .ToList();
        //    }      

        //    foreach (var lesson in lessons)
        //    {                   
        //        // Преподаватели
        //        var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
        //        if (jobRepository != null)
        //        {
        //            var chairTeachers = jobRepository.ActualTeachers(UserProfile.EducationYear, lesson.ChairId);
        //            lesson.ChairTeachers = chairTeachers
        //                .Select(
        //                    x =>
        //                        new TeacherViewModel
        //                        {
        //                            TeacherId = x.JobId,
        //                            TeacherFullName = x.FullName
        //                        })
        //                .ToList();
        //        }                   

        //        // Аудитории
        //        foreach (var lessonPart in lesson.LessonParts)
        //        {
        //            var chairId = lesson.ChairId;
        //            var housingId = lessonPart.HousingId;

        //            var auditoriumRepository = UnitOfWork.Repository<Auditorium>() as AuditoriumRepository;
        //            if (auditoriumRepository != null)
        //            {
        //                var auditoriums = auditoriumRepository.AuditoriumWithEmployment(chairId, housingId, weekNumber, dayNumber, classNumber, groupId);
        //                lessonPart.Auditoriums = auditoriums
        //                    .Select(
        //                        x =>
        //                            new AuditoriumViewModel
        //                            {
        //                                AuditoriumId = x.AuditoriumId,
        //                                AuditoriumNumber = x.AuditoriumNumber,
        //                                AuditoriumTypeName = x.AuditoriumTypeName,
        //                                ChairId = x.ChairId,
        //                                Places = x.Places,
        //                                Comment = x.Comment,
        //                                Employment = x.Employment
        //                            })
        //                            .ToList();
        //            }
        //        }

        //    }

        //    viewModel.Lessons = lessons;

        //    return Json(viewModel, JsonRequestBehavior.AllowGet);
        //}

        //[HttpPost]
        //public ActionResult EditLesson(EditLessonViewModel viewModel)
        //{
        //    if (!Request.IsAjaxRequest())
        //    {
        //        return new HttpStatusCodeResult(404);
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        var allErrors = ViewData.ModelState.Values.SelectMany(modelState => modelState.Errors).ToList();
        //        foreach (ModelError error in allErrors)
        //        {
        //            var message = error.ErrorMessage;
        //        }

        //        return null;
        //    }

        //    // Удаление занятия(ий)
        //    var viewModelLessonIds = viewModel.Lessons.SelectMany(x => x.LessonParts).Select(p => p.LessonId);
        //    var lessonsForDelete = UnitOfWork.Repository<Lesson>()
        //        .Get(x => x.GroupId == viewModel.GroupId && x.WeekNumber == viewModel.WeekNumber
        //                && x.DayNumber == viewModel.DayNumber && x.ClassNumber == viewModel.ClassNumber
        //                && x.DeletedAt == null)
        //        .Where(x => !viewModelLessonIds.Contains(x.LessonId));
        //    foreach (var lesson in lessonsForDelete)
        //    {
        //        lesson.DeletedAt = DateTime.Now;
        //        UnitOfWork.Repository<Lesson>().Update(lesson);
        //        UnitOfWork.Save();

        //        Logger.Info("Занятие помечено как удалённое : LessonId=" + lesson.LessonId);
        //    }

        //    // Создание и обновление занятий
        //    foreach (var lessonViewModel in viewModel.Lessons)
        //    {
        //        foreach (var lessonPartViewModel in lessonViewModel.LessonParts)
        //        {             
        //            var lessonId = lessonPartViewModel.LessonId;
        //            var lesson = UnitOfWork.Repository<Lesson>()
        //                .Get(x => x.LessonId == lessonId)
        //                .SingleOrDefault();

        //            // Обновление занятия
        //            if (lesson != null)
        //            {
        //                lesson.LessonTypeId = lessonViewModel.LessonTypeId;
        //                lesson.DisciplineId = lessonViewModel.DisciplineId;
        //                lesson.AuditoriumId = lessonPartViewModel.AuditoriumId;
        //                lesson.JobId = lessonPartViewModel.TeacherId;
        //                // lesson.IsNotActive = lessonPartViewModel.IsNotActive;
        //                lesson.UpdatedAt = DateTime.Now;

        //                UnitOfWork.Repository<Lesson>().Update(lesson);
        //                UnitOfWork.Save();

        //                Logger.Info("Обновлено занятие : LessonId=" + lesson.LessonId);
        //            }
        //            // Создание нового занятия
        //            else
        //            {
        //                var classDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart,
        //                    viewModel.WeekNumber, viewModel.DayNumber);
        //                lesson = new Lesson
        //                {
        //                    LessonGuid = Guid.NewGuid(),                           
        //                    WeekNumber = viewModel.WeekNumber,
        //                    DayNumber = viewModel.DayNumber,
        //                    ClassNumber = viewModel.ClassNumber,
        //                    ClassDate = classDate,
        //                    GroupId = viewModel.GroupId,
        //                    LessonTypeId = lessonViewModel.LessonTypeId,
        //                    DisciplineId = lessonViewModel.DisciplineId,
        //                    AuditoriumId = lessonPartViewModel.AuditoriumId,
        //                    JobId = lessonPartViewModel.TeacherId,
        //                    // IsNotActive = lessonPartViewModel.IsNotActive,
        //                    CreatedAt = DateTime.Now,
        //                };

        //                UnitOfWork.Repository<Lesson>().Insert(lesson);
        //                UnitOfWork.Save();

        //                Logger.Info("Создано новое занятие : LessonId=" + lesson.LessonId);
        //            }    
        //        }
        //    }

        //    var lessonCell = GetLessonViewModel(viewModel.GroupId, viewModel.WeekNumber, viewModel.DayNumber,
        //        viewModel.ClassNumber);
        //    return PartialView("_LessonCell", lessonCell);
        //}

        ///// <summary>
        ///// Копирование занятия
        ///// </summary>
        //// TODO: Убрать из параметров weekNumber, он есть в профиле пользователя 
        //[HttpPost]
        //public ActionResult CopyLesson(int weekNumber, int sourceGroupId, int sourceDayNumber, int sourceClassNumber,
        //    int targetGroupId, int targetDayNumber, int targetClassNumber)
        //{
        //    var targetLessons = UnitOfWork.Repository<Lesson>()
        //        .Get(x => x.GroupId == targetGroupId && x.WeekNumber == weekNumber
        //            && x.DayNumber == targetDayNumber && x.ClassNumber == targetClassNumber
        //            && x.DeletedAt == null);
        //    foreach (var targetLesson in targetLessons)
        //    {
        //        targetLesson.DeletedAt = DateTime.Now;
        //        UnitOfWork.Repository<Lesson>().Update(targetLesson);
        //        UnitOfWork.Save();
        //    }

        //    var sourceLessons = UnitOfWork.Repository<Lesson>()
        //        .Get(x => x.GroupId == sourceGroupId && x.WeekNumber == weekNumber
        //            && x.DayNumber == sourceDayNumber && x.ClassNumber == sourceClassNumber
        //            && x.DeletedAt == null);
        //    foreach (var sourceLesson in sourceLessons)
        //    {
        //        var targetClassDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart,
        //            weekNumber, targetDayNumber);
        //        var targetLesson = new Lesson
        //        {
        //            AuditoriumId = sourceLesson.AuditoriumId,
        //            DisciplineId = sourceLesson.DisciplineId,
        //            GroupId = targetGroupId,
        //            JobId = sourceLesson.JobId,
        //            LessonTypeId = sourceLesson.LessonTypeId,
        //            WeekNumber = weekNumber,
        //            DayNumber = targetDayNumber,
        //            ClassNumber = targetClassNumber,
        //            ClassDate = targetClassDate,
        //            CreatedAt = DateTime.Now,
        //            LessonGuid = Guid.NewGuid()
        //        };

        //        UnitOfWork.Repository<Lesson>().Insert(targetLesson);
        //        UnitOfWork.Save();

        //        Logger.Info("Скопировано занятие : SourceLessonId={0}, TargetLessonId={1}", sourceLesson.LessonId, targetLesson.LessonId);
        //    }

        //    var lessonCell = GetLessonViewModel(targetGroupId, weekNumber, targetDayNumber, targetClassNumber);
        //    return PartialView("_LessonCell", lessonCell);
        //}

        ///// <summary>
        ///// Удаление занятия
        ///// </summary>
        //// TODO: Убрать из параметров weekNumber, он есть в профиле пользователя 
        //[HttpPost]
        //public ActionResult RemoveLesson(int groupId, int weekNumber, int dayNumber, int classNumber)
        //{
        //    var lessons = UnitOfWork.Repository<Lesson>()
        //        .Get(x => x.GroupId == groupId && x.WeekNumber == weekNumber
        //            && x.DayNumber == dayNumber && x.ClassNumber == classNumber
        //            && x.DeletedAt == null);
        //    foreach (var lesson in lessons)
        //    {
        //        lesson.DeletedAt = DateTime.Now;
        //        UnitOfWork.Repository<Lesson>().Update(lesson);
        //        UnitOfWork.Save();

        //        Logger.Info("Занятие помечено как удалённое : LessonId=" + lesson.LessonId);
        //    }

        //    return null;
        //}

        ///// <summary>
        ///// Возвращает данные для обновления занятий преподавателей
        ///// Например, для того чтобы подсветилась подсказка об окнах в расписании преподавателей
        ///// </summary>
        //[HttpPost]
        //public ActionResult RefreshLesson(int groupId, int dayNumber, int classNumber, int[] teacherIds)
        //{
        //    if (!Request.IsAjaxRequest() || teacherIds == null) return null;

        //    var editableGroups = GetEditableGroups()
        //        .Select(x => x.GroupId);

        //    var teachersPersonIds = UnitOfWork.Repository<Job>()
        //        .GetQ(t => teacherIds.Contains(t.JobId))
        //        .Select(x => x.Employee.PersonId);

        //    var changedLessons = UnitOfWork.Repository<Lesson>()
        //       .GetQ(x => teachersPersonIds.Contains(x.Job.Employee.PersonId) && editableGroups.Contains(x.GroupId)
        //           && x.WeekNumber == UserProfile.WeekNumber && x.DayNumber == dayNumber 
        //           && x.ClassNumber != classNumber && x.DeletedAt == null)
        //       .ToList();

        //    var lessonCellsForRefresh = new List<RefreshCellViewModel>();
        //    foreach (var lesson in changedLessons)
        //    {
        //        var targetGroupId = lesson.GroupId;
        //        var targetClassNumber = lesson.ClassNumber;
        //        var lessonViewModel = GetLessonViewModel(targetGroupId, UserProfile.WeekNumber, dayNumber, targetClassNumber);

        //        var lessonCellContent = RenderPartialToString(this, "_LessonCell", lessonViewModel, ViewData, TempData); 
        //        var lessonCellForRefresh = new RefreshCellViewModel
        //        {
        //            DayNumber = dayNumber,
        //            ClassNumber = targetClassNumber,
        //            GroupId = targetGroupId,
        //            Content = lessonCellContent
        //        };
        //        lessonCellsForRefresh.Add(lessonCellForRefresh);
        //    }

        //    return Json(lessonCellsForRefresh);
        //}

        ///// <summary>
        ///// Выбор потока / курса / группы
        ///// </summary>
        //[HttpGet]
        //public ActionResult SelectFlow()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult SelectFlowData(int? groupSetId)
        //{
        //    if (!Request.IsAjaxRequest())
        //    {
        //        return new HttpStatusCodeResult(404);
        //    }

        //    var viewModel = new SelectFlowViewModel
        //    {
        //        GroupSets = new List<GroupSetViewModel>(),
        //        Groups = new List<GroupViewModel>()
        //    };

        //    if (groupSetId != null)
        //    {
        //        var groupsSet = UserProfile.GroupSets
        //            .SingleOrDefault(x => x.GroupSetId == groupSetId);
        //        if (groupsSet != null)
        //        {
        //            viewModel.GroupSetName = groupsSet.GroupSetName;

        //            var firstGroup = groupsSet.GroupSetGroups.First();
        //            var facultyId = firstGroup.Group.Course.FacultyId;
        //            var educationFormId = firstGroup.Group.ProgramOfEducation.EducationFormId;
        //            var educationLevelId = firstGroup.Group.ProgramOfEducation.EducationLevelId;
        //            var courseNumber = firstGroup.Group.Course.CourseNumber;

        //            viewModel.FacultyId = facultyId;
        //            viewModel.EducationFormId = educationFormId;
        //            viewModel.EducationLevelId = educationLevelId;
        //            viewModel.CourseNumber = courseNumber;

        //            viewModel.CourseNumbers = UnitOfWork.Repository<Course>()
        //                .GetQ(
        //                    filter: x => x.IsDeleted != true && x.FacultyId == facultyId && x.YearStart != null
        //                        && x.Groups.Any(g => g.ProgramOfEducation.EducationFormId == educationFormId
        //                            && g.ProgramOfEducation.EducationLevelId == educationLevelId
        //                            && g.IsDeleted != true)
        //                        && x.YearStart + x.CourseNumber == UserProfile.EducationYear.YearEnd,
        //                    orderBy: o => o.OrderBy(n => n.CourseNumber))
        //                .Select(x => x.CourseNumber)
        //                .Distinct()
        //                .ToList();

        //            var groups = UnitOfWork.Repository<Group>()
        //                .GetQ(
        //                    filter: x => x.IsDeleted != true && x.Course.FacultyId == facultyId
        //                        && x.ProgramOfEducation.EducationFormId == educationFormId 
        //                        && x.ProgramOfEducation.EducationLevelId == educationLevelId &&
        //                        UserProfile.EducationYear.YearStart - x.Course.YearStart + 1 == courseNumber,
        //                    orderBy: o => o.OrderBy(n => n.DivisionName));
        //            foreach (var group in groups)
        //            {
        //                var groupViewModel = new GroupViewModel
        //                {
        //                    GroupId = group.GroupId,
        //                    GroupName = group.DivisionName
        //                };

        //                var groupSetGroup = groupsSet.GroupSetGroups.SingleOrDefault(g => g.GroupId == group.GroupId);
        //                if (groupSetGroup != null)
        //                {
        //                    groupViewModel.IsSelected = true;
        //                    groupViewModel.Order = groupSetGroup.Order;
        //                }

        //                viewModel.Groups.Add(groupViewModel);
        //            }    
        //        }
        //    }
        //    else
        //    {
        //        viewModel.EducationFormId = (int)EducationForms.FullTime;

        //        var groupSets = UserProfile.GroupSets
        //            .Select(x => new GroupSetViewModel
        //            {
        //                GroupSetId = x.GroupSetId,
        //                GroupSetName = x.GroupSetName,
        //                GroupNames = String.Join(", ", x.GroupSetGroups.OrderBy(o => o.Order).Select(g => g.Group.DivisionName))
        //            })
        //            .ToList();
        //        viewModel.GroupSets = groupSets;

        //        if (User.IsInRole("Administrator"))
        //        {
        //            viewModel.Faculties = UnitOfWork.Repository<Faculty>()
        //                .GetQ(f => f.IsDeleted != true, orderBy: o => o.OrderBy(n => n.DivisionName))
        //                .Select(x => new FacultyViewModel { FacultyId = x.FacultyId, FacultyName = x.DivisionName })
        //                .ToList();
        //        }
        //        else
        //        {
        //            viewModel.Faculties = UserProfile.Faculties
        //                .Select(x => new FacultyViewModel { FacultyId = x.FacultyId, FacultyName = x.DivisionName })
        //                .OrderBy(n => n.FacultyName)
        //                .ToList();
        //        }

        //        viewModel.EducationLevels = UnitOfWork.Repository<EducationLevel>()
        //            .Get(x => x.IsDeleted != true)
        //            .Select(x => new EducationLevelViewModel { EducationLevelId = x.EducationLevelId, EducationLevelName = x.EducationLevelName })
        //            .ToList();

        //        viewModel.EducationForms = UnitOfWork.Repository<EducationForm>()
        //            .GetQ(x => x.IsDeleted != true)
        //            .Select(x => new EducationFormViewModel { EducationFormId = x.EducationFormId, EducationFormName = x.EducationFormName })
        //            .ToList();
        //    }

        //    return Json(viewModel);
        //}

        //[HttpPost]
        //public ActionResult SelectFlow(SelectFlowViewModel viewModel)
        //{
        //    var groupSets = UnitOfWork.Repository<GroupSet>()
        //        .Get(x => x.ApplicationUserId == UserProfile.Id)
        //        .ToList();
        //    foreach (var groupSet in groupSets)
        //    {
        //        groupSet.IsSelected = false;
        //        UnitOfWork.Repository<GroupSet>().Update(groupSet);
        //    }

        //    var groupSetGroups = viewModel.Groups
        //        .Select(x => new GroupSetGroup
        //        {
        //            GroupSetId = viewModel.GroupSetId, 
        //            GroupId = x.GroupId, 
        //            Order = x.Order ?? 0
        //        })
        //        .ToList();

        //    var currentGroupSet = groupSets.SingleOrDefault(x => x.GroupSetId == viewModel.GroupSetId);
        //    if (currentGroupSet == null)
        //    {
        //        currentGroupSet = new GroupSet
        //        {
        //            ApplicationUserId = UserProfile.Id,
        //            GroupSetName = viewModel.GroupSetName
        //        };

        //        UnitOfWork.Repository<GroupSet>().Insert(currentGroupSet);
        //    }
        //    else
        //    {
        //        UnitOfWork.Repository<GroupSetGroup>().DeleteRange(x => x.GroupSetId == currentGroupSet.GroupSetId);
        //        UnitOfWork.Repository<GroupSet>().Update(currentGroupSet);
        //    }

        //    currentGroupSet.GroupSetGroups = groupSetGroups;
        //    currentGroupSet.IsSelected = true;
        //    UnitOfWork.Save();

        //    return Json(new { result = "Redirect", url = Url.Action("Index", "Home") });
        //}

        //[HttpPost]
        //public ActionResult EditableWeeks(bool showWeekType = true)
        //{
        //    if (Request.IsAjaxRequest())
        //    {
        //        var groups = GetEditableGroups()
        //            .ToList();

        //        var yearStartDate = UserProfile.EducationYear.DateStart;
        //        var currentWeek = ScheduleHelpers.WeekOfLesson(yearStartDate, DateTime.Now);

        //        var viewModel = new ChangeWeekViewModel
        //        {
        //            EditedWeek = UserProfile.WeekNumber,
        //            EditedWeekStartDate = ScheduleHelpers.DateOfLesson(yearStartDate, UserProfile.WeekNumber, 1).ToString("dd.MM.yyyy"),
        //            EditedWeekEndDate = ScheduleHelpers.DateOfLesson(yearStartDate, UserProfile.WeekNumber, 7).ToString("dd.MM.yyyy"), 
        //            CurrentWeek = currentWeek,
        //            CurrentWeekStartDate = ScheduleHelpers.DateOfLesson(yearStartDate, currentWeek, 1).ToString("dd.MM.yyyy"), 
        //            CurrentWeekEndDate = ScheduleHelpers.DateOfLesson(yearStartDate, currentWeek, 7).ToString("dd.MM.yyyy"), 
        //            Weeks = new List<WeekViewModel>()
        //        };

        //        // Необходим график учебного процесса чтобы узнать количество недель
        //        var courseSchedules = new List<CourseSchedule>();
        //        foreach (var group in groups)
        //        {
        //            var courseNumber = group.Course.CourseNumber;
        //            var academicPlan = group.ProgramOfEducation.AcademicPlans
        //                .OrderByDescending(d => d.UploadedAt)
        //                .FirstOrDefault();

        //            if (academicPlan != null)
        //            {
        //                var courseSchedule = academicPlan.CourseSchedules
        //                    .SingleOrDefault(x => x.CourseNumber == courseNumber);

        //                courseSchedules.Add(courseSchedule);
        //            }
        //        }

        //        // Если для всех групп загружен учебный план
        //        if (groups.Count == courseSchedules.Count && showWeekType)
        //        {
        //            var allEquals = courseSchedules.All(o => o.Schedule == courseSchedules[0].Schedule);
        //            if (allEquals)
        //            {
        //                var courseSchedule = courseSchedules.First();
        //                for (int index = 1; index <= courseSchedule.Schedule.Length; index++)
        //                {
        //                    var weekStartDate = ScheduleHelpers.DateOfLesson(yearStartDate, index, 1);
        //                    var weekEndDate = ScheduleHelpers.DateOfLesson(yearStartDate, index, 7);

        //                    var currentAbbr = courseSchedule.Schedule[index - 1];
        //                    var scheduleType = ScheduleHelpers.ScheduleTypeByAbbr(currentAbbr);

        //                    var week = new WeekViewModel
        //                    {
        //                        WeekNumber = index,
        //                        WeekStartDate = weekStartDate.ToString("dd.MM.yyyy"), 
        //                        WeekEndDate = weekEndDate.ToString("dd.MM.yyyy"), 
        //                        ScheduleTypeName = scheduleType["Name"],
        //                        ScheduleTypeColor = scheduleType["Color"]
        //                    };

        //                    viewModel.Weeks.Add(week);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            int index = 1;
        //            while (true)
        //            {
        //                var weekStartDate = ScheduleHelpers.DateOfLesson(yearStartDate, index, 1);
        //                var weekEndDate = ScheduleHelpers.DateOfLesson(yearStartDate, index, 7);

        //                var weekInEducationYear = DateHelpers.DatesIsActual(UserProfile.EducationYear, weekStartDate, weekEndDate);
        //                if (!weekInEducationYear) break;

        //                var week = new WeekViewModel
        //                {
        //                    WeekNumber = index,
        //                    WeekStartDate = weekStartDate.ToString("dd.MM.yyyy"), 
        //                    WeekEndDate = weekEndDate.ToString("dd.MM.yyyy") 
        //                };

        //                viewModel.Weeks.Add(week);                    

        //                index++;
        //            }
        //        }            

        //        return Json(viewModel);
        //    }

        //    return null;
        //}

        //[HttpGet]
        //public ActionResult ChangeWeek(int weekNumber)
        //{
        //    UserProfile.WeekNumber = weekNumber;
        //    UserManager.Update(UserProfile);

        //    return RedirectToAction("Index");
        //}

        //#region Вспомогательные методы

        /// <summary>
        /// Возвращает список групп, для которых редактируется расписание
        /// </summary>
        public IQueryable<Group> GetEditableGroups()
        {
            var groups = _context.GroupSets
                .Where(x => x.IsSelected && x.ApplicationUserId == UserProfile.Id)
                .SelectMany(x => x.GroupSetGroups)
                .OrderBy(x => x.Order)
                .Select(x => x.Group);

            return groups;
        }

        //// TODO: Убрать из параметров weekNumber, он есть в профиле пользователя
        //public List<LessonViewModel> GetLessonViewModel(int groupId, int weekNumber, int dayNumber, int classNumber, bool checkDowntimes = true)
        //{
        //    var lessons = UnitOfWork.Repository<Lesson>()
        //        .GetQ(x => x.GroupId == groupId && x.WeekNumber == weekNumber
        //            && x.DayNumber == dayNumber && x.ClassNumber == classNumber
        //            && x.DeletedAt == null)
        //        .GroupBy(x => new
        //        {
        //            x.DisciplineId,
        //            x.Discipline.DisciplineName,
        //            x.Discipline.ChairId,
        //            x.Discipline.Chair.DivisionName,
        //            x.LessonTypeId
        //        })
        //        .Select(x => new LessonViewModel
        //        {
        //            DisciplineId = x.Key.DisciplineId,
        //            DisciplineName = x.Key.DisciplineName,
        //            ChairId = x.Key.ChairId,
        //            ChairName = x.Key.DivisionName,
        //            LessonTypeId = x.Key.LessonTypeId ?? 0,
        //            LessonParts = x
        //                .Select(y => new LessonPartViewModel
        //                {
        //                    LessonId = y.LessonId,
        //                    HousingId = y.Auditorium.HousingId,
        //                    AuditoriumId = y.AuditoriumId,
        //                    TeacherId = y.JobId,
        //                    AuditoriumName = y.Auditorium.AuditoriumNumber + y.Auditorium.Housing.Abbreviation + ".",
        //                    TeacherLastName = y.Job.Employee.Person.LastName,
        //                    TeacherFirstName = y.Job.Employee.Person.FirstName,
        //                    TeacherMiddleName = y.Job.Employee.Person.MiddleName,
        //                    // IsNotActive = y.IsNotActive
        //                })
        //        })
        //        .ToList();

        //    // Проверка на окна у преподавателей
        //    if (checkDowntimes)
        //    {
        //        var lessonTeacherIds = lessons
        //            .SelectMany(x => x.LessonParts)
        //            .Select(p => p.TeacherId)
        //            .Distinct();

        //        var jobRepository = UnitOfWork.Repository<Job>() as JobRepository;
        //        if (jobRepository != null)
        //        {
        //            foreach (var teacherId in lessonTeacherIds)
        //            {
        //                var teacherDowntime = jobRepository.TeachersDowntime(UserProfile.WeekNumber, teacherId, maxDiff: 2)
        //                    .Where(x => x.DayNumber == dayNumber && x.ClassNumber == classNumber)
        //                    .Distinct();

        //                if (teacherDowntime.Any())
        //                {
        //                    lessons.SelectMany(x => x.LessonParts)
        //                       .Where(p => p.TeacherId == teacherId)
        //                       .All(c => { c.TeacherHasDowntime = true; return true; });
        //                }
        //            }
        //        }   
        //    }        

        //    return lessons;
        //}

        //public static string RenderPartialToString(Controller controller, string partialViewName, object model, ViewDataDictionary viewData, TempDataDictionary tempData)
        //{
        //    ViewEngineResult result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName);

        //    if (result.View != null)
        //    {
        //        controller.ViewData.Model = model;
        //        StringBuilder sb = new StringBuilder();
        //        using (StringWriter sw = new StringWriter(sb))
        //        {
        //            using (HtmlTextWriter output = new HtmlTextWriter(sw))
        //            {
        //                ViewContext viewContext = new ViewContext(controller.ControllerContext, result.View, viewData, tempData, output);
        //                result.View.Render(viewContext, output);
        //            }
        //        }

        //        return sb.ToString();
        //    }

        //    return String.Empty;
        //}

        //#endregion
    }
}