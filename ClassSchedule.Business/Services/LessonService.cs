using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ClassSchedule.Business.Interfaces;
using ClassSchedule.Business.Models.Schedule;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.Models;
using ClassSchedule.Business.Helpers;
using ClassSchedule.Business.Models.CopySchedule;
using ClassSchedule.Business.Exceptions;

namespace ClassSchedule.Business.Services
{
    public class LessonService : ILessonService
    {
        private readonly ApplicationDbContext _context;
        private readonly IGroupService _groupService;
        private readonly IJobService _jobService;

        public LessonService(ApplicationDbContext context, IGroupService groupService, IJobService jobService)
        {
            _context = context;
            _groupService = groupService;
            _jobService = jobService;
        }

        public List<ScheduleViewModel> GetScheduleForGroups(int[] groupsIds, int educationYearId, int weekNumber
            , int? dayNumber = null, int? classNumber = null, bool checkDowntimes = false)
        {
            var schedule = _context.Schedule
                .Include(x => x.EducationYear)
                .Include(x => x.Group)
                .Include(x => x.Lessons.Select(ls => ls.LessonType))
                .Include(x => x.Lessons.Select(ls => ls.Discipline.Chair))
                .Include(x => x.Lessons.Select(ls => ls.Discipline.DisciplineName))
                .Include(x => x.Lessons.Select(ls => ls.LessonDetails.Select(ld => ld.Auditorium.Housing)))
                .Include(x => x.Lessons.Select(ls => ls.LessonDetails.Select(ld => ld.PlannedChairJob.Job.Employee.Person)))
                .Where(x => groupsIds.Contains(x.GroupId) && x.EducationYearId == educationYearId
                    && x.WeekNumber == weekNumber && x.DeletedAt == null);

            if (dayNumber != null)
            {
                schedule = schedule.Where(x => x.DayNumber == dayNumber);
            }

            if (classNumber != null)
            {
                schedule = schedule.Where(x => x.ClassNumber == classNumber);
            }

            var scheduleViewModel = schedule
                .Select(x => new ScheduleViewModel
                {
                    ScheduleId = x.ScheduleId,
                    EducationYearId = x.EducationYearId,
                    EducationYear = x.EducationYear.EducationYearName,
                    WeekNumber = x.WeekNumber,
                    DayNumber = x.DayNumber,
                    ClassNumber = x.ClassNumber,
                    GroupId = x.GroupId,
                    GroupName = x.Group.GroupName,
                    ClassDate = x.ClassDate,
                    IsNotActive = x.IsNotActive,
                    Lessons = x.Lessons.Where(ls => ls.DeletedAt == null)
                        .Select(ls => new LessonViewModel
                        {
                            LessonId = ls.LessonId,
                            ScheduleId = ls.ScheduleId,
                            LessonTypeId = ls.LessonTypeId ?? 0,
                            DisciplineId = ls.DisciplineId,
                            DisciplineName = ls.Discipline != null && ls.Discipline.DisciplineName != null ? ls.Discipline.DisciplineName.Name : "",
                            ChairId = ls.Discipline != null ? ls.Discipline.ChairId : 0,
                            ChairName = ls.Discipline != null && ls.Discipline.Chair != null ? ls.Discipline.Chair.DivisionName : "",
                            Order = ls.Order,
                            LessonDetails = ls.LessonDetails.Where(ld => ld.DeletedAt == null)
                                .Select(ld => new LessonDetailViewModel
                                {
                                    LessonDetailId = ld.LessonDetailId,
                                    LessonId = ld.LessonId,
                                    AuditoriumId = ld.AuditoriumId,
                                    AuditoriumName = ld.Auditorium != null && ld.Auditorium.Housing != null 
                                        ? ld.Auditorium.AuditoriumNumber + (ld.Auditorium.AuditoriumNumber != ld.Auditorium.Housing.Abbreviation ? ld.Auditorium.Housing.Abbreviation : "") + "." : "",
                                    HousingId = ld.Auditorium != null ? ld.Auditorium.HousingId : 0,
                                    PlannedChairJobId = ld.PlannedChairJobId,
                                    TeacherLastName = ld.PlannedChairJob != null
                                            && ld.PlannedChairJob.Job != null
                                            && ld.PlannedChairJob.Job.Employee != null
                                            && ld.PlannedChairJob.Job.Employee.Person != null
                                        ? ld.PlannedChairJob.Job.Employee.Person.LastName : ld.PlannedChairJob.PlannedChairJobComment,
                                    TeacherFirstName = ld.PlannedChairJob != null
                                            && ld.PlannedChairJob.Job != null
                                            && ld.PlannedChairJob.Job.Employee != null
                                            && ld.PlannedChairJob.Job.Employee.Person != null
                                        ? ld.PlannedChairJob.Job.Employee.Person.FirstName : "",
                                    TeacherMiddleName = ld.PlannedChairJob != null
                                            && ld.PlannedChairJob.Job != null
                                            && ld.PlannedChairJob.Job.Employee != null
                                            && ld.PlannedChairJob.Job.Employee.Person != null
                                        ? ld.PlannedChairJob.Job.Employee.Person.MiddleName : "",
                                    Order = ld.Order
                                })
                                .ToList()
                        })
                        .ToList()
                })
                .ToList();

            if (checkDowntimes)
            {
                var downtimes = _jobService.TeachersDowntime(weekNumber, maxDiff: 2, groupsIds: groupsIds);
                foreach (var downtime in downtimes)
                {
                    scheduleViewModel.Where(x => /*x.GroupId == downtime.GroupId && */x.DayNumber == downtime.DayNumber /*&& x.ClassNumber == downtime.ClassNumber*/)
                        .SelectMany(g => g.Lessons.SelectMany(d => d.LessonDetails))
                        .Where(p => p.PlannedChairJobId == downtime.PlannedChairJobId)
                        .All(c => { c.TeacherHasDowntime = true; return true; });
                }
            }

            // Сортировка в порядке идентификаторов групп
            scheduleViewModel = groupsIds
                .Join(scheduleViewModel, i => i, s => s.GroupId, (i, s) => new { i, s })
                .Select(x => x.s)
                .ToList();

            var index = 0;
            var groups = _context.Groups
                .Where(x => groupsIds.Contains(x.GroupId) && x.DeletedAt == null)
                .ToList();
            foreach (var group in groups)
            {
                var contains = scheduleViewModel.Any(s => s.GroupId == group.GroupId);
                if (!contains)
                {
                    scheduleViewModel.Insert(index, new ScheduleViewModel
                    {
                        GroupId = group.GroupId,
                        GroupName = group.GroupName,
                        EducationYearId = educationYearId,
                        WeekNumber = weekNumber,
                        DayNumber = dayNumber ?? 0,
                        ClassNumber = classNumber ?? 0
                    });
                }

                index++;
            }

            return scheduleViewModel;
        }

