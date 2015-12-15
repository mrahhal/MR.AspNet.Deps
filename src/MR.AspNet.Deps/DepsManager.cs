using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.FileProviders;
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
		private const string DepsFileName = "deps.json";
		private IApplicationEnvironment _appEnv;
		private IHostingEnvironment _env;
		private IMemoryCache _memoryCache;
		private IAppRootFileProviderAccessor _appRootFileProviderAccessor;
		private DepsOptions _options;
		private Uri _relative;
		private FileVersionProvider _fileVersionProvider;

		public DepsManager(
			IApplicationEnvironment appEnv,
			IHostingEnvironment env,
			IMemoryCache memoryCache,
			IAppRootFileProviderAccessor appRootFileProviderAccessor,
			IOptions<DepsOptions> options)
		{
			_appEnv = appEnv;
			_env = env;
			_memoryCache = memoryCache;
			_options = options.Value;
			_appRootFileProviderAccessor = appRootFileProviderAccessor;
			_relative = new Uri(Path.Combine(_appEnv.ApplicationBasePath, _options.WebRoot));
		}

		[HtmlAttributeNotBound]
		[ViewContext]
		public ViewContext ViewContext { get; set; }

		public HtmlString RenderJs(string bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException(nameof(bundle));
			}

			return RenderCore(bundle, BundleKind.Script);
		}

		public HtmlString RenderCss(string bundle)
		{
			if (bundle == null)
			{
				throw new ArgumentNullException(nameof(bundle));
			}

			return RenderCore(bundle, BundleKind.Style);
		}

		private HtmlString RenderCore(string name, BundleKind kind)
		{
			var deps = LoadDeps();

			var section = kind == BundleKind.Script ? deps.Js : deps.Css;
			var bundle = section
				.FirstOrDefault(b => string.Equals(b.Name, name, StringComparison.OrdinalIgnoreCase));

			if (bundle == null)
			{
				return null;
			}

			if (_env.IsDevelopment())
			{
				var sb = new StringBuilder();
				var files = ExpandFiles(name, bundle.Base, bundle.Files, kind);
				foreach (var realFile in files)
				{
					sb.AppendLine(CreateLinkTag(realFile, false));
				}
				return new HtmlString(sb.ToString());
			}
			else
			{
				var fullName = GetFullName(bundle, kind);
				return kind == BundleKind.Script ?
					new HtmlString(CreateLinkTag(fullName, true)) :
					new HtmlString(CreateScriptTag(fullName, true));
			}
		}

		private string GetFullName(Bundle bundle, BundleKind kind)
		{
			var ext = GetExtension(kind);
			var basePath = GetBasePath(bundle, kind);
			return Path.Combine(basePath, bundle.Name + ext);
		}

		private string GetExtension(BundleKind kind)
		{
			switch (kind)
			{
				case BundleKind.Script:
					return ".js";

				case BundleKind.Style:
					return ".css";
			}
			return null;
		}

		private string GetBasePath(Bundle bundle, BundleKind kind)
		{
			switch (kind)
			{
				case BundleKind.Script:
					return bundle.Target ?? _options.ScriptsBasePath;

				case BundleKind.Style:
					return bundle.Target ?? _options.StylesBasePath;
			}
			return null;
		}

		private Deps LoadDeps()
		{
			var deps = default(Deps);
			if (_memoryCache.TryGetValue(DepsFileName, out deps))
			{
				var provider = _appRootFileProviderAccessor.AppRootFileProvider;
				deps = JsonConvert.DeserializeObject<Deps>(ReadAllText(provider.GetFileInfo(DepsFileName)));
				var cacheEntryOptions =
					new MemoryCacheEntryOptions().AddExpirationToken(provider.Watch(DepsFileName));
				_memoryCache.Set(DepsFileName, deps, cacheEntryOptions);
			}
			return deps;
		}

		private string ReadAllText(IFileInfo fileInfo)
		{
			using (var stream = fileInfo.CreateReadStream())
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		private string[] ExpandFiles(string bundle, string @base, IList<string> files, BundleKind kind)
		{
			@base = Path.Combine(_appEnv.ApplicationBasePath, _options.WebRoot, @base);
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
