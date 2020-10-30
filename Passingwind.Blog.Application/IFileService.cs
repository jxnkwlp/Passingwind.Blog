using Passingwind.Blog.DependencyInjection;
using Passingwind.Blog.Services.Models;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IFileService : ITransientDependency
	{
		Task<FileWriteOutputModel> WriteAsync(FileWriteInputModel input);
		Task DeleteAsync(string path);
	}
}
