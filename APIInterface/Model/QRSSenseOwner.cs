using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface.Model
{
    public class QRSSenseOwner
    {
        public string id { get; set; }
        public string userId { get; set; }
        public string userDirectory { get; set; }
        public string name { get; set; }
        public string privileges { get; set; }
    }
}
