using System;
using System.IO;
using Microsoft.AspNet.FileProviders;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using Moq;
using Xunit;

namespace MR.AspNet.Deps.Tests
{
	public class DepsManagerTest
	{
		[Fact]
		public void CanCreate()
		{
			var provider = CreateDefaultServiceCollection().BuildServiceProvider();
			var manager = provider.GetService<DepsManager>();

			Assert.NotNull(manager);
		}

		[Fact]
		public void CanReadDeps()
		{
			var provider = CreateDefaultServiceCollection(
				isDevelopment: true,
				depsFileName: "deps1.json")
				.BuildServiceProvider();
			var manager = provider.GetService<DepsManager>();

			manager.RenderJs("app");
		}

		[Fact]
		public void Render_UrlStartsWithSlash()
		{
			var provider = CreateDefaultServiceCollection(
				isDevelopment: false,
				depsFileName: "deps1.json")
				.BuildServiceProvider();
			var manager = provider.GetService<DepsManager>();

			var result = manager.RenderJs("app").ToString();

			Assert.Contains("/js/app.js", result);
		}

		private ServiceCollection CreateDefaultServiceCollection(
			Action<DepsOptions> configureOptions = null,
			string depsFileName = null,
			bool isDevelopment = false)
		{
			var services = new ServiceCollection();

			services.AddOptions();

			services.AddInstance(new Mock<IMemoryCache>().Object);

			services.Configure<DepsOptions>((opts) =>
			{
				opts.WebRoot = "testwebroot/";
				if (depsFileName != null)
				{
					opts.DepsFileName = depsFileName;
				}
			});
			services.Configure(
				configureOptions ??
				((DepsOptions _) => { }));

			var path = Path.GetDirectoryName(Path.GetFullPath("testwebroot"));
			var webrootPath = Path.Combine(path, "testwebroot");
			var approotPath = Path.Combine(path, "testapproot");

			var appEnv = new Mock<IApplicationEnvironment>();
			appEnv.Setup(m => m.ApplicationBasePath).Returns(path);
			services.AddInstance(appEnv.Object);

			var env = new Mock<IHostingEnvironment>();
			env
				.Setup(m => m.EnvironmentName)
				.Returns(isDevelopment ? EnvironmentName.Development : EnvironmentName.Production);
			env.Setup(m => m.WebRootPath).Returns(webrootPath);
			env.Setup(m => m.WebRootFileProvider).Returns(new PhysicalFileProvider(webrootPath));
			services.AddInstance(env.Object);

			var appRootFileProviderAccessor = new Mock<IAppRootFileProviderAccessor>();
			appRootFileProviderAccessor
				.Setup(m => m.AppRootFileProvider)
				.Returns(new PhysicalFileProvider(approotPath));
			services.AddInstance(appRootFileProviderAccessor.Object);

			var glob = new Mock<IGlob>();
			glob.Setup(m => m.Expand(It.IsAny<string>())).Returns((string _) => new[] { _ });
			services.AddInstance(glob.Object);

			services.AddSingleton<DepsManager>();
			return services;
		}
	}
}
