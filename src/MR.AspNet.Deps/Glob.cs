using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace MR.AspNet.Deps
{
	public class Glob
	{
		private string[] _excludePatterns;
		private string[] _includePatterns;

		public Glob(IEnumerable<string> patterns)
		{
			_includePatterns = patterns.Where(p => p[0] != '!').ToArray();
			_excludePatterns = patterns.Where(p => p[0] == '!').Select(p => p.Substring(1)).ToArray();
		}

		public virtual PatternMatchingResult Execute(DirectoryInfoBase directoryInfo)
		{
			var files = new List<FilePatternMatch>();
			foreach (var includePattern in _includePatterns)
			{
				var matcher = new Matcher();
				matcher.AddInclude(includePattern);
				matcher.AddExcludePatterns(_excludePatterns);
				var result = matcher.Execute(directoryInfo);
				if (result.Files.Any())
				{
					files.AddRange(result.Files);
				}
			}
			return new PatternMatchingResult(files);
		}
	}
}
