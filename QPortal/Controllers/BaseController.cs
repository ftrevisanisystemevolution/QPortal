using QPortal.Models;
using QPortal.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public string GetCookie(string key)
        {
            if (Request.Cookies.AllKeys.Contains(key))
            {
                return Request.Cookies[key].Value;
            }
            return "";
        }

        public string GetResponseCookie(string key)
        {
            if (Response.Cookies.AllKeys.Contains(key))
            {
                return Response.Cookies[key].Value;
            }
            return "";
        }

        // this needs too be activated on every page that displays the secondary navigation request, except Home-Index
        // a new controller can be created, or leave it here
        // if it will be moved, the Roles needs to be passed to it as well.. store them in cookie? or get them at each page load? or constructor
        [ChildActionOnly]
        public ActionResult SecondaryNavbarRequest()
        {
            //get farm from FarmName cookie
            string FarmName = GetCookie("FarmName");
            string FarmId = GetCookie("FarmId");

            //create a model and pass it to the view so that you don't need to set a Role cookie
            SecondaryNavBarModel model = new SecondaryNavBarModel();

            if (!String.IsNullOrWhiteSpace(FarmId))
            {
                model.FarmName = FarmName;
                model.Role = RolesUtility.GetFarmRoleById(Convert.ToInt32(FarmId), Roles);
            }

            //build the secondary navbar here? so that we remove the logic from inside the partial view?
            return PartialView("_SecNavBar", model);
        }

        [ChildActionOnly]
        public ActionResult SecondaryNavbarResponse()
        {
            //get farm from FarmName cookie
            string FarmName = GetResponseCookie("FarmName");
            string FarmId = GetResponseCookie("FarmId");

            //create a model and pass it to the view so that you don't need to set a Role cookie
            SecondaryNavBarModel model = new SecondaryNavBarModel();

            if (!String.IsNullOrWhiteSpace(FarmId))
            {
                model.FarmName = FarmName;
                model.Role = RolesUtility.GetFarmRoleById(Convert.ToInt32(FarmId), Roles);
            }

            //build the secondary navbar here? so that we remove the logic from inside the partial view?
            return PartialView("_SecNavBar", model);
        }

    }
}