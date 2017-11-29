using Qlik.Engine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace APIInterface
{
    public class ConnectionManager
    {
        public ILocation location { get; set; }
        public bool IsConnected { get; set; }

        public ConnectionManager(string qlikUri)
        {
            try
            { 
                // TODO: disabilitare conrollo di versione Qlik

                Uri uri = new Uri(qlikUri);

                location = Location.FromUri(uri);

                location.AsNtlmUserViaProxy(proxyUsesSsl: true, certificateValidation: false);

                IsConnected = true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
            }
        }

        public ConnectionManager(string qlikUri, string userID, string userDirectory, string path)
        {
            try
            {
                // TODO: disabilitare conrollo di versione Qlik

                Uri uri = new Uri(qlikUri + ":4747");

                X509Certificate2 x509 = new X509Certificate2();
                //Create X509Certificate2 object from .cer file.
                byte[] rawData = File.ReadAllBytes(path);
                x509.Import(rawData, "SysEvo", X509KeyStorageFlags.MachineKeySet);
                X509Certificate2Collection certificateCollection = new X509Certificate2Collection(x509);
                // Defining the location as a direct connection to Qlik Sense Server
                location = Location.FromUri(uri);

                location.AsDirectConnection(userDirectory, userID, certificateValidation:false, certificateCollection: certificateCollection);                

                IsConnected = true;
            }
            catch (Exception ex)
            {
                IsConnected = false;
            }
        }
    }
}
