using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IIPAddressService
	{
		Task<string> GetIPLocationAsync(string ip);
	}
}
