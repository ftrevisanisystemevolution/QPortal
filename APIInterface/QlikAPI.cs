using Qlik.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface
{
    public class QlikAPI
    {
        ConnectionManager conn = null;

        private string qlikUri;

        public QlikAPI(string qlikUri)
        {
            this.qlikUri = qlikUri;
            conn = new ConnectionManager(qlikUri);
        }

        public bool CreateApp(string name, string description, out string appId)
        {
            appId = "";
            try
            {
                if (!conn.IsConnected) { return false; }

                using (IHub hub = conn.location.Hub())
                {
                    var createAppRes = hub.CreateApp(name);
                    if (!createAppRes.Success) { return false; }
                    var app = hub.OpenApp(createAppRes.AppId);
                    NxAppProperties properties = app.GetAppProperties();
                    properties.Set<string>("description", description);
                    app.SetAppProperties(properties);
                    appId = createAppRes.AppId;
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
