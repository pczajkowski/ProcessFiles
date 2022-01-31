using System.Collections.Generic;
using System.IO;
using Xunit;

namespace ProcessFilesTests
{
    public class ProcessFilesTests
    {
        private readonly string testFolder = "./testFiles";
        private readonly string testFile = "./testFiles/test1.txt";

        private List<string> expectedInSubFolder = new List<string>
        {
            "test2.txt",
            "test3.txt"
        };

        private string exptectedInFolder = "test1.txt";

        [Fact]
        public void ProcessFolderTest()
        {
            var result = string.Empty;
            void Callback(string value)
            {
                result = Path.GetFileName(value);
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFolder }, "txt", Callback);
            Assert.Empty(errors);
            Assert.Equal(exptectedInFolder, result);
        }

        private bool CheckResult(List<string> result, List<string> expected)
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
            var expected = new List<string>
            {
                exptectedInFolder
            };
            expected.AddRange(expectedInSubFolder);

            var result = new List<string>();
            void Callback(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFolder }, "txt", Callback, true);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expected));
        }

        [Fact]
        public void ProcessFolderAndFileTest()
        {
            var expected = new List<string>
            {
                exptectedInFolder
            };
            expected.AddRange(expectedInSubFolder);

            var result = new List<string>();
            void Callback(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { "./testFiles/subFolder", testFile }, "txt", Callback);
            Assert.Empty(errors);
            Assert.True(CheckResult(result, expected));
        }

        [Fact]
        public void ProcessFileTest()
        {
            var result = string.Empty;
            void Callback(string value)
            {
                result = Path.GetFileName(value);
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFile }, "txt", Callback);
            Assert.Empty(errors);
            Assert.Equal(exptectedInFolder, result);
        }

        [Fact]
        public void ProcessFileNotExistTest()
        {
            var result = string.Empty;
            void Callback(string value)
            {
                result = value;
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { "./testFiles/test.txt" }, "txt", Callback);
            Assert.NotEmpty(errors);
            Assert.NotEqual(exptectedInFolder, result);
        }

        [Fact]
        public void ProcessFileNotMatchExtensionTest()
        {
            var result = string.Empty;
            void Callback(string value)
            {
                result = value;
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { testFile }, "abc", Callback);
            Assert.NotEmpty(errors);
            Assert.Empty(result);
        }
    }
}
