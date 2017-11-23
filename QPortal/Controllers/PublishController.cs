using APIInterface;
using APIInterface.Model;
using QPortal.Utility;
using QPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
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
            QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
            List<SenseApplication> notPublishedApps = new List<SenseApplication>();
            List<SenseStream> myStreams;
            if (qlikAPI.GetMyStreams(out myStreams))
            {
                model = ReportViewModel.CreateReportToPublishViewModel(notPublishedApps, myStreams);
            }
            else
            {
                ViewBag.Error = "Errore nel reperimento dei tuoi documenti.";
            }
            return View(model);
        }

        public ActionResult GetReportsGrid(string idCountry)
        {
            string path = Server.MapPath("~/cert/client.pfx");
            ReportViewModel model = new ReportViewModel();
            QlikAPI qlikAPI = new QlikAPI(FarmsUtility.GetFarmNode(GetCookie("FarmId"), GetCookie("NodeId")).Link, GetCookie("UserID"), GetCookie("UserDirectory"), path);
            List<SenseApplication> notPublishedApps;
            List<SenseStream> myStreams = new List<SenseStream>();
            if (qlikAPI.GetMyReportsNotPublished(out notPublishedApps))
            {
                model = ReportViewModel.CreateReportToPublishViewModel(notPublishedApps, myStreams);
            }
            return PartialView("ReportsGrid", model);
        }
    }
}