using APIInterface;
using QPortal.Utility;
using QPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QPortal.Controllers
{
    public class ReportsController : BaseController
    {
        // GET: Reports
        public ActionResult Index()
        {
            return View();
        }

        // GET: CreateReport
        public ActionResult CreateReport()
        {
            ViewBag.FarmName = GetCookie("FarmName");
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            ViewBag.FarmList = GetCookie("FarmId") + "|" + GetCookie("NodeId");
            return View();
        }

        [HttpPost]
        public ActionResult CreateReport(string Name, string Description, string FarmList)
        {
            string appId = "";
            
            QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link);
            qlikAPI.CreateApp(Name, Description, out appId);
            return RedirectToAction("Hub", "Home", new { FarmList = FarmList });
            //return Redirect("https://desktop-29ba4mu/vp/sense/app/" + appId);
        }

        //GET: Pubblicazione
        public ActionResult Pubblicazione()
        {
            List<string> streams = new List<string>();
            streams.Add("Stream 1");
            streams.Add("Stream 2");
            streams.Add("Stream 3");

            Report one = new Report();
            one.Owner = "Giacomo Guilizzoni";
            one.ReportName = "Founder & CEO";

            Report two = new Report();
            two.Owner = "Giacomo Guilizzoni";
            two.ReportName = "CFO";

            Report three = new Report();
            three.Owner = "Test PErson";
            three.ReportName = "NAN";

            ReportViewModel rModel = new ReportViewModel();
            rModel.Streams = new List<string>(streams);

            List<Report> rList = new List<Report>();
            rList.Add(one);
            rList.Add(two);
            rList.Add(three);
            rModel.Reports = rList;

            //string partialView = "_ReportPerPubblicazione";
            return View("ReportPerPubblicazione",rModel);
        }

    }
}