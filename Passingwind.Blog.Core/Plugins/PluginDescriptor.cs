using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins
{
	public class PluginDescriptor : PluginPackage
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Version { get; set; }
		public string Author { get; set; }
	}
}
