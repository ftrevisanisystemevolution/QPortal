using APIInterface.Model;
using QPortal.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QPortal.ViewModels
{
    public class Report
    {
        public string Id { get; set; }
        public string Owner { get; set; }
        public string ReportName { get; set; }
        public string Description { get; set; }
        public bool Selected { get; set; }
    }

    public class Stream
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ReportViewModel
    {
        public List<Stream> Streams { get; set; }
        public List<Report> Reports { get; set; }

        public static ReportViewModel CreateReportToPublishViewModel(List<SenseApplication> notPublishedApps, List<SenseStream> myStreams)
        {

            ReportViewModel rModel = new ReportViewModel();

            rModel.Reports = new List<Report>();

            foreach (var notPublishedApp in notPublishedApps)
            {
                rModel.Reports.Add(new Report()
                {
                    Id = notPublishedApp.AppId,
                    Description = notPublishedApp.Description,
                    ReportName = notPublishedApp.Name
                });
            }

            rModel.Streams = new List<Stream>();
            foreach (var myStream in myStreams)
            {
                rModel.Streams.Add(new Stream() { Id = myStream.Id, Name = myStream.Name });
            }            

            return rModel;
        }
    }

    
}