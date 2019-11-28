using System;
using System.Reflection;

namespace Passingwind.Blog.Plugins
{
	public class PluginPackage
	{
		public Type PluginType { get; set; }

		public Assembly Assembly { get; set; }

		public string ContentPath { get; set; }

		public string RelativePath { get; set; }
	}
}
