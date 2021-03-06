﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using Microsoft.Extensions.Options;
using MR.Json;
using Newtonsoft.Json;

namespace MR.AspNet.Deps
{
	public class DepsManager
	{
		private IHostingEnvironment _env;

		private IMemoryCache _memoryCache;
		private IAppRootFileProviderAccessor _appRootFileProviderAccessor;
		private DepsOptions _options;
		private string _webroot;
		private PathHelper _pathHelper;
		private FileVersionProvider _fileVersionProvider;
		private ElementRenderer _renderer = new ElementRenderer();
		private IServiceProvider _provider;

		public DepsManager(
			IHostingEnvironment env,
			IMemoryCache memoryCache,
			IAppRootFileProviderAccessor appRootFileProviderAccessor,
			IServiceProvider provider,
			IOptions<DepsOptions> options)
		{
			_env = env;
			_memoryCache = memoryCache;
			_appRootFileProviderAccessor = appRootFileProviderAccessor;
			_provider = provider;
			_options = options.Value;

			Initialize();
		}

		public HtmlString Render(string sectionName, string bundleName)
		{
			if (string.IsNullOrWhiteSpace(sectionName))
			{
				throw new ArgumentException(nameof(sectionName));
			}

			if (string.IsNullOrWhiteSpace(bundleName))
			{
				throw new ArgumentException(nameof(bundleName));
			}

			sectionName = sectionName.Trim().ToLowerInvariant();
			bundleName = bundleName.Trim().ToLowerInvariant();

			var sectionProcessor = _options.Processors
				.FirstOrDefault(p => p.Section.Equals(sectionName, StringComparison.OrdinalIgnoreCase));

			if (sectionProcessor == null)
			{
				throw new InvalidOperationException(
					$"The section {sectionName} doesn't have an associated processor.");
			}

			var elements = Process(sectionName, bundleName, sectionProcessor.Processor);
			return RenderElements(elements);
		}

		private List<Element> Process(string sectionName, string bundleName, IProcessor processor)
		{
			var deps = LoadDeps();
			var section = deps[sectionName] as IList<dynamic>;
			if (section == null)
			{
				throw new InvalidOperationException($"The section {sectionName} doesn't exist.");
			}

			var bundle = section
				.FirstOrDefault(b => string.Equals(bundleName, b.name, StringComparison.OrdinalIgnoreCase));

			if (bundle == null)
			{
				throw new InvalidOperationException($"The bundle {bundleName} doesn't exist.");
			}

			NormalizeBundle(bundle);
			var strongBundle = ExtractBundle(bundle);

			var output = new OutputHelper();
			var context = new ProcessingContext()
			{
				Bundle = strongBundle,
				Original = bundle,
				HostingEnvironment = _env,
				FileProvider = _env.WebRootFileProvider,
				FileVersionProvider = _fileVersionProvider,
			};

			processor.Process(context, output);

			return output.Elements;
		}

		private void NormalizeBundle(dynamic bundle)
		{
			if (bundle["base"] == null)
			{
				bundle["base"] = string.Empty;
			}

			if (bundle.src == null)
			{
				// Can't do List<string> because List is not covariant.
				bundle.src = new List<object>();
			}

			if (bundle.dest == null)
			{
				bundle.dest = string.Empty;
			}
		}

		private Bundle ExtractBundle(dynamic bundle)
		{
			var @base = bundle["base"];
			var dest = bundle.dest?.ToString().Trim('/');
			var src = ExpandFiles(@base, bundle.src);

			return new Bundle(bundle.name, @base, dest, src);
		}

		private HtmlString RenderElements(List<Element> elements)
		{
			return _renderer.Render(elements);
		}

		private void EnsureInitialized(dynamic deps)
		{
			if (_pathHelper == null)
			{
				_webroot = deps.config.webroot;
				_pathHelper = new PathHelper(_env, _webroot);
			}
		}

		private dynamic LoadDeps()
		{
			dynamic deps = null;
			var depsFileName = _options.DepsFileName;
			var key = GetDepsFileNameKey();
			if (!_memoryCache.TryGetValue(key, out deps))
			{
				var provider = _appRootFileProviderAccessor.AppRootFileProvider;
				deps = JsonConvert.DeserializeObject<GracefulExpandoObject>(
					ReadAllText(provider.GetFileInfo(depsFileName)), new GracefulExpandoObjectConverter());
				var cacheEntryOptions =
					new MemoryCacheEntryOptions().AddExpirationToken(provider.Watch(depsFileName));
				_memoryCache.Set(key, (object)deps, cacheEntryOptions);
			}
			EnsureInitialized(deps);
			return deps;
		}

		private string GetDepsFileNameKey()
			=> $"deps._{_options.DepsFileName}";

		private string ReadAllText(IFileInfo fileInfo)
		{
			using (var stream = fileInfo.CreateReadStream())
			using (var reader = new StreamReader(stream))
			{
				return reader.ReadToEnd();
			}
		}

		private string[] ExpandFiles(string @base, IList<object> files)
		{
			var matcher = CreateMatcher();
			var fullFiles = files.Select(f => _pathHelper.MakeFull(@base, f.ToString()));
			var glob = new Glob(fullFiles);
			var result = glob.Execute(new DirectoryInfoWrapper(
				new DirectoryInfo(_env.ContentRootPath)));
			return result.Files
				.Select(f => Path.Combine(_env.ContentRootPath, f.Path))
				.Select(f => _pathHelper.MakeRelative(f))
				.Where(f => _pathHelper.FileExists(f)).ToArray();
		}

		private string CorrectUrl(string href)
		{
			if (href[0] != '/')
			{
				href = "/" + href;
			}
			return href;
		}

		private void Initialize()
		{
			// Add the default fallback processors.
			var defaultProcessors = new List<SectionProcessor>()
			{
				new SectionProcessor("js", new JsProcessor()),
				new SectionProcessor("css", new CssProcessor())
			};
			foreach (var processor in defaultProcessors)
			{
				_options.Processors.Add(processor);
			}

			// Create the FileVersionProvider.
			_fileVersionProvider = new FileVersionProvider(
					_env.WebRootFileProvider,
					_memoryCache,
					new PathString());
		}

		private Matcher CreateMatcher()
			=> _provider.GetService(typeof(Matcher)) as Matcher;
	}
}
