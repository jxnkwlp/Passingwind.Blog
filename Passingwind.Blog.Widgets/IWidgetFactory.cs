using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Passingwind.Blog.Widgets
{
	public interface IWidgetFactory : IWidgetContainer
	{
		IServiceCollection Services { get; }
		IServiceProvider ServiceProvider { get; }

		void Initialize(IApplicationBuilder applicationBuilder);
	}
}
