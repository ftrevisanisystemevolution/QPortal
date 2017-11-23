using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace QPortal.Controllers
{
    public class LineeGuidaController : BaseController
    {
        // GET: LineeGuida
        public ActionResult Index()
        {
            ViewBag.PageType = "GuideLines";

            SetCookie("FarmName", "");
            ViewBag.FarmName = "";

            ViewBag.UserIdentity = GetCookie("UserIdentity");
            
            if (string.IsNullOrEmpty(ViewBag.UserIdentity)) { return RedirectToAction("Index", "Home"); }
            
            return View();
        }
    }
}