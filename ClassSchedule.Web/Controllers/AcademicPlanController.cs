using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AcademicPlan.Parser.Models;
using AcademicPlan.Parser.Models.Plan.Title;
using AcademicPlan.Parser.Service;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Models;
using ClassSchedule.Web.Helpers;
using ClassSchedule.Web.Models;
using ClassSchedule.Web.Models.GroupInfo;
using AcademicPlan = ClassSchedule.Domain.Models.AcademicPlan;
using CourseSchedule = ClassSchedule.Domain.Models.CourseSchedule;
using SemesterSchedule = ClassSchedule.Domain.Models.SemesterSchedule;

namespace ClassSchedule.Web.Controllers
{
    public class AcademicPlanController : BaseController
    {
        public AcademicPlanController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Upload(int? programOfEducationId)
        {
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf == null || hpf.ContentLength == 0 || hpf.FileName == null)
                {
                    continue;
                }
               
                try
                {
                    if (programOfEducationId == null)
                    {
                        return AcademicPlanInfo(hpf);
                    }

                    string savedFileName = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(hpf.FileName));
                    hpf.SaveAs(savedFileName);

                    var academicPlan = Parse(savedFileName);
                    if (academicPlan != null)
                    {
                        academicPlan.ProgramOfEducationId = programOfEducationId.Value;
                        UnitOfWork.Repository<Domain.Models.AcademicPlan>().Insert(academicPlan);
                        UnitOfWork.Save();
                    }
                }
                catch (Exception ex)
                {
                    return Content("{\"Status\":\"Error\", \"ErrorMessage\":\"" + ex.Message + "\"}", "application/json");
                }       
            }

            return Content("{\"Status\":\"Success\"}", "application/json");
        }

        public ActionResult AcademicPlanInfo(HttpPostedFileBase file)
        {
            BinaryReader reader = new BinaryReader(file.InputStream);
            byte[] binData = reader.ReadBytes(file.ContentLength);
            string xml = System.Text.Encoding.UTF8.GetString(binData);
            var document = xml.ParseXml<Document>();

            string educationFormPlan = document.Plan.EducationForm.Substring(0, 3); 
            string directionCodePlan = document.Plan.PlanTitle.DirectionCode;

            var educationForm = UnitOfWork.Repository<EducationForm>()
                .Get(x => x.IsDeleted != true && x.EducationFormName.StartsWith(educationFormPlan))
                .SingleOrDefault();

            var direction = UnitOfWork.Repository<EducationDirection>()
                .Get(x => x.EducationDirectionCode == directionCodePlan || x.EducationDirectionCode.Replace(".", "") == directionCodePlan)
                .FirstOrDefault();

            string yearStartPlan = document.Plan.PlanTitle.YearStart;
            if (yearStartPlan == null)
            {
                throw new Exception("В файле не указан или неверно указан год начала обучения.");
            }
            int yearStart = Convert.ToInt32(yearStartPlan);  

            var properties = new List<string>();
            if (educationForm == null)
            {
                throw new Exception("В файле не указана или неверно указана форма обучения.");
            }

            properties.Add("\"EducationFormId\":\"" + educationForm.EducationFormId + "\"");
            properties.Add("\"EducationFormName\":\"" + educationForm.EducationFormName.Replace("ое", "ая").ToLower() + "\"");

            if (direction == null)
            {
                throw new Exception("В файле не указан или указан неверный код направления.");
            }

            properties.Add("\"EducationDirectionId\":\"" + direction.EducationDirectionId + "\"");
            properties.Add("\"EducationDirectionCode\":\"" + direction.EducationDirectionCode + "\"");
            properties.Add("\"EducationDirectionName\":\"" + direction.EducationDirectionName + "\"");
            properties.Add("\"YearStart\":\"" + yearStart + "\"");
            properties.Add("\"Status\":\"Info\"");
            string result = "{" + String.Join(",", properties) + "}";

            return Content(result, "application/json");
        }

        public Domain.Models.AcademicPlan Parse(string fileName)
        {
            string xml = System.IO.File.ReadAllText(fileName);
            var document = xml.ParseXml<Document>();

            //var discs = document.Plan.Disciplines
            //    .Select(x => new TempDiscipline { DisciplineName = x.DisciplineName, ChairCode = x.ChairCode })
            //    .OrderBy(o => o.DisciplineName)
            //    .Distinct();
            //foreach (var disc in discs)
            //{
            //    var discInDb = UnitOfWork.Repository<TempDiscipline>()
            //        .Get(d => d.DisciplineName == disc.DisciplineName && d.ChairCode == disc.ChairCode)
            //        .SingleOrDefault();
            //    if (discInDb == null)
            //    {
            //        UnitOfWork.Repository<TempDiscipline>().Insert(disc);
            //        UnitOfWork.Save();
            //    }
            //}

            //return null;

            var academicPlan = new Domain.Models.AcademicPlan
            {
                AcademicPlanName = document.Plan.PlanTitle.PlanFullName,
                // NumberOfSemesters = document.Plan.PlanTitle.NumberOfSemesters,
                FilePath = fileName,
                UploadedAt = DateTime.Now
            };

            // Расписание на курс
            var courseSchedules = new List<CourseSchedule>();
            foreach (var courseSchedulePlan in document.Plan.PlanTitle.CourseSchedules)
            {
                bool isRealSchedule = courseSchedulePlan.Schedule.Length !=
                                      courseSchedulePlan.Schedule.Count(x => x == '=');
                if (!isRealSchedule)
                {
                    continue;
                }

                var courseSchedule = new CourseSchedule
                {
                    CourseNumber = courseSchedulePlan.CourseNumber,
                    ExamSessionWeeks = courseSchedulePlan.ExamSessionWeeks,
                    FinalQualifyingWorkWeeks = courseSchedulePlan.FinalQualifyingWorkWeeks,
                    PracticalTrainingWeeks = courseSchedulePlan.PracticalTrainingWeeks,
                    StateExamsWeeks = courseSchedulePlan.StateExamsWeeks,
                    StudyTrainingWeeks = courseSchedulePlan.StudyTrainingWeeks,
                    TheoreticalTrainingWeeks = courseSchedulePlan.TheoreticalTrainingWeeks,
                    WeeksOfHolidays = courseSchedulePlan.WeeksOfHolidays,
                    FirstMaxLoad = courseSchedulePlan.FirstMaxLoad,
                    SecondMaxLoad = courseSchedulePlan.SecondMaxLoad,
                    Schedule = courseSchedulePlan.Schedule,
                    ResearchWorkWeeks = courseSchedulePlan.ResearchWorkWeeks,
                    SemesterSchedules = new List<SemesterSchedule>()
                };

                // Расписание на семестр определенного курса
                foreach (var semesterSchedulePlan in courseSchedulePlan.SemesterSchedules)
                {                   
                    var semesterSchedule = new SemesterSchedule
                    {
                        SemesterNumber = semesterSchedulePlan.SemesterNumber,
                        ExamSessionWeeks = semesterSchedulePlan.ExamSessionWeeks,
                        FinalQualifyingWorkWeeks = semesterSchedulePlan.FinalQualifyingWorkWeeks,
                        PracticalTrainingWeeks = semesterSchedulePlan.PracticalTrainingWeeks,
                        StateExamsWeeks = semesterSchedulePlan.StateExamsWeeks,
                        StudyTrainingWeeks = semesterSchedulePlan.StudyTrainingWeeks,
                        TheoreticalTrainingWeeks = semesterSchedulePlan.TheoreticalTrainingWeeks,
                        WeeksOfHolidays = semesterSchedulePlan.WeeksOfHolidays,
                        NumberOfFirstWeek = semesterSchedulePlan.NumberOfFirstWeek,
                        Schedule = semesterSchedulePlan.Schedule,
                        ResearchWorkWeeks = semesterSchedulePlan.ResearchWorkWeeks,
                        DisciplineSemesterPlans = new List<DisciplineSemesterPlan>()
                    };

                    // План по каждой дисциплине на каждый семестр
                    foreach (var disciplinePlan in document.Plan.Disciplines)
                    {
                        var docDsp =
                            disciplinePlan.DisciplineSemesterPlans.FirstOrDefault(
                                p => p.SemesterNumber == (courseSchedule.CourseNumber - 1) * 2 + semesterSchedule.SemesterNumber);
                        if (docDsp == null) continue;
                        
                        var chair = UnitOfWork.Repository<Chair>()
                            .Get(x => x.IsDeleted != true && x.IsFaculty != true
                                      && x.DivisionCodeVpo == disciplinePlan.ChairCode)
                            .SingleOrDefault();                      
                        if (chair == null) continue;

                        var discipline = UnitOfWork.Repository<Discipline>()
                            .Get(d => d.DisciplineName == disciplinePlan.DisciplineName && d.ChairId == chair.ChairId)
                            .FirstOrDefault();
                        if (discipline == null)
                        {
                            discipline = new Discipline { DisciplineName = disciplinePlan.DisciplineName, ChairId = chair.ChairId };
                            UnitOfWork.Repository<Discipline>().Insert(discipline);
                            UnitOfWork.Save();
                        }

                        var disciplineSemesterPlan = new DisciplineSemesterPlan
                        {
                            DisciplineId = discipline.DisciplineId,
                            Discipline = discipline,
                            HoursOfLectures = docDsp.HoursOfLectures,
                            HoursOfPractice = docDsp.HoursOfPractice,
                            HoursOfLaboratory = docDsp.HoursOfLaboratory,
                            LecturesPerWeek = docDsp.LecturesPerWeek,
                            PracticePerWeek = docDsp.PracticePerWeek,
                            LaboratoryPerWeek = docDsp.LaboratoryPerWeek
                        };

                        if (docDsp.Exam != 0) disciplineSemesterPlan.SessionControlTypeId = (int)SessionControlType.Exam;
                        if (docDsp.Check != 0) disciplineSemesterPlan.SessionControlTypeId = (int)SessionControlType.Check;

                        semesterSchedule.DisciplineSemesterPlans.Add(disciplineSemesterPlan);
                    }
                   
                    courseSchedule.SemesterSchedules.Add(semesterSchedule);
                }

                courseSchedules.Add(courseSchedule);
            }
            academicPlan.CourseSchedules = courseSchedules;
            
            return academicPlan;
        }

        [HttpGet]
        public ActionResult Info(int groupId)
        {
            var group = UnitOfWork.Repository<Group>()
                .Get(x => x.GroupId == groupId && x.IsDeleted != true)
                .SingleOrDefault();
            if (group == null)
            {
                return new HttpStatusCodeResult(404, "Группы с данным идентификатором не существует.");
            }

            var academicPlan = UnitOfWork.Repository<Domain.Models.AcademicPlan>()
                .GetQ(filter: x => x.ProgramOfEducation.Groups.Any(g => g.GroupId == groupId))
                .OrderByDescending(d => d.UploadedAt)
                .FirstOrDefault();
            if (academicPlan == null)
            {
                return new HttpStatusCodeResult(404);
            }

            var courseSchedule = academicPlan.CourseSchedules
                .SingleOrDefault(x => x.CourseNumber == UserProfile.EducationYear.YearStart - academicPlan.ProgramOfEducation.YearStart + 1);
            if (courseSchedule == null)
            {
                return new HttpStatusCodeResult(404, "Учебный план загружен некорректно.");
            }
            
            // Общая информация о группе и график обучения на каждый семестр учебного года
            var viewModel = new GroupInfoViewModel
            {
                GroupId = groupId,
                GroupName = group.DivisionName,
                NumberOfStudents = group.NumberOfStudents,
                Profile = group.ProgramOfEducation.EducationProfile.EducationDirection.EducationDirectionCode 
                    + " " + group.ProgramOfEducation.EducationProfile.EducationProfileName,
                EducationForm = group.ProgramOfEducation.EducationForm.EducationFormName,
                EducationLevel = group.ProgramOfEducation.EducationLevel.EducationLevelName,
                NumberOfSemesters = courseSchedule.SemesterSchedules.Count,
                SemesterSchedules = courseSchedule.SemesterSchedules
                    .Select(x => new SemesterScheduleViewModel
                    {
                        SemesterNumber = x.SemesterNumber,
                        SemesterStartDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart, x.NumberOfFirstWeek, 1),
                        SemesterEndDate = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart, (x.NumberOfFirstWeek - 1) + x.NumberOfLastWeek, 7),
                        TheoreticalTrainingWeeks = x.TheoreticalTrainingWeeks,
                        ExamSessionWeeks = x.ExamSessionWeeks,
                        WeeksOfHolidays = x.WeeksOfHolidays,
                        FinalQualifyingWorkWeeks = x.FinalQualifyingWorkWeeks,
                        StudyTrainingWeeks = x.StudyTrainingWeeks,
                        PracticalTrainingWeeks = x.PracticalTrainingWeeks,
                        StateExamsWeeks = x.StateExamsWeeks,
                        ResearchWorkWeeks = x.ResearchWorkWeeks
                    })
                    .OrderBy(n => n.SemesterNumber)
                    .ToList()
            };

            // План по каждой дисциплине
            var disciplines = courseSchedule.SemesterSchedules
                .SelectMany(x => x.DisciplineSemesterPlans)
                .GroupBy(g => new { g.Discipline })
                .Select(x => new DisciplineViewModel
                {
                    DisciplineId = x.Key.Discipline.DisciplineId,
                    DisciplineName = x.Key.Discipline.DisciplineName,
                    ChairId = x.Key.Discipline.ChairId,
                    ChairName = x.Key.Discipline.Chair.DivisionName,
                    DisciplineSemesterPlans = x.Select(y => 
                        new DisciplineSemesterPlanViewModel
                        {
                            HoursOfLaboratory = y.HoursOfLaboratory,
                            HoursOfLectures = y.HoursOfLectures,
                            HoursOfPractice = y.HoursOfPractice,
                            //LecturesPerWeek = y.LecturesPerWeek,
                            //LaboratoryPerWeek = y.LaboratoryPerWeek,
                            //PracticePerWeek = y.PracticePerWeek,
                            LecturesPerWeek = (float) Math.Round((double)(y.HoursOfLectures ?? 0) / y.SemesterSchedule.TheoreticalTrainingWeeks, MidpointRounding.AwayFromZero),
                            LaboratoryPerWeek = (float)Math.Round((double)(y.HoursOfLaboratory ?? 0) / y.SemesterSchedule.TheoreticalTrainingWeeks, MidpointRounding.AwayFromZero),
                            PracticePerWeek = (float)Math.Round((double)(y.HoursOfPractice ?? 0) / y.SemesterSchedule.TheoreticalTrainingWeeks, MidpointRounding.AwayFromZero),
                            HoursOfLecturesFilled = y.Discipline.Lessons
                                .Count(ls => ls.DeletedAt == null && ls.GroupId == groupId && ls.LessonTypeId == (int) LessonTypes.Lection 
                                    && ls.WeekNumber >= y.SemesterSchedule.NumberOfFirstWeek && ls.WeekNumber <= y.SemesterSchedule.NumberOfLastWeek) * 2,
                            HoursOfPracticeFilled = y.Discipline.Lessons
                                .Count(ls => ls.DeletedAt == null && ls.GroupId == groupId 
                                    && (ls.LessonTypeId == (int)LessonTypes.PracticalLesson || ls.LessonTypeId == (int)LessonTypes.Seminar || ls.LessonTypeId == (int)LessonTypes.Training)
                                    && ls.WeekNumber >= y.SemesterSchedule.NumberOfFirstWeek && ls.WeekNumber <= y.SemesterSchedule.NumberOfLastWeek) * 2,
                            HoursOfLaboratoryFilled = y.Discipline.Lessons
                                .Count(ls => ls.DeletedAt == null && ls.GroupId == groupId && ls.LessonTypeId == (int)LessonTypes.LaboratoryWork
                                    && ls.WeekNumber >= y.SemesterSchedule.NumberOfFirstWeek && ls.WeekNumber <= y.SemesterSchedule.NumberOfLastWeek) * 2,
                            SemesterNumber = y.SemesterSchedule.SemesterNumber
                        })
                        .OrderBy(n => n.SemesterNumber)
                    .ToList()
                })
                .OrderBy(x => x.DisciplineName)
                .ToList();

            viewModel.Disciplines = disciplines;
          
            return View(viewModel);    
        }

        [HttpPost]
        public ActionResult ChartData(int groupId)
        {
            if (!Request.IsAjaxRequest())
            {
                return null;
            }

            var academicPlan = UnitOfWork.Repository<Domain.Models.AcademicPlan>()
                .GetQ(filter: x => x.ProgramOfEducation.Groups.Any(g => g.GroupId == groupId))
                .OrderByDescending(d => d.UploadedAt)
                .FirstOrDefault();
            if (academicPlan == null)
            {
                return new HttpStatusCodeResult(404);
            }

            var courseSchedule = academicPlan.CourseSchedules
                .SingleOrDefault(x => x.CourseNumber == UserProfile.EducationYear.YearStart - academicPlan.ProgramOfEducation.YearStart + 1);
            if (courseSchedule == null)
            {
                return new HttpStatusCodeResult(404, "Учебный план загружен некорректно.");
            }

            var chartSeries = new List<ChartSeriesViewModel>();

            // Вычисляем периоды в графике обучения
            int startWeek = 1;
            for (int currentWeek = 1; currentWeek <= courseSchedule.Schedule.Length - 1; currentWeek++)
            {
                var currentAbbr = courseSchedule.Schedule[currentWeek - 1];
                var nextAbbr = courseSchedule.Schedule[currentWeek];
                if (currentAbbr != nextAbbr || currentWeek == courseSchedule.Schedule.Length - 1)
                {
                    var series = chartSeries.SingleOrDefault(x => x.name == currentAbbr + "");
                    if (series == null)
                    {
                        var scheduleType = ScheduleHelpers.ScheduleTypeByAbbr(currentAbbr);
                        series = new ChartSeriesViewModel
                        {
                            name = scheduleType["Name"],
                            color = scheduleType["Color"],
                            pointWidth = 12,
                            pointRange = 24 * 3600 * 1000 
                        };
                        chartSeries.Add(series);
                    }

                    if (series.data == null)
                    {
                        series.data = new List<ChartIntervalViewModel>();
                    }

                    // Т.к. индексация массива начинается с 0, чтобы перейти к последней неделе
                    if (currentWeek == courseSchedule.Schedule.Length - 1) currentWeek++;

                    var low = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart, startWeek, 1);
                    var high = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart, currentWeek, 7).AddDays(1).AddSeconds(-1);

                    var interval = new ChartIntervalViewModel
                    {
                        x = 0,
                        low = (long)(low - new DateTime(1970, 1, 1)).TotalMilliseconds,
                        high = (long)(high - new DateTime(1970, 1, 1)).TotalMilliseconds,
                        lowWeek = startWeek,
                        highWeek = currentWeek
                    };
                    series.data.Add(interval);

                    startWeek = currentWeek + 1;
                    currentAbbr = nextAbbr;
                }
            }

            return Json(
                new
                {
                    educationYear = UserProfile.EducationYear.YearStart + "/" + UserProfile.EducationYear.YearEnd, 
                    chartSeries = chartSeries
                });
        }
    }
}