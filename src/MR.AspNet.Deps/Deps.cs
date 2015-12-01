using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public class Deps
	{
		public IList<string> Fonts { get; set; }
		public IList<Bundle> Css { get; set; }
		public IList<Bundle> Js { get; set; }
		public IList<Bundle> Sass { get; set; }
	}
}
