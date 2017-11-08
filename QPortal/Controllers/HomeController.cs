using Profiler;
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
#if DEBUG
            Session["UserID"] = "bixth";
            Session["UserDirectory"] = "desktop-29ba4mu";
#else
            UserRequest userRequest = new UserRequest(Request.Headers.AllKeys.Count(), Request.Headers.AllKeys, Request.Headers);
            Session["UserID"] = userRequest.UserID;
            Session["UserDirectory"] = userRequest.UserDirectory;
            UserProfile userProfile = new UserProfile(userRequest.SWAProfileID, userRequest.LinkSWP);
            foreach(var role in userProfile.Profiles)
            {
                roles.Add(role.RoleID);
            }
#endif
            string path = Server.MapPath(Url.Content(FilePaths.RolesXML));
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

        public ActionResult Hub(string server, string vp)
        {
            string UserID = Session["UserID"].ToString();
            string UserDirectory = Session["UserDirectory"].ToString();

            QSession qSession = new QSession("POST", server, vp, UserID, UserDirectory);
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
            UserRequest userRequest = new UserRequest(Request.Headers.AllKeys.Count(), Request.Headers.AllKeys, Request.Headers);
            ViewBag.IsValid = userRequest.IsValid;
            ViewBag.UserDirectory = userRequest.UserDirectory;
            ViewBag.UserID = userRequest.UserID;
            ViewBag.LinkSWP = userRequest.LinkSWP;
            UserProfile userProfile = new UserProfile(userRequest.SWAProfileID, userRequest.LinkSWP + "ServiceSoap");
            //return View(userProfile.Profiles);
            ViewBag.ProfileCount = userProfile.IsValid ? userProfile.Profiles.Count : 0;
            return View();
        }
    }
}