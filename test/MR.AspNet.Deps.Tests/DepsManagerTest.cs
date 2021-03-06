﻿using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Options;
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

			manager.Render("js", "app");
		}

		[Fact]
		public void Render_SectionDoesNotExist_Throws()
		{
			var provider = CreateDefaultServiceCollection(
				depsFileName: "deps1.json")
				.BuildServiceProvider();
			var manager = provider.GetService<DepsManager>();

			Assert.Throws<InvalidOperationException>(() =>
			{
				manager.Render("__something__", "app");
			});
		}

		[Fact]
		public void Render_BundleDoesNotExist_Throws()
		{
			var provider = CreateDefaultServiceCollection(
				depsFileName: "deps1.json")
				.BuildServiceProvider();
			var manager = provider.GetService<DepsManager>();

			Assert.Throws<InvalidOperationException>(() =>
		   {
			   manager.Render("js", "__something__");
		   });
		}

		[Fact]
		public void Render_SectionDoesNotHaveAnAssociatedProcessor_Throws()
		{
			var provider = CreateDefaultServiceCollection(
				depsFileName: "deps1.json")
				.BuildServiceProvider();
			var manager = provider.GetService<DepsManager>();

			Assert.Throws<InvalidOperationException>(() =>
			{
				manager.Render("some", "app");
			});
		}

		[Fact]
		public void Render_WithCustomProcessor_Succeeds()
		{
			var provider = CreateDefaultServiceCollection(
				depsFileName: "deps1.json",
				configureOptions: (opts) =>
				{
					var customProcessor = new Mock<IProcessor>();
					customProcessor.Setup(m => m.Process(It.IsAny<ProcessingContext>(), It.IsAny<OutputHelper>()));
					opts.Processors.Add(new SectionProcessor("some", customProcessor.Object));
				})
				.BuildServiceProvider();
			var manager = provider.GetService<DepsManager>();

			manager.Render("some", "app");
		}

		private ServiceCollection CreateDefaultServiceCollection(
			Action<DepsOptions> configureOptions = null,
			string depsFileName = null,
			bool isDevelopment = false)
		{
			var services = new ServiceCollection();

			services.AddOptions();

			services.AddSingleton<IMemoryCache>(new MemoryCache(new OptionsWrapper<MemoryCacheOptions>(new MemoryCacheOptions())));

			services.Configure<DepsOptions>((opts) =>
			{
				//opts.WebRoot = "testwebroot/";
				if (depsFileName != null)
				{
					opts.DepsFileName = depsFileName;
				}
			});
			services.Configure(
				configureOptions ??
				((DepsOptions _) => { }));

			var path = Path.GetFullPath(Path.GetDirectoryName(Path.Combine(Path.GetFullPath("testwebroot"), "../../../../../")));
			var webrootPath = Path.Combine(path, "testwebroot");
			var approotPath = Path.Combine(path, "testapproot");

			var env = new Mock<IHostingEnvironment>();
			env.Setup(m => m.ContentRootPath).Returns(path);
			env
				.Setup(m => m.EnvironmentName)
				.Returns(isDevelopment ? EnvironmentName.Development : EnvironmentName.Production);
			env.Setup(m => m.WebRootPath).Returns(webrootPath);
			env.Setup(m => m.WebRootFileProvider).Returns(new PhysicalFileProvider(webrootPath));
			services.AddSingleton(env.Object);

			var appRootFileProviderAccessor = new Mock<IAppRootFileProviderAccessor>();
			appRootFileProviderAccessor
				.Setup(m => m.AppRootFileProvider)
				.Returns(new PhysicalFileProvider(approotPath));
			services.AddSingleton(appRootFileProviderAccessor.Object);

			var httpContextAccessor = new Mock<IHttpContextAccessor>();
			httpContextAccessor.Setup(m => m.HttpContext).Returns((HttpContext)null);
			services.AddSingleton(httpContextAccessor.Object);

			var matcherMock = new Mock<Matcher>();
			matcherMock.Setup(m => m.AddInclude(It.IsAny<string>())).Returns((string _) => matcherMock.Object);
			matcherMock.Setup(m => m.AddExclude(It.IsAny<string>())).Returns((string _) => matcherMock.Object);
			matcherMock.Setup(m => m.Execute(It.IsAny<DirectoryInfoBase>())).Returns((DirectoryInfoBase _) => new PatternMatchingResult(new FilePatternMatch[0]));
			services.AddSingleton(matcherMock.Object);

			services.AddSingleton<DepsManager>();
			return services;
		}
	}
}
