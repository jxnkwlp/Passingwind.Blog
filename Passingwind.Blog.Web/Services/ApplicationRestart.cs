using Microsoft.Extensions.Hosting;

namespace Passingwind.Blog.Web.Services
{
	public class ApplicationRestart : IApplicationRestart
	{
		private readonly IHostApplicationLifetime _hostApplicationLifetime;

		public ApplicationRestart(IHostApplicationLifetime hostApplicationLifetime)
		{
			_hostApplicationLifetime = hostApplicationLifetime;
		}

		public void Restart()
		{
			_hostApplicationLifetime.StopApplication();
		}
	}
}
