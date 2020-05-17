using Passingwind.Blog.Services.Models;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface IFileService
	{
		Task<FileWriteOutputModel> WriteAsync(FileWriteInputModel input);
		Task DeleteAsync(string path);
	}
}
