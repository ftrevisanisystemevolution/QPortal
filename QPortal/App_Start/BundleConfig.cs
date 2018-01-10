﻿using System.Web;
using System.Web.Optimization;

namespace QPortal
{
    public class BundleConfig
    {
        // For more information on bundling, visit https://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at https://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      //"~/Scripts/BootstrapMenu.min.js",
                      "~/Scripts/respond.js",
                      "~/Scripts/toastr.js",
                      "~/Scripts/jquery.fancybox.js",
                      "~/Scripts/cookieconsent/cookieconsent.js",
                      "~/Scripts/initial/initial.min.js"));

            bundles.Add(new StyleBundle("~/Content/css").Include(
                      //"~/Content/bootswatch/cosmo/bootstrap.css",
                      "~/Content/bootstrap-cosmo.css",
                      "~/Content/datatables/css/datatables.bootstrap.css",
                      "~/Content/datatables/css/select.bootstrap.css",
                      "~/Content/font-awesome.css",
                      "~/Content/toastr.css",
                      "~/Content/cookieconsent/cookieconsent.7.css",
                      "~/Content/site.css"));
        }
    }
}
