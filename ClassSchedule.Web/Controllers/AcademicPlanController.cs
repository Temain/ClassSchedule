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
            var academicPlan = UnitOfWork.Repository<Domain.Models.AcademicPlan>()
                .GetQ(filter: x => x.ProgramOfEducation.Groups.Any(g => g.GroupId == groupId), includeProperties: "CourseSchedules")
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
            
            var viewModel = new GroupInfoViewModel
            {
                GroupId = groupId,
                TheoreticalTrainingWeeks = courseSchedule.TheoreticalTrainingWeeks,
                ExamSessionWeeks = courseSchedule.ExamSessionWeeks,
                WeeksOfHolidays = courseSchedule.WeeksOfHolidays,
                FinalQualifyingWorkWeeks = courseSchedule.FinalQualifyingWorkWeeks,
                StudyTrainingWeeks = courseSchedule.StudyTrainingWeeks,
                PracticalTrainingWeeks = courseSchedule.PracticalTrainingWeeks,
                StateExamsWeeks = courseSchedule.StateExamsWeeks,
                ResearchWorkWeeks = courseSchedule.ResearchWorkWeeks
            };
          
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
                .GetQ(filter: x => x.ProgramOfEducation.Groups.Any(g => g.GroupId == groupId), includeProperties: "CourseSchedules")
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
            int startWeek = 0;
            for (int currentWeek = 0; currentWeek < courseSchedule.Schedule.Length - 1; currentWeek++)
            {
                var currentAbbr = courseSchedule.Schedule[currentWeek];
                var nextAbbr = courseSchedule.Schedule[currentWeek + 1];
                if (currentAbbr != nextAbbr)
                {
                    var series = chartSeries.SingleOrDefault(x => x.Name == currentAbbr + "");
                    if (series == null)
                    {
                        series = new ChartSeriesViewModel
                        {
                            Name = currentAbbr + "",
                            PointWidth = 12
                        };
                        chartSeries.Add(series);
                    }

                    if (series.Data == null)
                    {
                        series.Data = new List<ChartIntervalViewModel>();
                    }

                    //ScheduleAbbreviations abbrValue;
                    //Enum.TryParse(currentAbbr + "", out abbrValue);
                    var abbreviations = Enum.GetValues(typeof(ScheduleAbbreviations));
                    var abbrIndex = Array.IndexOf(abbreviations, (ScheduleAbbreviations)currentAbbr);

                    var interval = new ChartIntervalViewModel
                    {
                        X = abbrIndex,
                        Low = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart, startWeek, 1),
                        High = ScheduleHelpers.DateOfLesson(UserProfile.EducationYear.DateStart, currentWeek, 7)
                    };
                    series.Data.Add(interval);

                    startWeek = currentWeek;
                    currentAbbr = nextAbbr;
                }
            }

            return Json(chartSeries);
        }
    }
}