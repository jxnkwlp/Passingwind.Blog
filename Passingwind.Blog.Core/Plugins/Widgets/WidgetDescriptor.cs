using System;

namespace Passingwind.Blog.Plugins.Widgets
{
	public class WidgetDescriptor : PluginDescriptor
	{
		public Guid Id { get; set; }

		public WidgetDescriptor()
		{
		}

		public WidgetDescriptor(PluginDescriptor descriptor)
		{
			Assembly = descriptor.Assembly;
			AssemblyName = descriptor.AssemblyName;
			Author = descriptor.Author;
			ContentPath = descriptor.ContentPath;
			Description = descriptor.Description;
			Group = descriptor.Group;
			Name = descriptor.Name;
			PluginType = descriptor.PluginType;
			RelativePath = descriptor.RelativePath;
			Version = descriptor.Version;
		}
	}
}
