using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
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
            }

            return Content("{\"name\":\"" + "Name" + "\",\"type\":\"" + "Type" + "\",\"size\":\"" + string.Format("{0} bytes", 555) + "\"}", "application/json");
        }
    }
}