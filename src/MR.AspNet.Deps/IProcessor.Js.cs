using Microsoft.AspNet.Hosting;

namespace MR.AspNet.Deps
{
	public class JsProcessor : IProcessor
	{
		public void Process(ProcessingContext context, OutputHelper output)
		{
			var bundle = context.Bundle;

			if (context.HostingEnvironment.IsDevelopment())
			{
				foreach (var f in bundle.Src)
				{
					output.GenerateScript(f);
				}
			}
			else
			{
				output.GenerateScript(context.FileVersionProvider.AddFileVersionToPath($"/{bundle.Dest}/{bundle.Name}.js"));
			}
		}
	}
}
