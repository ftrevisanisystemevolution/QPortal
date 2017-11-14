using QPortal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QPortal.ViewModels
{
    public class Report
    {
        public string Owner { get; set; }
        public string ReportName { get; set; }
        public bool Selected { get; set; }
    }

    public class ReportViewModel
    {
        public List<string> Streams { get; set; }
        public List<Report> Reports { get; set; }
    }
}