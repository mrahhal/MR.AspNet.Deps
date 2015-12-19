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
			var appEnv = new Mock<IApplicationEnvironment>();
			appEnv.Setup(m => m.ApplicationBasePath).Returns("C:/Project/");
			var helper = new PathHelper(appEnv.Object, "wwwroot");

			var result = helper.MakeRelative("C:/Project/wwwroot/js/foo.js");

			Assert.Equal("/js/foo.js", result);
		}
	}
}
