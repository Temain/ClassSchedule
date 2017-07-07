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
            var groupsIdentifiers = _groupService.GetEditableGroupsIdentifiers(UserProfile.Id);
            var schedule = _lessonService.GetScheduleForGroups(groupsIdentifiers, UserProfile.EducationYearId, UserProfile.WeekNumber, checkDowntimes: true);

            var viewModel = new ShowScheduleViewModel
            {
                WeekNumber = UserProfile.WeekNumber,
                FirstDayOfWeek = UserProfile.FirstDayOfWeek,
                LastDayOfWeek = UserProfile.LastDayOfWeek,
                NumberOfGroups = groupsIdentifiers.Length,
                Schedule = schedule
            };

            return View(viewModel);
        }

        /// <summary>
        /// Редактирование занятия
        /// </summary>
        [HttpGet]
        public ActionResult EditLesson(int groupId, int dayNumber, int classNumber)
        {
            if (!Request.IsAjaxRequest()) return null;

            var schedule = _lessonService
                .GetScheduleForGroups(new int[] { groupId }, UserProfile.EducationYearId, UserProfile.WeekNumber, dayNumber, classNumber, checkDowntimes: false)
                .SingleOrDefault();
            if (schedule != null)
            {
                if (schedule.Lessons == null)
                {
                    schedule.Lessons = new List<LessonViewModel>();
                }

                schedule.Disciplines = _dictionaryService.GetDisciplines(groupId, educationYearId: UserProfile.EducationYearId);
                schedule.LessonTypes = _dictionaryService.GetLessonTypes();
                schedule.Housings = _dictionaryService.GetHousingEqualLength();

                foreach (var lesson in schedule.Lessons)
                {
                    lesson.ChairTeachers = _dictionaryService.GetTeacherWithEmployment(UserProfile.EducationYearId, UserProfile.WeekNumber, dayNumber, classNumber, groupId, lesson.DisciplineId, lesson.ChairId);
 
                    foreach (var lessonDetail in lesson.LessonDetails)
                    {
                        var chairId = lesson.ChairId;
                        var housingId = lessonDetail.HousingId;

                        lessonDetail.Auditoriums = _dictionaryService.GetAuditoriumWithEmployment(housingId, UserProfile.WeekNumber, dayNumber, classNumber, groupId, chairId);
                    }
                }
            }
            else
            {
                schedule = new ScheduleViewModel
                {
                    GroupId = groupId,
                    WeekNumber = UserProfile.WeekNumber,
                    DayNumber = dayNumber,
                    ClassNumber = classNumber,
                    Lessons = new List<LessonViewModel> { new LessonViewModel() }
                };
            }           

            return Json(schedule, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult EditLesson(EditLessonViewModel viewModel)
        {
            if (!Request.IsAjaxRequest()) return new HttpNotFoundResult();

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                return new JsonErrorResult(HttpStatusCode.BadRequest) { Data = errors };
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
                        Lessons = new List<Lesson> { }
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
                var lessons = _context.Lessons
                    .Include(x => x.Schedule)
                    .Include(x => x.LessonDetails)
                    .Where(x => x.Schedule.EducationYearId == UserProfile.EducationYearId
                        && x.Schedule.GroupId == viewModel.GroupId && x.Schedule.WeekNumber == viewModel.WeekNumber
                        && x.Schedule.DayNumber == viewModel.DayNumber && x.Schedule.ClassNumber == viewModel.ClassNumber
                        && x.DeletedAt == null && x.Schedule.DeletedAt == null);
                foreach (var lesson in lessons)
                {
                    if (lesson.LessonId != 0)
                    {
                        var lessonViewModel = viewModel.Lessons.SingleOrDefault(x => x.LessonId == lesson.LessonId);
                        if (lessonViewModel == null)
                        {
                            lesson.DeletedAt = DateTime.Now;

                            foreach (var lessonDetail in lesson.LessonDetails)
                            {
                                lessonDetail.DeletedAt = DateTime.Now;
                            }
                        }
                        else
                        {
                            foreach (var lessonDetail in lesson.LessonDetails)
                            {
                                if (lessonDetail.LessonDetailId != 0)
                                {
                                    var lessonDetailViewModel = lessonViewModel.LessonDetails.SingleOrDefault(x => x.LessonDetailId == lessonDetail.LessonDetailId);
                                    if (lessonDetailViewModel == null)
                                    {
                                        lessonDetail.DeletedAt = DateTime.Now;
                                    }
                                }
                            }
                        }
                    }

                    // Logger.Info("Занятие помечено как удалённое : LessonId=" + lesson.LessonId);
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

            var scheduleViewModel = _lessonService
               .GetScheduleForGroups(new int[] { viewModel.GroupId }, UserProfile.EducationYearId, UserProfile.WeekNumber, viewModel.DayNumber, viewModel.ClassNumber, checkDowntimes: true)
               .SingleOrDefault();
            if (scheduleViewModel != null)
            {
                return PartialView("_LessonCell", scheduleViewModel.Lessons);
            }

            return null;
        }

        /// <summary>
        /// Копирование занятия
        /// </summary>
        [HttpPost]
        public ActionResult CopyLesson(int sourceGroupId, int sourceDayNumber, int sourceClassNumber,
            int targetGroupId, int targetDayNumber, int targetClassNumber)
        {
            // Чистка старого занятия
            var targetSchedule = _context.Schedule
                .Include(x => x.Lessons.Select(l => l.LessonDetails))
                .Where(x => x.EducationYearId == UserProfile.EducationYearId && x.GroupId == targetGroupId
                    && x.WeekNumber == UserProfile.WeekNumber && x.DayNumber == targetDayNumber && x.ClassNumber == targetClassNumber
                    && x.DeletedAt == null)
                .SingleOrDefault();
            if (targetSchedule != null)
            {
                var targetLessons = targetSchedule.Lessons.Where(x => x.DeletedAt == null);
                foreach (var targetLesson in targetLessons)
                {
                    targetLesson.DeletedAt = DateTime.Now;

                    var targetLessonDetails = targetLesson.LessonDetails.Where(x => x.DeletedAt == null);
                    foreach (var targetLessonDetail in targetLessonDetails)
                    {
                        targetLessonDetail.DeletedAt = DateTime.Now;
                    }

                    _context.SaveChanges();
                }
            }
            else
            {
                targetSchedule = new Schedule
                {
                    EducationYearId = UserProfile.EducationYearId,
                    GroupId = targetGroupId,
                    WeekNumber = UserProfile.WeekNumber,
                    DayNumber = targetDayNumber,
                    ClassNumber = targetClassNumber,
                    ClassDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart, UserProfile.WeekNumber, targetDayNumber),
                    Lessons = new List<Lesson>()
                };

                _context.Schedule.Add(targetSchedule);
            }

            // Копирование
            var sourceSchedule = _context.Schedule
                .Include(x => x.Lessons.Select(l => l.LessonDetails))
                .Where(x => x.EducationYearId == UserProfile.EducationYearId && x.GroupId == sourceGroupId
                    && x.WeekNumber == UserProfile.WeekNumber && x.DayNumber == sourceDayNumber && x.ClassNumber == sourceClassNumber
                    && x.DeletedAt == null)
                .SingleOrDefault();
            if (sourceSchedule != null)
            {
                foreach (var sourceLesson in sourceSchedule.Lessons.Where(x => x.DeletedAt == null))
                {
                    var targetLesson = new Lesson
                    {
                        DisciplineId = sourceLesson.DisciplineId,
                        LessonTypeId = sourceLesson.LessonTypeId,
                        Order = sourceLesson.Order,
                        LessonDetails = sourceLesson.LessonDetails
                            .Where(x => x.DeletedAt == null)
                            .Select(d => new LessonDetail
                            {
                                PlannedChairJobId = d.PlannedChairJobId,
                                AuditoriumId = d.AuditoriumId,
                                Order = d.Order
                            })
                            .ToList()
                    };

                    targetSchedule.Lessons.Add(targetLesson);
                }

                _context.SaveChanges();

                // Logger.Info("Скопировано занятие : SourceLessonId={0}, TargetLessonId={1}", sourceLesson.LessonId, targetLesson.LessonId);

                var scheduleViewModel = _lessonService
                  .GetScheduleForGroups(new int[] { targetGroupId }, UserProfile.EducationYearId, UserProfile.WeekNumber, targetDayNumber, targetClassNumber, checkDowntimes: true)
                  .SingleOrDefault();
                if (scheduleViewModel != null)
                {
                    return PartialView("_LessonCell", scheduleViewModel.Lessons);
                }
            }

            return null;
        }

        /// <summary>
        /// Удаление занятия
        /// </summary>
        [HttpPost]
        public ActionResult RemoveLesson(int groupId, int dayNumber, int classNumber)
        {
            var lessons = _context.Lessons
                .Include(x => x.Schedule)
                .Include(x => x.LessonDetails)
                .Where(x => x.Schedule.GroupId == groupId && x.Schedule.EducationYearId == UserProfile.EducationYearId
                    && x.Schedule.WeekNumber == UserProfile.WeekNumber && x.Schedule.DayNumber == dayNumber && x.Schedule.ClassNumber == classNumber
                    && x.DeletedAt == null)
                .ToList();
            foreach (var lesson in lessons)
            {
                lesson.DeletedAt = DateTime.Now;

                var lessonDetails = lesson.LessonDetails.Where(x => x.DeletedAt == null);
                foreach (var lessonDetail in lessonDetails)
                {
                    lessonDetail.DeletedAt = DateTime.Now;
                }
            }

            _context.SaveChanges();

            // Logger.Info("Занятие помечено как удалённое : LessonId=" + lesson.LessonId);

            return null;
        }

        /// <summary>
        /// Возвращает данные для обновления занятий преподавателей
        /// Например, для того чтобы подсветилась подсказка об окнах в расписании преподавателей
        /// </summary>
        [HttpPost]
        public ActionResult RefreshLesson(int groupId, int dayNumber, int classNumber, int[] plannedChairJobIds)
        {
            if (!Request.IsAjaxRequest() || plannedChairJobIds == null) return null;

            var editableGroups = _groupService.GetEditableGroupsIdentifiers(UserProfile.Id);

            var changedLessonDetails = _context.LessonDetails
                .Include(x => x.Lesson.Schedule)
                .Where(x => plannedChairJobIds.Contains(x.PlannedChairJobId ?? 0) && editableGroups.Contains(x.Lesson.Schedule.GroupId)
                    && x.Lesson.Schedule.EducationYearId == UserProfile.EducationYearId
                    && x.Lesson.Schedule.WeekNumber == UserProfile.WeekNumber && x.Lesson.Schedule.DayNumber == dayNumber
                    && x.Lesson.Schedule.ClassNumber != classNumber && x.DeletedAt == null && x.Lesson.DeletedAt == null && x.Lesson.Schedule.DeletedAt == null)
                .ToList();

            var lessonCellsForRefresh = new List<RefreshCellViewModel>();
            foreach (var lessonDetail in changedLessonDetails)
            {
                var targetGroupId = lessonDetail.Lesson.Schedule.GroupId;
                var targetClassNumber = lessonDetail.Lesson.Schedule.ClassNumber;
                var dayScheduleViewModel = _lessonService
                   .GetScheduleForGroups(new int[] { targetGroupId }, UserProfile.EducationYearId, UserProfile.WeekNumber, dayNumber, targetClassNumber, checkDowntimes: true)
                   .SingleOrDefault();

                var lessonCellContent = RenderPartialToString(this, "_LessonCell", dayScheduleViewModel.Lessons, ViewData, TempData);
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

                var groupSets = _context.GroupSets
                    .Include(x => x.GroupSetGroups.Select(g => g.Group))
                    .Where(x => x.ApplicationUserId == UserProfile.Id)
                    .ToList()
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

                if (currentWeek <= 0)
                {
                    currentWeek = 0;

                    viewModel.CurrentWeekStartDate = string.Format("Текущая дата вне {0} учебного года", UserProfile.EducationYear.EducationYearName);
                    viewModel.CurrentWeekEndDate = "";
                }

                // Необходим график учебного процесса чтобы узнать количество недель
                //var courseSchedules = new List<CourseSchedule>();
                //foreach (var group in groups)
                //{
                    // var courseNumber = group.Course.CourseNumber;
                    //var academicPlan = group.BaseProgramOfEducation.AcademicPlans
                    //    .OrderByDescending(d => d.UploadedAt)
                    //    .FirstOrDefault();

                    //if (academicPlan != null)
                    //{
                    //    var courseSchedule = academicPlan.CourseSchedules
                    //        .SingleOrDefault(x => x.CourseNumber == courseNumber);

                    //    courseSchedules.Add(courseSchedule);
                    //}
                //}

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