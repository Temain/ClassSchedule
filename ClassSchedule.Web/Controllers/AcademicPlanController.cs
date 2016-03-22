using System;
using System.Collections.Generic;
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

        public ActionResult Load()
        {
            return View();
        }
    }
}