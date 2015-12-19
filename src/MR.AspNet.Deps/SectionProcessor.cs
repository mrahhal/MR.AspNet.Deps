namespace MR.AspNet.Deps
{
	public class SectionProcessor
	{
		public SectionProcessor(string section, IProcessor processor)
		{
			Section = section;
			Processor = processor;
		}

		public string Section { get; set; }
		public IProcessor Processor { get; set; }
	}
}
