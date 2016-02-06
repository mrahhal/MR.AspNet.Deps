namespace MR.AspNet.Deps
{
	public class JsProcessor : EnvironmentAwareProcessorBase
	{
		public override string Extension => ".js";

		public override void Generate(string file, OutputHelper output)
		{
			output.GenerateScript(file);
		}
	}
}
