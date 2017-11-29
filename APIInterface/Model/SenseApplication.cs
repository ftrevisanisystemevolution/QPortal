using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface.Model
{
    public class SenseApplication
    {
        public string AppId { get; private set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Stream { get; set; }
        public string StreamID { get; set; }
        public string OwnerUserID { get; set; }
        public string OwnerUserName { get; set; }
        public string OwnerUserDirectory { get; set; }
        public DateTime PublishDate { get; set; }

        public SenseApplication(string appId)
        {
            AppId = appId;
        }
    }
}
