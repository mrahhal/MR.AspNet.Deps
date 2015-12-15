namespace MR.AspNet.Deps
{
	public class DepsOptions
	{
		/// <summary>
		/// Gets or sets the name of the deps file. Default is "deps.json".
		/// </summary>
		public string DepsFileName { get; set; } = "deps.json";

		/// <summary>
		/// Gets or sets the web root's path. Default is "wwwroot/".
		/// </summary>
		public string WebRoot { get; set; } = "wwwroot/";

		/// <summary>
		/// Gets or sets the scripts base path relative to <see cref="WebRoot"/>. Default is "js/".
		/// </summary>
		public string ScriptsBasePath { get; set; } = "js/";

		/// <summary>
		/// Gets or sets the styles base path relative to <see cref="WebRoot"/>. Default is "css/".
		/// </summary>
		public string StylesBasePath { get; set; } = "css/";
	}
}
