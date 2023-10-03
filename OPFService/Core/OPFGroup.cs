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
using System.IO;
using System.Linq;
using Topshelf.Logging;

namespace OPFService.Core
{
    public class OPFGroup
    {
        private readonly LogWriter _logger = HostLogger.Get<OPFGroup>();

        private readonly string _groupsToCheckFilePath;
        private HashSet<string> groupsToCheck;
        private DateTime groupsToCheckFileModTime;

        public OPFGroup(string groupsToCheckFilePath)
        {
            _groupsToCheckFilePath = groupsToCheckFilePath;
        }

        public bool Contains(string username)
        {
            var methodName = $"{nameof(OPFGroup)}::{nameof(Contains)}";

            var contains = false;

            ReadGroupsFileIfModified();

            if (groupsToCheck.Any())
            {
                try
                {
                    using (var ctx = new PrincipalContext(ContextType.Domain))
                    {
                        foreach (var groupname in groupsToCheck)
                        {
                            using (var groupCtx = GroupPrincipal.FindByIdentity(ctx, groupname))
                            {
                                if (groupCtx != null)
                                {
                                    if (groupCtx.GetMembers(true).Any(p => p.SamAccountName.Equals(username)))
                                    {
                                        _logger.Info($"User is in a group to check, so their password will be validated. " +
                                            $"Username = {username}. Group = {groupname}");

                                        contains = true;
                                        break;
                                    }
                                }
                            }
                        }

                        if (!contains)
                        {
                            _logger.Info($"User is not in a group to check. Username = {username}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"[{methodName}] - Unexpected exception occured while processing. " +
                        $"Error = {ex.Message}");

                    throw;
                }
            }
            else
            {
                _logger.Info($"[{methodName}] - No groups were found. User's password will be validated.");

                contains = true;
            }

            return contains;
        }

        private void ReadGroupsFileIfModified()
        {
            if (FileUtilities.IsFileModified(_groupsToCheckFilePath, groupsToCheckFileModTime))
            {
                groupsToCheckFileModTime = File.GetLastWriteTime(_groupsToCheckFilePath);
                groupsToCheck = FileUtilities.ReadTextFile(_groupsToCheckFilePath);
            }
        }
    }
}
