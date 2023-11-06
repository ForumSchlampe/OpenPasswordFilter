using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Topshelf.Logging;

namespace OPFService.Utilities;

public sealed class FileUtilities
{
    private static readonly LogWriter logger = HostLogger.Get<FileUtilities>();

    public static HashSet<string> ReadTextFileIfModified(string filePath, ref DateTime currentLastModTime, out bool isModified)
    {
        isModified = IsFileModified(filePath, currentLastModTime);
        if (isModified)
        {
            currentLastModTime = File.GetLastWriteTime(filePath);
            return ReadTextFile(filePath);
        }

        return new();
    }

    public static HashSet<Regex> ReadRegexesFileIfModified(string filePath, ref DateTime currentLastModTime, out bool isModified)
    {
        isModified = IsFileModified(filePath, currentLastModTime);
        if (isModified)
        {
            currentLastModTime = File.GetLastWriteTime(filePath);
            return ReadRegexesFile(filePath);
        }

        return new();
    }

    public static bool IsFileModified(string filePath, DateTime currentLastModTime)
    {
        var methodName = $"{nameof(FileUtilities)}::{nameof(IsFileModified)}";

        var isFileModified = false;

        if (!string.IsNullOrEmpty(filePath))
        {
            var fileInfo = new FileInfo(filePath);
            if (fileInfo.Exists)
            {
                isFileModified = DateTime.Compare(currentLastModTime.ToUniversalTime(), fileInfo.LastWriteTimeUtc) < 0;

                logger.Debug($"[{methodName}] - File is modified = {isFileModified}. " +
                    $"File full name = {fileInfo.FullName}");
            }
        }
        else
        {
            logger.Error($"[{methodName}] - Given file path is null or empty");
        }

        return isFileModified;
    }

    public static HashSet<string> ReadTextFile(string filePath)
    {
        var methodName = $"{nameof(FileUtilities)}::{nameof(ReadTextFile)}";

        HashSet<string> fileData = default;

        if (!string.IsNullOrEmpty(filePath))
        {
            if (File.Exists(filePath))
            {
                fileData = File.ReadAllLines(filePath).ToHashSet();
                logger.Debug($"[{methodName}] - File was read successfully. " +
                        $"Line count = {fileData.Count}");
            }
            else
            {
                logger.Error($"[{methodName}] - " +
                    $"File does not exist at given path. Path = {filePath}");
            }
        }
        else
        {
            logger.Error($"[{methodName}] - Given file path is null or empty");
        }

        return fileData ?? new();
    }

    public static HashSet<Regex> ReadRegexesFile(string filePath)
    {
        var methodName = $"{nameof(FileUtilities)}::{nameof(ReadRegexesFile)}";

        HashSet<Regex> forbiddenRegexes = new();

        int lineNumber = 1;
        foreach (var line in ReadTextFile(filePath))
        {
            try
            {
                forbiddenRegexes.Add(new Regex(line));
            }
            catch (Exception ex)
            {
                logger.Error($"[{methodName}] - Error on line number #{lineNumber}." +
                    $"Error = {ex.Message}", ex);
            }

            lineNumber++;
        }

        return forbiddenRegexes;
    }
}
