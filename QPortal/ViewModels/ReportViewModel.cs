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
        public string StreamName { get; set; }
        public bool Selected { get; set; }
    }

    public class QStream
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }

    public class ReportViewModel
    {
        public List<QStream> Streams { get; set; }
        public List<Report> Reports { get; set; }

        public static ReportViewModel CreateReportViewModel(List<SenseApplication> apps, List<SenseStream> streams)
        {

            ReportViewModel rModel = new ReportViewModel();

            rModel.Reports = new List<Report>();

            foreach (var app in apps)
            {
                rModel.Reports.Add(new Report()
                {
                    Id = app.AppId,
                    Description = app.Description,
                    ReportName = app.Name,
                    StreamName = app.Stream,
                    Owner = app.OwnerUserName
                });
            }

            rModel.Streams = new List<QStream>();
            foreach (var myStream in streams)
            {
                rModel.Streams.Add(new QStream() { Id = myStream.Id, Name = myStream.Name });
            }            

            return rModel;
        }
    }

    
}