using OPFService.Utilities;
using System;
using System.IO;
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

namespace OPFService.Core;

public sealed class PwnedService
{
    private readonly LogWriter logger = HostLogger.Get<PwnedService>();

    public PwnedService()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    }

    public bool CheckPassword(string password)
    {
        var methodName = $"{nameof(PwnedService)}::{nameof(CheckPassword)}";

        var isPasswordForbidden = false;

        var passwordHash = StringUtilities.GetPasswordHash(password);
        var passwordHashPrefix = passwordHash.Substring(0, 5);

        Uri uri = new($"https://api.pwnedpasswords.com/range/{passwordHashPrefix}");
        HttpWebRequest request = WebRequest.CreateHttp(uri);
        request.UserAgent = "OpenPasswordFilter";
        try
        {
            using var response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                using var responseStream = response.GetResponseStream();
                using var streamReader = new StreamReader(responseStream);
                isPasswordForbidden = streamReader.ReadToEnd().Contains(passwordHash.Substring(5));

                this.logger.Debug($"[{methodName}] - Password was validated on {uri}. Is password forbidden = {isPasswordForbidden}");
            }
            else
            {
                this.logger.Debug($"[{methodName}] - request to {uri} has failed. Error = {response.StatusCode}");
            }
        }
        catch (Exception ex)
        {
            this.logger.Error($"[{methodName}] - Unexpected exception. Error = {ex.Message}", ex);
        }

        return isPasswordForbidden;
    }
}
