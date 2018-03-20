using APIInterface;
using APIInterface.Model;
using QPortal.Models;
using QPortal.Utility;
using QPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QPortal.Controllers
{
    public class ShareController : BaseController
    {
        // GET: Share
        public ActionResult Index()
        {
            ViewBag.PageType = "InternalAction";

            ViewBag.AmbitoName = GetCookie("AmbitoName");
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            ViewBag.AmbitoList = GetCookie("AmbitoId") + "|" + GetCookie("NodeId");

            string path = Server.MapPath("~/cert/client.pfx");
            ReportViewModel model = new ReportViewModel();

            // Prendo il nodo dei Report Distribuiti
            var ambito = AmbitiUtility.GetAmbitoById(new List<string>() { GetCookie("AmbitoId") }).FirstOrDefault();
            var distrNode = (from f in ambito.Nodes where f.NodeType == "D" select f).FirstOrDefault();
            //

            // Prendo tutti gli stream del nodo dei report distribuiti
            QRSQlikAPI QRSqlikAPIMaster = new QRSQlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), distrNode.Id.ToString()).Server, path);
            List<SenseApplication> publishedApps = new List<SenseApplication>();
            List<SenseStream> streams;
            string errorMessage = "";
            //if (QRSqlikAPIMaster.GetStreamsBuCustomProperty(ConfigurationManager.AppSettings["QlikUser"], ConfigurationManager.AppSettings["QlikUserDirectory"], "StreamType", "Production", out streams, out errorMessage))
            if (QRSqlikAPIMaster.GetStreamsByCustomProperty(GetCookie("UserID"), GetCookie("UserDirectory"), "StreamType", "Production", out streams, out errorMessage))
            {
                model = ReportViewModel.CreateReportViewModel(publishedApps, streams);
            }
            else
            {
                ViewBag.Error = "Errore nel reperimento degli stream di distribuzione." + errorMessage;
            }
            if (string.IsNullOrEmpty(ViewBag.Error) && (streams == null || streams.Count == 0)) { ViewBag.Error = "Non sei abilitato a nessuno stream dell'ambito Report Distribuiti su cui distribuire il report."; }
            return View(model);
        }
        public ActionResult GetReportsGrid(string idStream, string desStream)
        {
            ViewBag.idStream = idStream;
            ViewBag.desStream = desStream;

            string path = Server.MapPath("~/cert/client.pfx");
            ReportViewModel model = new ReportViewModel();
            
            List<SenseApplication> publishedApps;
            List<SenseStream> streams = new List<SenseStream>();

            string errorMessage = "";

            // Prendo gli stream di self-service e creo una lista
            QRSQlikAPI QRSqlikAPIMaster = new QRSQlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Server, path);
            QRSqlikAPIMaster.GetStreamsByCustomProperty(GetCookie("UserID"), GetCookie("UserDirectory"), "StreamType", "Self-Service", out streams, out errorMessage);
            List<string> streamIdList = new List<string>();
            if (streams != null)
            {
                streamIdList = (from s in streams select s.Id).ToList();
            }
            //

            // Prendo le app pubblicate in self-service controllando che siano nella lista creata prima
            QlikAPI qlikAPIMaster = new QlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
            if (qlikAPIMaster.GetPublishedAppsInSelectedStreams(streamIdList ,out publishedApps))
            {
                foreach (var app in publishedApps)
                {
                    errorMessage = "";
                    QRSSenseAppDetail appDetail = new QRSSenseAppDetail();
                    //if (QRSqlikAPIMaster.GetAppDetail(ConfigurationManager.AppSettings["QlikUser"], ConfigurationManager.AppSettings["QlikUserDirectory"], app.AppId, out appDetail, out errorMessage))
                    if (QRSqlikAPIMaster.GetAppDetail(GetCookie("UserID"), GetCookie("UserDirectory"), app.AppId, out appDetail, out errorMessage))
                    {
                        app.OwnerUserName = appDetail.owner.name;
                    }
                }
                model = ReportViewModel.CreateReportViewModel(publishedApps, streams);
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
            
            // Prendo il nodo dei Report Distribuiti
            var ambito = AmbitiUtility.GetAmbitoById(new List<string>() { GetCookie("AmbitoId") }).FirstOrDefault();
            var distrNode = (from f in ambito.Nodes where f.NodeType == "D" select f).FirstOrDefault();
            //

            // Prendo le app pubblicate nel nodo dei Report Distribuiti
            List<SenseApplication> publishedApps;
            QlikAPI qlikAPIMaster = new QlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), distrNode.Id.ToString()).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
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

            return View("ToShare", model);
        }

        [HttpPost]
        public ActionResult ToShare(string AppId, string AppName, string OverwriteRequired, string StreamID, string StreamName, string AppToOverwriteId, string checkOverwrite, string AmbitoList)
        {
            string path = Server.MapPath("~/cert/client.pfx");

            // Prendo il nodo dei Report Distribuiti
            var ambito = AmbitiUtility.GetAmbitoById(GetCookie("AmbitoId"));
            var distrNode = (from f in ambito.Nodes where f.NodeType == "D" select f).FirstOrDefault();
            //

            // Duplico l'app
            QRSSenseApp newApp = new QRSSenseApp();
            string errorMessage = "";
            QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Server, path);
            QRSqlikAPI.CopyApp(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, AppName, out newApp, out errorMessage);
            bool publishResult = true;

            if (OverwriteRequired.ToLower() == "false")
            {                
                // Pubblico l'app                
                QlikAPI qlikAPI = new QlikAPI(AmbitiUtility.GetAmbitoNode(GetCookie("AmbitoId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                publishResult = qlikAPI.PublishApp(newApp.id, AppName, StreamID, out errorMessage);                
            }
            else
            {
                // Rimpiazzo l'app
                QlikAPI qlikAPI = new QlikAPI(ambito.centralnode, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                publishResult = qlikAPI.ReplaceApp(newApp.id, AppToOverwriteId, out errorMessage);

                qlikAPI.DeleteApp(newApp.id);

                
            }

            return RedirectToAction("Hub", "Home", new { AmbitoList = AmbitoList });
        }
    }
}