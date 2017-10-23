using QlikSenseSession;
using QPortal.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QPortal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public ActionResult Hub()
        {
            string Server = "desktop-29ba4mu";
            string VP = "vp";
            string UserID = "bixth";
            string UserDirectory = "desktop-29ba4mu";

            QSession qSession = new QSession("POST", Server, VP, UserID, UserDirectory);
            qSession.OpenSession(HttpContext.ApplicationInstance.Context);
            Request.Cookies.Add(qSession.GetCookie());
            Response.Cookies.Add(qSession.GetCookie());

            return Redirect(qSession.GetHubURL());
        }

        public ActionResult TestHeader()
        {
            ViewBag.HeadersKeysCount = Request.Headers.AllKeys.Count();
            ViewBag.HeadersKeys = Request.Headers.AllKeys;
            ViewBag.Headers = Request.Headers;
            return View();
        }
    }
}