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

                string savedFileName = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(hpf.FileName));
                hpf.SaveAs(savedFileName);

                if (programOfEducationId == null)
                {
                    return AcademicPlanInfo(savedFileName);
                }

                Parse(savedFileName);               
            }

            return Content("{\"Status\":\"Success\"}", "application/json");
        }

        public ActionResult AcademicPlanInfo(string fileName)
        {
            string xml = System.IO.File.ReadAllText(fileName);
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

        public void Parse(string fileName)
        {
            string xml = System.IO.File.ReadAllText(fileName);
            var document = xml.ParseXml<Document>();

            var academicPlan = new Domain.Models.AcademicPlan
            {
                AcademicPlanName = document.Plan.PlanTitle.PlanFullName,
                NumberOfSemesters = document.Plan.PlanTitle.NumberOfSemesters
            };


        }
    }
}