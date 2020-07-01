using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.DependencyInjection;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IBlogMLExporter : IScopedDependency
	{
		Task<byte[]> ExportAsync(User user, BlogMLExportOptions options);
	}

	public class BlogMLExportOptions
	{
		public bool ExportAllUser { get; set; } = false;
		public bool ExportComments { get; set; } = true;
		public bool ExportPages { get; set; } = true;
	}
}
