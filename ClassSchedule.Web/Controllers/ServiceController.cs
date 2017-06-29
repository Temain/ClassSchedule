using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ClassSchedule.Domain.Context;
using ClassSchedule.Domain.DataAccess.Interfaces;

namespace ClassSchedule.Web.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly ApplicationDbContext _context;

        public ServiceController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public ActionResult AuditoriumSchedule()
        {
            return View();
        }

        [HttpGet]
        public ActionResult TeacherSchedule()
        {
            return View();
        }

        [HttpGet]
        public ActionResult AvailableAuditoriums()
        {
            return View();
        }
    }
}