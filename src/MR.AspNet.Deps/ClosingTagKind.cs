namespace MR.AspNet.Deps
{
	public enum ClosingTagKind
	{
		/// <summary>
		/// &lt;tag &gt;
		/// </summary>
		None,

		/// <summary>
		/// &lt;tag /&gt;
		/// </summary>
		SelfClosing,

		/// <summary>
		/// &lt;tag&gt;&lt;/tag&gt;
		/// </summary>
		Normal,
	}
}
