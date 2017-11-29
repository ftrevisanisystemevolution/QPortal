using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface.Model
{
    public class QRSSenseCustomProperty
    {
        public string id { get; set; }
        public string value { get; set; }
        public QRSSenseCustomPropertyDefinition definition { get; set; }
    }
}
