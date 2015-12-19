using Xunit;

namespace MR.AspNet.Deps.Tests
{
	public class ElementRendererTest
	{
		[Fact]
		public void Render_KindNone()
		{
			var renderer = new ElementRenderer();
			var e = new Element("tag", ClosingTagKind.None);

			var result = renderer.Render(e).ToString();

			Assert.Equal("<tag >", result);
		}

		[Fact]
		public void Render_KindSelfClosing()
		{
			var renderer = new ElementRenderer();
			var e = new Element("tag", ClosingTagKind.SelfClosing);

			var result = renderer.Render(e).ToString();

			Assert.Equal("<tag />", result);
		}

		[Fact]
		public void Render_KindNormal()
		{
			var renderer = new ElementRenderer();
			var e = new Element("tag", ClosingTagKind.Normal);

			var result = renderer.Render(e).ToString();

			Assert.Equal("<tag></tag>", result);
		}

		[Fact]
		public void Render_WithAttribute()
		{
			var renderer = new ElementRenderer();
			var e = new Element("tag", ClosingTagKind.Normal);
			e.Attributes.Add(new ElementAttribute("foo", "bar"));

			var result = renderer.Render(e).ToString();

			Assert.Equal("<tag foo=\"bar\"></tag>", result);
		}

		[Fact]
		public void Render_WithAttributes()
		{
			var renderer = new ElementRenderer();
			var e = new Element("tag", ClosingTagKind.Normal);
			e.Attributes.Add(new ElementAttribute("foo", "bar"));
			e.Attributes.Add(new ElementAttribute("baz", "some"));

			var result = renderer.Render(e).ToString();

			Assert.Equal("<tag foo=\"bar\" baz=\"some\"></tag>", result);
		}
	}
}
