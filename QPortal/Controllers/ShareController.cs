﻿using APIInterface;
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

            ViewBag.FarmName = GetCookie("FarmName");
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            ViewBag.FarmList = GetCookie("FarmId") + "|" + GetCookie("NodeId");

            string path = Server.MapPath("~/cert/client.pfx");
            ReportViewModel model = new ReportViewModel();

            // Prendo il nodo dei Report Distribuiti
            var farm = FarmsUtility.GetFarmsById(new List<string>() { GetCookie("FarmId") }).FirstOrDefault();
            var distrNode = (from f in farm.Nodes where f.NodeType == "D" select f).FirstOrDefault();
            //

            // Prendo tutti gli stream del nodo dei report distribuiti
            QRSQlikAPI QRSqlikAPIMaster = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), distrNode.Id.ToString()).Server, path);
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
            QRSQlikAPI QRSqlikAPIMaster = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
            QRSqlikAPIMaster.GetStreamsByCustomProperty(GetCookie("UserID"), GetCookie("UserDirectory"), "StreamType", "Self-Service", out streams, out errorMessage);
            List<string> streamIdList = new List<string>();
            if (streams != null)
            {
                streamIdList = (from s in streams select s.Id).ToList();
            }
            //

            // Prendo le app pubblicate in self-service controllando che siano nella lista creata prima
            QlikAPI qlikAPIMaster = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
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
            
            // Prendo il nodo dei Report Distribuiti
            var farm = FarmsUtility.GetFarmsById(new List<string>() { GetCookie("FarmId") }).FirstOrDefault();
            var distrNode = (from f in farm.Nodes where f.NodeType == "D" select f).FirstOrDefault();
            //

            // Prendo le app pubblicate nel nodo dei Report Distribuiti
            List<SenseApplication> publishedApps;
            QlikAPI qlikAPIMaster = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), distrNode.Id.ToString()).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
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
        public ActionResult ToShare(string AppId, string AppName, string OverwriteRequired, string StreamID, string StreamName, string AppToOverwriteId, string checkOverwrite, string FarmList)
        {
            string path = Server.MapPath("~/cert/client.pfx");

            // Prendo il nodo dei Report Distribuiti
            var farm = FarmsUtility.GetFarmById(GetCookie("FarmId"));
            var distrNode = (from f in farm.Nodes where f.NodeType == "D" select f).FirstOrDefault();
            //

            // Duplico l'app
            QRSSenseApp newApp = new QRSSenseApp();
            string errorMessage = "";
            QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Server, path);
            //QRSQlikAPI QRSqlikAPI = new QRSQlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), distrNode.Id.ToString()).Server, path);
            QRSqlikAPI.CopyApp(GetCookie("UserID"), GetCookie("UserDirectory"), AppId, AppName, out newApp, out errorMessage);
            bool publishResult = true;

            if (OverwriteRequired.ToLower() == "false")
            {                
                // Pubblico l'app                
                QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                publishResult = qlikAPI.PublishApp(newApp.id, AppName, StreamID, out errorMessage);                
            }
            else
            {
                // Dovrebbe funzionare così ma non funziona!!!               

                // Rimpiazzo l'app
                //QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                QlikAPI qlikAPI = new QlikAPI(farm.centralnode, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                publishResult = qlikAPI.ReplaceApp(newApp.id, AppToOverwriteId, out errorMessage);

                // Elimino l'app duplicata
                qlikAPI.DeleteApp(newApp.id);


                ////// Allora faccio così  

                //List<SenseApplication> publishedApps;
                //QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
                //publishResult = qlikAPI.PublishApp(newApp.id, AppName, StreamID, out errorMessage);
                //qlikAPI.GetPublishedApps(out publishedApps);
                //string appToDelete = "";
                //DateTime publishTime = DateTime.MaxValue;
                //int count = 0;
                //foreach (var publishedApp in publishedApps)
                //{
                //    if (publishedApp.StreamID == StreamID && publishedApp.Name == AppName)
                //    {
                //        count++;
                //        if (publishedApp.PublishDate.CompareTo(publishTime) < 0)
                //        {
                //            publishTime = publishedApp.PublishDate;
                //            appToDelete = publishedApp.AppId;
                //        }
                //    }
                //}
                //if (count == 2 && !string.IsNullOrEmpty(appToDelete)) { qlikAPI.DeleteApp(appToDelete); }

            }
            //ViewBag.UserID = GetCookie("UserID");
            //ViewBag.UserDirectory = GetCookie("UserDirectory");
            //ViewBag.newAppId = newApp.id;
            //ViewBag.AppName = AppName;
            //ViewBag.StreamID = StreamID;
            //ViewBag.distrNode = distrNode.Id.ToString();
            //ViewBag.OverwriteRequired = AppToOverwriteId;
            //ViewBag.Link = FarmsUtility.GetFarmNode(GetCookie("FarmId"), distrNode.Id.ToString()).Link;
            //ViewBag.publishResult = publishResult.ToString();
            //ViewBag.errorMessage = errorMessage;
            //return View("ToShare2");
            return RedirectToAction("Hub", "Home", new { FarmList = FarmList });
        }
    }
}