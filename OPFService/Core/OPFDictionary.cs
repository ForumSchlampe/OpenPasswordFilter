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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Topshelf.Logging;

namespace OPFService.Core
{
    public class OPFDictionary
    {
        private readonly LogWriter _logger = HostLogger.Get<OPFDictionary>();

        private readonly string _forbiddenPasswordsFilePath;
        private readonly string _forbiddenSubstringsFilePath;
        private readonly string _forbiddenRegexesFilePath;
        private HashSet<string> forbiddenPasswords;
        private HashSet<string> forbiddenSubstrings;
        private HashSet<Regex> forbiddenRegexes;
        private DateTime forbiddenPasswordsFileModTime;
        private DateTime forbiddenSubstringsFileModTime;
        private DateTime forbiddenRegexesFileModTime;

        public OPFDictionary(string forbiddenPasswordsFilePath, string forbiddenSubstringsFilePath, string forbiddenRegexesFilePath)
        {
            _forbiddenPasswordsFilePath = forbiddenPasswordsFilePath;
            _forbiddenSubstringsFilePath = forbiddenSubstringsFilePath;
            _forbiddenRegexesFilePath = forbiddenRegexesFilePath;
        }

        public bool Contains(string password, string username)
        {
            var methodName = $"{nameof(OPFDictionary)}::{nameof(Contains)}";

            ReadSubstringFileIfModified();
            ReadForbiddenPasswordsFileIfModified();
            ReadPatternsFileIfModified();

            var contains = IsPasswordForbidden(password) ||
                DoesPasswordHaveForbiddenSubstring(password) ||
                DoesPasswordMatchForbiddenPattern(password) ||
                DoesPasswordHaveUserInfo(username, password);

            _logger.Info($"[{methodName}] - Password passed custom filter. Contains = {contains}");

            return contains;
        }

        private bool DoesPasswordHaveForbiddenSubstring(string password)
        {
            var methodName = $"{nameof(OPFDictionary)}::{nameof(DoesPasswordHaveForbiddenSubstring)}";

            var doesPasswordHaveForbiddenSubstring = false;

            var forbiddenSubstring = forbiddenSubstrings.FirstOrDefault(s => password.Contains(s));
            if (string.IsNullOrEmpty(forbiddenSubstring))
            {
                _logger.Info($"[{methodName}] - Given password contains forbidden substring. " +
                    $"Password = . Forbidden substring = {forbiddenSubstring}");

                doesPasswordHaveForbiddenSubstring = true;
            }

            return doesPasswordHaveForbiddenSubstring;
        }

        private void ReadSubstringFileIfModified()
        {
            if (FileUtilities.IsFileModified(_forbiddenSubstringsFilePath, forbiddenSubstringsFileModTime))
            {
                forbiddenSubstringsFileModTime = File.GetLastWriteTime(_forbiddenSubstringsFilePath);
                forbiddenSubstrings = FileUtilities.ReadTextFile(_forbiddenSubstringsFilePath);
            }
        }

        private void ReadPatternsFileIfModified()
        {
            if (FileUtilities.IsFileModified(_forbiddenRegexesFilePath, forbiddenRegexesFileModTime))
            {
                forbiddenRegexesFileModTime = File.GetLastWriteTime(_forbiddenRegexesFilePath);
                forbiddenRegexes = FileUtilities.ReadForbiddenRegexesFile(_forbiddenRegexesFilePath);
            }
        }

        private void ReadForbiddenPasswordsFileIfModified()
        {
            if (FileUtilities.IsFileModified(_forbiddenPasswordsFilePath, forbiddenPasswordsFileModTime))
            {
                forbiddenPasswordsFileModTime = File.GetLastWriteTime(_forbiddenPasswordsFilePath);
                forbiddenPasswords = FileUtilities.ReadTextFile(_forbiddenPasswordsFilePath);
            }
        }

        private bool IsPasswordForbidden(string password)
        {
            var methodName = $"{nameof(OPFDictionary)}::{nameof(IsPasswordForbidden)}";

            var isPasswordForbidden = false;

            var forbiddenPassword = forbiddenPasswords.FirstOrDefault(p => p == password);
            if (string.IsNullOrEmpty(forbiddenPassword))
            {
                _logger.Info($"[{methodName}] - Given password is forbidden. " +
                    $"Forbidden password = {forbiddenPassword}");

                isPasswordForbidden = true;
            }

            return isPasswordForbidden;
        }

        private bool DoesPasswordMatchForbiddenPattern(string password)
        {
            var methodName = $"{nameof(OPFDictionary)}::{nameof(DoesPasswordMatchForbiddenPattern)}";

            var doesPasswordMatchForbiddenPattern = false;

            var forbiddenRegex = forbiddenRegexes.FirstOrDefault(r => r.Match(password).Success);
            if (forbiddenRegex != null)
            {
                _logger.Info($"[{methodName}] - Given password matches with forbidden pattern. " +
                    $"Password = . Pattern = {forbiddenRegex}");

                doesPasswordMatchForbiddenPattern = true;
            }

            return doesPasswordMatchForbiddenPattern;
        }

        private bool DoesPasswordHaveUserInfo(string username, string password)
        {
            var methodName = $"{nameof(OPFDictionary)}::{nameof(DoesPasswordHaveUserInfo)}";

            var doesPasswordHaveUserInfo = false;

            var userInfo = ActiveDirectoryUtilities.GetUserInfo(username);
            var passwordContainingUserInfo = userInfo.FirstOrDefault(u => password.Contains(u.Value)).Key;

            if (!string.IsNullOrEmpty(passwordContainingUserInfo))
            {
                _logger.Info($"[{methodName}] - Given password contains user info. " +
                    $"Password = . User Info = {passwordContainingUserInfo}");

                doesPasswordHaveUserInfo = true;
            }

            return doesPasswordHaveUserInfo;
        }
    }
}
