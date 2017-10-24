using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace MR.AspNet.Deps
{
	public class PathHelper
	{
		private IHostingEnvironment _env;
		private string _webroot;
		private Uri _base;

		public PathHelper(IHostingEnvironment env, string webroot)
		{
			_env = env;
			_webroot = EnsureEndsInSlash(webroot);
			_base = new Uri(Path.Combine(_env.ContentRootPath, _webroot));
		}

		public string MakeRelative(string path)
		{
			return "/" + _base.MakeRelativeUri(new Uri(path)).ToString();
		}

		public string MakeFull(string @base, string path)
		{
			var full = string.Empty;
			if (path[0] == '!')
			{
				full = "!";
				path = path.Substring(1);
			}
			return full + Path.Combine(_webroot, @base, path);
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
