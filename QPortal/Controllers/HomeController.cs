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
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {

            List<string> roles = new List<string>();

#if DEBUG
            if (!Request.Cookies.AllKeys.Contains("IsAuthenticated"))
            {
                SetCookie("IsAuthenticated", "true");

                SetCookie("UserID", "bixth");
                SetCookie("UserDirectory", "desktop-29ba4mu");
                SetCookie("UserIdentity", "Fabrizio Trevisani");
                //SetCookie("UserID", "SYSSPIMI");
                //SetCookie("UserDirectory", "U0J4169");
                //SetCookie("UserIdentity", "Davide Carbone");
            }

            ViewBag.UserIdentity = GetCookie("UserIdentity");
            roles.Add("YA2C04");

#else
            
            if (!Request.Cookies.AllKeys.Contains("IsAuthenticated"))
            {
                SetCookie("IsAuthenticated", "true");
            
                SetCookie("UserID", "SYSSPIMI");
                SetCookie("UserDirectory", "U0J4169");
                SetCookie("UserIdentity", "Davide Carbone");
            }

            ViewBag.UserIdentity = GetCookie("UserIdentity");
            roles.Add("YA2C04");


            //UserProfile userProfile = null;
            //UserRequest userRequest = new UserRequest(Request.Headers.AllKeys.Count(), Request.Headers.AllKeys, Request.Headers);

            //if (userRequest.IsValid)
            //{
            //    SetCookie("IsAuthenticated", "true");
            //    SetCookie("UserID", userRequest.UserID);
            //    SetCookie("UserDirectory", userRequest.UserDirectory);
            //    SetCookie("UserIdentity", userRequest.UserIdentity);
            //    SetCookie("SWAProfileID", userRequest.SWAProfileID);
            //    SetCookie("LinkSWP", userRequest.LinkSWP);
            //    ViewBag.UserIdentity = userRequest.UserIdentity;
            //    userProfile = new UserProfile(userRequest.SWAProfileID, userRequest.LinkSWP);
            //}
            //else
            //{
            //    if (!Request.Cookies.AllKeys.Contains("IsAuthenticated"))
            //    {
            //        SetCookie("UserIdentity", "Utenza non riconosciuta");
            //        ViewBag.UserIdentity = GetCookie("UserIdentity");
            //    }
            //    else
            //    {
            //        userProfile = new UserProfile(GetCookie("SWAProfileID"), GetCookie("LinkSWP"));
            //    }

            //    if (userProfile != null && userProfile.IsValid)
            //    {
            //        foreach (var role in userProfile.Profiles)
            //        {
            //            roles.Add(role.RoleID);
            //        }
            //    }
            //}
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
            string farmNode = Request["FarmList"];
            if (string.IsNullOrEmpty(farmNode) || !farmNode.Contains("|")) { return RedirectToAction("Index");  }
            int farmId = Convert.ToInt32(farmNode.Split('|')[0]);
            int nodeId = Convert.ToInt32(farmNode.Split('|')[1]);
            var node = FarmsUtility.GetFarmNode(farmId, nodeId);
            var farm = FarmsUtility.GetFarmsById(new List<string>() { farmId.ToString() });
            ViewBag.FarmName = farm.FirstOrDefault().Name + " - " + node.Name;
            SetCookie("FarmName", ViewBag.FarmName);
            ViewBag.Server = node.Server;
            ViewBag.VirtualProxy = node.VirtualProxy;
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            return View();
        }

        public ActionResult OpenFrame(string server, string vp)
        {
            string UserID = GetCookie("UserID");
            string UserDirectory = GetCookie("UserDirectory");
            try
            {
                QSession qSession = new QSession("POST", server, vp, UserID, UserDirectory);
                qSession.OpenSession(HttpContext.ApplicationInstance.Context);
                Request.Cookies.Add(qSession.GetCookie());
                Response.Cookies.Add(qSession.GetCookie());
                return Redirect(qSession.GetHubURL());
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("{0} - {1}", UserID, UserDirectory), ex);
            }

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