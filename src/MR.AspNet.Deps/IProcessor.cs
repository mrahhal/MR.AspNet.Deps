using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public interface IProcessor
	{
		void Process(ProcessingContext context, OutputHelper output);
	}
}
