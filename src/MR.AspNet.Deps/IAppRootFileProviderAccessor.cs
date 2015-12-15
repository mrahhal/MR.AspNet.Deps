using Microsoft.AspNet.FileProviders;
using Microsoft.Extensions.PlatformAbstractions;

namespace MR.AspNet.Deps
{
	public interface IAppRootFileProviderAccessor
	{
		IFileProvider AppRootFileProvider { get; }
	}

	public class PhysicalAppRootFileProviderAccessor : IAppRootFileProviderAccessor
	{
		private IApplicationEnvironment _appEnv;

		public PhysicalAppRootFileProviderAccessor(IApplicationEnvironment appEnv)
		{
			_appEnv = appEnv;
		}

		public IFileProvider AppRootFileProvider
		{
			get
			{
				return new PhysicalFileProvider(_appEnv.ApplicationBasePath);
			}
		}
	}
}
