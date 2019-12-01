using System;
using System.Reflection;
using System.Runtime.Loader;

namespace Passingwind.Blog.Plugins
{
	public class PluginAssemblyLoadContext : AssemblyLoadContext
	{
		private readonly AssemblyDependencyResolver _assemblyDependencyResolver;

		public PluginAssemblyLoadContext(string path, string name = null) : base(name: name, isCollectible: true)
		{
			_assemblyDependencyResolver = new AssemblyDependencyResolver(path);
		}
		 
		protected override Assembly Load(AssemblyName assemblyName)
		{
			//string assemplyPath = _assemblyDependencyResolver.ResolveAssemblyToPath(assemblyName);
			//if (assemplyPath != null)
			//{
			//	return LoadFromAssemblyPath(assemplyPath);
			//}
			return null;
		}

		protected override IntPtr LoadUnmanagedDll(string unmanagedDllName)
		{
			string libraryPath = _assemblyDependencyResolver.ResolveUnmanagedDllToPath(unmanagedDllName);
			if (libraryPath != null)
			{
				return LoadUnmanagedDllFromPath(libraryPath);
			}

			return IntPtr.Zero;
		}

	}

}
