using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileSystemGlobbing;

namespace MR.AspNet.Deps
{
	public static class DepsServiceCollectionExtensions
	{
		public static void AddDeps(this IServiceCollection services)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			services.AddSingleton<IAppRootFileProviderAccessor, PhysicalAppRootFileProviderAccessor>();
			services.AddTransient<Matcher>();
			services.AddSingleton<DepsManager>();
		}

		public static void AddDeps(this IServiceCollection services, Action<DepsOptions> configure)
		{
			if (services == null)
			{
				throw new ArgumentNullException(nameof(services));
			}

			if (configure == null)
			{
				throw new ArgumentNullException(nameof(configure));
			}

			services.Configure(configure);
			services.AddDeps();
		}
	}
}
