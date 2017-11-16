using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QPortal.Controllers
{
    public class BaseController : Controller
    {
        
        public void SetCookie(string key, string value)
        {
            if (!Request.Cookies.AllKeys.Contains(key))
            {
                HttpCookie cookie = new HttpCookie(key);
                cookie.Value = value;
                Response.Cookies.Add(cookie);
            }
            else { Request.Cookies[key].Value = value; }
        }

        public string GetCookie(string key)
        {
            if (Request.Cookies.AllKeys.Contains(key))
            {
                return Request.Cookies[key].Value;
            }
            return "";
        }
    }
}