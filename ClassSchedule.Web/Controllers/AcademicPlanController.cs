using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AcademicPlan.Parser.Models;
using AcademicPlan.Parser.Service;
using ClassSchedule.Domain.DataAccess.Interfaces;

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
        public ActionResult Upload()
        {
            foreach (string file in Request.Files)
            {
                HttpPostedFileBase hpf = Request.Files[file] as HttpPostedFileBase;
                if (hpf == null || hpf.ContentLength == 0)
                {
                    continue;
                }

                if (hpf.FileName != null)
                {
                    string savedFileName = Path.Combine(Server.MapPath("~/App_Data"), Path.GetFileName(hpf.FileName));
                    hpf.SaveAs(savedFileName);

                    Parse(savedFileName);
                }
            }

            return Content("{\"name\":\"" + "Name" + "\",\"type\":\"" + "Type" + "\",\"size\":\"" + string.Format("{0} bytes", 555) + "\"}", "application/json");
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