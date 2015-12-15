using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public interface IGlob
	{
		IEnumerable<string> Expand(string pattern);
	}

	public class GlobAdapter : IGlob
	{
		public IEnumerable<string> Expand(string pattern)
		{
			return Glob.ExpandNames(pattern);
		}
	}
}
