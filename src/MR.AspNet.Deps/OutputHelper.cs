using System.Collections.Generic;

namespace MR.AspNet.Deps
{
	public class OutputHelper
	{
		private List<Element> _elements = new List<Element>();

		public void Add(Element element)
		{
			_elements.Add(element);
		}

		public void Add(List<Element> elements)
		{
			_elements.AddRange(elements);
		}

		public List<Element> Elements { get { return _elements; } }
	}

	public static class OutputHelperExtensions
	{
		/// <summary>
		/// Generates a &lt;link &gt; tag.
		/// </summary>
		public static void GenerateLink(this OutputHelper @this, string href)
		{
			var e = new Element("link", ClosingTagKind.None);
			e.Attributes.Add(new ElementAttribute("rel", "stylesheet"));
			e.Attributes.Add(new ElementAttribute("href", href));
			@this.Add(e);
		}

		/// <summary>
		/// Generates a &lt;script&gt;&lt;/script&gt; tag.
		/// </summary>
		public static void GenerateScript(this OutputHelper @this, string src, string type = null)
		{
			var e = new Element("script", ClosingTagKind.Normal);
			e.Attributes.Add(new ElementAttribute("src", src));
			if (type != null)
			{
				e.Attributes.Add(new ElementAttribute("type", type));
			}
			@this.Add(e);
		}
	}
}
