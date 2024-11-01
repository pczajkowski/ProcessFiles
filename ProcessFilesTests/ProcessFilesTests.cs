using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using ProcessFiles;

namespace ProcessFilesTests;

public class ProcessFilesTests
{
    private const string TestFolder = "./testFiles";
    private const string TestFile = "./testFiles/test1.txt";

    private const string ExpectedInFolder = "test1.txt";

    private readonly List<string> expectedInSubFolder =
    [
        ExpectedInFolder,
        "test2.txt",
        "test3.txt"
    ];

    private readonly List<string> expectedInSubFolderMultipleExtensions =
    [
        "test.json"
    ];

    public ProcessFilesTests()
    {
        expectedInSubFolderMultipleExtensions.AddRange(expectedInSubFolder);
    }

    [Fact]
    public void ProcessFolderTest()
    {
        var result = string.Empty;

        var test = new FileProcessing();
        var errors = test.Process([TestFolder], "txt", TestAction);
        Assert.Empty(errors);
        Assert.Equal(ExpectedInFolder, result);
        return;

        void TestAction(string value)
        {
            result = Path.GetFileName(value);
        }
    }

    private static bool CheckResult(List<string> result, List<string> expected)
    {
        return result.Count.Equals(expected.Count) && result.All(expected.Contains);
    }

    [Fact]
    public void ProcessFolderRecursiveTest()
    {
        var result = new List<string>();

        var test = new FileProcessing();
        var errors = test.Process([TestFolder], "txt", TestAction, true);
        Assert.Empty(errors);
        Assert.True(CheckResult(result, expectedInSubFolder));
        return;

        void TestAction(string value)
        {
            result.Add(Path.GetFileName(value));
        }
    }

    [Fact]
    public void ProcessFolderAndFileTest()
    {   
        var result = new List<string>();

        var test = new FileProcessing();
        var errors = test.Process(["./testFiles/subFolder", TestFile], "txt", TestAction);
        Assert.Empty(errors);
        Assert.True(CheckResult(result, expectedInSubFolder));
        return;

        void TestAction(string value)
        {
            result.Add(Path.GetFileName(value));
        }
    }

    [Fact]
    public void ProcessFileTest()
    {
        var result = string.Empty;

        var test = new FileProcessing();
        var errors = test.Process([TestFile], "txt", TestAction);
        Assert.Empty(errors);
        Assert.Equal(ExpectedInFolder, result);
        return;

        void TestAction(string value)
        {
            result = Path.GetFileName(value);
        }
    }

    [Fact]
    public void ProcessFileNotExistTest()
    {
        var result = string.Empty;

        var test = new FileProcessing();
        var errors = test.Process(["./testFiles/test.txt"], "txt", TestAction);
        Assert.NotEmpty(errors);
        Assert.Empty(result);
        return;

        void TestAction(string value)
        {
            result = value;
        }
    }

    [Fact]
    public void ProcessFileNotMatchExtensionTest()
    {
        var result = string.Empty;

        var test = new FileProcessing();
        var errors = test.Process([TestFile], "abc", TestAction);
        Assert.NotEmpty(errors);
        Assert.Empty(result);
        return;

        void TestAction(string value)
        {
            result = value;
        }
    }

    [Fact]
    public void ProcessFolderRecursiveMultipleExtensionsTest()
    {
        var result = new List<string>();

        var test = new FileProcessing();
        var errors = test.Process([TestFolder], ["txt", "json"], TestAction, true);
        Assert.Empty(errors);
        Assert.True(CheckResult(result, expectedInSubFolderMultipleExtensions));
        return;

        void TestAction(string value)
        {
            result.Add(Path.GetFileName(value));
        }
    }
}