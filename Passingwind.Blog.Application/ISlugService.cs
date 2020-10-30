using Passingwind.Blog.DependencyInjection;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface ISlugService : IScopedDependency
	{
		Task<string> NormalarAsync(string input);
	}
}
