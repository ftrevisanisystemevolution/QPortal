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
        // GET /api/Pubblicazione
        //public IHttpActionResult GetReportsPubblicazione()
        //{
        //    List<string> streams = new List<string>();
        //    streams.Add("Stream 1");
        //    streams.Add("Stream 2");
        //    streams.Add("Stream 3");

        //    Report zero = new Report();
        //    zero.Id = "0";
        //    zero.Owner = "Giacomo Guilizzoni";
        //    zero.ReportName = "Founder & CEO";
            
        //    Report one = new Report();
        //    one.Id = "0";
        //    one.Owner = "Giacomo Guilizzoni";
        //    one.ReportName = "CFO";

        //    Report two = new Report();
        //    two.Id = "0";
        //    two.Owner = "Test PErson";
        //    two.ReportName = "NAN";

        //    ReportViewModel rModel = new ReportViewModel();
        //    rModel.Streams = new List<string>(streams);

        //    List<Report> rList = new List<Report>();
        //    rList.Add(zero);
        //    rList.Add(one);
        //    rList.Add(two);
        //    rModel.Reports = rList;

        //    return Ok(rList);
        //}

        // POST /api/ReportsPubblicazione
        [HttpPost]
        public IHttpActionResult CreatePubblicazione(string stream, int id)
        {


            return Ok();
        }

    }
}
