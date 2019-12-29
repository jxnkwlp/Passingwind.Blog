using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
	public class WidgetConfigViewModel
	{
		public Dictionary<string, string> Positions { get; set; } = new Dictionary<string, string>();

		public string Position { get; set; }

		public List<PluginDescriptorModel> AllPlugins { get; set; } = new List<PluginDescriptorModel>();

		public IList<ConfigPluginDescriptorModel> Plugins { get; set; } = new List<ConfigPluginDescriptorModel>();

	}

	public class PluginDescriptorModel
	{
		public string Name { get; set; }
		public string Description { get; set; }
		public string Version { get; set; }
		public string Author { get; set; }
		public string Group { get; set; }

		public bool CanConfigration { get; set; }
		public string ConfigrationPath { get; set; }
	}

	public class ConfigPluginDescriptorModel : PluginDescriptorModel
	{
		public Guid Id { get; set; }

		public int Order { get; set; }


		public ConfigPluginDescriptorModel()
		{

		}

		public ConfigPluginDescriptorModel(PluginDescriptorModel model)
		{
			this.Name = model.Name;
			this.Description = model.Description;
			this.Version = model.Version;
			this.Author = model.Author;
			this.Group = model.Group;
			this.CanConfigration = model.CanConfigration;
			this.ConfigrationPath = model.ConfigrationPath;
		}
	}
}
