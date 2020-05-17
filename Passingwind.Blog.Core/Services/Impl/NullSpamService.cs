using Passingwind.Blog.Services.Models;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class NullSpamService : ISpamService
	{
		public Task ProcessAsync(SpamProcessContext context)
		{
			context.Passed = true;

			return Task.CompletedTask;
		}
	}
}
