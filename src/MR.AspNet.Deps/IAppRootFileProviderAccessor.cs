using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.FileProviders;

namespace MR.AspNet.Deps
{
	public interface IAppRootFileProviderAccessor
	{
		IFileProvider AppRootFileProvider { get; }
	}

	public class PhysicalAppRootFileProviderAccessor : IAppRootFileProviderAccessor
	{
		private IHostingEnvironment _env;

		public PhysicalAppRootFileProviderAccessor(IHostingEnvironment env)
		{
			_env = env;
		}

		public IFileProvider AppRootFileProvider
			=> _env.ContentRootFileProvider;
	}
}
