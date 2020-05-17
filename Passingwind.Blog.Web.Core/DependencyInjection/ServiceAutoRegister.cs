using Microsoft.Extensions.DependencyInjection;
using Passingwind.Blog.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;

namespace Passingwind.Blog.Web.DependencyInjection
{
	public static class ServiceAutoRegister
	{
		public static IServiceCollection ScanAndRegister(this IServiceCollection services, params Assembly[] assemblies)
		{
			return ScanAndRegister(services, assemblies.SelectMany(t => t.ExportedTypes).ToArray());
		}

		public static IServiceCollection ScanAndRegister(this IServiceCollection services, params Type[] types)
		{
			var allImplTypes = types.Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract);
			foreach (var type in allImplTypes)
			{
				var interfaces = type.GetInterfaces();
				if (interfaces.Contains(typeof(ISingletonDependency)))
				{
					if (type.GetInterfaces().Length == 1)
						services.AddSingleton(type);
					else
						foreach (var @interfaceType in type.GetInterfaces().Where(t => t != typeof(ISingletonDependency)))
						{
							services.AddSingleton(@interfaceType, type);
						}
				}
				else if (interfaces.Contains(typeof(IScopedDependency)))
				{
					if (type.GetInterfaces().Length == 1)
						services.AddScoped(type);
					else
						foreach (var @interfaceType in type.GetInterfaces().Where(t => t != typeof(IScopedDependency)))
						{
							services.AddScoped(@interfaceType, type);
						}
				}
				else if (interfaces.Contains(typeof(ITransientDependency)))
				{
					if (type.GetInterfaces().Length == 1)
						services.AddTransient(type);
					else
						foreach (var @interfaceType in type.GetInterfaces().Where(t => t != typeof(ITransientDependency)))
						{
							services.AddTransient(@interfaceType, type);
						}
				}
			}

			return services;
		}
	}
}
