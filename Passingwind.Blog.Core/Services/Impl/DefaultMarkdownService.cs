using Markdig;

namespace Passingwind.Blog.Services.Impl
{
	public class DefaultMarkdownService : IMarkdownService
	{
		public string ConventToHtml(string source)
		{
			if (string.IsNullOrWhiteSpace(source))
				return null;

			var p = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

			return Markdown.ToHtml(source, p);
		}
	}
}
