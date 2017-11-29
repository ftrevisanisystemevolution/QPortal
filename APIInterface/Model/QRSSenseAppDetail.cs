using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface.Model
{
    public class QRSSenseAppDetail
    {
        public string id { get; set; }
        public string name { get; set; }
        public string modifiedByUserName { get; set; }
        public string description { get; set; }
        public string thumbnail { get; set; }
        public QRSSenseStream stream { get; set; }
        public QRSSenseOwner owner { get; set; }
    }
}
