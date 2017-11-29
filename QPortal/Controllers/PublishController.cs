using APIInterface;
using APIInterface.Model;
using QPortal.Utility;
using QPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace QPortal.Controllers
{
    public class PublishController : BaseController
    {
        // GET: Publish
        public ActionResult Index()
        {
            ViewBag.PageType = "InternalAction";

            ViewBag.FarmName = GetCookie("FarmName");
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            ViewBag.FarmList = GetCookie("FarmId") + "|" + GetCookie("NodeId");

            string path = Server.MapPath("~/cert/client.pfx");

            ReportViewModel model = new ReportViewModel();
            List<SenseStream> myStreams;

            QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
            List<SenseApplication> notPublishedApps = new List<SenseApplication>();
            string errorMessage = "";
            ViewBag.Error = "";

            if (QRSqlikAPI.GetStreamsByCustomProperty(GetCookie("UserID"), GetCookie("UserDirectory"), "StreamType", "Self-Service", out myStreams, out errorMessage))
            {
                model = ReportViewModel.CreateReportViewModel(notPublishedApps, myStreams);
            }
            else
            {
                ViewBag.Error = "Errore nel reperimento degli streams. " + errorMessage;
            }
            return View(model);
        }

        public ActionResult GetReportsGrid(string idStream, string desStream)
        {
            ViewBag.idStream = idStream;
            ViewBag.desStream = desStream;
            string path = Server.MapPath("~/cert/client.pfx");
            ReportViewModel model = new ReportViewModel();

            QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
            List<SenseApplication> notPublishedApps;
            List<SenseStream> myStreams = new List<SenseStream>();
            if (qlikAPI.GetMyReportsNotPublished(out notPublishedApps))
            {
                model = ReportViewModel.CreateReportViewModel(notPublishedApps, myStreams);
            }
            return PartialView("ReportsGrid", model);
        }

        public ActionResult Detail(string id, string name, string idstream, string selstream, string FarmList)
        {
            ViewBag.id = id;
            ViewBag.name = name;
            ViewBag.idStream = idstream;
            ViewBag.desStream = selstream;
            return View();
        }

        [HttpPost]
        public ActionResult Detail(string AppName, string AppId, string StreamId, string FarmList, string StreamName, string dummy)
        {
            AppToPublishViewModel model = new AppToPublishViewModel();
            model.AppId = AppId;
            model.AppName = AppName;
            model.StreamName = StreamName;
            model.StreamID = StreamId;

            ViewBag.FarmList = FarmList;

            string path = Server.MapPath("~/cert/client.pfx");

            QRSSenseAppDetail detail = new QRSSenseAppDetail();
            QRSSenseApp newApp = new QRSSenseApp();
            //string errorMessage = "";
            //// Prendo i dettagli dell'app da pubblicare
            //QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
            //QRSqlikAPI.GetAppDetail(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, out detail, out errorMessage);

            // Prendo la lista delle app pubblicate
            List<SenseApplication> publishedApps;
            QlikAPI qlikAPIMaster = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, ConfigurationManager.AppSettings["QlikUser"], ConfigurationManager.AppSettings["QlikUserDirectory"], path);
            qlikAPIMaster.GetPublishedApps(out publishedApps);

            model.OverwriteRequired = false;
            // Controllo se c'è già un'app nello stream che si chiama così
            foreach (var publishedApp in publishedApps)
            {
                if (publishedApp.StreamID == StreamId && publishedApp.Name == AppName)
                {
                    model.OverwriteRequired = true;
                    model.AppToOverwriteId = publishedApp.AppId;
                }
            }

            return View("ToPublish", model);

            //QRSqlikAPI.CopyApp(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, detail.name, out newApp, out errorMessage);

            //QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
            //qlikAPI.PublishApp(AppId, AppName, StreamId);
            //string appToDelete = "";
            //DateTime publishTime = DateTime.MaxValue;
            //int count = 0;
            //foreach (var publishedApp in publishedApps)
            //{
            //    if (publishedApp.StreamID == StreamId && publishedApp.Name == AppName)
            //    {
            //        count++;
            //        if (publishedApp.PublishDate.CompareTo(publishTime) < 0)
            //        {
            //            publishTime = publishedApp.PublishDate;
            //            appToDelete = publishedApp.AppId;
            //        }
            //    }
            //}
            //if (count ==2 && !string.IsNullOrEmpty(appToDelete)) { qlikAPIMaster.DeleteApp(appToDelete); }
            //return RedirectToAction("Hub", "Home", new { FarmList = FarmList });
        }


        [HttpPost]
        public ActionResult ToPublish(string AppId, string AppName, string OverwriteRequired, string StreamID, string StreamName, string AppToOverwriteId, string checkOverwrite, string FarmList)
        {
            string path = Server.MapPath("~/cert/client.pfx");

            if (OverwriteRequired.ToLower() == "false")
            {
                // Duplico l'app
                QRSSenseApp newApp = new QRSSenseApp();
                string errorMessage = "";
                QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
                QRSqlikAPI.CopyApp(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, AppName, out newApp, out errorMessage);

                // Pubblico l'app
                QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                qlikAPI.PublishApp(AppId, AppName, StreamID);
            }
            else
            {
                string errorMessage = "";
                //QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
                //QRSqlikAPI.ReplaceApp(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, AppToOverwriteId, out errorMessage);
                QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                qlikAPI.ReplaceApp(AppId, AppToOverwriteId);
            }
            //QRSSenseAppDetail detail = new QRSSenseAppDetail();
            //QRSSenseApp newApp = new QRSSenseApp();
            //string errorMessage = "";
            //// Prendo i dettagli dell'app da pubblicare
            //QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
            //QRSqlikAPI.GetAppDetail(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, out detail, out errorMessage);

            //// Prendo la lista delle app pubblicate
            //List<SenseApplication> publishedApps;
            //QlikAPI qlikAPIMaster = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, ConfigurationManager.AppSettings["QlikUser"], ConfigurationManager.AppSettings["QlikUserDirectory"], path);
            //qlikAPIMaster.GetPublishedApps(out publishedApps);

            //model.OverwriteRequired = false;
            //// Controllo se c'è già un'app nello stream che si chiama così
            //foreach (var publishedApp in publishedApps)
            //{
            //    if (publishedApp.StreamID == StreamId && publishedApp.Name == AppName)
            //    {
            //        model.OverwriteRequired = true;
            //        model.AppToOverwriteId = publishedApp.AppId;
            //    }
            //}

            //return View("ToPublish", model);

            //QRSqlikAPI.CopyApp(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, detail.name, out newApp, out errorMessage);

            //QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
            //qlikAPI.PublishApp(AppId, AppName, StreamId);
            //string appToDelete = "";
            //DateTime publishTime = DateTime.MaxValue;
            //int count = 0;
            //foreach (var publishedApp in publishedApps)
            //{
            //    if (publishedApp.StreamID == StreamId && publishedApp.Name == AppName)
            //    {
            //        count++;
            //        if (publishedApp.PublishDate.CompareTo(publishTime) < 0)
            //        {
            //            publishTime = publishedApp.PublishDate;
            //            appToDelete = publishedApp.AppId;
            //        }
            //    }
            //}
            //if (count == 2 && !string.IsNullOrEmpty(appToDelete)) { qlikAPIMaster.DeleteApp(appToDelete); }
            return RedirectToAction("Hub", "Home", new { FarmList = FarmList });
        }
    }
}