using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProcessFiles;
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

        private const string ExpectedInFolder = "test1.txt";

        public ProcessFilesTests()
        {
            expectedInSubFolder.Add(ExpectedInFolder);
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
            Assert.Equal(ExpectedInFolder, result);
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

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process(["./testFiles/test.txt"], "txt", TestAction);
            Assert.NotEmpty(errors);
            Assert.Empty(result);
        }
        
        [Fact]
        public void ProcessDirectoryNotExistTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = value;
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process(["./nonExistent"], "txt", TestAction);
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
        public void ProcessFileOnceTest()
        {
            var result = new Dictionary<string, int>();
            void TestAction(string value)
            {
                if (result.ContainsKey(value)) result[value]++;
                else result.Add(value, 1);
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors = test.Process([testFolder], ["txt", "TxT"], TestAction);
            Assert.Empty(errors);
            Assert.Empty(result.Values.Where(x => x > 1).ToList());
        }
        
        [Fact]
        public void ProcessThrowsIfInvalidExtension()
        {
            var test = new ProcessFiles.ProcessFiles();
            
            foreach (var character in ProcessFiles.ProcessFiles.InvalidCharactersForExtension)
            {
                Assert.Throws<ArgumentException>(() => test.Process([testFolder], $"test{character}test", Console.WriteLine));
            }
        }
        
        [Fact]
        public void ProcessErrorsDontAccumulate()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = value;
            }

            var test = new ProcessFiles.ProcessFiles();
            var errors1 = test.Process(["./testFiles/test.txt"], "txt", TestAction);
            var errors2 = test.Process(["./nonExistent"], "txt", TestAction);
            Assert.NotEmpty(errors1);
            Assert.NotEmpty(errors2);
            Assert.DoesNotContain(errors1, x => errors2.Contains(x));
        }
    }
}
