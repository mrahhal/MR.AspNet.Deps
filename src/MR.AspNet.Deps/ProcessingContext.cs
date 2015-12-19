using Microsoft.AspNet.Hosting;

namespace MR.AspNet.Deps
{
	public class ProcessingContext
	{
		public Bundle Bundle { get; set; }

		public dynamic Original { get; set; }

		public IHostingEnvironment HostingEnvironment { get; set; }

		public FileVersionProvider FileVersionProvider { get; set; }
	}
}
