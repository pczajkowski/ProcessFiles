using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace ProcessFilesTests
{
    public class ProcessFilesTests
    {
        private readonly string testFolder = "./testFiles";
        private readonly string testFile = "./testFiles/test1.txt";

        private readonly List<string> expectedInSubFolder =
        [
            "test2.txt",
            "test3.txt"
        ];

        private readonly List<string> expectedInSubFolderMultipleExtensions = ["test.json"];

        private const string expectedInFolder = "test1.txt";

        public ProcessFilesTests()
        {
            expectedInSubFolder.Add(expectedInFolder);
            expectedInSubFolderMultipleExtensions.AddRange(expectedInSubFolder);
        }

        [Fact]
        public void ProcessFolderTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = Path.GetFileName(value);
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process([testFolder], "txt", TestAction);
            Assert.Empty(errors);
            Assert.Equal(expectedInFolder, result);
        }

        private static bool CheckResult(List<string> result, List<string> expected)
        {
            if (!result.Count.Equals(expected.Count))
                return false;

            foreach (var item in result)
            {
                if (!expected.Contains(item))
                    return false;
            }

            return true;
        }

        [Fact]
        public void ProcessFolderRecursiveTest()
        {
            var result = new List<string>();
            void TestAction(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process([testFolder], "txt", TestAction, true);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expectedInSubFolder));
        }

        [Fact]
        public void ProcessFolderAndFileTest()
        {   
            var result = new List<string>();
            void TestAction(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process(["./testFiles/subFolder", testFile], "txt", TestAction);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expectedInSubFolder));
        }

        [Fact]
        public void ProcessFileTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = Path.GetFileName(value);
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process([testFile], "txt", TestAction);
            Assert.Empty(errors);
            Assert.Equal(expectedInFolder, result);
        }

        [Fact]
        public void ProcessFileNotExistTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = value;
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process(["./testFiles/test.txt"], "txt", TestAction);
            Assert.NotEmpty(errors);
            Assert.Empty(result);
        }

        [Fact]
        public void ProcessFileNotMatchExtensionTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = value;
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process([testFile], "abc", TestAction);
            Assert.NotEmpty(errors);
            Assert.Empty(result);
        }

        [Fact]
        public void ProcessFolderRecursiveMultipleExtensionsTest()
        {
            var result = new List<string>();
            void TestAction(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process([testFolder], ["txt", "json"], TestAction, true);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expectedInSubFolderMultipleExtensions));
        }
        
        [Fact]
        public void ProcessWhenActionInvokesProcessShouldPreserveOuterErrors()
        {
            var missingPath = Path.Combine(testFolder, "missing.txt");
            var nestedMissingPath = Path.Combine(testFolder, "nested-missing.txt");

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process(
                [missingPath, testFile],
                "txt",
                _ => test.Process([nestedMissingPath], "txt", _ => { })).ToList();

            Assert.True(errors.Exists(x => x.StartsWith($"Problem getting attributes of {missingPath}")));
            Assert.True(errors.Exists(x => x.StartsWith($"Problem getting attributes of {nestedMissingPath}")));
        }

        [Fact]
        public void ProcessDirectoryEnumerationFailureShouldBeReportedAsError()
        {
            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process(
                [testFolder],
                ["txt["], // invalid search pattern forces Directory.GetFiles to throw
                _ => { }).ToList();

            Assert.Single(errors);
            Assert.Contains(testFolder, errors[0]);
            Assert.Contains("txt[", errors[0]);
        }
    }
}
