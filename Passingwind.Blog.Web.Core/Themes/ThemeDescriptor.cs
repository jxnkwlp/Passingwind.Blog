namespace Passingwind.Blog.Web.Themes
{
	public class ThemeDescriptor
	{
		public ThemeDescription Description { get; set; }

		public string Name { get; set; }

		public string Path { get; set; }

		public string ViewPath => System.IO.Path.Combine(Path, "Views");

		public string ContentRootPath => System.IO.Path.Combine(Path, "wwwroot");

	}
}
