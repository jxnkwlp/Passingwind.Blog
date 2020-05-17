using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Passingwind.Blog.Widgets
{
	internal interface IWidgetsManager : IDisposable
	{
		/// <summary>
		///  Register the widget to system.
		/// </summary> 
		void Register(IServiceCollection services);

		/// <summary>
		///  Initial the widgets.
		/// </summary> 
		void Initialize(IApplicationBuilder app);
	}
}
