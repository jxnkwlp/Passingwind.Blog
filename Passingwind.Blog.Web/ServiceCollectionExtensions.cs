using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web
{
    public interface ITransientService
    { }

    public interface ISingletonService
    { }

    public interface IScopedService
    { }

    public static class ServiceCollectionExtensions
    {
        public static void AddAutoRegisterService(this IServiceCollection services)
        {
            var classTypes = Assembly.GetExecutingAssembly().ExportedTypes.Select(t => IntrospectionExtensions.GetTypeInfo(t)).Where(t => t.IsClass && !t.IsAbstract);

            foreach (var type in classTypes)
            {
                var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo());

                if (interfaces.Count() > 0)
                {
                    foreach (var handlerType in interfaces.Where(i => i.IsGenericType))
                    {
                        if (type.IsAssignableFrom(typeof(ITransientService)))
                        {
                            services.AddTransient(handlerType.AsType(), type.AsType());
                        }
                        else if (type.IsAssignableFrom(typeof(ISingletonService)))
                        {
                            services.AddSingleton(handlerType.AsType(), type.AsType());
                        }
                        else if (type.IsAssignableFrom(typeof(IScopedService)))
                        {
                            services.AddScoped(handlerType.AsType(), type.AsType());
                        }
                    }
                }
                else
                {
                    if (type.IsAssignableFrom(typeof(ITransientService)))
                    {
                        services.AddTransient(type.AsType());
                    }
                    else if (type.IsAssignableFrom(typeof(ISingletonService)))
                    {
                        services.AddSingleton(type.AsType());
                    }
                    else if (type.IsAssignableFrom(typeof(IScopedService)))
                    {
                        services.AddScoped(type.AsType());
                    }
                }

            }

        }

        //public static IServiceCollection AddHandlers(this IServiceCollection services, string assemblyName)
        //{
        //    var assemblyPath = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), assemblyName + ".dll");
        //    var assembly = Assembly.Load(AssemblyLoadContext.GetAssemblyName(assemblyPath));

        //    var classTypes = assembly.ExportedTypes.Select(t => IntrospectionExtensions.GetTypeInfo(t)).Where(t => t.IsClass && !t.IsAbstract);

        //    foreach (var type in classTypes)
        //    {
        //        var interfaces = type.ImplementedInterfaces.Select(i => i.GetTypeInfo());

        //        foreach (var handlerType in interfaces.Where(i => i.IsGenericType))
        //        {
        //            services.AddTransient(handlerType.AsType(), type.AsType());
        //        }
        //    }

        //    return services;
        //}
    }
}
