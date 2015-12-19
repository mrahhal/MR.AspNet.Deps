using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public class Element
	{
		public Element(string tagName, ClosingTagKind closingTagKind)
		{
			TagName = tagName;
			ClosingTagKind = closingTagKind;
		}

		public string TagName { get; private set; }

		public ClosingTagKind ClosingTagKind { get; set; }

		public IList<ElementAttribute> Attributes { get; set; } = new List<ElementAttribute>();

		public string Content { get; set; }
	}
}
