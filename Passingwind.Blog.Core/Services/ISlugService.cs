using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface ISlugService
	{
		Task<string> NormalarAsync(string input);
	}
}
