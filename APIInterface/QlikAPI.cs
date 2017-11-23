using APIInterface.Model;
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

        public QlikAPI(string qlikUri, string userID, string userDirectory, string path)
        {
            this.qlikUri = qlikUri;
            conn = new ConnectionManager(qlikUri, userID, userDirectory, path);
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

        public bool GetMyReportsNotPublished(out List<SenseApplication> notPublishedApps)
        {
            notPublishedApps = new List<SenseApplication>();
            try
            {
                if (!conn.IsConnected) { return false; }

                var appIdentifiers  = conn.location.GetAppIdentifiers(true);
                foreach (var appIdentifier in appIdentifiers)
                {
                    var stream = appIdentifier.Meta.Get<AbstractStructure>("stream");
                    if (stream == null)
                    {
                        // Non è pubblicato in uno stream, aggiungo l'entry al dictionary di ritorno
                        notPublishedApps.Add(new SenseApplication(appIdentifier.AppId) { Description = appIdentifier.Meta.Get<string>("description"), Name =  appIdentifier.AppName });
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

        public bool GetMyStreams(out List<SenseStream> myStreams)
        {
            myStreams = new List<SenseStream>();
            try
            {
                if (!conn.IsConnected) { return false; }
                
                using (IHub hub = conn.location.Hub())
                {
                    var streams = hub.GetStreamList();
                    foreach(var stream in streams)
                    {
                        myStreams.Add(new SenseStream(stream.Id) { Name = stream.Name });
                    }
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
