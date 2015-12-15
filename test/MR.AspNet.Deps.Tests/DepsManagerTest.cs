using System;
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
			var services = CreateDefaultServiceCollection();
			var appEnv = new Mock<IApplicationEnvironment>();
			appEnv.Setup(m => m.ApplicationBasePath).Returns("C:\\project");
			services.AddInstance(appEnv.Object);
			services.AddInstance(new Mock<IHostingEnvironment>().Object);
			services.AddInstance(new Mock<IAppRootFileProviderAccessor>().Object);
			var provider = services.BuildServiceProvider();

			var manager = provider.GetService<DepsManager>();

			Assert.NotNull(manager);
		}

		private ServiceCollection CreateDefaultServiceCollection(Action<DepsManager> configureOptions = null)
		{
			var services = new ServiceCollection();
			services.AddOptions();
			services.AddInstance(new Mock<IMemoryCache>().Object);
			services.Configure(configureOptions ?? ((_) => { }));
			services.AddSingleton<DepsManager>();
			return services;
		}
	}
}
