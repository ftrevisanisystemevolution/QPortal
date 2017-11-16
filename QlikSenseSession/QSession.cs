using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.SessionState;

namespace QlikSenseSession
{
    public class QSession
    {
        public string Method { get; private set; }
        public string Server { get; private set; }
        public string VirtualProxy { get; private set; }
        public string User { get; private set; }
        public string UserDirectory { get; private set; }
        public string SessionID { get; private set; }
        public X509Certificate2 CertificateFoo { get; private set; }
        public string ResponseSerialized { get; private set; }
        public string[] GetSessionArray { get; private set; }
        public string[] GetSessionCode { get; private set; }

        private HttpCookie QCookie = null;

        public QSession(string method, string server, string virtualProxy, string user, string userdirectory)
        {
            Method = method;
            Server = server;
            VirtualProxy = virtualProxy;
            User = user;
            UserDirectory = userdirectory;
        }

        public void OpenSession(HttpContext context)
        {
            SessionID = CreateSession(context);
#if DEBUG
            CertificateFoo = GetCertificate(StoreLocation.CurrentUser);
#else
            CertificateFoo = GetCertificate(StoreLocation.LocalMachine);
#endif

            //Create URL to REST endpoint for tickets
            string url = "https://" + Server + ":4243/qps/" + VirtualProxy + "/session";



            //Create the HTTP Request and add required headers and content in Xrfkey
            string Xrfkey = "0123456789abcdef";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url + "?Xrfkey=" + Xrfkey);
            request.ClientCertificates.Add(CertificateFoo);
            request.Method = Method;
            request.Accept = "application/json";
            request.Headers.Add("X-Qlik-Xrfkey", Xrfkey);

            //The body message sent to the Qlik Sense Proxy api will add the session to Qlik Sense for authentication
            string body = "{ 'UserId':'" + User + "','UserDirectory':'" + UserDirectory + "',";
            body += "'Attributes': [],";
            body += "'SessionId': '" + SessionID + "'";
            body += "}";
            byte[] bodyBytes = Encoding.UTF8.GetBytes(body);

            if (!string.IsNullOrEmpty(body))
            {
                request.ContentType = "application/json";
                request.ContentLength = bodyBytes.Length;
                Stream requestStream = request.GetRequestStream();
                requestStream.Write(bodyBytes, 0, bodyBytes.Length);
                requestStream.Close();
            }

            // make the web request and return the content
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Stream stream = response.GetResponseStream();
            ResponseSerialized = stream != null ? new StreamReader(stream).ReadToEnd() : string.Empty;
            GetSessionArray = ResponseSerialized.Split(new Char[] { ',' });
            GetSessionCode = GetSessionArray[3].Split(new Char[] { ':' });
        }

        public HttpCookie GetCookie(bool forceCreate = false)
        {
            if (forceCreate || QCookie == null)
            {
                DateTime now = DateTime.Now;
                QCookie = new HttpCookie("X-Qlik-Session-" + VirtualProxy);
                QCookie.Value = GetSessionCode[1].Trim(new Char[] { '"' });
                QCookie.Expires = DateTime.MinValue;
                QCookie.HttpOnly = true;
                //add the domain for the cookie to ensure the Qlik Sense server uses the cookie created by this page located on the IIS web server
                //QCookie.Domain = getDomain(txtServer.Text);
            }
            return QCookie;
        }

        public void GoToHub(HttpResponseBase Response)
        {
            Response.Redirect(string.Format("https://{0}/{1}/hub/", Server, VirtualProxy));
        }

        public string GetHubURL()
        {
            return string.Format("http://{0}/{1}/hub/", Server, VirtualProxy);
        }

        private X509Certificate2 GetCertificate(StoreLocation storelocation)
        {
            // First locate the Qlik Sense certificate
            //X509Store store = new X509Store(StoreName.My, storelocation);
            X509Store store = new X509Store(StoreName.My, storelocation);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2 certificateFoo = store.Certificates.Cast<X509Certificate2>().FirstOrDefault(c => c.FriendlyName == "QlikClient");
            store.Close();

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11;
            //The following line is required because the root certificate for the above server certificate is self-signed.
            //Using a certificate from a trusted root certificate authority will allow this line to be removed.
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            return certificateFoo;
        }

        protected string CreateSession(HttpContext Context)
        {
            SessionIDManager Manager = new SessionIDManager();
            string NewID = Manager.CreateSessionID(Context);
            string OldID = Context.Session.SessionID;
            bool redirected = false;
            bool IsAdded = false;
            Manager.SaveSessionID(Context, NewID, out redirected, out IsAdded);
            return NewID;
        }
    }
}
