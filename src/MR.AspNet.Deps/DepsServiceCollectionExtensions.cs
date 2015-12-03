using Microsoft.Extensions.DependencyInjection;

namespace MR.AspNet.Deps
{
	public static class DepsServiceCollectionExtensions
	{
		public static void AddDeps(this IServiceCollection services)
		{
			services.AddSingleton<DepsManager>();
		}
	}
}