        /// <summary>
        /// Сохранение занятия
        /// </summary>
        public EntityState SaveLesson(EditLessonViewModel viewModel, ApplicationUser user, ref string changeLog)
        {
            var scheduleState = EntityState.Modified;
            var schedule = _context.Schedule
                .Include(x => x.Lessons.Select(s => s.LessonDetails))
                .SingleOrDefault(x => x.ScheduleId == viewModel.ScheduleId && x.DeletedAt == null);

            if (schedule == null)
            {
                schedule = new Schedule
                {
                    GroupId = viewModel.GroupId,
                    EducationYearId = user.EducationYearId,
                    WeekNumber = user.WeekNumber,
                    DayNumber = viewModel.DayNumber,
                    ClassNumber = viewModel.ClassNumber,
                    ClassDate = DateHelpers.DateOfLesson(user.EducationYear.DateStart,
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
                    if (lesson.LessonTypeId != lessonViewModel.LessonTypeId)
                    {
                        changeLog += string.Format("\r\nИзменён тип занятия. [LessonId: {0}] [LessonTypeId: {1} => {2}]"
                            , lesson.LessonId, lesson.LessonTypeId, lessonViewModel.LessonTypeId);
                    }
                    lesson.LessonTypeId = lessonViewModel.LessonTypeId;

                    if (lesson.DisciplineId != lessonViewModel.DisciplineId)
                    {
                        changeLog += string.Format("\r\nИзменена дисциплина. [LessonId: {0}] [DisciplineId: {1} => {2}]"
                            , lesson.LessonId, lesson.DisciplineId, lessonViewModel.DisciplineId);
                    }
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
                            if (lessonDetail.AuditoriumId != lessonDetailViewModel.AuditoriumId)
                            {
                                changeLog += string.Format("\r\nИзменена аудитория. [LessonId: {0}] [LessonDetailId: {1}] [AuditoriumId: {1} => {2}]"
                                    , lesson.LessonId, lessonDetail.LessonDetailId, lessonDetail.AuditoriumId, lessonDetailViewModel.AuditoriumId);
                            }
                            lessonDetail.AuditoriumId = lessonDetailViewModel.AuditoriumId;

                            if (lessonDetail.PlannedChairJobId != lessonDetailViewModel.PlannedChairJobId)
                            {
                                changeLog += string.Format("\r\nИзменён преподаватель. [LessonId: {0}] [LessonDetailId: {1}] [PlannedChairJobId: {1} => {2}]"
                                    , lesson.LessonId, lessonDetail.LessonDetailId, lessonDetail.PlannedChairJobId, lessonDetailViewModel.PlannedChairJobId);
                            }
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
                .Where(x => x.Schedule.EducationYearId == user.EducationYearId
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
                        // changeLog += string.Format("\r\nУдалено занятие. [LessonId: {0}]", lesson.LessonId);

                        var lessonDetails = lesson.LessonDetails.Where(x => x.DeletedAt == null);
                        foreach (var lessonDetail in lessonDetails)
                        {
                            lessonDetail.DeletedAt = DateTime.Now;
                            changeLog += string.Format("\r\nУдалено занятие. [LessonId: {0}] [lessonDetailId: {1}]", lesson.LessonId, lessonDetail.LessonDetailId);
                        }
                    }
                    else
                    {
                        var lessonDetails = lesson.LessonDetails.Where(x => x.DeletedAt == null);
                        foreach (var lessonDetail in lessonDetails)
                        {
                            if (lessonDetail.LessonDetailId != 0)
                            {
                                var lessonDetailViewModel = lessonViewModel.LessonDetails.SingleOrDefault(x => x.LessonDetailId == lessonDetail.LessonDetailId);
                                if (lessonDetailViewModel == null)
                                {
                                    lessonDetail.DeletedAt = DateTime.Now;
                                    changeLog += string.Format("\r\nУдалено занятие. [LessonId: {0}] [lessonDetailId: {1}]", lesson.LessonId, lessonDetail.LessonDetailId);
                                }
                            }
                        }
                    }
                }
            }

            if (scheduleState == EntityState.Added)
            {
                _context.Schedule.Add(schedule);
            }

            _context.SaveChanges();

            // Логгирование
            var message = (scheduleState == EntityState.Added) ? "Создано новое занятие. " : "Обновлено занятие. ";
            message += string.Format("[ScheduleId: {0}, EducationYearId: {1}, WeekNumber: {2}, DayNumber: {3}, ClassNumber: {4}, ClassDate: {5}]"
                , schedule.ScheduleId, user.EducationYearId, user.WeekNumber, schedule.DayNumber, schedule.ClassNumber, schedule.ClassDate);
            changeLog = message + changeLog;

            return scheduleState;
        }

        /// <summary>
        /// Удаление занятия
        /// </summary>
        public void RemoveLesson(int groupId, int educationYearId, int weekNumber, int dayNumber, int classNumber, ref string changeLog)
        {
            var schedule = _context.Schedule
                .Include(x => x.Lessons.Select(l => l.LessonDetails))
                .Where(x => x.EducationYearId == educationYearId && x.GroupId == groupId
                    && x.WeekNumber == weekNumber && x.DayNumber == dayNumber && x.ClassNumber == classNumber
                    && x.DeletedAt == null)
                .SingleOrDefault();
            if (schedule != null)
            {
                var lessons = schedule.Lessons.Where(x => x.DeletedAt == null);
                foreach (var lesson in lessons)
                {
                    lesson.DeletedAt = DateTime.Now;

                    var lessonDetails = lesson.LessonDetails.Where(x => x.DeletedAt == null);
                    foreach (var lessonDetail in lessonDetails)
                    {
                        lessonDetail.DeletedAt = DateTime.Now;
                        changeLog += string.Format("Удалено занятие. [LessonId: {0}] [lessonDetailId: {1}]", lesson.LessonId, lessonDetail.LessonDetailId);
                    }
                }

                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Копирование занятия
        /// </summary>
        public void CopyLesson(int sourceGroupId, int sourceDayNumber, int sourceClassNumber, int targetGroupId
            , int targetDayNumber, int targetClassNumber, ApplicationUser user, ref string changeLog)
        {
            changeLog += "Копирование занятия.\r\n";
            var targetSchedule = _context.Schedule
                .Include(x => x.Lessons.Select(l => l.LessonDetails))
                .Where(x => x.EducationYearId == user.EducationYearId && x.GroupId == targetGroupId
                    && x.WeekNumber == user.WeekNumber && x.DayNumber == targetDayNumber && x.ClassNumber == targetClassNumber
                    && x.DeletedAt == null)
                .SingleOrDefault();
            if (targetSchedule != null)
            {
                // RemoveLesson(targetGroupId, user.EducationYearId, user.WeekNumber, targetDayNumber, targetClassNumber, ref changeLog);

                var targetLessons = targetSchedule.Lessons.Where(x => x.DeletedAt == null);
                foreach (var targetLesson in targetLessons)
                {
                    targetLesson.DeletedAt = DateTime.Now;

                    var targetLessonDetails = targetLesson.LessonDetails.Where(x => x.DeletedAt == null);
                    foreach (var targetLessonDetail in targetLessonDetails)
                    {
                        targetLessonDetail.DeletedAt = DateTime.Now;
                        changeLog += string.Format("Удалено занятие. [LessonId: {0}] [lessonDetailId: {1}]\r\n", targetLesson.LessonId, targetLessonDetail.LessonDetailId);
                    }
                }
            }
            else
            {
                targetSchedule = new Schedule
                {
                    EducationYearId = user.EducationYearId,
                    GroupId = targetGroupId,
                    WeekNumber = user.WeekNumber,
                    DayNumber = targetDayNumber,
                    ClassNumber = targetClassNumber,
                    ClassDate = DateHelpers.DateOfLesson(user.EducationYear.DateStart, user.WeekNumber, targetDayNumber),
                    Lessons = new List<Lesson>()
                };

                _context.Schedule.Add(targetSchedule);
            }

            // Копирование
            var sourceSchedule = _context.Schedule
                .Include(x => x.Group)
                .Include(x => x.Lessons.Select(l => l.Discipline))
                .Include(x => x.Lessons.Select(l => l.LessonDetails))
                .Where(x => x.EducationYearId == user.EducationYearId && x.GroupId == sourceGroupId
                    && x.WeekNumber == user.WeekNumber && x.DayNumber == sourceDayNumber && x.ClassNumber == sourceClassNumber
                    && x.DeletedAt == null)
                .SingleOrDefault();
            if (sourceSchedule != null)
            {
                // Поиск дисциплины
                var targetGroup = _context.Groups.SingleOrDefault(x => x.GroupId == targetGroupId && x.DeletedAt == null);
                var sourceGroup = sourceSchedule.Group;
                if (targetGroup != null && sourceGroup != null)
                {
                    var sourceLessons = sourceSchedule.Lessons.Where(x => x.DeletedAt == null);
                    foreach (var sourceLesson in sourceLessons)
                    {
                        Discipline targetDiscipline = null;
                        var sourceDiscipline = sourceLesson.Discipline;

                        // Если дисциплина не относится ни к какой определенной образовательной программе или если совпадают образовательные программы
                        // В остальных случаях ищется аналогичная дисциплина для целевой группы
                        if (sourceDiscipline.BaseProgramOfEducationId == null || targetGroup.BaseProgramOfEducationId == sourceGroup.BaseProgramOfEducationId)
                        {
                            targetDiscipline = sourceDiscipline;
                        }
                        else
                        {
                            targetDiscipline = _context.Disciplines
                                .SingleOrDefault(x => x.BaseProgramOfEducationId == targetGroup.BaseProgramOfEducationId && x.ChairId == sourceDiscipline.ChairId
                                    && ((x.DisciplineNameId != null && sourceDiscipline.DisciplineNameId != null && x.DisciplineNameId == sourceDiscipline.DisciplineNameId)
                                        || (x.StudyLoadCalculationStringName != null && sourceDiscipline.StudyLoadCalculationStringName != null && x.StudyLoadCalculationStringName == sourceDiscipline.StudyLoadCalculationStringName))
                                    && (x.EducationSemesterId != null && sourceDiscipline.EducationSemesterId != null && x.EducationSemesterId == sourceDiscipline.EducationSemesterId)
                                    && x.DeletedAt == null);
                        }

                        if (targetDiscipline == null)
                        {
                            throw new DisciplineNotFoundException();
                        }

                        var targetLesson = new Lesson
                        {
                            DisciplineId = targetDiscipline.DisciplineId,
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
                }

                changeLog += string.Format("Копировано занятие. [SourceScheduleId: {0} => TargetScheduleId: {1}]\r\n[SourceGroup: {2}] => TargetGroup: {3}]\r\n[EducationYearId: {4}]\r\n[SourceDayNumber: {5} => TargetDayNumber: {6}]\r\n[SourceClassNumber: {7} => TargetClassNumber: {8}]"
                    , sourceSchedule.ScheduleId, (targetSchedule.ScheduleId != 0 ? targetSchedule.ScheduleId + "" : "New"), "(" + sourceGroup.GroupId + ")" + sourceGroup.GroupName, "(" + targetGroup.GroupId + ")" + targetGroup.GroupName, user.EducationYearId, sourceDayNumber, targetDayNumber, sourceClassNumber, targetClassNumber);
            }

            _context.SaveChanges();
        }

        /// <summary>
        /// Копирование расписания на неделю
        /// </summary>
        public void CopySchedule(CopyScheduleViewModel viewModel, ApplicationUser user, ref string changeLog)
        {
            changeLog += "Копирование расписания.";
            var sourceSchedules = GetScheduleForGroups(viewModel.SelectedGroups, user.EducationYearId, user.WeekNumber)
                .Where(x => viewModel.SelectedDays.Contains(x.DayNumber));
            var targetSchedules = _context.Schedule
                .Include(x => x.Lessons.Select(l => l.LessonDetails))
                .Where(x => viewModel.SelectedGroups.Contains(x.GroupId) && x.EducationYearId == user.EducationYearId
                    && viewModel.SelectedWeeks.Contains(x.WeekNumber) && viewModel.SelectedDays.Contains(x.DayNumber)
                    && x.DeletedAt == null);
            foreach (var schedule in sourceSchedules)
            {
                foreach (var targetWeek in viewModel.SelectedWeeks)
                {
                    var targetSchedule = targetSchedules.SingleOrDefault(x => x.GroupId == schedule.GroupId
                        && x.DayNumber == schedule.DayNumber && x.ClassNumber == schedule.ClassNumber && x.WeekNumber == targetWeek);
                    if (targetSchedule == null)
                    {
                        targetSchedule = new Schedule
                        {
                            EducationYearId = schedule.EducationYearId,
                            WeekNumber = targetWeek,
                            GroupId = schedule.GroupId,
                            ClassDate = DateHelpers.DateOfLesson(user.EducationYear.DateStart, targetWeek, schedule.DayNumber),
                            DayNumber = schedule.DayNumber,
                            ClassNumber = schedule.ClassNumber,
                            IsNotActive = schedule.IsNotActive,
                            Lessons = new List<Lesson>()
                        };

                        _context.Schedule.Add(targetSchedule);
                    }

                    foreach (var targetLesson in targetSchedule.Lessons)
                    {
                        targetLesson.DeletedAt = DateTime.Now;

                        foreach (var targetLessonDetail in targetLesson.LessonDetails)
                        {
                            targetLessonDetail.DeletedAt = DateTime.Now;
                            changeLog += string.Format("\r\nУдалено занятие. [LessonId: {0}] [lessonDetailId: {1}]", targetLesson.LessonId, targetLessonDetail.LessonDetailId);
                        }
                    }

                    foreach (var lesson in schedule.Lessons)
                    {
                        var targetLesson = new Lesson
                        {
                            DisciplineId = lesson.DisciplineId,
                            LessonTypeId = lesson.LessonTypeId,
                            Order = lesson.Order,
                            LessonDetails = new List<LessonDetail>()
                        };

                        foreach (var lessonDetail in lesson.LessonDetails)
                        {
                            var targetLessonDetail = new LessonDetail
                            {
                                AuditoriumId = lessonDetail.AuditoriumId,
                                PlannedChairJobId = lessonDetail.PlannedChairJobId,
                                Order = lessonDetail.Order
                            };

                            targetLesson.LessonDetails.Add(targetLessonDetail);
                        }

                        targetSchedule.Lessons.Add(targetLesson);
                    }
                }
            }

            changeLog += string.Format("\r\nКопировано расписание. [GroupIds: {0}] [EducationYearId: {1}] [SourceWeek: {2} => TargetWeeks: {3}] [Days: {4}]"
                , string.Join(", ", viewModel.SelectedGroups), user.EducationYearId, viewModel.EditedWeek, string.Join(", ", viewModel.SelectedWeeks), string.Join(", ", viewModel.SelectedDays));

            _context.SaveChanges();
        }
    }
}
