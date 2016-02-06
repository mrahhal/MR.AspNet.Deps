using Microsoft.AspNet.Hosting;

namespace MR.AspNet.Deps
{
	public abstract class EnvironmentAwareProcessorBase : IProcessor
	{
		public void Process(ProcessingContext context, OutputHelper output)
		{
			var bundle = context.Bundle;

			if (context.HostingEnvironment.IsDevelopment())
			{
				foreach (var f in bundle.Src)
				{
					Generate(f, output);
				}
			}
			else
			{
				Generate(context.FileVersionProvider.AddFileVersionToPath(
					$"/{bundle.Dest}/{bundle.Name}{Extension}"), output);
			}
		}

		/// <summary>
		/// Gets the extension with the leading dot.
		/// </summary>
		public abstract string Extension { get; }

		/// <summary>
		/// Generates the resource link for the specified file.
		/// </summary>
		public abstract void Generate(string file, OutputHelper output);
	}
}
