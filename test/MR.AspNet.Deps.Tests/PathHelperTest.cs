using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;
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
			env.Setup(m => m.ContentRootPath).Returns("C:/Project/");
			var helper = new PathHelper(env.Object, "wwwroot");

			var result = helper.MakeRelative("C:/Project/wwwroot/js/foo.js");

			Assert.Equal("/js/foo.js", result);
		}

		[Fact]
		public void MakeFull()
		{
			var env = new Mock<IHostingEnvironment>();
			env.Setup(m => m.ContentRootPath).Returns("C:/Project/");
			var helper = new PathHelper(env.Object, "wwwroot");

			var result = helper.MakeFull("src/", "foo.js");

			Assert.Equal("wwwroot/src/foo.js", result);
		}

		[Fact]
		public void MakeFull_HandlesNegation()
		{
			var env = new Mock<IHostingEnvironment>();
			env.Setup(m => m.ContentRootPath).Returns("C:/Project/");
			var helper = new PathHelper(env.Object, "wwwroot");

			var result = helper.MakeFull("src/", "!foo.js");

			Assert.Equal("!wwwroot/src/foo.js", result);
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
			env.Setup(m => m.ContentRootPath).Returns("C:/Project/");
			return new PathHelper(env.Object, "wwwroot");
		}
	}
}
