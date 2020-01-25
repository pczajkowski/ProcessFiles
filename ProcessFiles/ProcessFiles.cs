using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProcessFiles
{
    public static class ProcessFiles
    {
        private static List<string> _errors;
        private static void ProcessDir(string path, string fileExtension, Action<string> callback, bool recursive = false)
        {
            var searchOption = SearchOption.TopDirectoryOnly;
            if (recursive)
                searchOption = SearchOption.AllDirectories;

            var files = Directory.GetFiles(path, $"*.{fileExtension}", searchOption);
            if (!files.Any())
            {
                _errors.Add($"There are no {fileExtension} files in {path}!");
                return;
            }

            foreach (var file in files)
                callback(file);
        }

        private static bool CheckForFile(string argument, string fileExtension, Action<string> callback)
        {
            if (!File.Exists(argument))
                return false;

            var extension = Path.GetExtension(argument);
            if (string.IsNullOrWhiteSpace(extension))
            {
                _errors.Add($"Can't establish extension of {argument}!");
                return false;
            }

            if (!extension.Equals($".{fileExtension}"))
            {
                _errors.Add($"Extension of {argument} doesn't match {fileExtension}!");
                return false;
            }

            callback(argument);
            return true;
        }

        public static IEnumerable<string> Process(IEnumerable<string> arguments, string fileExtension, Action<string> callback, bool recursive = false)
        {
            _errors = new List<string>();

            foreach (var argument in arguments)
            {
                if (CheckForFile(argument, fileExtension, callback))
                    continue;

                if (!Directory.Exists(argument))
                {
                    _errors.Add($"{argument} is neither file nor directory!");
                    continue;
                }

                ProcessDir(argument, fileExtension, callback, recursive);
            }

            return _errors;
        }
    }
}
