using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using Topshelf.Logging;

namespace OPFService.Utilities
{
    public class ActiveDirectoryUtilities
    {
        private readonly LogWriter _logger = HostLogger.Get<ActiveDirectoryUtilities>();
        public static Dictionary<string, string> GetUserInfo(string username)
        {
            var userInfo = new Dictionary<string, string>();

            using (PrincipalContext context = new PrincipalContext(ContextType.Domain))
            {
                using (UserPrincipal user = UserPrincipal.FindByIdentity(context, username))
                {
                    if (user != null)
                    {
                        userInfo.Add("FullName", user.DisplayName);
                        userInfo.Add("GivenName", user.GivenName);
                        userInfo.Add("Surname", user.Surname);
                        userInfo.Add("SamAccountName", user.SamAccountName);
                    }
                }
            }

            return userInfo;
        }
    }
}
