using OPFService.Utilities;
using System;
using System.Linq;
using System.Net;
using Topshelf.Logging;

// Troy Hunt has kindly made available a k-anonymity type query API for checking
// password hashes against his massive collection of breach corpuses (corpi?).
// This adds an implementation of that to OpenPasswordFilter.
//
// We compute the sha1 hash of the potential password and send the first five
// characters over to Troy, who replies with a list of all the suffixes that 
// share that same hash prefix. We can thus check for previously pwned passwords
// without disclosing sha1 hashes in full, which would just be silly.

namespace OPFService.Core
{
    public class PwnedAPI
    {
        private const string HAVEIBEENPWNED_URL = "https://api.pwnedpasswords.com/range/{0}";
        private const string HAVEIBEENPWNED_USERAGENT = "OpenPasswordFilter";
        private const int CHARS_IN_PREFIX_COUNT = 5;

        private readonly LogWriter _logger = HostLogger.Get<PwnedAPI>();

        public bool CheckPassword(string password)
        {
            var methodName = $"{nameof(PwnedAPI)}::{nameof(CheckPassword)}";

            var isPasswordForbidden = false;

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var passwordHash = StringUtilities.GetPasswordHash(password);
            var passwordHashPrefix = passwordHash.Substring(0, CHARS_IN_PREFIX_COUNT);
            var uri = string.Format(HAVEIBEENPWNED_URL, passwordHashPrefix);

            Program.OpenPasswordFilterClient.BaseAddress = new Uri(uri);
            Program.OpenPasswordFilterClient.DefaultRequestHeaders
                .Add("User-Agent", HAVEIBEENPWNED_USERAGENT);

            try
            {
                var response = Program.OpenPasswordFilterClient.GetAsync(passwordHashPrefix).Result;
                if (response.IsSuccessStatusCode)
                {
                    var dataObjects = response.Content.ReadAsStringAsync().Result;
                    isPasswordForbidden = dataObjects.Split('\n')
                        .Any(l =>
                        {
                            var suffix = l.Split(':').First().ToLower();
                            return passwordHash.Equals(passwordHashPrefix + suffix);
                        });

                    _logger.Info($"[{methodName}] - Password was validated on {uri}. Is password forbidden = {isPasswordForbidden}");
                }
                else
                {
                    _logger.Error($"[{methodName}] - request to {uri} has failed." +
                        $"Error = {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"[{methodName}] - Unexpected exception. Error = {ex.Message}");
            }

            return isPasswordForbidden;
        }
    }
}
