using Passingwind.Blog.DependencyInjection;

namespace Passingwind.Blog.Services
{
	public interface IMarkdownService : ISingletonDependency
	{
		string ConventToHtml(string source);
	}
}
