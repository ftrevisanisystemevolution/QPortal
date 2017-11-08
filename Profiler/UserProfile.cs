using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Profiler
{
    /// <summary>
    /// This class contains the informations about the organization profile of a user.
    /// The source of those informations is SWP GetProfile web service.
    /// </summary>
    public class UserProfile
    {
        public string UserID { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string FiscalCode { get; private set; }
        public string Professional { get; private set; }
        public string EnterpriseID { get; private set; }
        public string EnterpriseDescription { get; private set; }
        public string UO { get; private set; }
        public string UODescription { get; private set; }
        public string CompanyID { get; private set; }
        public string CompanyDescription { get; private set; }
        public bool IsValid { get; private set; }
        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }
        public int ProfileNumber { get; private set; }
        public List<Profile> Profiles { get; private set; }

        /// <summary>
        /// Use this method to search the profile of a user in SWP. Use IsValid field to test if the user is present in SWP.
        /// </summary>
        /// <param name="userID">The ID of a user</param>
        /// <param name="webServiceURL">The URL of the SWP web service</param>
        public UserProfile(string userID)
        {
            Init(userID, "");
        }

        /// <summary>
        /// Use this method to search the profile of a user in SWP. Use IsValid field to test if the user is present in SWP.
        /// </summary>
        /// <param name="userID">The ID of a user</param>
        /// <param name="webServiceURL">The URL of the SWP web service</param>
        public UserProfile(string userID, string webServiceURL)
        {
            Init(userID, webServiceURL);
        }

        private void Init(string userID, string webServiceURL)
        {
            if (string.IsNullOrEmpty(userID))
            {
                ErrorMessage = "The userID is null or empty";
                ErrorCode = "404";
                IsValid = false;
            }
            else
            {
                try
                {
                    UserID = userID;

                    ProfilerService.ServiceSoap srv = new ProfilerService.ServiceSoap();

                    if (!string.IsNullOrEmpty(webServiceURL)) { srv.Url = webServiceURL; }              

                    // Call the web service
                    var result = srv.getProfilazione(userID, "", "", "", "", "true", "true", "true", "", "", "", "", "", "", "Qlik");

                    // Test the result status
                    if (result.responseStatus.retCode == "000" && result.responseStatus.numeroElementi > 0)
                    {
                        // Valid user!

                        // Set basic fields
                        var basic = result.profilazione.anagraficaUtente;
                        SetBasicFields(basic);

                        // Set user profiles
                        ProfileNumber = result.responseStatus.numeroElementi;
                        var profile = result.profilazione.userinfo[0].userprofile[0];
                        Profiles = new List<Profile>();
                        for (int j = 0; j < ProfileNumber; j++)
                        {
                            Profiles.Add(new Profile(profile.abilitazioni[j].codice,
                                profile.abilitazioni[j].anagraficaFunzione));
                        }
                        SetResult("", "", true);
                    }
                    else
                    {
                        // Invalid user !
                        SetResult(result.responseStatus.message, result.responseStatus.retCode, false);
                    }
                }
                catch (Exception ex)
                {
                    // Exception management
                    SetExceptionResult(ex);
                }
            }
        }

        private void SetBasicFields(ProfilerService.anagraficaUtenteBodyType basic)
        {
            if (basic != null)
            {
                FirstName = basic.nome;
                LastName = basic.cognome;
                FiscalCode = basic.codiceFiscale;
                Professional = basic.figuraProfessionale;
                EnterpriseID = basic.azienda;
                EnterpriseDescription = basic.descAzienda;
                UO = basic.uo;
                UODescription = basic.descUo;
                CompanyID = basic.societa;
                CompanyDescription = basic.descSocieta;
            }
        }

        private void SetExceptionResult(Exception ex)
        {
            string errorMessage = ex.Message;
            while (ex.InnerException != null)
            {
                ex = ex.InnerException;
                errorMessage += "-->" + ex.Message;
            }
            SetResult(ErrorMessage, "999", false);
        }

        private void SetResult(string errorMessage, string errorCode, bool isValid)
        {
            ErrorMessage = errorMessage;
            ErrorCode = errorCode;
            IsValid = isValid;
        }
    }
}
