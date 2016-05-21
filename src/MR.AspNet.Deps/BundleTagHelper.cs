using Microsoft.AspNetCore.Razor.TagHelpers;

namespace MR.AspNet.Deps
{
	public class BundleTagHelper : TagHelper
	{
		public BundleTagHelper(DepsManager depsManager)
		{
			DepsManager = depsManager;
		}

		public string Section { get; set; }

		public string Bundle { get; set; }

		private DepsManager DepsManager { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			var content = DepsManager.Render(Section, Bundle);
			output.Content.SetHtmlContent(content.Value);
		}
	}
}
