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
using Microsoft.AspNet.Identity;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Net;
using ClassSchedule.Business.Models.CopySchedule;
using ClassSchedule.Business.Helpers;

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

                return new JsonErrorResult(HttpStatusCode.BadRequest) { Data = "Ошибка валидации. " + errors };
            }

            try
            {
                var changeLog = string.Empty;
                _lessonService.SaveLesson(viewModel, UserProfile, ref changeLog);

                Logger.Info(changeLog);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при сохранении занятия");

                return new JsonErrorResult(HttpStatusCode.InternalServerError) { Data = string.Format("ErrorMessage : {0}, StackTrace: {1}", ex.Message, ex.StackTrace) };
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
            try
            {
                var changeLog = string.Empty;
                _lessonService.CopyLesson(sourceGroupId, sourceDayNumber, sourceClassNumber, targetGroupId, targetDayNumber, targetClassNumber, UserProfile, ref changeLog);

                Logger.Info(changeLog);

                var scheduleViewModel = _lessonService
                  .GetScheduleForGroups(new int[] { targetGroupId }, UserProfile.EducationYearId, UserProfile.WeekNumber, targetDayNumber, targetClassNumber, checkDowntimes: true)
                  .SingleOrDefault();
                if (scheduleViewModel != null)
                {
                    return PartialView("_LessonCell", scheduleViewModel.Lessons);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при копировании занятия");

                return new JsonErrorResult(HttpStatusCode.InternalServerError) { Data = "ErrorMessage:" + ex.Message + ", StackTrace:" + ex.StackTrace };
            }
         
            return null;
        }

        /// <summary>
        /// Удаление занятия
        /// </summary>
        [HttpPost]
        public ActionResult RemoveLesson(int groupId, int dayNumber, int classNumber)
        {
            try
            {
                var changeLog = string.Empty;
                _lessonService.RemoveLesson(groupId, UserProfile.EducationYearId, UserProfile.WeekNumber, dayNumber, classNumber, ref changeLog);

                Logger.Info(changeLog);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Ошибка при удалении занятия");

                return new JsonErrorResult(HttpStatusCode.InternalServerError) { Data = "ErrorMessage:" + ex.Message + ", StackTrace:" + ex.StackTrace };
            }

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
                    if (firstGroup != null)
                    {
                        var facultyId = firstGroup.Group.Course.FacultyId;
                        var educationFormId = firstGroup.Group.BaseProgramOfEducation.EducationFormId;
                        var educationLevelId = firstGroup.Group.BaseProgramOfEducation.EducationLevelId;
                        var courseNumber = firstGroup.Group.Course.CourseNumber;

                        viewModel.FacultyId = facultyId;
                        viewModel.EducationFormId = educationFormId;
                        viewModel.EducationLevelId = educationLevelId;
                        viewModel.CourseNumber = courseNumber ?? 0;

                        viewModel.CourseNumbers = _dictionaryService.GetCourseNumbers(facultyId, educationFormId, educationLevelId);

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

                        viewModel.SelectedGroups = groupsSet.GroupSetGroups
                            .OrderBy(x => x.Order)
                            .Select(x => new GroupViewModel
                            {
                                GroupId = x.GroupId,
                                GroupName = x.Group.GroupName,
                                IsSelected = true,
                                Order = x.Order
                            })
                            .ToList();
                    }                 
                }
            }
            else
            {
                viewModel.EducationFormId = (int)EducationForms.FullTime;

                var groupSets = _context.GroupSets
                    .Include(x => x.GroupSetGroups.Select(g => g.Group))
                    .Where(x => x.ApplicationUserId == UserProfile.Id)
                    .OrderByDescending(x => x.Counter)
                    .ToList()
                    .Select(x => new GroupSetViewModel
                    {
                        GroupSetId = x.GroupSetId,
                        GroupSetName = x.GroupSetName,
                        GroupNames = string.Join(", ", x.GroupSetGroups.OrderBy(o => o.Order).Select(g => g.Group.GroupName))
                    })
                    .ToList();
                viewModel.GroupSets = groupSets;

                if (User.IsInRole("Administrator"))
                {
                    viewModel.Faculties = _dictionaryService.GetFaculties();
                }
                else
                {
                    viewModel.Faculties = _dictionaryService.GetFaculties(UserProfile.Id);
                }

                viewModel.EducationLevels = _dictionaryService.GetEducationLevels();
                viewModel.EducationForms = _dictionaryService.GetEducationForms();
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
            currentGroupSet.Counter = (currentGroupSet.Counter ?? 0) + 1;

            _context.SaveChanges();

            return Json(new { result = "Redirect", url = Url.Action("Index", "Home") });
        }

        [HttpPost]
        public ActionResult EditableWeeks()
        {
            if (!Request.IsAjaxRequest()) return null;

            var yearStartDate = UserProfile.EducationYear.DateStart;
            var currentWeek = DateHelpers.WeekOfLesson(yearStartDate, DateTime.Now);

            var viewModel = new ChangeWeekViewModel
            {
                EditedWeek = UserProfile.WeekNumber,
                EditedWeekStartDate = DateHelpers.DateOfLesson(yearStartDate, UserProfile.WeekNumber, 1).ToString("dd.MM.yyyy"),
                EditedWeekEndDate = DateHelpers.DateOfLesson(yearStartDate, UserProfile.WeekNumber, 7).ToString("dd.MM.yyyy"),
                CurrentWeek = currentWeek,
                CurrentWeekStartDate = DateHelpers.DateOfLesson(yearStartDate, currentWeek, 1).ToString("dd.MM.yyyy"),
                CurrentWeekEndDate = DateHelpers.DateOfLesson(yearStartDate, currentWeek, 7).ToString("dd.MM.yyyy"),
                Weeks = new List<WeekViewModel>()
            };

            if (currentWeek <= 0)
            {
                currentWeek = 0;

                viewModel.CurrentWeekStartDate = string.Format("Текущая дата вне {0} учебного года", UserProfile.EducationYear.EducationYearName);
                viewModel.CurrentWeekEndDate = "";
            }

            int index = 1;
            while (true)
            {
                var weekStartDate = DateHelpers.DateOfLesson(yearStartDate, index, 1);
                var weekEndDate = DateHelpers.DateOfLesson(yearStartDate, index, 7);

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

            return Json(viewModel);
        }

        [HttpGet]
        public ActionResult ChangeWeek(int weekNumber)
        {
            UserProfile.WeekNumber = weekNumber;
            UserManager.Update(UserProfile);

            return RedirectToAction("Index");
        }

        /// <summary>
        /// Копирование расписания на неделю
        /// </summary>
        [HttpGet]
        public ActionResult CopySchedule()
        {
            if (!Request.IsAjaxRequest()) return null;

            var yearStartDate = UserProfile.EducationYear.DateStart;
            var currentWeek = DateHelpers.WeekOfLesson(yearStartDate, DateTime.Now);

            var viewModel = new CopyScheduleViewModel
            {
                EditedWeek = UserProfile.WeekNumber,
                EditedWeekStartDate = DateHelpers.DateOfLesson(yearStartDate, UserProfile.WeekNumber, 1).ToString("dd.MM.yyyy"),
                EditedWeekEndDate = DateHelpers.DateOfLesson(yearStartDate, UserProfile.WeekNumber, 7).ToString("dd.MM.yyyy"),
                CurrentWeek = currentWeek,
                CurrentWeekStartDate = DateHelpers.DateOfLesson(yearStartDate, currentWeek, 1).ToString("dd.MM.yyyy"),
                CurrentWeekEndDate = DateHelpers.DateOfLesson(yearStartDate, currentWeek, 7).ToString("dd.MM.yyyy"),
                Weeks = new List<WeekViewModel>()
            };

            if (currentWeek <= 0)
            {
                currentWeek = 0;

                viewModel.CurrentWeekStartDate = string.Format("Текущая дата вне {0} учебного года", UserProfile.EducationYear.EducationYearName);
                viewModel.CurrentWeekEndDate = "";
            }

            int index = 1;
            while (true)
            {
                var weekStartDate = DateHelpers.DateOfLesson(yearStartDate, index, 1);
                var weekEndDate = DateHelpers.DateOfLesson(yearStartDate, index, 7);

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

            viewModel.Days = _dictionaryService.GetDays();
            viewModel.SelectedDays = new int[] { 1, 2, 3, 4, 5, 6 };

            viewModel.Groups = _groupService.GetEditableGroups(UserProfile.Id)
                .Select(x => new GroupViewModel { GroupId = x.GroupId, GroupName = x.GroupName, IsSelected = true })
                .ToList();
            viewModel.SelectedGroups = viewModel.Groups.Select(x => x.GroupId).ToArray();

            return Json(viewModel, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult CopySchedule(CopyScheduleViewModel viewModel)
        {
            try
            {
                var changeLog = string.Empty;
                _lessonService.CopySchedule(viewModel, UserProfile, ref changeLog);

                Logger.Info(changeLog);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Произошла ошибка при копировании расписания");

                return new JsonErrorResult(HttpStatusCode.InternalServerError) { Data = string.Format("ErrorMessage : {0}, StackTrace: {1}", ex.Message, ex.StackTrace) };
            }         

            return null;
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