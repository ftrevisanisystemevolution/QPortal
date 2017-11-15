using QPortal.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace QPortal.Controllers.Api
{
    public class ReportsController : ApiController
    {
        // GET /api/customers
        public IHttpActionResult GetReportsPubblicazione()
        {
            List<string> streams = new List<string>();
            streams.Add("Stream 1");
            streams.Add("Stream 2");
            streams.Add("Stream 3");

            Report one = new Report();
            one.Owner = "Giacomo Guilizzoni";
            one.ReportName = "Founder & CEO";

            Report two = new Report();
            two.Owner = "Giacomo Guilizzoni";
            two.ReportName = "CFO";

            ReportViewModel rModel = new ReportViewModel();
            rModel.Streams = new List<string>(streams);

            List<Report> rList = new List<Report>();
            rList.Add(one);
            rList.Add(two);
            rModel.Reports = rList;


            return Ok(rModel);
        }
    }
}
