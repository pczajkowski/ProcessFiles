using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ProcessFiles
{
    public enum Result
    {
        File,
        Directory,
        Failure
    }

    public static class ProcessFiles
    {
        private static List<string> _errors;

        private static Result WhatIsIt(string argument)
        {
            try
            {
                var attr = File.GetAttributes(argument);
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                    return Result.Directory;

                return Result.File;
            }
            catch (Exception e)
            {
                _errors.Add(e.ToString());
                return Result.Failure;
            }
        }

        private static bool IsValid(string argument, string fileExtension)
        {
            if (string.IsNullOrWhiteSpace(argument) || !File.Exists(argument))
            {
                _errors.Add($"{argument} doesn't exist!");
                return false;
            }

            var extension = Path.GetExtension(argument);
            if (string.IsNullOrWhiteSpace(extension))
            {
                _errors.Add($"Can't establish extension of {argument}!");
                return false;
            }

            if (!extension.TrimStart('.').Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                _errors.Add($"Extension of {argument} doesn't match {fileExtension}!");
                return false;
            }

            return true;
        }

        private static void ProcessFile(string path, string fileExtension, Action<string> callback)
        {
            if (!IsValid(path, fileExtension))
                return;

            try
            {
                callback(path);
            }
            catch (Exception e)
            {
                _errors.Add(e.ToString());
            }
        }

        private static void ProcessDir(string path, string fileExtension, Action<string> callback, bool recursive = false)
        {
            if (!Directory.Exists(path))
            {
                _errors.Add($"{path} doesn't exist!");
                return;
            }

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
                ProcessFile(file, fileExtension, callback);
        }

        public static IEnumerable<string> Process(IEnumerable<string> arguments, string fileExtension, Action<string> callback, bool recursive = false)
        {
            _errors = new List<string>();

            foreach (var argument in arguments)
            {
                switch (WhatIsIt(argument))
                {
                    case Result.File:
                        ProcessFile(argument, fileExtension, callback);
                        break;
                    case Result.Directory:
                        ProcessDir(argument, fileExtension, callback, recursive);
                        break;
                    case Result.Failure:
                        continue;
                    default:
                        break;
                }
            }

            return _errors;
        }
    }
}
