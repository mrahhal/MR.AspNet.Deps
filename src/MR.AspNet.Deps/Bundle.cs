using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public class Bundle
	{
		public string Name { get; set; }
		public string Base { get; set; }
		public string Target { get; set; }
		public IList<string> Files { get; set; }
	}
}
