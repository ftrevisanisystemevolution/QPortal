using QPortal.ViewModels;
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

        //GET: Pubblicazione
        public ActionResult Pubblicazione()
        {
            List<string> streams = new List<string>();
            streams.Add("Stream 1");
            streams.Add("Stream 2");
            streams.Add("Stream 3");

            Report zero = new Report();
            zero.Id = 0;
            zero.Owner = "Giacomo Guilizzoni";
            zero.ReportName = "Founder & CEO";

            Report one = new Report();
            one.Id = 1;
            one.Owner = "Giacomo Guilizzoni";
            one.ReportName = "CFO";

            Report two = new Report();
            two.Id = 2;
            two.Owner = "Test PErson";
            two.ReportName = "NAN";

            ReportViewModel rModel = new ReportViewModel();
            rModel.Streams = new List<string>(streams);

            List<Report> rList = new List<Report>();
            rList.Add(zero);
            rList.Add(one);
            rList.Add(two);
            rModel.Reports = rList;

            //string partialView = "_ReportPerPubblicazione";
            return View("Pubblicazione",rModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Publish(ReportViewModel report)
        {
            if (!ModelState.IsValid)
            {
                return Content("Not");
            }

            //if (!(String.IsNullOrWhiteSpace(report.Owner) && String.IsNullOrWhiteSpace(report.ReportName)))
            //    return Content(report.Owner + " - " + report.ReportName);
            else
            {
                return Content("Not");
            }

            //return RedirectToAction("Index", "Reports");
        }
    }
}