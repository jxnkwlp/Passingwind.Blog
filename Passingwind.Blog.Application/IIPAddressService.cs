using Passingwind.Blog.DependencyInjection;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IIPAddressService : ISingletonDependency
	{
		Task<string> GetIPLocationAsync(string ip);
	}
}
