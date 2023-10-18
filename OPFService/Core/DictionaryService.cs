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
using System.Linq;
using System.Text.RegularExpressions;
using Topshelf.Logging;

namespace OPFService.Core;

public sealed class DictionaryService
{
    private readonly LogWriter logger = HostLogger.Get<DictionaryService>();
    private readonly string forbiddenPasswordsFilePath;
    private readonly string forbiddenSubstringsFilePath;
    private readonly string forbiddenRegexesFilePath;
    private HashSet<string> forbiddenPasswords;
    private HashSet<string> forbiddenSubstrings;
    private HashSet<Regex> forbiddenRegexes;
    private DateTime forbiddenPasswordsFileModTime;
    private DateTime forbiddenSubstringsFileModTime;
    private DateTime forbiddenRegexesFileModTime;

    public DictionaryService(string forbiddenPasswordsFilePath, string forbiddenSubstringsFilePath, string forbiddenRegexesFilePath)
    {
        this.forbiddenPasswordsFilePath = forbiddenPasswordsFilePath;
        this.forbiddenSubstringsFilePath = forbiddenSubstringsFilePath;
        this.forbiddenRegexesFilePath = forbiddenRegexesFilePath;
    }

    public bool DoesPasswordHaveForbiddenSubstring(string password)
    {
        var methodName = $"{nameof(DictionaryService)}::{nameof(DoesPasswordHaveForbiddenSubstring)}";

        this.forbiddenSubstrings = FileUtilities.ReadTextFileIfModified(this.forbiddenSubstringsFilePath, ref this.forbiddenSubstringsFileModTime);

        var forbiddenSubstring = this.forbiddenSubstrings.FirstOrDefault(password.Contains);
        if (!string.IsNullOrEmpty(forbiddenSubstring))
        {
            this.logger.Debug($"[{methodName}] - Given password contains forbidden substring. " +
                    $"Password = . Forbidden substring = {forbiddenSubstring}");

            return true;
        }

        return false;
    }

    public bool IsPasswordForbidden(string password)
    {
        var methodName = $"{nameof(DictionaryService)}::{nameof(IsPasswordForbidden)}";

        this.forbiddenPasswords = FileUtilities.ReadTextFileIfModified(this.forbiddenPasswordsFilePath, ref this.forbiddenPasswordsFileModTime);

        if (this.forbiddenPasswords.Contains(password))
        {
            this.logger.Debug($"[{methodName}] - Given password is forbidden. Forbidden password = {password}");
            return true;
        }

        return false;
    }

    public bool DoesPasswordMatchForbiddenPattern(string password)
    {
        var methodName = $"{nameof(DictionaryService)}::{nameof(DoesPasswordMatchForbiddenPattern)}";

        this.forbiddenRegexes = FileUtilities.ReadRegexesFileIfModified(this.forbiddenRegexesFilePath, ref forbiddenRegexesFileModTime);

        var regex = this.forbiddenRegexes.FirstOrDefault(r => r.Match(password).Success);
        if (regex is not null)
        {
            this.logger.Debug($"[{methodName}] - Given password matches with forbidden pattern. " +
                $"Password = . Pattern = {regex}");

            return true;
        }

        return false;
    }
}
