using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Plugins
{
	public interface IPluginLoader : IDisposable
	{
		IEnumerable<PluginPackage> Load();
	}
}