using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QPortal.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        // GET: CreateReport
        public ActionResult CreateReport()
        {
            return View("CreateReport");
        }

    }
}