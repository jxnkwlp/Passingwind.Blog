using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.Services.Models;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface ISpamService : ITransientDependency
	{
		Task ProcessAsync(SpamProcessContext context);
	} 
}
