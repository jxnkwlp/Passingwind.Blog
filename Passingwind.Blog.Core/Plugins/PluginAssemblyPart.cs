using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Razor.Hosting;
using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.Blog.Plugins
{
	public class PluginAssemblyPart : AssemblyPart, ICompilationReferencesProvider
	{
		private readonly Assembly _assembly;

		public PluginAssemblyPart(Assembly assembly) : base(assembly)
		{
			_assembly = assembly;
		}

		public override string Name => _assembly.GetName().Name;

		public IEnumerable<string> GetReferencePaths()
		{
			yield return _assembly.Location;
		}
	}

	public class PluginRazorAssemblyPart : ApplicationPart, ICompilationReferencesProvider, IRazorCompiledItemProvider
	{
		private readonly CompiledRazorAssemblyPart _compiledRazorAssemblyPart;

		public override string Name => _compiledRazorAssemblyPart.Assembly.GetName().Name;

		public PluginRazorAssemblyPart(CompiledRazorAssemblyPart compiledRazorAssemblyPart)
		{
			_compiledRazorAssemblyPart = compiledRazorAssemblyPart;
		}

		public IEnumerable<RazorCompiledItem> CompiledItems => (_compiledRazorAssemblyPart as IRazorCompiledItemProvider).CompiledItems;

		public IEnumerable<string> GetReferencePaths()
		{
			yield return _compiledRazorAssemblyPart.Assembly.Location;
		}
	}
}
