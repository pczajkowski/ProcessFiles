using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProcessFiles.Interfaces;
using ProcessFiles.Models;

namespace ProcessFiles;

public class FileProcessing(IFileSystem? fileSystem = null)
{
    private readonly IFileSystem fileSystem = fileSystem ?? new DefaultFileSystem();
    private List<string> errors = [];

    private Result WhatIsIt(string path)
    {
        try
        {
            var attr = fileSystem.File.GetAttributes(path);
            return attr.HasFlag(FileAttributes.Directory) ? Result.Directory : Result.File;
        }
        catch (Exception e)
        {
            errors.Add($"Problem getting {path} attributes:\n" +
                        $"{e.Message} - {e.Source} - {e.TargetSite}");
                
            return Result.Failure;
        }
    }

    private string? GetExtension(string path)
    {
        var extension = fileSystem.Path.GetExtension(path)?.TrimStart('.');
        if (!string.IsNullOrWhiteSpace(extension)) return extension;
            
        errors.Add($"Can't establish extension of {path}!");
        return null;
    }

    private static bool CheckExtension(string extension, string[] validExtensions)
        => validExtensions.Any(x => x.Equals(extension, StringComparison.InvariantCultureIgnoreCase));
    private bool IsValid(string path, string[] fileExtensions)
    {
        if (!fileSystem.File.Exists(path))
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

    private void PerformAction(string path, Action<string> action)
    {
        try
        {
            action(path);
        }
        catch (Exception e)
        {
            errors.Add($"{path}:\n{e.Message} - {e.Source} - {e.TargetSite}");
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
        if (!fileSystem.Directory.Exists(path))
        {
            errors.Add($"{path} doesn't exist!");
            return;
        }

        var searchOption = recursive switch
        {
            false => SearchOption.TopDirectoryOnly,
            true => SearchOption.AllDirectories
        };

        List<string> files = [];
        foreach (var extension in fileExtensions)
        {
            files.AddRange(fileSystem.Directory.GetFiles(path, $"*.{extension}", searchOption));
        }

        if (files.Count == 0)
        {
            errors.Add($"There are no files in {path} with given extensions ({string.Join(", ", fileExtensions)})!");
            return;
        }

        foreach (var file in files)
            ProcessFile(file, fileExtensions, action);
    }

    public IEnumerable<string> Process(IEnumerable<string> arguments, string fileExtension, Action<string> action, bool recursive = false)
    {
        return Process(arguments, [fileExtension], action, recursive);
    }

    public IEnumerable<string> Process(IEnumerable<string> arguments, string[] fileExtensions, Action<string> action, bool recursive = false)
    {
        errors = [];
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
                    continue;
                default:
                    throw new ArgumentOutOfRangeException(nameof(arguments));
            }
        }

        return errors;
    }
}