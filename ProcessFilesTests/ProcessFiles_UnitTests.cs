using NSubstitute;
using ProcessFiles.Interfaces;
using Xunit;
using Xunit.Sdk;

namespace ProcessFilesTests
{
    public class ProcessFiles_UnitTests
    {
        [Fact]
        public void ProcessFileNotExistTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = value;
            }

            var fakeFileSystem = Substitute.For<IFileSystem>();
            fakeFileSystem.File.Exists(Arg.Any<string>()).Returns(false);

            var test = new ProcessFiles.ProcessFiles(fakeFileSystem);
            var errors = test.Process(["./imaginaryFolder/imaginaryTest.txt"], "txt", TestAction);
            Assert.NotEmpty(errors);
            Assert.Empty(result);

            fakeFileSystem.File.Received().Exists(Arg.Any<string>());
        }

        [Fact]
        public void ProcessFileNoExtensionTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = value;
            }

            var fakeFileSystem = Substitute.For<IFileSystem>();
            fakeFileSystem.File.Exists(Arg.Any<string>()).Returns(true);
            fakeFileSystem.Path.GetExtension(Arg.Any<string>()).Returns(string.Empty);

            var test = new ProcessFiles.ProcessFiles(fakeFileSystem);
            var errors = test.Process(["imaginaryNoExtension"], "abc", TestAction);
            Assert.NotEmpty(errors);
            Assert.Empty(result);

            fakeFileSystem.File.Received().Exists(Arg.Any<string>());
            fakeFileSystem.Path.Received().GetExtension(Arg.Any<string>());
        }

        [Fact]
        public void ProcessFileNotMatchExtensionTest()
        {
            var result = string.Empty;
            void TestAction(string value)
            {
                result = value;
            }

            var fakeFileSystem = Substitute.For<IFileSystem>();
            fakeFileSystem.File.Exists(Arg.Any<string>()).Returns(true);
            fakeFileSystem.Path.GetExtension(Arg.Any<string>()).Returns("def");

            var test = new ProcessFiles.ProcessFiles(fakeFileSystem);
            var errors = test.Process(["imaginaryFile"], "abc", TestAction);
            Assert.NotEmpty(errors);
            Assert.Empty(result);

            fakeFileSystem.File.Received().Exists(Arg.Any<string>());
            fakeFileSystem.Path.Received().GetExtension(Arg.Any<string>());
        }
    }
}
