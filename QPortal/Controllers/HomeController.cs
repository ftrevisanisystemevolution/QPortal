using QlikSenseSession;
using QPortal.Models;
using QPortal.Utility;
using QPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;

namespace QPortal.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            //receive the list of roles
            List<string> roles = new List<string>();
            //uncomment to add roles
            //roles.Add("YA0002");
            roles.Add("YA0001");
            roles.Add("YA0005");
            //roles.Add("YA0006");
            //roles.Add("YA0004");
            //roles.Add("YA0005");
            //roles.Add("YA0001");
            //roles.Add("YA0001");
            //roles.Add("YA0003");
            //roles.Add("test");
            //roles.Add("");

            string path = Server.MapPath(Url.Content("~/Roles.xml"));
            XDocument root = FarmsUtility.GetXmlDocument(path);

            List<FarmRoles> FarmRoles = new List<FarmRoles>(RolesUtility.GetRoles(root, roles));

            if (FarmRoles.Count != 0)
            {
                List<Farms> FarmList = new List<Farms>(RolesUtility.AssignFarmRoles(FarmRoles));

                var viewModel = new FarmsViewModel()
                {
                    FarmList = FarmList
                };

                return View(viewModel);
            }

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