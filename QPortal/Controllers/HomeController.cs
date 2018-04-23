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
        

        public HomeController()
        {
            //populate roles here?
            //Roles = new List<string>();
            //Roles.Add("YA2C03");
        }
        

        public ActionResult Index()
        {
            ViewBag.PageType = "HomePage";

#if DEBUG
            //if (!Request.Cookies.AllKeys.Contains("IsAuthenticated"))
            //{
                SetCookie("IsAuthenticated", "true");

                SetCookie("UserID", "Test");
                SetCookie("UserDirectory", "desktop-29ba4mu");
                //SetCookie("UserID", "ftrevisani");
                //SetCookie("UserDirectory", "METSYS");
                SetCookie("UserIdentity", "Fabrizio Trevisani");
                Roles = new List<string>();
                Roles.Add("YA2C03");
                Roles.Add("YA3C03");
            SetRolesCookie(Roles);
            //}
            //else { Roles = GetRolesCookie(); }
            ViewBag.UserIdentity = GetCookie("UserIdentity");

#else

            // Da utilizzare se NON c'è SWA SWP
            SetCookie("IsAuthenticated", "true");

            //// Per sviluppi su mia macchina
            //SetCookie("IsAuthenticated", "true");
            SetCookie("UserID", "Test");
            SetCookie("UserDirectory", "desktop-29ba4mu");
            SetCookie("UserIdentity", "Test");
            Roles = new List<string>();
            Roles.Add("YA2C03");
            Roles.Add("YA3C03");
            SetRolesCookie(Roles);
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            ////

            //////Per sviluppi su macchina Intesa SCWAMOT0027.syssede.systest.sanpaoloimi.com
            ////SetCookie("UserID", "U0I4169");
            ////SetCookie("UserDirectory", "SYSSPIMI");
            ////SetCookie("UserIdentity", "Davide Carbone");
            ////Roles = new List<string>();
            ////Roles.Add("YA2C03");
            ////Roles.Add("YA3C03");
            ////SetRolesCookie(Roles);
            ////ViewBag.UserIdentity = GetCookie("UserIdentity");
            //// oppure 
            ////SetCookie("UserID", "U0J1748");
            ////SetCookie("UserDirectory", "SYSSPIMI");
            ////SetCookie("UserIdentity", "Fabrizio Trevisani");
            ////Roles = new List<string>();
            ////Roles.Add("YA2C03");
            ////Roles.Add("YA3C03");
            ////SetRolesCookie(Roles);
            ////ViewBag.UserIdentity = GetCookie("UserIdentity");
            //////

            ////FINE NON SWA SWP


            ////// Da utilizzare se c'è SWA SWP

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
            //    userProfile = new UserProfile(userRequest.UserID, userRequest.LinkSWP);
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

            //}

            //if (userProfile != null && userProfile.IsValid)
            //{
            //    Roles = new List<string>();
            //    foreach (var role in userProfile.Profiles)
            //    {
            //        Roles.Add(role.RoleID);
            //    }
            //    SetRolesCookie(Roles);
            //}

            ////// FINE SWA SWP

            // Da usare solo per test!!!!
            //if (userProfile == null) { throw new Exception("User Profile Null"); }
            //if (userProfile != null && !userProfile.IsValid) { throw new Exception("User Profile Invalid. " + userProfile.ErrorMessage); }
#endif

            string path = Server.MapPath(Url.Content(FilePaths.RolesXML));
            XDocument root = AmbitiUtility.GetXmlDocument(path);

            List<AmbitoRoles> AmbitoRoles = new List<AmbitoRoles>();
            if (Roles != null) { AmbitoRoles = RolesUtility.GetRoles(root, Roles); }

            if (AmbitoRoles.Count != 0)
            {
                List<Ambiti> AmbitoList = new List<Ambiti>(RolesUtility.AssignAmbitoRoles(AmbitoRoles));

                var viewModel = new AmbitiViewModel()
                {
                    AmbitoList = AmbitoList
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
            ViewBag.PageType = "Hub";

            string ambitoNode = Request["AmbitoList"];
            if (string.IsNullOrEmpty(ambitoNode) || !ambitoNode.Contains("|")) { return RedirectToAction("Index");  }            
            int ambitoId = Convert.ToInt32(ambitoNode.Split('|')[0]);
            int nodeId = Convert.ToInt32(ambitoNode.Split('|')[1]);
            var node = AmbitiUtility.GetAmbitoNode(ambitoId, nodeId);
            var ambito = AmbitiUtility.GetAmbitoById(new List<string>() { ambitoId.ToString() });
            SetCookie("AmbitoId", ambitoNode.Split('|')[0]);
            SetCookie("NodeId", ambitoNode.Split('|')[1]);
            SetCookie("AmbitoName", ambito.FirstOrDefault().Name + " - " + node.Name);
            SetCookie("UrlWebTicket", node.UrlWebTicket);
            ViewBag.AmbitoName = GetCookie("AmbitoName");
            ViewBag.Server = node.Server;
            ViewBag.VirtualProxy = node.VirtualProxy;
            ViewBag.UserIdentity = GetCookie("UserIdentity");
            return View();
        }

        public ActionResult OpenFrame(string server, string vp)
        {
            string UserID = GetCookie("UserID");
            string UserDirectory = GetCookie("UserDirectory");
            string UrlWebTicket = GetCookie("UrlWebTicket");
            var ambito = AmbitiUtility.GetAmbitoById(GetCookie("AmbitoId"));
            
            SetCookie("Server", server);
            SetCookie("VirtualProxy", vp);
            try
            {
                QSession qSession = new QSession("POST", server, vp, UserID, UserDirectory, UrlWebTicket, ambito.customproperty);
                qSession.OpenSession(HttpContext.ApplicationInstance.Context);
                Request.Cookies.Add(qSession.GetCookie(server, true));
                Response.Cookies.Add(qSession.GetCookie(server, true));
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