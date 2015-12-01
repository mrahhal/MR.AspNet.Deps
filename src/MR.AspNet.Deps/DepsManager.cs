using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Mvc.Rendering;
using Microsoft.Extensions.PlatformAbstractions;
using Newtonsoft.Json;

namespace MR.AspNet.Deps
{
	public class DepsManager
	{
		private ConcurrentDictionary<string, string[]> _cache = new ConcurrentDictionary<string, string[]>();
		private Deps deps;
		private Uri _relative;

		public DepsManager(IApplicationEnvironment appEnv, IHostingEnvironment env)
		{
			ApplicationEnvironment = appEnv;
			HostingEnvironment = env;
			_relative = new Uri(Path.Combine(ApplicationEnvironment.ApplicationBasePath, "wwwroot/"));

			deps = JsonConvert.DeserializeObject<Deps>(
				File.ReadAllText(Path.Combine(ApplicationEnvironment.ApplicationBasePath, "deps.json")));
		}

		private IApplicationEnvironment ApplicationEnvironment { get; set; }

		private IHostingEnvironment HostingEnvironment { get; set; }

		public HtmlString RenderJs(string bundle)
		{
			if (bundle == null)
				throw new ArgumentNullException(nameof(bundle));

			if (HostingEnvironment.IsDevelopment())
			{
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
				return new HtmlString(CreateScriptTag(bundle + ".js", true));
			}
		}

		public HtmlString RenderCss(string bundle)
		{
			if (bundle == null)
				throw new ArgumentNullException(nameof(bundle));

			if (HostingEnvironment.IsDevelopment())
			{
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
				return new HtmlString(CreateLinkTag(bundle + ".css", true));
			}
		}

		private string[] ExpandFiles(string bundle, string @base, IList<string> files, BundleKind kind)
		{
			@base = Path.Combine(ApplicationEnvironment.ApplicationBasePath, "wwwroot", @base);
			return _cache.GetOrAdd(GetKeyForBundle(bundle, kind), (_) =>
			{
				return files.SelectMany(
					(fi) =>
					{
						return Glob
							.ExpandNames(Path.Combine(@base, fi))
							.Select((f) => "/" + _relative.MakeRelativeUri(new Uri(f)).ToString());
					})
					.ToArray();
			});
		}

		private string CreateScriptTag(string src, bool appendVersion)
		{
			var appendText = GetAppendText(appendVersion);
			return $"<script src=\"{src}\" {appendText}></script>";
		}

		private string CreateLinkTag(string href, bool appendVersion)
		{
			var appendText = GetAppendText(appendVersion);
			return $"<link rel=\"stylesheet\" href=\"{href}\" {appendText} />";
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

		private string GetAppendText(bool appendVersion)
		{
			return appendVersion ? "asp-append-version=\"true\"" : string.Empty;
		}
	}
}
