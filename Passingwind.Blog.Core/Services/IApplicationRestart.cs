using Passingwind.Blog.DependencyInjection;

namespace Passingwind.Blog.Services
{
	public interface IApplicationRestart : ISingletonDependency
	{
		void Restart();
	}
}
