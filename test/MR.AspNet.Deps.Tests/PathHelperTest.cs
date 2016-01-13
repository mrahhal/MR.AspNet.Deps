using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Moq;
using Xunit;

namespace MR.AspNet.Deps.Tests
{
	public class PathHelperTest
	{
		[Fact]
		public void MakeRelative()
		{
			var env = new Mock<IHostingEnvironment>();
			var appEnv = new Mock<IApplicationEnvironment>();
			appEnv.Setup(m => m.ApplicationBasePath).Returns("C:/Project/");
			var helper = new PathHelper(env.Object, appEnv.Object, "wwwroot");

			var result = helper.MakeRelative("C:/Project/wwwroot/js/foo.js");

			Assert.Equal("/js/foo.js", result);
		}

		[Fact]
		public void FileExists_ExistsButDirectory_ReturnsFalse()
		{
			var helper = SetupForFileExists(true, true);

			var result = helper.FileExists("foo");

			Assert.False(result);
		}

		[Fact]
		public void FileExists_DoesNotExist_ReturnsFalse()
		{
			var helper = SetupForFileExists(false, false);

			var result = helper.FileExists("foo");

			Assert.False(result);
		}

		[Fact]
		public void FileExists_ExistsAndFile_ReturnsTrue()
		{
			var helper = SetupForFileExists(true, false);

			var result = helper.FileExists("foo");

			Assert.True(result);
		}

		private PathHelper SetupForFileExists(bool exists, bool isDirectory)
		{
			var env = new Mock<IHostingEnvironment>();
			var provider = new Mock<IFileProvider>();
			var fileInfo = new Mock<IFileInfo>();
			fileInfo.Setup(m => m.Exists).Returns(exists);
			fileInfo.Setup(m => m.IsDirectory).Returns(isDirectory);
			provider.Setup(m => m.GetFileInfo(It.IsAny<string>())).Returns(fileInfo.Object);
			env.Setup(m => m.WebRootFileProvider).Returns(provider.Object);
			var appEnv = new Mock<IApplicationEnvironment>();
			appEnv.Setup(m => m.ApplicationBasePath).Returns("C:/Project/");
			return new PathHelper(env.Object, appEnv.Object, "wwwroot");
		}
	}
}
