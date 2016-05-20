using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Hosting;

namespace MR.AspNet.Deps
{
	/// <summary>
	/// Represents the processing context in an <see cref="IProcessor"/>.
	/// </summary>
	public class ProcessingContext
	{
		/// <summary>
		/// Gets a strong-typed bundle object that contains the most common properties.
		/// </summary>
		public Bundle Bundle { get; set; }

		/// <summary>
		/// Gets a dynamic object over the bundle that contains all properties.
		/// </summary>
		public dynamic Original { get; set; }

		/// <summary>
		/// Gets the <see cref="IHostingEnvironment"/>.
		/// </summary>
		public IHostingEnvironment HostingEnvironment { get; set; }

		/// <summary>
		/// Gets an <see cref="IFileProvider"/> pointing to the webroot of the application.
		/// </summary>
		public IFileProvider FileProvider { get; set; }

		/// <summary>
		/// Gets a <see cref="FileVersionProvider"/> that can be used to append a file version to a path.
		/// </summary>
		public FileVersionProvider FileVersionProvider { get; set; }
	}
}
