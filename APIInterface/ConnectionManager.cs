using Qlik.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
