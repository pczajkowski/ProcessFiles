using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace ProcessFilesTests
{
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
            void TestAction(string value)
            {
                result = Path.GetFileName(value);
            }

            var test = new FileOperations.ProcessFiles();
            var errors = test.Process([TestFolder], "txt", TestAction);
            Assert.Empty(errors);
            Assert.Equal(ExpectedInFolder, result);
        }

        private static bool CheckResult(List<string> result, List<string> expected)
        {
            return result.Count.Equals(expected.Count) && result.All(expected.Contains);
        }

        [Fact]
        public void ProcessFolderRecursiveTest()
        {
            var result = new List<string>();
            void TestAction(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var test = new FileOperations.ProcessFiles();
            var errors = test.Process([TestFolder], "txt", TestAction, true);
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

            var test = new FileOperations.ProcessFiles();
            var errors = test.Process(["./testFiles/subFolder", TestFile], "txt", TestAction);
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

            var test = new FileOperations.ProcessFiles();
            var errors = test.Process([TestFile], "txt", TestAction);
            Assert.Empty(errors);
            Assert.Equal(ExpectedInFolder, result);
        }

        [Fact]
        public void ProcessFileNotExistTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = value;
            }

            var test = new FileOperations.ProcessFiles();
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

            var test = new FileOperations.ProcessFiles();
            var errors = test.Process([TestFile], "abc", TestAction);
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

            var test = new FileOperations.ProcessFiles();
            var errors = test.Process([TestFolder], ["txt", "json"], TestAction, true);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expectedInSubFolderMultipleExtensions));
        }
    }
}
