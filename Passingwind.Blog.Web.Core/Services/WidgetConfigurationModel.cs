using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Web.Services
{
	//public class WidgetConfigurationDataModel
	//{
	//	public IList<string> Install { get; set; } = new List<string>();

	//	public Dictionary<string, IList<WidgetZoneConfigModel>> Config { get; set; } = new Dictionary<string, IList<WidgetZoneConfigModel>>();
	//}

	public class WidgetPositionConfigModel
	{
		public Guid Id { get; set; }
		public string WidgetId { get; set; }
		public string Name { get; set; }
		public int Order { get; set; }
	}
}
