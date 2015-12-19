using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public class Bundle
	{
		public Bundle(string name, string @base, string dest, IList<string> src)
		{
			Name = name;
			Base = @base;
			Dest = dest;
			Src = src;
		}

		/// <summary>
		/// Gets the name of the bundle.
		/// </summary>
		public string Name { get; private set; }

		/// <summary>
		/// Gets the base relative to the webroot without starting with a slash.
		/// </summary>
		public string Base { get; private set; }

		/// <summary>
		/// Gets the dest relative to the webroot without starting with a slash.
		/// </summary>
		public string Dest { get; private set; }

		/// <summary>
		/// Gets the expanded src files relative to the webroot.
		/// </summary>
		public IList<string> Src { get; private set; }
	}
}
