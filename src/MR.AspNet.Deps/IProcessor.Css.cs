using Microsoft.AspNet.Hosting;

namespace MR.AspNet.Deps
{
	public class CssProcessor : IProcessor
	{
		public void Process(ProcessingContext context, OutputHelper output)
		{
			var bundle = context.Bundle;

			if (context.HostingEnvironment.IsDevelopment())
			{
				foreach (var f in bundle.Src)
				{
					output.GenerateLink(f);
				}
			}
			else
			{
				output.GenerateLink(context.FileVersionProvider.AddFileVersionToPath($"/{bundle.Dest}/{bundle.Name}.css"));
			}
		}
	}
}
