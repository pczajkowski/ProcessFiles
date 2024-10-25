using NSubstitute;
using ProcessFiles.Interfaces;
using Xunit;

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
    }
}
