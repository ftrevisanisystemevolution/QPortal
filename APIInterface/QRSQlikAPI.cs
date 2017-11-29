using APIInterface.Model;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface
{
    public class QRSQlikAPI
    {
        QRSConnectionManager conn = null;
        RestRequest restRequest = null;

        public QRSQlikAPI(string server, string certPath)
        {
            conn = new QRSConnectionManager(server, certPath);
        }

        public bool GetStreams(string userID, string userDirectory, out List<SenseStream> myStreams, out string errorMessage)
        {
            myStreams = new List<SenseStream>();
            errorMessage = "";
            try
            {
                RestClient client = conn.CreateRestClient("stream/full", userID, userDirectory, out restRequest);

                var response = client.Execute<List<QRSSenseStream>>(restRequest);

                if (response.IsSuccessful)
                {
                    foreach (var item in response.Data)
                    {                        
                        myStreams.Add(new SenseStream(item.id) { Name = item.name });
                    }
                }
                else
                {
                    errorMessage += response.StatusCode + "  |  ";
                    errorMessage += response.StatusDescription + "  |  ";
                    errorMessage += response.ResponseStatus.ToString() + "  |  ";
                    errorMessage += response.Server + "  |  ";
                }

                return response.IsSuccessful;
            }
            catch (Exception ex)
            {
                errorMessage = "Errore non Gestito: " + ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorMessage += " -> " + ex.Message;
                }
                return false;
            }
        }

        public bool GetStreamsByCustomProperty(string userID, string userDirectory, string customPropName, string customPropValue, out List<SenseStream> myStreams, out string errorMessage)
        {
            myStreams = new List<SenseStream>();
            errorMessage = "";
            try
            {
                RestClient client = conn.CreateRestClient("stream/full", userID, userDirectory, out restRequest);

                var response = client.Execute<List<QRSSenseStream>>(restRequest);

                if (response.IsSuccessful)
                {
                    foreach (var item in response.Data)
                    {
                        if (item.customProperties.Count > 0)
                        {
                            foreach (var cp in item.customProperties)
                            {
                                if (cp.definition != null && cp.definition.name == customPropName && cp.value == customPropValue)
                                {
                                    myStreams.Add(new SenseStream(item.id) { Name = item.name });
                                }
                            }
                        }
                    }
                }
                else
                {
                    errorMessage += response.StatusCode + "  |  ";
                    errorMessage += response.StatusDescription + "  |  ";
                    errorMessage += response.ResponseStatus.ToString() + "  |  ";
                    errorMessage += response.Server + "  |  ";
                }

                return response.IsSuccessful;
            }
            catch (Exception ex)
            {
                errorMessage = "Errore non Gestito: " + ex.Message;
                while (ex.InnerException != null)
                {
                    ex = ex.InnerException;
                    errorMessage += " -> " + ex.Message;
                }
                return false;
            }
        }

        public bool GetAppDetail(string userID, string userDirectory, string appID, out QRSSenseAppDetail appDetail, out string errorMessage)
        {
            appDetail = new QRSSenseAppDetail();
            errorMessage = "";
            try
            {
                RestClient client = conn.CreateRestClient("app/" + appID, userID, userDirectory, out restRequest);

                var response = client.Execute<QRSSenseAppDetail>(restRequest);

                if (response.IsSuccessful)
                {
                    appDetail = response.Data;
                }
                else
                {
                    errorMessage = response.ErrorMessage;
                }

                return response.IsSuccessful;
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

        public bool GetApps(string userID, string userDirectory, out List<QRSSenseApp> apps, out string errorMessage)
        {
            apps = new List<QRSSenseApp>();
            errorMessage = "";
            try
            {
                RestClient client = conn.CreateRestClient("app", userID, userDirectory, out restRequest);

                var response = client.Execute<List<QRSSenseApp>>(restRequest);

                if (response.IsSuccessful)
                {
                    apps = response.Data;
                }
                else
                {
                    errorMessage = response.ErrorMessage;
                }

                return response.IsSuccessful;
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

        public bool CopyApp(string userID, string userDirectory, string appID, string appDestName, out QRSSenseApp app, out string errorMessage)
        {
            app = new QRSSenseApp();
            errorMessage = "";
            try
            {
                RestClient client = conn.CreateRestClientPost("app/" + appID + "/copy?name=" + appDestName, userID, userDirectory, out restRequest);

                var response = client.Execute<QRSSenseApp>(restRequest);

                if (response.IsSuccessful)
                {
                    app = response.Data;
                }
                else
                {
                    errorMessage = response.ErrorMessage;
                }

                return response.IsSuccessful;
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

        public bool ReplaceApp(string userID, string userDirectory, string sourceAppID, string destAppID, out string errorMessage)
        {
            errorMessage = "";
            try
            {
                RestClient client = conn.CreateRestClientPost("app/" + destAppID + "/replace?app=" + sourceAppID, userID, userDirectory, out restRequest);

                var response = client.Execute<QRSSenseApp>(restRequest);

                if (!response.IsSuccessful)
                {
                    errorMessage = response.ErrorMessage;
                }

                return response.IsSuccessful;
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