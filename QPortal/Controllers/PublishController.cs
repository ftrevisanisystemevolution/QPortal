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

            ViewBag.AmbitoName = GetCookie("AmbitoName");
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            ViewBag.AmbitoList = GetCookie("AmbitoId") + "|" + GetCookie("NodeId");

            string path = Server.MapPath("~/cert/client.pfx");

            ReportViewModel model = new ReportViewModel();
            List<SenseStream> myStreams;

            QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Server, path);
            List<SenseApplication> notPublishedApps = new List<SenseApplication>();
            string errorMessage = "";
            ViewBag.Error = "";
            var ambito = AmbitiUtility.GetAmbitoById(GetCookie("AmbitoId"));

            if (QRSqlikAPI.GetStreamsByCustomProperty(GetCookie("UserID"), GetCookie("UserDirectory"), "StreamType", "Self-Service", ambito.customproperty, out myStreams, out errorMessage))
            {
                model = ReportViewModel.CreateReportViewModel(notPublishedApps, myStreams);
            }
            else
            {
                ViewBag.Error = "Errore nel reperimento degli streams. " + errorMessage;
            }
            if (string.IsNullOrEmpty(ViewBag.Error) && (myStreams == null || myStreams.Count == 0)) { ViewBag.Error = "Non sei abilitato a nessuno stream dell'ambito Report Self-Service BI su cui distribuire il report."; }
            return View(model);
        }

        public ActionResult GetReportsGrid(string idStream, string desStream)
        {
            ViewBag.idStream = idStream;
            ViewBag.desStream = desStream;
            string path = Server.MapPath("~/cert/client.pfx");
            ReportViewModel model = new ReportViewModel();

            QlikAPI qlikAPI = new QlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
            List<SenseApplication> notPublishedApps;
            List<SenseStream> myStreams = new List<SenseStream>();
            if (qlikAPI.GetMyReportsNotPublished(out notPublishedApps))
            {
                model = ReportViewModel.CreateReportViewModel(notPublishedApps, myStreams);
            }
            return PartialView("ReportsGrid", model);
        }

        public ActionResult Detail(string id, string name, string idstream, string selstream, string AmbitoList)
        {
            ViewBag.id = id;
            ViewBag.name = name;
            ViewBag.idStream = idstream;
            ViewBag.desStream = selstream;
            return View();
        }

        [HttpPost]
        public ActionResult Detail(string AppName, string AppId, string StreamId, string AmbitoList, string StreamName, string dummy)
        {
            AppToPublishViewModel model = new AppToPublishViewModel();
            model.AppId = AppId;
            model.AppName = AppName;
            model.StreamName = StreamName;
            model.StreamID = StreamId;

            ViewBag.AmbitoList = AmbitoList;

            string path = Server.MapPath("~/cert/client.pfx");

            QRSSenseAppDetail detail = new QRSSenseAppDetail();
            QRSSenseApp newApp = new QRSSenseApp();
            
            // Prendo la lista delle app pubblicate
            List<SenseApplication> publishedApps;
            QlikAPI qlikAPIMaster = new QlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
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
        }


        [HttpPost]
        public ActionResult ToPublish(string AppId, string AppName, string OverwriteRequired, string StreamID, string StreamName, string AppToOverwriteId, string checkOverwrite, string AmbitoList)
        {
            string path = Server.MapPath("~/cert/client.pfx");
            var ambito = AmbitiUtility.GetAmbitoById(GetCookie("AmbitoId"));
            if (OverwriteRequired.ToLower() == "false")
            {
                // Duplico l'app
                QRSSenseApp newApp = new QRSSenseApp();
                string errorMessage = "";
                QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Server, path);
                QRSqlikAPI.CopyApp(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, AppName, out newApp, out errorMessage);

                // Pubblico l'app
                QlikAPI qlikAPI = new QlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Link, ambito.superuserid, ambito.superuserdom, path);
                qlikAPI.PublishApp(AppId, AppName, StreamID);
            }
            else
            {
                string errorMessage = "";                
                QlikAPI qlikAPI = new QlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Link, ambito.superuserid, ambito.superuserdom, path);
                qlikAPI.ReplaceApp(AppId, AppToOverwriteId);
            }
            
            return RedirectToAction("Hub", "Home", new { AmbitoList = AmbitoList });
        }
    }
}