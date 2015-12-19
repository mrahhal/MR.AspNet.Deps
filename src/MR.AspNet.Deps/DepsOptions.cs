using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public class DepsOptions
	{
		/// <summary>
		/// Gets or sets the name of the deps file. Default is "deps.json".
		/// </summary>
		public string DepsFileName { get; set; } = "deps.json";

		/// <summary>
		/// Gets or sets the section processors to use.
		/// </summary>
		public IList<SectionProcessor> Processors { get; set; } = new List<SectionProcessor>();
	}
}
