namespace MR.AspNet.Deps
{
	public class ElementAttribute
	{
		public ElementAttribute(string name, string value)
		{
			Name = name;
			Value = value;
		}

		public string Name { get; set; }

		public string Value { get; set; }
	}
}
