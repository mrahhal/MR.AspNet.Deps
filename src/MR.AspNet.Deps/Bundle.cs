using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public class Bundle
	{
		public string Name { get; set; }
		public string Base { get; set; }
		public string Dest { get; set; }
		public IList<string> Src { get; set; }
	}
}
