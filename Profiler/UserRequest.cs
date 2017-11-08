using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler
{
    public class UserRequest
    {
        public string UserID { get; private set; }
        public string UserDirectory { get; private set; }
        public string SWAProfileID { get; private set; }
        public string LinkSWP { get; private set; }
        public string UserIdentity { get; private set; }
        public bool IsValid { get; private set; }

        public UserRequest(int headersKeysCount, string[] headersKeys, NameValueCollection headers)
        {
            UserID = "";
            UserDirectory = "";
            LinkSWP = "";
            UserIdentity = "";
            for (int i = 0; i < headersKeysCount; i++)
            {
                switch (headersKeys[i].Trim())
                {
                    case "ProfileServicesUrl":
                        LinkSWP = headers[i];
                        break;
                    case "SWAProfileID":
                        SWAProfileID = headers[i];
                        string[] splitted = SWAProfileID.Split('\\');
                        if (splitted.Length == 2)
                        {
                            UserDirectory = splitted[0];
                            UserID = splitted[1];
                        }
                        break;
                    case "SWAUserIdentity":
                        UserIdentity = headers[i];
                        break;
                }
            }
            IsValid = (!string.IsNullOrEmpty(LinkSWP) && !string.IsNullOrEmpty(UserID));
        }
    }
}
