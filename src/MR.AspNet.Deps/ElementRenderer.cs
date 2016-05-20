using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MR.AspNet.Deps
{
	public class ElementRenderer
	{
		public HtmlString Render(IList<Element> elements)
		{
			if (elements == null)
			{
				throw new ArgumentNullException(nameof(elements));
			}

			var sb = new StringBuilder();
			foreach (var element in elements)
			{
				sb.Append(RenderCore(element));
			}

			return new HtmlString(sb.ToString());
		}

		public HtmlString Render(Element element)
		{
			if (element == null)
			{
				throw new ArgumentNullException(nameof(element));
			}

			return new HtmlString(RenderCore(element));
		}

		private string RenderCore(Element element)
		{
			var sb = new StringBuilder();

			sb.Append($"<{element.TagName}");

			if (element.Attributes.Any())
			{
				sb.Append(" ");
			}

			for (int i = 0; i < element.Attributes.Count; i++)
			{
				var attribute = element.Attributes[i];
				sb.Append($"{attribute.Name}=\"{attribute.Value}\"");
				if (i != element.Attributes.Count - 1)
				{
					sb.Append(" ");
				}
			}

			if (!string.IsNullOrWhiteSpace(element.Content) && element.ClosingTagKind != ClosingTagKind.Normal)
			{
				throw new InvalidOperationException(
					"Elements with a non-normal closing tag kind cannot have content.");
			}

			switch (element.ClosingTagKind)
			{
				case ClosingTagKind.None:
					sb.Append(" >");
					break;
				case ClosingTagKind.SelfClosing:
					sb.Append(" />");
					break;
				case ClosingTagKind.Normal:
					sb.Append(">");
					if (!string.IsNullOrWhiteSpace(element.Content))
					{
						sb.Append(element.Content);
					}
					sb.Append($"</{element.TagName}>");
					break;
			}

			return sb.ToString();
		}
	}
}
