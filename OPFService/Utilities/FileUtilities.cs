using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Topshelf.Logging;

namespace OPFService.Utilities
{
    public class FileUtilities
    {
        private static readonly LogWriter _logger = HostLogger.Get<FileUtilities>();

        public static bool IsFileModified(string filePath, DateTime currentLastModTime)
        {
            var methodName = $"{nameof(FileUtilities)}::{nameof(IsFileModified)}";

            var isFileModified = false;

            if (!string.IsNullOrEmpty(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    isFileModified = currentLastModTime.ToUniversalTime() != fileInfo.LastWriteTimeUtc;

                    _logger.Info($"[{methodName}] - File is modified = {isFileModified}. " +
                        $"File full name = {fileInfo.FullName}");
                }
            }
            else
            {
                _logger.Error($"[{methodName}] - Given file path is null or empty");
            }

            return isFileModified;
        }

        public static HashSet<string> ReadTextFile(string filePath)
        {
            var methodName = $"{nameof(FileUtilities)}::{nameof(ReadTextFile)}";

            var fileData = new HashSet<string>();

            if (!string.IsNullOrEmpty(filePath))
            {
                var fileInfo = new FileInfo(filePath);
                if (fileInfo.Exists)
                {
                    using (var streamReader = new StreamReader(fileInfo.FullName))
                    {
                        while (!streamReader.EndOfStream)
                        {
                            fileData.Add(streamReader.ReadLine());
                        }

                        _logger.Info($"[{methodName}] - File was read successfully. " +
                            $"Line count = {fileData.Count}");
                    }
                }
                else
                {
                    _logger.Error($"[{methodName}] - " +
                        $"File does not exist at given path. Path = {filePath}");
                }
            }
            else
            {
                _logger.Error($"[{methodName}] - Given file path is null or empty");
            }

            return fileData;
        }

        public static HashSet<Regex> ReadForbiddenRegexesFile(string filePath)
        {
            var methodName = $"{nameof(FileUtilities)}::{nameof(ReadForbiddenRegexesFile)}";

            var forbiddenRegexes = new HashSet<Regex>();

            if (File.Exists(filePath))
            {
                try
                {
                    using (var streamReader = new StreamReader(filePath))
                    {
                        for (int lineNumber = 1; !streamReader.EndOfStream; lineNumber++)
                        {
                            try
                            {
                                forbiddenRegexes.Add(new Regex(streamReader.ReadLine()));
                            }
                            catch (Exception ex)
                            {
                                _logger.Error($"[{methodName}] - Error on line number #{lineNumber}." +
                                    $"Error = {ex.Message}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error($"[{methodName}] - Unexpected exception. Error = {ex.Message}");
                }
            }
            else
            {
                _logger.Error($"[{methodName}] - File does not exist at given path. " +
                    $"Path = {filePath}");
            }

            return forbiddenRegexes;
        }
    }
}
