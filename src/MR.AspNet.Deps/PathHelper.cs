using System;
using System.IO;
using Microsoft.AspNet.Hosting;
using Microsoft.Extensions.PlatformAbstractions;

namespace MR.AspNet.Deps
{
	public class PathHelper
	{
		private IHostingEnvironment _env;
		private IApplicationEnvironment _appEnv;
		private string _webroot;
		private Uri _base;

		public PathHelper(IHostingEnvironment env, IApplicationEnvironment appEnv, string webroot)
		{
			_env = env;
			_appEnv = appEnv;
			_webroot = EnsureEndsInSlash(webroot);
			_base = new Uri(Path.Combine(_appEnv.ApplicationBasePath, _webroot));
		}

		public string MakeRelative(string path)
		{
			return "/" + _base.MakeRelativeUri(new Uri(path)).ToString();
		}

		public bool FileExists(string path)
		{
			var fileInfo = _env.WebRootFileProvider.GetFileInfo(path);
			return fileInfo.Exists && !fileInfo.IsDirectory;
		}

		private string EnsureEndsInSlash(string path)
		{
			if (!path.EndsWith("/"))
			{
				path += "/";
			}
			return path;
		}
	}
}
