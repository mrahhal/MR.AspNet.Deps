using Microsoft.AspNet.Razor.TagHelpers;

namespace MR.AspNet.Deps
{
	public class BundleTagHelper : TagHelper
	{
		public BundleTagHelper(DepsManager depsManager)
		{
			DepsManager = depsManager;
		}

		public string Name { get; set; }

		public BundleKind Kind { get; set; }

		private DepsManager DepsManager { get; set; }

		public override void Process(TagHelperContext context, TagHelperOutput output)
		{
			output.TagName = null;
			switch (Kind)
			{
				case BundleKind.Script:
					output.PreContent.SetContent(DepsManager.RenderJs(Name));
					break;

				case BundleKind.Style:
					output.PreContent.SetContent(DepsManager.RenderCss(Name));
					break;
			}
		}
	}
}
