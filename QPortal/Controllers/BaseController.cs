using QPortal.Models;
using QPortal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace QPortal.Controllers
{
    public class BaseController : Controller
    {
        private List<string> _roles;
        public List<string> Roles
        {
            get { return _roles; }
            set { _roles = value; }
        }

        public void SetCookie(string key, string value)
        {
            if (!Request.Cookies.AllKeys.Contains(key))
            {
                HttpCookie cookie = new HttpCookie(key);
                cookie.Value = value;
                Response.Cookies.Add(cookie);
            }
            else { Response.Cookies[key].Value = value; }
        }

        public void SetRolesCookie(List<string> roles)
        {
            string myObjectJson = new JavaScriptSerializer().Serialize(roles);
            if (!Request.Cookies.AllKeys.Contains("rolescookie"))
            {
                HttpCookie cookie = new HttpCookie("rolescookie");
                cookie.Value = myObjectJson;
                Response.Cookies.Add(cookie);
            }
            else { Response.Cookies["rolescookie"].Value = myObjectJson; }           
        }

        public string GetCookie(string key)
        {
            if (Request.Cookies.AllKeys.Contains(key))
            {
                return Request.Cookies[key].Value;
            }
            return "";
        }

        public List<string> GetRolesCookie()
        {
            if (Request.Cookies.AllKeys.Contains("rolescookie"))
            {
                string serialized = Request.Cookies["rolescookie"].Value;
                return (List<string>) new JavaScriptSerializer().Deserialize(serialized, typeof(List<string>));
            }
            else if (Response.Cookies.AllKeys.Contains("rolescookie"))
            {
                string serialized = Response.Cookies["rolescookie"].Value;
                return (List<string>)new JavaScriptSerializer().Deserialize(serialized, typeof(List<string>));
            }
            return new List<string>();
        }

        public string GetResponseCookie(string key)
        {
            if (Response.Cookies.AllKeys.Contains(key))
            {
                return Response.Cookies[key].Value;
            }
            return "";
        }

        public List<string> GetResponseRolesCookie()
        {
            if (Response.Cookies.AllKeys.Contains("rolescookie"))
            {
                string serialized = Response.Cookies["rolescookie"].Value;
                return (List<string>)new JavaScriptSerializer().Deserialize(serialized, typeof(List<string>));
            }
            return new List<string>();
        }

        // this needs too be activated on every page that displays the secondary navigation request, except Home-Index
        // a new controller can be created, or leave it here
        // if it will be moved, the Roles needs to be passed to it as well.. store them in cookie? or get them at each page load? or constructor
        [ChildActionOnly]
        public ActionResult SecondaryNavbarRequest()
        {
            //get ambito from AmbitoName cookie
            string AmbitoName = GetCookie("AmbitoName");
            string AmbitoId = GetCookie("AmbitoId");
            Roles = GetRolesCookie();

            //create a model and pass it to the view so that you don't need to set a Role cookie
            SecondaryNavBarModel model = new SecondaryNavBarModel();

            if (!String.IsNullOrWhiteSpace(AmbitoId))
            {
                model.AmbitoName = AmbitoName;
                model.Role = RolesUtility.GetAmbitoRoleById(Convert.ToInt32(AmbitoId), Roles);
            }

            //build the secondary navbar here? so that we remove the logic from inside the partial view?
            return PartialView("_SecNavBar", model);
        }

        [ChildActionOnly]
        public ActionResult SecondaryNavbarResponse()
        {
            //get ambito from AmbitoName cookie
            string AmbitoName = GetResponseCookie("AmbitoName");
            string AmbitoId = GetResponseCookie("AmbitoId");
            Roles = GetRolesCookie();

            //create a model and pass it to the view so that you don't need to set a Role cookie
            SecondaryNavBarModel model = new SecondaryNavBarModel();

            if (!String.IsNullOrWhiteSpace(AmbitoId))
            {
                model.AmbitoName = AmbitoName;
                model.Role = RolesUtility.GetAmbitoRoleById(Convert.ToInt32(AmbitoId), Roles);
            }

            //build the secondary navbar here? so that we remove the logic from inside the partial view?
            return PartialView("_SecNavBar", model);
        }

    }
}