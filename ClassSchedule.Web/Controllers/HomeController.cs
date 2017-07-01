using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Business.Models.Schedule;
using ClassSchedule.Business.Models.SelectFlow;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.Models;
using System.Data.Entity;
using ClassSchedule.Business.Models;
using ClassSchedule.Web.Helpers;
using ClassSchedule.Business.Models.ChangeWeek;
using ClassSchedule.Domain.Helpers;
using Microsoft.AspNet.Identity;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Net;

namespace ClassSchedule.Web.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly ILessonService _lessonService;
        private readonly IJobService _jobService;
        private readonly IGroupService _groupService;
        private readonly IDictionaryService _dictionaryService;

        public HomeController(ApplicationDbContext context, ILessonService lessonService
            , IJobService jobService, IGroupService groupService, IDictionaryService dictionaryService)
        {
            _context = context;
            _lessonService = lessonService;
            _jobService = jobService;
            _groupService = groupService;
            _dictionaryService = dictionaryService;
        }

        /// <summary>
        /// Показ расписания на неделю
        /// </summary>
        public ActionResult Index()
        {
            var viewModel = new ScheduleViewModel
            {
                GroupLessons = _lessonService.GetScheduleForGroups(UserProfile.Id),
                WeekNumber = UserProfile.WeekNumber,
                FirstDayOfWeek = UserProfile.FirstDayOfWeek,
                LastDayOfWeek = UserProfile.LastDayOfWeek
            };

            // Окна между занятиями у преподавателей
            var downtimes = _jobService.TeachersDowntime(UserProfile.WeekNumber, maxDiff: 2);
            foreach (var downtime in downtimes)
            {
                var groupId = downtime.GroupId;
                var dayNumber = downtime.DayNumber;
                var classNumber = downtime.ClassNumber;
                // var jobId = downtime.JobId;
                var plannedChairJobId = downtime.PlannedChairJobId;

                viewModel.GroupLessons
                    .Where(g => g.GroupId == groupId)
                    .SelectMany(g => g.Lessons)
                    .Where(x => x.DayNumber == dayNumber && x.ClassNumber == classNumber)
                    .SelectMany(x => x.LessonDetails)
                    .Where(p => p.PlannedChairJobId == plannedChairJobId) 
                    .All(c => { c.TeacherHasDowntime = true; return true; });
            }

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
            viewModel.LessonTypes = _dictionaryService.GetLessonTypes();

            // Корпуса
            viewModel.Housings = _dictionaryService.GetHousingEqualLength();

            if (lessons != null)
            {
                foreach (var lesson in lessons)
                {
                    lesson.ChairTeachers = _jobService.ActualTeachers(UserProfile.EducationYearId ?? 0, lesson.ChairId);
 
                    // Аудитории
                    foreach (var lessonDetail in lesson.LessonDetails)
                    {
                        var chairId = lesson.ChairId;
                        var housingId = lessonDetail.HousingId;

                        lessonDetail.Auditoriums = _dictionaryService.GetAuditoriumWithEmployment(chairId, housingId, weekNumber, dayNumber, classNumber, groupId);
                    }
                }
            }
            else
            {
                lessons = new List<LessonViewModel>();
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

            var scheduleState = EntityState.Modified;
            var schedule = _context.Schedule
                .Include(x => x.Lessons.Select(s => s.LessonDetails))
                .SingleOrDefault(x => x.ScheduleId == viewModel.ScheduleId && x.DeletedAt == null);

            try
            {
                if (schedule == null)
                {
                    schedule = new Schedule
                    {
                        GroupId = viewModel.GroupId,
                        EducationYearId = UserProfile.EducationYearId,
                        WeekNumber = UserProfile.WeekNumber,
                        DayNumber = viewModel.DayNumber,
                        ClassNumber = viewModel.ClassNumber,
                        ClassDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart,
                            viewModel.WeekNumber, viewModel.DayNumber),
                        Lessons = new List<Lesson> { new Lesson { LessonDetails = new List<LessonDetail>() } }
                    };

                    scheduleState = EntityState.Added;
                }

                // Создание и обновление занятий
                foreach (var lessonViewModel in viewModel.Lessons)
                {
                    var lesson = schedule.Lessons
                        .Where(x => x.LessonId == lessonViewModel.LessonId && x.LessonId != 0 && x.DeletedAt == null)
                        .SingleOrDefault();
                    if (lesson == null)
                    {
                        lesson = new Lesson
                        {
                            DisciplineId = lessonViewModel.DisciplineId,
                            LessonTypeId = lessonViewModel.LessonTypeId,
                            LessonDetails = lessonViewModel.LessonDetails
                                .Select(s => new LessonDetail
                                {
                                    AuditoriumId = s.AuditoriumId,
                                    PlannedChairJobId = s.PlannedChairJobId,
                                    Order = s.Order ?? 0
                                })
                                .ToList()
                        };

                        schedule.Lessons.Add(lesson);
                    }
                    else
                    {
                        lesson.LessonTypeId = lessonViewModel.LessonTypeId;
                        lesson.DisciplineId = lessonViewModel.DisciplineId;
                        lesson.Order = lessonViewModel.Order;
                        lesson.UpdatedAt = DateTime.Now;

                        foreach (var lessonDetailViewModel in lessonViewModel.LessonDetails)
                        {
                            var lessonDetail = lesson.LessonDetails
                                .SingleOrDefault(x => x.LessonDetailId == lessonDetailViewModel.LessonDetailId
                                    && x.LessonDetailId != 0 && x.DeletedAt == null);
                            if (lessonDetail == null)
                            {
                                lessonDetail = new LessonDetail
                                {
                                    AuditoriumId = lessonDetailViewModel.AuditoriumId,
                                    PlannedChairJobId = lessonDetailViewModel.PlannedChairJobId,
                                    Order = lessonDetailViewModel.Order
                                };

                                lesson.LessonDetails.Add(lessonDetail);
                            }
                            else
                            {
                                lessonDetail.AuditoriumId = lessonDetailViewModel.AuditoriumId;
                                lessonDetail.PlannedChairJobId = lessonDetailViewModel.PlannedChairJobId;
                                lessonDetail.Order = lessonDetailViewModel.Order;
                                lessonDetail.UpdatedAt = DateTime.Now;
                            }
                        }
                    }
                }

                // Удаление занятия(ий)
                var viewModelLessonIds = viewModel.Lessons.Select(x => x.LessonId);
                var lessonsForDelete = _context.Lessons
                    .Where(x => x.Schedule.EducationYearId == UserProfile.EducationYearId 
                        && x.Schedule.GroupId == viewModel.GroupId && x.Schedule.WeekNumber == viewModel.WeekNumber
                        && x.Schedule.DayNumber == viewModel.DayNumber && x.Schedule.ClassNumber == viewModel.ClassNumber
                        && x.DeletedAt == null)
                    .Where(x => !viewModelLessonIds.Contains(x.LessonId));
                foreach (var lesson in lessonsForDelete)
                {
                    lesson.DeletedAt = DateTime.Now;

                    Logger.Info("Занятие помечено как удалённое : LessonId=" + lesson.LessonId);
                }

                if (scheduleState == EntityState.Added)
                {
                    _context.Schedule.Add(schedule);
                }

                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Произошла ошибка при сохранении занятия.");

                return new JsonErrorResult(HttpStatusCode.InternalServerError) { Data = string.Format("ErrorMessage : {0}, StackTrace: {1}", ex.Message, ex.StackTrace) };
            }        

            if (scheduleState == EntityState.Added)
            {
                // Logger.Info("Создано новое занятие : LessonId=" + lesson.LessonId);
            }
            else
            {
                // Logger.Info("Обновлено занятие : LessonId=" + lesson.LessonId);
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
            var targetLessons = _context.Lessons
                .Where(x => x.Schedule.GroupId == targetGroupId && x.Schedule.WeekNumber == weekNumber
                    && x.Schedule.DayNumber == targetDayNumber && x.Schedule.ClassNumber == targetClassNumber
                    && x.DeletedAt == null);
            foreach (var targetLesson in targetLessons)
            {
                targetLesson.DeletedAt = DateTime.Now;
                _context.SaveChanges();
            }

            var sourceLessons = _context.Lessons
                .Where(x => x.Schedule.GroupId == sourceGroupId && x.Schedule.WeekNumber == weekNumber
                    && x.Schedule.DayNumber == sourceDayNumber && x.Schedule.ClassNumber == sourceClassNumber
                    && x.DeletedAt == null);
            foreach (var sourceLesson in sourceLessons)
            {
                var targetClassDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart,
                    weekNumber, targetDayNumber);
                var targetLesson = new Lesson
                {
                    // AuditoriumId = sourceLesson.AuditoriumId,
                    DisciplineId = sourceLesson.DisciplineId,
                    // GroupId = targetGroupId,
                    // JobId = sourceLesson.JobId,
                    LessonTypeId = sourceLesson.LessonTypeId,
                    // WeekNumber = weekNumber,
                    // DayNumber = targetDayNumber,
                    // ClassNumber = targetClassNumber,
                    // ClassDate = targetClassDate,
                    CreatedAt = DateTime.Now,
                    LessonGuid = Guid.NewGuid()
                };

                _context.Lessons.Add(targetLesson);
                _context.SaveChanges();

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
            var lessons = _context.Lessons
                .Where(x => x.Schedule.GroupId == groupId && x.Schedule.WeekNumber == weekNumber
                    && x.Schedule.DayNumber == dayNumber && x.Schedule.ClassNumber == classNumber
                    && x.DeletedAt == null);
            foreach (var lesson in lessons)
            {
                lesson.DeletedAt = DateTime.Now;
                _context.SaveChanges();

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

            var teachersPersonIds = _context.Jobs
                .Where(t => teacherIds.Contains(t.JobId))
                .Select(x => x.Employee.PersonId);

            //var changedLessons = _context.Lessons
            //   .Where(x => teachersPersonIds.Contains(x.Job.Employee.PersonId) && editableGroups.Contains(x.Schedule.GroupId)
            //       && x.Schedule.WeekNumber == UserProfile.WeekNumber && x.Schedule.DayNumber == dayNumber
            //       && x.Schedule.ClassNumber != classNumber && x.DeletedAt == null)
            //   .ToList();

            var lessonCellsForRefresh = new List<RefreshCellViewModel>();
            //foreach (var lesson in changedLessons)
            //{
            //    var targetGroupId = lesson.GroupId;
            //    var targetClassNumber = lesson.ClassNumber;
            //    var lessonViewModel = GetLessonViewModel(targetGroupId, UserProfile.WeekNumber, dayNumber, targetClassNumber);

            //    var lessonCellContent = RenderPartialToString(this, "_LessonCell", lessonViewModel, ViewData, TempData);
            //    var lessonCellForRefresh = new RefreshCellViewModel
            //    {
            //        DayNumber = dayNumber,
            //        ClassNumber = targetClassNumber,
            //        GroupId = targetGroupId,
            //        Content = lessonCellContent
            //    };
            //    lessonCellsForRefresh.Add(lessonCellForRefresh);
            //}

            return Json(lessonCellsForRefresh);
        }

        /// <summary>
        /// Выбор потока / курса / группы
        /// </summary>
        [HttpGet]
        public ActionResult SelectFlow()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SelectFlowData(int? groupSetId)
        {
            if (!Request.IsAjaxRequest())
            {
                return new HttpStatusCodeResult(404);
            }

            var viewModel = new SelectFlowViewModel
            {
                GroupSets = new List<GroupSetViewModel>(),
                Groups = new List<GroupViewModel>()
            };

            if (groupSetId != null)
            {
                var groupsSet = _context.GroupSets
                    .Include(x => x.GroupSetGroups.Select(g => g.Group.Course))
                    .Include(x => x.GroupSetGroups.Select(g => g.Group.BaseProgramOfEducation))
                    .SingleOrDefault(x => x.GroupSetId == groupSetId);
                if (groupsSet != null)
                {
                    viewModel.GroupSetName = groupsSet.GroupSetName;

                    var firstGroup = groupsSet.GroupSetGroups.First();
                    var facultyId = firstGroup.Group.Course.FacultyId;
                    var educationFormId = firstGroup.Group.BaseProgramOfEducation.EducationFormId;
                    var educationLevelId = firstGroup.Group.BaseProgramOfEducation.EducationLevelId;
                    var courseNumber = firstGroup.Group.Course.CourseNumber;

                    viewModel.FacultyId = facultyId;
                    viewModel.EducationFormId = educationFormId;
                    viewModel.EducationLevelId = educationLevelId;
                    viewModel.CourseNumber = courseNumber ?? 0;

                    viewModel.CourseNumbers = _context.Courses 
                        .Where(x => x.DeletedAt == null && x.FacultyId == facultyId && x.YearStart != null
                            && x.Groups.Any(g => g.BaseProgramOfEducation.EducationFormId == educationFormId
                                && g.BaseProgramOfEducation.EducationLevelId == educationLevelId
                                && g.DeletedAt == null)
                            && x.YearStart + x.CourseNumber == UserProfile.EducationYear.YearEnd)
                        .OrderBy(x => x.CourseNumber)
                        .Select(x => x.CourseNumber ?? 0)
                        .Distinct()
                        .ToList();

                    var groups = _context.Groups
                        .Where(x => x.DeletedAt == null && x.Course.FacultyId == facultyId
                            && x.BaseProgramOfEducation.EducationFormId == educationFormId
                            && x.BaseProgramOfEducation.EducationLevelId == educationLevelId 
                            && UserProfile.EducationYear.YearStart - x.Course.YearStart + 1 == courseNumber)
                        .OrderBy(n => n.GroupName);
                    foreach (var group in groups)
                    {
                        var groupViewModel = new GroupViewModel
                        {
                            GroupId = group.GroupId,
                            GroupName = group.GroupName
                        };

                        var groupSetGroup = groupsSet.GroupSetGroups.SingleOrDefault(g => g.GroupId == group.GroupId);
                        if (groupSetGroup != null)
                        {
                            groupViewModel.IsSelected = true;
                            groupViewModel.Order = groupSetGroup.Order;
                        }

                        viewModel.Groups.Add(groupViewModel);
                    }
                }
            }
            else
            {
                viewModel.EducationFormId = (int)EducationForms.FullTime;

                var groupSets = UserProfile.GroupSets
                    .Select(x => new GroupSetViewModel
                    {
                        GroupSetId = x.GroupSetId,
                        GroupSetName = x.GroupSetName,
                        GroupNames = String.Join(", ", x.GroupSetGroups.OrderBy(o => o.Order).Select(g => g.Group.GroupName))
                    })
                    .ToList();
                viewModel.GroupSets = groupSets;

                if (User.IsInRole("Administrator"))
                {
                    viewModel.Faculties = _context.Faculties
                        .Where(f => f.DeletedAt == null)
                        .OrderBy(n => n.DivisionName)
                        .Select(x => new FacultyViewModel { FacultyId = x.FacultyId, FacultyName = x.DivisionName })
                        .ToList();
                }
                else
                {
                    viewModel.Faculties = UserProfile.Faculties
                        .Select(x => new FacultyViewModel { FacultyId = x.FacultyId, FacultyName = x.DivisionName })
                        .OrderBy(n => n.FacultyName)
                        .ToList();
                }

                viewModel.EducationLevels = _context.EducationLevels
                    .Where(x => x.DeletedAt == null)
                    .Select(x => new EducationLevelViewModel { EducationLevelId = x.EducationLevelId, EducationLevelName = x.EducationLevelName })
                    .ToList();

                viewModel.EducationForms = _context.EducationForms
                    .Where(x => x.DeletedAt == null)
                    .Select(x => new EducationFormViewModel { EducationFormId = x.EducationFormId, EducationFormName = x.EducationFormName })
                    .ToList();
            }

            return Json(viewModel);
        }

        [HttpPost]
        public ActionResult SelectFlow(SelectFlowViewModel viewModel)
        {
            var groupSets = _context.GroupSets
                .Where(x => x.ApplicationUserId == UserProfile.Id)
                .ToList();
            foreach (var groupSet in groupSets)
            {
                groupSet.IsSelected = false;
            }

            var groupSetGroups = viewModel.Groups
                .Select(x => new GroupSetGroup
                {
                    GroupSetId = viewModel.GroupSetId,
                    GroupId = x.GroupId,
                    Order = x.Order ?? 0
                })
                .ToList();

            var currentGroupSet = groupSets.SingleOrDefault(x => x.GroupSetId == viewModel.GroupSetId);
            if (currentGroupSet == null)
            {
                currentGroupSet = new GroupSet
                {
                    ApplicationUserId = UserProfile.Id,
                    GroupSetName = viewModel.GroupSetName
                };

                _context.GroupSets.Add(currentGroupSet);
            }
            else
            {
                var groupSetGroupsForRemove = _context.GroupSetGroups.Where(x => x.GroupSetId == currentGroupSet.GroupSetId);
                _context.GroupSetGroups.RemoveRange(groupSetGroupsForRemove);
            }

            currentGroupSet.GroupSetGroups = groupSetGroups;
            currentGroupSet.IsSelected = true;

            _context.SaveChanges();

            return Json(new { result = "Redirect", url = Url.Action("Index", "Home") });
        }

        [HttpPost]
        public ActionResult EditableWeeks(bool showWeekType = true)
        {
            if (Request.IsAjaxRequest())
            {
                var groups = _groupService.GetEditableGroups(UserProfile.Id);

                var yearStartDate = UserProfile.EducationYear.DateStart;
                var currentWeek = ScheduleHelpers.WeekOfLesson(yearStartDate, DateTime.Now);

                var viewModel = new ChangeWeekViewModel
                {
                    EditedWeek = UserProfile.WeekNumber,
                    EditedWeekStartDate = ScheduleHelpers.DateOfLesson(yearStartDate, UserProfile.WeekNumber, 1).ToString("dd.MM.yyyy"),
                    EditedWeekEndDate = ScheduleHelpers.DateOfLesson(yearStartDate, UserProfile.WeekNumber, 7).ToString("dd.MM.yyyy"),
                    CurrentWeek = currentWeek,
                    CurrentWeekStartDate = ScheduleHelpers.DateOfLesson(yearStartDate, currentWeek, 1).ToString("dd.MM.yyyy"),
                    CurrentWeekEndDate = ScheduleHelpers.DateOfLesson(yearStartDate, currentWeek, 7).ToString("dd.MM.yyyy"),
                    Weeks = new List<WeekViewModel>()
                };

                // Необходим график учебного процесса чтобы узнать количество недель
                var courseSchedules = new List<CourseSchedule>();
                foreach (var group in groups)
                {
                    var courseNumber = group.Course.CourseNumber;
                    //var academicPlan = group.BaseProgramOfEducation.AcademicPlans
                    //    .OrderByDescending(d => d.UploadedAt)
                    //    .FirstOrDefault();

                    //if (academicPlan != null)
                    //{
                    //    var courseSchedule = academicPlan.CourseSchedules
                    //        .SingleOrDefault(x => x.CourseNumber == courseNumber);

                    //    courseSchedules.Add(courseSchedule);
                    //}
                }

                // Если для всех групп загружен учебный план
                //if (groups.Count == courseSchedules.Count && showWeekType)
                //{
                //    var allEquals = courseSchedules.All(o => o.Schedule == courseSchedules[0].Schedule);
                //    if (allEquals)
                //    {
                //        var courseSchedule = courseSchedules.First();
                //        for (int index = 1; index <= courseSchedule.Schedule.Length; index++)
                //        {
                //            var weekStartDate = ScheduleHelpers.DateOfLesson(yearStartDate, index, 1);
                //            var weekEndDate = ScheduleHelpers.DateOfLesson(yearStartDate, index, 7);

                //            var currentAbbr = courseSchedule.Schedule[index - 1];
                //            var scheduleType = ScheduleHelpers.ScheduleTypeByAbbr(currentAbbr);

                //            var week = new WeekViewModel
                //            {
                //                WeekNumber = index,
                //                WeekStartDate = weekStartDate.ToString("dd.MM.yyyy"),
                //                WeekEndDate = weekEndDate.ToString("dd.MM.yyyy"),
                //                ScheduleTypeName = scheduleType["Name"],
                //                ScheduleTypeColor = scheduleType["Color"]
                //            };

                //            viewModel.Weeks.Add(week);
                //        }
                //    }
                //}
                //else
                //{
                    int index = 1;
                    while (true)
                    {
                        var weekStartDate = ScheduleHelpers.DateOfLesson(yearStartDate, index, 1);
                        var weekEndDate = ScheduleHelpers.DateOfLesson(yearStartDate, index, 7);

                        var weekInEducationYear = DateHelpers.DatesIsActual(UserProfile.EducationYear, weekStartDate, weekEndDate);
                        if (!weekInEducationYear) break;

                        var week = new WeekViewModel
                        {
                            WeekNumber = index,
                            WeekStartDate = weekStartDate.ToString("dd.MM.yyyy"),
                            WeekEndDate = weekEndDate.ToString("dd.MM.yyyy")
                        };

                        viewModel.Weeks.Add(week);

                        index++;
                    }
                //}

                return Json(viewModel);
            }

            return null;
        }

        [HttpGet]
        public ActionResult ChangeWeek(int weekNumber)
        {
            UserProfile.WeekNumber = weekNumber;
            UserManager.Update(UserProfile);

            return RedirectToAction("Index");
        }

        #region Вспомогательные методы

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

        // TODO: Убрать из параметров weekNumber, он есть в профиле пользователя
        public List<LessonViewModel> GetLessonViewModel(int groupId, int weekNumber, int dayNumber, int classNumber, bool checkDowntimes = true)
        {
            var schedule = _context.Schedule
                .Where(x => x.EducationYearId == UserProfile.EducationYearId 
                    && x.GroupId == groupId && x.WeekNumber == weekNumber
                    && x.DayNumber == dayNumber && x.ClassNumber == classNumber
                    && x.DeletedAt == null)
                .SingleOrDefault();
            if (schedule == null)
            {
                return null;
            }

            var lessons = _context.Lessons
                .Where(x => x.ScheduleId == schedule.ScheduleId && x.DeletedAt == null)
                .GroupBy(x => new
                {
                    x.DisciplineId,
                    x.Discipline.DisciplineName.Name,
                    x.Discipline.ChairId,
                    x.Discipline.Chair.DivisionName,
                    x.LessonTypeId
                })
                .Select(x => new LessonViewModel
                {
                    DisciplineId = x.Key.DisciplineId,
                    DisciplineName = x.Key.Name,
                    ChairId = x.Key.ChairId,
                    ChairName = x.Key.DivisionName,
                    LessonTypeId = x.Key.LessonTypeId ?? 0,
                    LessonDetails = x.SelectMany(d => d.LessonDetails)
                        .Select(y => new LessonDetailViewModel
                        {
                            LessonId = y.LessonId,
                            AuditoriumId = y.AuditoriumId,
                            AuditoriumName = y.Auditorium.AuditoriumNumber + y.Auditorium.Housing.Abbreviation + ".",
                            HousingId = y.Auditorium.HousingId,
                            PlannedChairJobId = y.PlannedChairJobId ?? 0,
                            TeacherLastName = y.PlannedChairJob != null
                                    && y.PlannedChairJob.Job != null
                                    && y.PlannedChairJob.Job.Employee != null
                                    && y.PlannedChairJob.Job.Employee.Person != null
                                ? y.PlannedChairJob.Job.Employee.Person.LastName : "",
                            TeacherFirstName = y.PlannedChairJob != null
                                    && y.PlannedChairJob.Job != null
                                    && y.PlannedChairJob.Job.Employee != null
                                    && y.PlannedChairJob.Job.Employee.Person != null
                                ? y.PlannedChairJob.Job.Employee.Person.FirstName : "",
                            TeacherMiddleName = y.PlannedChairJob != null
                                    && y.PlannedChairJob.Job != null
                                    && y.PlannedChairJob.Job.Employee != null
                                    && y.PlannedChairJob.Job.Employee.Person != null
                                ? y.PlannedChairJob.Job.Employee.Person.MiddleName : ""
                        })
                })
                .ToList();

            // Проверка на окна у преподавателей
            if (checkDowntimes)
            {
                var lessonTeacherIds = lessons
                    .SelectMany(x => x.LessonDetails)
                    .Select(p => p.PlannedChairJobId) // PlannedChairJobId !!!!!!!!!!!!!!!!!!!!!!!!!!
                    .Distinct();

                foreach (var teacherId in lessonTeacherIds)
                {
                    var teacherDowntime = _jobService.TeachersDowntime(UserProfile.WeekNumber, teacherId, maxDiff: 2)
                        .Where(x => x.DayNumber == dayNumber && x.ClassNumber == classNumber)
                        .Distinct();

                    if (teacherDowntime.Any())
                    {
                        lessons.SelectMany(x => x.LessonDetails)
                           .Where(p => p.PlannedChairJobId == teacherId) // PlannedChairJobId !!!!!!!!!!!!!!!!!!!!!!!!!!
                           .All(c => { c.TeacherHasDowntime = true; return true; });
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