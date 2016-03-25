using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AcademicPlan.Parser.Models;
using AcademicPlan.Parser.Service;
using ClassSchedule.Domain.DataAccess.Interfaces;
using ClassSchedule.Domain.Models;
using AcademicPlan = ClassSchedule.Domain.Models.AcademicPlan;

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

                if (programOfEducationId == null)
                {
                    return AcademicPlanInfo(hpf);
                }

                try
                {
                    string savedFileName = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(hpf.FileName));
                    hpf.SaveAs(savedFileName);

                    var academicPlan = Parse(savedFileName);
                    academicPlan.ProgramOfEducationId = programOfEducationId.Value;

                    UnitOfWork.Repository<Domain.Models.AcademicPlan>().Insert(academicPlan);
                    UnitOfWork.Save();
                }
                catch (Exception ex)
                {
                    return Content("{\"Status\":\"Error\"}", "application/json");
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

            int yearStartPlan = Convert.ToInt32(document.Plan.PlanTitle.YearStart);

            var properties = new List<string>();           
            if (educationForm != null)
            {
                properties.Add("\"EducationFormId\":\"" + educationForm.EducationFormId + "\"");
                properties.Add("\"EducationFormName\":\"" + educationForm.EducationFormName.Replace("ое","ая").ToLower() + "\"");
            }
            if (direction != null)
            {
                properties.Add("\"EducationDirectionId\":\"" + direction.EducationDirectionId + "\"");
                properties.Add("\"EducationDirectionCode\":\"" + direction.EducationDirectionCode + "\"");
                properties.Add("\"EducationDirectionName\":\"" + direction.EducationDirectionName + "\"");
            }
            properties.Add("\"YearStart\":\"" + yearStartPlan + "\"");
            properties.Add("\"Status\":\"Info\"");
            string result = "{" + String.Join(",", properties) + "}";

            return Content(result, "application/json");
        }

        public Domain.Models.AcademicPlan Parse(string fileName)
        {
            string xml = System.IO.File.ReadAllText(fileName);
            var document = xml.ParseXml<Document>();

             // var dis = document.Plan.Disciplines.Select(x => x.DisciplineName).OrderBy(o => o).Distinct();

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
    }
}