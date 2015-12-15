using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.AspNet.Mvc.ViewFeatures;
using Microsoft.AspNet.Razor.TagHelpers;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.OptionsModel;
using Microsoft.Extensions.PlatformAbstractions;
using MR.AspNet.Deps.Microsoft.AspNet.Mvc.TagHelpers.Internal;
using Newtonsoft.Json;

namespace MR.AspNet.Deps
{
	public class DepsManager
	{
		private ConcurrentDictionary<string, string[]> _cache = new ConcurrentDictionary<string, string[]>();
		private IApplicationEnvironment _appEnv;
		private IHostingEnvironment _env;
		private IMemoryCache _memoryCache;
		private DepsOptions _options;
		private Deps deps;
		private Uri _relative;
		private FileVersionProvider _fileVersionProvider;

		public DepsManager(
			IApplicationEnvironment appEnv,
			IHostingEnvironment env,
			IMemoryCache memoryCache,
			IOptions<DepsOptions> options)
		{
			_appEnv = appEnv;
			_env = env;
			_memoryCache = memoryCache;
			_options = options.Value;
			_relative = new Uri(Path.Combine(_appEnv.ApplicationBasePath, "wwwroot/"));

			if (_options.Cache || !_env.IsDevelopment())
			{
				LoadDeps();
			}
		}

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		private void EnsureFileVersionProvider()
		{
			if (_fileVersionProvider == null)
			{
				_fileVersionProvider = new FileVersionProvider(
					_env.WebRootFileProvider,
					_memoryCache,
					ViewContext?.HttpContext.Request.PathBase ?? new PathString());
			}
		}

		public HtmlString RenderJs(string bundle)
		{
			if (bundle == null)
				throw new ArgumentNullException(nameof(bundle));

			if (_env.IsDevelopment())
			{
				LoadDepsIfNecessary();
				foreach (var b in deps.Js)
				{
					if (string.Equals(b.Name, bundle))
					{
						var sb = new StringBuilder();
						var files = ExpandFiles(bundle, b.Base, b.Files, BundleKind.Script);
						foreach (var realFile in files)
						{
							sb.AppendLine(CreateScriptTag(realFile, false));
						}
						return new HtmlString(sb.ToString());
					}
				}
				return null;
			}
			else
			{
				return new HtmlString(CreateScriptTag("js/" + bundle + ".js", true));
			}
		}

		public HtmlString RenderCss(string bundle)
		{
			if (bundle == null)
				throw new ArgumentNullException(nameof(bundle));

			if (_env.IsDevelopment())
			{
				LoadDepsIfNecessary();
				foreach (var b in deps.Css)
				{
					if (string.Equals(b.Name, bundle))
					{
						var sb = new StringBuilder();
						var files = ExpandFiles(bundle, b.Base, b.Files, BundleKind.Style);
						foreach (var realFile in files)
						{
							sb.AppendLine(CreateLinkTag(realFile, false));
						}
						return new HtmlString(sb.ToString());
					}
				}
				return null;
			}
			else
			{
				return new HtmlString(CreateLinkTag("css/" + bundle + ".css", true));
			}
		}

		private void LoadDeps()
		{
			deps = JsonConvert.DeserializeObject<Deps>(
				File.ReadAllText(Path.Combine(_appEnv.ApplicationBasePath, "deps.json")));
		}

		private void LoadDepsIfNecessary()
		{
			if (!_options.Cache)
			{
				LoadDeps();
			}
		}

		private string[] ExpandFiles(string bundle, string @base, IList<string> files, BundleKind kind)
		{
			@base = Path.Combine(_appEnv.ApplicationBasePath, "wwwroot", @base);
			if (_options.Cache)
			{
				return _cache.GetOrAdd(GetKeyForBundle(bundle, kind), (_) => ExpandFilesCore(@base, files));
			}
			else
			{
				return ExpandFilesCore(@base, files);
			}
		}

		private string[] ExpandFilesCore(string @base, IList<string> files)
		{
			return files.SelectMany(
				(fi) =>
				{
					return Glob
					.ExpandNames(Path.Combine(@base, fi))
					.Select((f) => "/" + _relative.MakeRelativeUri(new Uri(f)).ToString());
				})
				.ToArray();
		}

		private string CreateScriptTag(string src, bool appendVersion)
		{
			src = appendVersion ? AppendVersion(src) : src;
			return $"<script src=\"{src}\"></script>";
		}

		private string CreateLinkTag(string href, bool appendVersion)
		{
			href = appendVersion ? AppendVersion(href) : href;
			return $"<link rel=\"stylesheet\" href=\"{href}\" />";
		}

		private string AppendVersion(string path)
		{
			EnsureFileVersionProvider();
			return _fileVersionProvider.AddFileVersionToPath(path);
		}

		private string GetKeyForBundle(string bundle, BundleKind kind)
		{
			switch (kind)
			{
				case BundleKind.Script:
					return "sc_" + bundle;

				case BundleKind.Style:
					return "st_" + bundle;

				default:
					return null;
			}
		}
	}
}
