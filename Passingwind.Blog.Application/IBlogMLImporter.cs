using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IBlogMLImporter : IScopedDependency
	{
		Task<BlogMLImporterResult> ImportAsync(User user, string xml);
	}

	public class BlogMLImporterResult
	{
		public int PostCount { get; set; }
		public int PageCount { get; set; }
		public int CategoryCount { get; set; }
	}
}
