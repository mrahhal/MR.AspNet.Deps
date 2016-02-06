namespace MR.AspNet.Deps
{
	public class CssProcessor : EnvironmentAwareProcessorBase
	{
		public override string Extension => ".css";

		public override void Generate(string file, OutputHelper output)
		{
			output.GenerateLink(file);
		}
	}
}
