using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins
{
	public class PluginPackage
	{
		public Type PluginType { get; set; }

		public Assembly Assembly { get; set; }

		public string ContentPath { get; set; }
	}
}
