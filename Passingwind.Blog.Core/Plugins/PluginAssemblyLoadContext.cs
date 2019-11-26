using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Passingwind.Blog.Plugins
{
	public class PluginAssemblyLoadContext : AssemblyLoadContext
	{
		// private readonly AssemblyDependencyResolver _assemblyDependencyResolver;

		public PluginAssemblyLoadContext() : base(name: "PluginAssembly", isCollectible: true)
		{
			// _assemblyDependencyResolver = new AssemblyDependencyResolver(path);
		}

		protected override Assembly Load(AssemblyName assemblyName)
		{
			return base.Load(assemblyName);
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
		{
			return base.LoadUnmanagedDll(unmanagedDllName);
		}

	}
}
