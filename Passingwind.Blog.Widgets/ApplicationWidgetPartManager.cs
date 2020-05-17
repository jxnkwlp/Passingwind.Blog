using Microsoft.AspNetCore.Mvc.ApplicationParts;
using System;
using System.Reflection;

namespace Passingwind.Blog.Widgets
{
	public class ApplicationWidgetPartManager : IApplicationWidgetPartManager
	{
		private readonly ApplicationPartManager _applicationPartManager;

		public ApplicationWidgetPartManager(ApplicationPartManager applicationPartManager)
		{
			_applicationPartManager = applicationPartManager;
		}

		public void Add(Assembly assembly)
		{
			AddToAppcationPart(_applicationPartManager, assembly);
		}

		public void Remove(Assembly assembly)
		{
			throw new NotImplementedException();
		}

		private static void AddToAppcationPart(ApplicationPartManager manager, Assembly assembly)
		{
			var partFactory = ApplicationPartFactory.GetApplicationPartFactory(assembly);

			var applicationParts = partFactory.GetApplicationParts(assembly);

			foreach (var part in applicationParts)
			{
				if (part is AssemblyPart assemblyPart)
				{
					manager.ApplicationParts.Add(new WidgetAssemblyPart(assembly));
				}
				else if (part is CompiledRazorAssemblyPart)
				{
					manager.ApplicationParts.Add(new WidgetRazorAssemblyPart(new CompiledRazorAssemblyPart(assembly)));
				}
				else
				{
					manager.ApplicationParts.Add(part);
				}
			}

			// load related assemblies
			var relatedAssemblies = RelatedAssemblyAttribute.GetRelatedAssemblies(assembly, true);

			foreach (var item in relatedAssemblies)
			{
				AddToAppcationPart(manager, item);
			}
		}
	}
}
