// This file is part of OpenPasswordFilter.
// 
// OpenPasswordFilter is free software; you can redistribute it and / or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
// 
// OpenPasswordFilter is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with OpenPasswordFilter; if not, write to the Free Software
// Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111 - 1307  USA
//

using OPFService.Utilities;
using System;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using Topshelf.Logging;

namespace OPFService.Core;

public sealed class DirectoryService
{
    private readonly LogWriter logger = HostLogger.Get<DirectoryService>();
    private readonly string groupsToCheckFilePath;
    private HashSet<string> groupsToCheck;
    private DateTime groupsToCheckFileModTime;

    public DirectoryService(string groupsToCheckFilePath)
    {
        this.groupsToCheckFilePath = groupsToCheckFilePath;
    }

    public bool ContainsInGroup(string username)
    {
        var methodName = $"{nameof(DirectoryService)}::{nameof(ContainsInGroup)}";

        this.groupsToCheck = FileUtilities.ReadTextFileIfModified(this.groupsToCheckFilePath, ref this.groupsToCheckFileModTime);

        if (!groupsToCheck.Any())
        {
            this.logger.Debug($"[{methodName}] - No groups were found. User's password will be validated.");
            return true;
        }

        try
        {
            using var ctx = new PrincipalContext(ContextType.Domain);
            foreach (var groupname in groupsToCheck)
            {
                using var groupCtx = GroupPrincipal.FindByIdentity(ctx, groupname);
                if (groupCtx is not null && groupCtx.GetMembers(true).Any(p => p.SamAccountName.Equals(username)))
                {
                    this.logger.Debug($"[{methodName}] - User is in a group to check, so their password will be validated. " +
                        $"Username = {username}. Group = {groupname}");

                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            logger.Error($"[{methodName}] - Unexpected exception occured while processing. " +
                $"Error = {ex.Message}", ex);
            throw;
        }

        this.logger.Debug($"[{methodName}] - User is not in a group to check. Username = {username}");
        return false;
    }

    public bool DoesPasswordHaveUserInfo(string username, string password)
    {
        var methodName = $"{nameof(DictionaryService)}::{nameof(DoesPasswordHaveUserInfo)}";

        Dictionary<string, string> userInfo = new();

        using PrincipalContext context = new(ContextType.Domain);
        using UserPrincipal user = UserPrincipal.FindByIdentity(context, username);
        if (user is not null)
        {
            userInfo.Add("FullName", user.DisplayName);
            userInfo.Add("GivenName", user.GivenName);
            userInfo.Add("Surname", user.Surname);
            userInfo.Add("SamAccountName", user.SamAccountName);
        }

        var passwordContainingUserInfo = userInfo.FirstOrDefault(u => password.Contains(u.Value)).Key;
        if (!string.IsNullOrEmpty(passwordContainingUserInfo))
        {
            this.logger.Debug($"[{methodName}] - Given password contains user info. " +
                $"Password = . User Info = {passwordContainingUserInfo}");

            return true;
        }

        return false;
    }
}
