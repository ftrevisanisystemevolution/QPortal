using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Profiler.ProfilerService;

namespace Profiler
{
    /// <summary>
    /// Contains one profile element
    /// </summary>
    public class Profile
    {

        public string RoleID { get; private set; }
        public string RoleDescription { get; private set; }
        public string RoleShortDescription { get; private set; }

        public Profile(string RoleID, string RoleDescription, string RoleShortDescription)
        {
            this.RoleID = RoleID;
            this.RoleDescription = RoleDescription;
            this.RoleShortDescription = RoleShortDescription;
        }

        public Profile(string RoleID, anagraficaFunzioneSlimType anagraficaFunzione)
        {
            this.RoleID = RoleID;
            if (anagraficaFunzione != null)
            {
                RoleDescription = anagraficaFunzione.descrizioneFunzioneLunga;
                RoleShortDescription = anagraficaFunzione.descrizioneFunzioneBreve;
            }
        }
    }
}
