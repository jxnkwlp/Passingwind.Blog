using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using System.Linq;

namespace Passingwind.Blog.Web
{
	public static class ServiceCollectionExtensions
	{
		public static IServiceCollection Replace<TService, TImplementation>(this IServiceCollection services, ServiceLifetime lifetime) where TService : class where TImplementation : class, TService
		{
#if DEBUG
			var registerServices = services.Where(d => d.ServiceType == typeof(TService));
#endif
			Debug.Assert(services.Any(d => d.ServiceType == typeof(TService)));
			Debug.Assert(services.Count(d => d.ServiceType == typeof(TService)) == 1);

			var descriptorToRemove = services.FirstOrDefault(d => d.ServiceType == typeof(TService));
			services.Remove(descriptorToRemove);

			var descriptorToAdd = new ServiceDescriptor(typeof(TService), typeof(TImplementation), lifetime);
			services.Add(descriptorToAdd);

			return services;
		}
	}
}
