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

    public class ProcessFiles
    {
        private readonly List<string> errors = [];
        private  readonly  List<char> invalidCharactersForExtension = ['?', '*'];

        private Result WhatIsIt(string path)
        {
            if (!File.Exists(path) && !Directory.Exists(path))
            {
                errors.Add($"{path} doesn't exist!");
                return Result.Failure;
            }
            
            try
            {
                var attr = File.GetAttributes(path);
                return (attr & FileAttributes.Directory) == FileAttributes.Directory ? Result.Directory : Result.File;
            }
            catch (Exception e)
            {
                errors.Add($"Problem getting attributes of {path}: {e.Message} ({e.Source})");
                return Result.Failure;
            }
        }

        private string? GetExtension(string path)
        {
            var extension = Path.GetExtension(path).TrimStart('.');
            if (!string.IsNullOrWhiteSpace(extension)) return extension;
            
            errors.Add($"Can't establish extension of {path}!");
            return null;
        }

        private static bool CheckExtension(string extension, string[] validExtensions)
        {
            return validExtensions.Any(validExtension => extension.Equals(validExtension, StringComparison.OrdinalIgnoreCase));
        }

        private bool IsValid(string path, string[] fileExtensions)
        {
            var extension = GetExtension(path);
            if (extension == null)
                return false;

            if (CheckExtension(extension, fileExtensions)) return true;
            
            errors.Add($"Extension of {path} doesn't match any extension ({string.Join(", ", fileExtensions)})!");
            return false;

        }

        private void PerformAction(string path, Action<string> action)
        {
            try
            {
                action(path);
            }
            catch (Exception e)
            {
                errors.Add($"Problem performing action on {path}: {e.Message} ({e.Source})");
            }
        }

        private void ProcessFile(string path, string[] fileExtensions, Action<string> action)
        {
            if (!IsValid(path, fileExtensions))
                return;

            PerformAction(path, action);
        }

        private void ProcessDir(string path, string[] fileExtensions, Action<string> action, bool recursive = false)
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

            string[] allFiles = [];
            try
            {
                allFiles = Directory.GetFiles(path, "*", searchOption);
            }
            catch (Exception e)
            {
                errors.Add($"Problem getting files: {e.Message} ({e.Source})");
                return;
            }

            var files = allFiles
                .Where(file => CheckExtension(Path.GetExtension(file).TrimStart('.'), fileExtensions))
                .Distinct()
                .ToList();
            if (files.Count == 0)
            {
                errors.Add($"There are no files in {path} with given extensions ({string.Join(", ", fileExtensions)})!");
                return;
            }

            foreach (var file in files)
                ProcessFile(file, fileExtensions, action);
        }
        
        private bool ValidExtension(string fileExtension) => !invalidCharactersForExtension.Any(fileExtension.Contains);
        
        private List<string> GetInvalidExtensions(string [] fileExtensions) => [.. fileExtensions.Where(x => !ValidExtension(x))];

        public List<string> Process(IEnumerable<string> arguments, string fileExtension, Action<string> action, bool recursive = false)
        {
            return Process(arguments, [fileExtension], action, recursive);
        }

        public List<string> Process(IEnumerable<string> arguments, string[] fileExtensions, Action<string> action, bool recursive = false)
        {
            var invalidExtensions = GetInvalidExtensions(fileExtensions);
            if (invalidExtensions.Count > 0) throw new ArgumentException($"Following extensions contain invalid characters ({string.Join(", ", invalidCharactersForExtension)}):\n{string.Join(Environment.NewLine, invalidExtensions)}");
            
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

            return [.. errors];
        }
    }
}
