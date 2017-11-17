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
        private List<string> _roles;
        public List<string> Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        public HomeController()
        {
            //populate roles here?
            Roles = new List<string>();
            Roles.Add("YA2C05");
        }

        // this needs too be activated on every page that displays the secondary navigation request, except Home-Index
        // a new controller can be created, or leave it here
        // if it will be moved, the Roles needs to be passed to it as well.. store them in cookie? or get them at each page load? or constructor
        [ChildActionOnly]
        public ActionResult SecondayNavbar()
        {
            //get farm from FarmName cookie
            //string FarmName = GetCookie("FarmName"); //commented for testing purposes

            //string FarmName = "Report Distribuiti";
            string FarmName = "Self-service BI";

            //create a model and pass it to the view so that you don't need to set a Role cookie
            SecondaryNavBarModel model = new SecondaryNavBarModel();

            if (!String.IsNullOrWhiteSpace(FarmName))
            {
                // we know the FarmName so we get the Id of the farm with that name
                int farmId = Convert.ToInt32(FarmsUtility.GetFarmIdByName(FarmName));

                model.FarmName = FarmName;
                model.Role = RolesUtility.GetFarmRoleById(farmId, Roles);
            }

            //build the secondary navbar here? so that we remove the logic from inside the partial view?
            return PartialView("_SecNavBar", model);
        }

        public ActionResult Index()
        {
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

#else
            
            if (!Request.Cookies.AllKeys.Contains("IsAuthenticated"))
            {
                SetCookie("IsAuthenticated", "true");
            
                SetCookie("UserID", "SYSSPIMI");
                SetCookie("UserDirectory", "U0J4169");
                SetCookie("UserIdentity", "Davide Carbone");
            }

            ViewBag.UserIdentity = GetCookie("UserIdentity");
            Roles.Add("YA2C04");


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
            //            Roles.Add(role.RoleID);
            //        }
            //    }
            //}
#endif


            string path = Server.MapPath(Url.Content(FilePaths.RolesXML));
            XDocument root = FarmsUtility.GetXmlDocument(path);
            
            List<FarmRoles> FarmRoles = new List<FarmRoles>(RolesUtility.GetRoles(root, Roles));

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
            string farmNode = Request["FarmList"];
            if (string.IsNullOrEmpty(farmNode) || !farmNode.Contains("|")) { return RedirectToAction("Index");  }            
            int farmId = Convert.ToInt32(farmNode.Split('|')[0]);
            int nodeId = Convert.ToInt32(farmNode.Split('|')[1]);
            var node = FarmsUtility.GetFarmNode(farmId, nodeId);
            var farm = FarmsUtility.GetFarmsById(new List<string>() { farmId.ToString() });
            SetCookie("FarmId", farmNode.Split('|')[0]);
            SetCookie("NodeId", farmNode.Split('|')[1]);
            SetCookie("FarmName", farm.FirstOrDefault().Name + " - " + node.Name);
            ViewBag.FarmName = GetCookie("FarmName");
            ViewBag.Server = node.Server;
            ViewBag.VirtualProxy = node.VirtualProxy;
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            return View();
        }

        public ActionResult OpenFrame(string server, string vp)
        {
            string UserID = GetCookie("UserID");
            string UserDirectory = GetCookie("UserDirectory");
            SetCookie("Server", server);
            SetCookie("VirtualProxy", vp);
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