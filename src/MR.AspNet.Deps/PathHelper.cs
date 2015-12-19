using System;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;

namespace MR.AspNet.Deps
{
	public class PathHelper
	{
		private IApplicationEnvironment _appEnv;
		private string _webroot;
		private Uri _base;

		public PathHelper(IApplicationEnvironment appEnv, string webroot)
		{
			_appEnv = appEnv;
			_webroot = EnsureEndsInSlash(webroot);
			_base = new Uri(Path.Combine(_appEnv.ApplicationBasePath, _webroot));
		}

		public string MakeRelative(string path)
		{
			return "/" + _base.MakeRelativeUri(new Uri(path)).ToString();
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
