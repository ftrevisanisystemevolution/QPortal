using APIInterface;
using APIInterface.Model;
using QPortal.Models;
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

            CreateReport model = new CreateReport() { Name = "", Description = "" };
            model.TemplateItems = new List<ReportTemplate>();
            string path = Server.MapPath("~/cert/client.pfx");
            var farm = FarmsUtility.GetFarmById(GetCookie("FarmId"));
            QRSQlikAPI qrsQlikApi = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
            List<SenseStream> myStreams;
            string errorMessage = "";
            qrsQlikApi.GetStreamsByCustomProperty(GetCookie("UserID"), GetCookie("UserDirectory"), "StreamType", "Template", out myStreams, out errorMessage);
            if (myStreams != null && myStreams.Count > 0)
            {
                QlikAPI qlikApi = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, farm.superuserid, farm.superuserdom, path);
                List<SenseApplication> apps = new List<SenseApplication>();
                if (qlikApi.GetPublishedAppsInSelectedStreams(myStreams.Select(s=>s.Id).ToList(), out apps))
                {
                    if (apps != null && apps.Count > 0)
                    {
                        foreach (var app in apps)
                        {
                            model.TemplateItems.Add(new ReportTemplate() { TemplateStreamID = app.StreamID, TemplateID = app.AppId, TemplateDescription = app.Name });
                        }
                    }
                }
            }
            return View(model);
        }        
        

        [HttpPost]
        public ActionResult CreateReport(string Name, string Description, string FarmList, string SelectedTemplate)
        {
            ViewBag.PageType = "InternalAction";

            string appId = "";

            string path = Server.MapPath("~/cert/client.pfx");

            if (string.IsNullOrEmpty(SelectedTemplate) || SelectedTemplate == "void")
            {
                QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                qlikAPI.CreateApp(Name, Description, out appId);
            }
            else
            {
                // Duplico l'app
                QRSSenseApp newApp = new QRSSenseApp();
                string errorMessage = "";
                QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
                QRSqlikAPI.CopyApp(GetCookie("UserID"), GetCookie("UserDirectory"), SelectedTemplate, Name, out newApp, out errorMessage);
                QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                qlikAPI.RenameApp(newApp.id, Description);
            }
            return RedirectToAction("Hub", "Home", new { FarmList = FarmList });
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