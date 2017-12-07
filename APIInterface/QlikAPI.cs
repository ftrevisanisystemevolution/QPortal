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

        public bool PublishApp(string appId, string name, string streamId)
        {
            try
            {
                if (!conn.IsConnected) { return false; }

                using (IHub hub = conn.location.Hub())
                {                                        
                    var app = hub.OpenApp(appId);
                    app.Publish(streamId, name);                       
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool PublishApp(string appId, string name, string streamId, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                if (!conn.IsConnected) { return false; }

                using (IHub hub = conn.location.Hub())
                {
                    var app = hub.OpenApp(appId);
                    app.Publish(streamId, name);
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorMessage += " -> " + ex.Message;
                }
                return false;
            }
        }

        public bool RenameApp(string appId, string newName)
        {
            try
            {
                if (!conn.IsConnected) { return false; }

                using (IHub hub = conn.location.Hub())
                {
                    var app = hub.OpenApp(appId);
                    NxAppProperties properties = app.GetAppProperties();
                    properties.Set<string>("Title", newName);
                    app.SetAppProperties(properties);                    
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
        }

        public bool GetPublishedApps(out List<SenseApplication> publishedApps)
        {
            publishedApps = new List<SenseApplication>();
            try
            {
                if (!conn.IsConnected) { return false; }

                var appIdentifiers = conn.location.GetAppIdentifiers(true);
                foreach (var appIdentifier in appIdentifiers)
                {
                    var stream = appIdentifier.Meta.Get<AbstractStructure>("stream");
                    if (stream != null)
                    {
                        // E' pubblicato in uno stream, aggiungo l'entry al dictionary di ritorno
                        publishedApps.Add(new SenseApplication(appIdentifier.AppId) { Description = appIdentifier.Meta.Get<string>("description"), Name = appIdentifier.AppName, Stream = stream.Get<string>("name"), StreamID = stream.Get<string>("id"),
                            PublishDate = appIdentifier.Meta.Get<DateTime>("publishTime")
                        });
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool GetPublishedAppsInSelectedStreams(List<string> streamIdList, out List<SenseApplication> publishedApps)
        {
            publishedApps = new List<SenseApplication>();
            try
            {
                if (!conn.IsConnected) { return false; }

                var appIdentifiers = conn.location.GetAppIdentifiers(true);
                foreach (var appIdentifier in appIdentifiers)
                {
                    var stream = appIdentifier.Meta.Get<AbstractStructure>("stream");
                    if (stream != null)
                    {
                        if (streamIdList.Contains(stream.Get<string>("id")))
                        {
                            // E' pubblicato in uno stream corretto, aggiungo l'entry al dictionary di ritorno
                            publishedApps.Add(new SenseApplication(appIdentifier.AppId)
                            {
                                Description = appIdentifier.Meta.Get<string>("description"),
                                Name = appIdentifier.AppName,
                                Stream = stream.Get<string>("name"),
                                StreamID = stream.Get<string>("id"),
                                PublishDate = appIdentifier.Meta.Get<DateTime>("publishTime")
                            });
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        [Obsolete]
        public bool GetStreams(out List<SenseStream> myStreams)
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

        public bool DeleteApp(string appID)
        {
            try
            {
                if (!conn.IsConnected) { return false; }

                foreach (IAppIdentifier appIdentifier in conn.location.GetAppIdentifiers(true))
                {
                    if (appIdentifier.AppId == appID) { return conn.location.Delete(appIdentifier); }
                }

                return true;
                
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ReplaceApp(string sourceAppID, string destAppID)
        {
            try
            {
                if (!conn.IsConnected) { return false; }

                using (IHub hub = conn.location.Hub())
                {
                    hub.ReplaceAppFromID(destAppID, sourceAppID, new List<string>());
                }

                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool ReplaceApp(string sourceAppID, string destAppID, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                if (!conn.IsConnected) { return false; }

                using (IHub hub = conn.location.Hub())
                {
                    hub.ReplaceAppFromID(destAppID, sourceAppID, new List<string>());
                }

                return true;

            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorMessage += " -> " + ex.Message;
                }
                return false; 
            }
        }

        
    }
}
