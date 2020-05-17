using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class NullIPAddressService : IIPAddressService
	{
		public Task<string> GetIPLocationAsync(string ip)
		{
			return Task.FromResult(string.Empty);
		}
	}
}
