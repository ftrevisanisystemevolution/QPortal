using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface
{
    public class QRSConnectionManager
    {
        private string xrfkey = "0123456789abcdef";
        private X509Certificate2 SenseCert = null;
        public string Server { get; private set; }
        public string CertPath { get; private set; }
        public QRSConnectionManager(string server, string certPath)
        {
            Server = server;
            CertPath = certPath;

            SenseCert = new X509Certificate2();
            byte[] rawData = File.ReadAllBytes(CertPath);
            SenseCert.Import(rawData, "SysEvo", X509KeyStorageFlags.MachineKeySet);
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
        }

        public HttpWebRequest CreateWebRequest(string APIUrl, string user, string userDirectory)
        {
            //HttpWebRequest request = (HttpWebRequest)WebRequest.Create(@"https://servername.com:4242/QRS/app?xrfkey=" + xrfkey);
            string URL = string.Format("https://{0}:4242/QRS/{1}?xrfkey={2}", Server, APIUrl, xrfkey);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
            request.Method = "GET";
            request.Accept = "application/json";
            request.Headers.Add("X-Qlik-xrfkey", xrfkey);
            // Add the certificate to the request and provide the user to execute as
            request.ClientCertificates.Add(SenseCert);
            //request.Headers.Add("X-Qlik-User", @"UserDirectory=internal;UserId=sa_repository");
            request.Headers.Add("X-Qlik-User", string.Format("UserDirectory={0};UserId={1}", userDirectory, user));
            return request;
        }

        public RestClient CreateRestClient(string APIUrl, string user, string userDirectory, out RestRequest request)
        {
            string URL = string.Format("https://{0}:4242", Server);
            var client = new RestClient(URL);

            URL = string.Format("QRS/{1}?xrfkey={2}", Server, APIUrl, xrfkey);
            request = new RestRequest(URL, Method.GET);
            request.AddHeader("X-Qlik-xrfkey", xrfkey);
            request.AddHeader("X-Qlik-User", string.Format("UserDirectory={0};UserId={1}", userDirectory, user));
            client.ClientCertificates = new X509CertificateCollection() { SenseCert };

            return client;
        }

        public RestClient CreateRestClientPost(string APIUrl, string user, string userDirectory, out RestRequest request)
        {
            string URL = string.Format("https://{0}:4242", Server);
            var client = new RestClient(URL);
            if (APIUrl.Contains("?"))
            {
                URL = string.Format("QRS/{1}&xrfkey={2}", Server, APIUrl, xrfkey);
            }
            else
            {
                URL = string.Format("QRS/{1}?xrfkey={2}", Server, APIUrl, xrfkey);
            }
            request = new RestRequest(URL, Method.POST);
            request.AddHeader("X-Qlik-xrfkey", xrfkey);
            request.AddHeader("X-Qlik-User", string.Format("UserDirectory={0};UserId={1}", userDirectory, user));
            client.ClientCertificates = new X509CertificateCollection() { SenseCert };

            return client;
        }

        public RestClient CreateRestClientPut(string APIUrl, string user, string userDirectory, out RestRequest request)
        {
            string URL = string.Format("https://{0}:4242", Server);
            var client = new RestClient(URL);
            if (APIUrl.Contains("?"))
            {
                URL = string.Format("QRS/{1}&xrfkey={2}", Server, APIUrl, xrfkey);
            }
            else
            {
                URL = string.Format("QRS/{1}?xrfkey={2}", Server, APIUrl, xrfkey);
            }
            request = new RestRequest(URL, Method.PUT);
            request.AddHeader("X-Qlik-xrfkey", xrfkey);
            request.AddHeader("X-Qlik-User", string.Format("UserDirectory={0};UserId={1}", userDirectory, user));
            client.ClientCertificates = new X509CertificateCollection() { SenseCert };

            return client;
        }

        public RestClient CreateRestClientWithParams(string APIUrl, string user, string userDirectory, out RestRequest request)
        {
            string URL = string.Format("https://{0}:4242", Server);
            var client = new RestClient(URL);

            URL = string.Format("QRS/{1}&xrfkey={2}", Server, APIUrl, xrfkey);
            request = new RestRequest(URL, Method.GET);
            request.AddHeader("X-Qlik-xrfkey", xrfkey);
            request.AddHeader("X-Qlik-User", string.Format("UserDirectory={0};UserId={1}", userDirectory, user));
            client.ClientCertificates = new X509CertificateCollection() { SenseCert };

            return client;
        }
    }
}
