using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Plugins
{
	public abstract class PluginBase : IPlugin
	{
		public PluginDescriptor Description { get; set; }
	}
}
