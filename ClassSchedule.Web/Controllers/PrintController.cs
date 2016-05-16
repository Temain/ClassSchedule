using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.DataAccess.Interfaces;

namespace ClassSchedule.Web.Controllers
{
    public class PrintController : BaseController
    {
        public PrintController(IUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }

        [HttpGet]
        public ActionResult GroupSetSchedule()
        {
            return View();
        }

        [HttpGet]
        public ActionResult ChairSchedule()
        {
            return View();
        }
    }
}