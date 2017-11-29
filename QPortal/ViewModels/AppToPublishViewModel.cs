using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace QPortal.ViewModels
{
    public class AppToPublishViewModel
    {
        public string AppId { get; set; }
        public string AppName { get; set; }
        public string StreamID { get; set; }
        public string StreamName { get; set; }
        public bool OverwriteRequired { get; set; }
        public string AppToOverwriteId { get; set; }
    }
}