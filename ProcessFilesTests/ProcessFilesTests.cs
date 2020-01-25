using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;

namespace ProcessFilesTests
{
    public class ProcessFilesTests
    {
        [Fact]
        public void ProcessFolderTest()
        {
            const string expected = "test1.txt";
            var result = string.Empty;

            void Callback(string value)
            {
               result = Path.GetFileName(value);
            }

            var errors = ProcessFiles.ProcessFiles.Process(new [] {"./testFiles"}, "txt", Callback);
            Assert.Empty(errors);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ProcessFolderRecursiveTest()
        {
            var expected = new[]
            {
                "test1.txt",
                "test2.txt"
            };

            var result = new List<string>();
            void Callback(string value)
            {
                result.Add(Path.GetFileName(value));
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { "./testFiles" }, "txt", Callback, true);
            Assert.Empty(errors);

            bool CheckResult()
            {
                foreach (var item in result)
                {
                    if (!expected.Contains(item))
                        return false;
                }

                return true;
            }

            Assert.True(CheckResult());
        }

        [Fact]
        public void ProcessFileTest()
        {
            var expected = @"./testFiles/test1.txt";
            var result = string.Empty;

            void Callback(string value)
            {
                result = value;
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { "./testFiles/test1.txt" }, "txt", Callback);
            Assert.Empty(errors);

            Assert.Equal(expected, result);
        }

        [Fact]
        public void ProcessFileNotExistTest()
        {
            var expected = @"./testFiles/test1.txt";
            var result = string.Empty;

            void Callback(string value)
            {
                result = value;
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { "./testFiles/test.txt" }, "txt", Callback);
            Assert.NotEmpty(errors);

            Assert.NotEqual(expected, result);
        }

        [Fact]
        public void ProcessFileNotMatchExtensionTest()
        {
            var expected = @"./testFiles/test1.txt";
            var result = string.Empty;

            void Callback(string value)
            {
                result = value;
            }

            var errors = ProcessFiles.ProcessFiles.Process(new[] { "./testFiles/test1.txt" }, "abc", Callback);
            Assert.NotEmpty(errors);

            Assert.NotEqual(expected, result);
        }
    }
}
