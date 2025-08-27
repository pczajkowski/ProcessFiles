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
        private static List<string> errors = new();

        private static Result WhatIsIt(string path)
        {
            try
            {
                var attr = File.GetAttributes(path);
                return (attr & FileAttributes.Directory) == FileAttributes.Directory ? Result.Directory : Result.File;
            }
            catch (Exception e)
            {
                errors.Add(e.ToString());
                return Result.Failure;
            }
        }

        private static string? GetExtension(string path)
        {
            var extension = Path.GetExtension(path).TrimStart('.');
            if (!string.IsNullOrWhiteSpace(extension)) return extension;
            
            errors.Add($"Can't establish extension of {path}!");
            return null;
        }

        private static bool CheckExtension(string extension, string[] validExtensions)
        {
            return validExtensions.Any(validExtension => extension.Equals(validExtension, StringComparison.InvariantCultureIgnoreCase));
        }

        private static bool IsValid(string path, string[] fileExtensions)
        {
            if (!File.Exists(path))
            {
                errors.Add($"{path} doesn't exist!");
                return false;
            }

            var extension = GetExtension(path);
            if (extension == null)
                return false;

            if (CheckExtension(extension, fileExtensions)) return true;
            
            errors.Add($"Extension of {path} doesn't match any extension ({string.Join(", ", fileExtensions)})!");
            return false;

        }

        private static void PerformAction(string path, Action<string> action)
        {
            try
            {
                action(path);
            }
            catch (Exception e)
            {
                errors.Add($"{path}:\n{e}");
            }
        }

        private static void ProcessFile(string path, string[] fileExtensions, Action<string> action)
        {
            if (!IsValid(path, fileExtensions))
                return;

            PerformAction(path, action);
        }

        private static void ProcessDir(string path, string[] fileExtensions, Action<string> action, bool recursive = false)
        {
            if (!Directory.Exists(path))
            {
                errors.Add($"{path} doesn't exist!");
                return;
            }

            var searchOption = recursive switch
            {
                false => SearchOption.TopDirectoryOnly,
                true => SearchOption.AllDirectories,
            };

            List<string> files = [];
            foreach (var extension in fileExtensions)
            {
                files.AddRange(Directory.GetFiles(path, $"*.{extension}", searchOption));
            }

            if (files.Count == 0)
            {
                errors.Add($"There are no files in {path} with given extensions ({string.Join(", ", fileExtensions)})!");
                return;
            }

            foreach (var file in files)
                ProcessFile(file, fileExtensions, action);
        }

        public static IEnumerable<string> Process(IEnumerable<string> arguments, string fileExtension, Action<string> action, bool recursive = false)
        {
            return Process(arguments, [fileExtension], action, recursive);
        }

        public static IEnumerable<string> Process(IEnumerable<string> arguments, string[] fileExtensions, Action<string> action, bool recursive = false)
        {
            errors = new List<string>();

            foreach (var argument in arguments)
            {
                switch (WhatIsIt(argument))
                {
                    case Result.File:
                        ProcessFile(argument, fileExtensions, action);
                        break;
                    case Result.Directory:
                        ProcessDir(argument, fileExtensions, action, recursive);
                        break;
                    case Result.Failure:
                    default:
                        continue;
                }
            }

            return errors;
        }
    }
}
