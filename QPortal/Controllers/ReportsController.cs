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
            ViewBag.PageType = "InternalAction";

            ViewBag.FarmName = GetCookie("FarmName");
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            ViewBag.FarmList = GetCookie("FarmId") + "|" + GetCookie("NodeId");
            return View();
        }        
        

        [HttpPost]
        public ActionResult CreateReport(string Name, string Description, string FarmList)
        {
            ViewBag.PageType = "InternalAction";

            string appId = "";

            string path = Server.MapPath("~/cert/client.pfx");

            QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
            //QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link);
            qlikAPI.CreateApp(Name, Description, out appId);
            return RedirectToAction("Hub", "Home", new { FarmList = FarmList });
            //return Redirect("https://desktop-29ba4mu/vp/sense/app/" + appId);
        }

        //GET: Pubblicazione
        //public ActionResult Pubblicazione()
        //{
        //    List<string> streams = new List<string>();
        //    streams.Add("Stream 1");
        //    streams.Add("Stream 2");
        //    streams.Add("Stream 3");

        //    Report zero = new Report();
        //    zero.Id = "0";
        //    zero.Owner = "Giacomo Guilizzoni";
        //    zero.ReportName = "Founder & CEO";

        //    Report one = new Report();
        //    one.Id = "1";
        //    one.Owner = "Giacomo Guilizzoni";
        //    one.ReportName = "CFO";

        //    Report two = new Report();
        //    two.Id = "2";
        //    two.Owner = "Test PErson";
        //    two.ReportName = "NAN";

        //    ReportViewModel rModel = new ReportViewModel();
        //    rModel.Streams = new List<string>(streams);

        //    List<Report> rList = new List<Report>();
        //    rList.Add(zero);
        //    rList.Add(one);
        //    rList.Add(two);
        //    rModel.Reports = rList;

        //    //string partialView = "_ReportPerPubblicazione";
        //    return View("Pubblicazione",rModel);
        //}

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