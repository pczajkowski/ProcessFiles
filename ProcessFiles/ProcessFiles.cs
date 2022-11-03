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

        private static Result WhatIsIt(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
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

        private static bool IsValid(string path, string fileExtension)
        {
            if (!File.Exists(path))
            {
                _errors.Add($"{path} doesn't exist!");
                return false;
            }

            var extension = Path.GetExtension(path)?.TrimStart('.');
            if (string.IsNullOrWhiteSpace(extension))
            {
                _errors.Add($"Can't establish extension of {path}!");
                return false;
            }

            if (!extension.Equals(fileExtension, StringComparison.InvariantCultureIgnoreCase))
            {
                _errors.Add($"Extension of {path} doesn't match {fileExtension}!");
                return false;
            }

            return true;
        }

        private static void ProcessFile(string path, string fileExtension, Action<string> action)
        {
            if (!IsValid(path, fileExtension))
                return;

            try
            {
                action(path);
            }
            catch (Exception e)
            {
                _errors.Add(e.ToString());
            }
        }

        private static void ProcessDir(string path, string fileExtension, Action<string> action, bool recursive = false)
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
                ProcessFile(file, fileExtension, action);
        }

        public static IEnumerable<string> Process(IEnumerable<string> arguments, string fileExtension, Action<string> action, bool recursive = false)
        {
            _errors = new List<string>();

            foreach (var argument in arguments)
            {
                switch (WhatIsIt(argument))
                {
                    case Result.File:
                        ProcessFile(argument, fileExtension, action);
                        break;
                    case Result.Directory:
                        ProcessDir(argument, fileExtension, action, recursive);
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
