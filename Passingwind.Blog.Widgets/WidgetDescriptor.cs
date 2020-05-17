using System;
using System.Reflection;

namespace Passingwind.Blog.Widgets
{
	/// <summary>
	/// </summary>
	public class WidgetDescriptor
	{
		/// <summary>
		///  The widget unique id
		/// </summary>
		public string Id { get; set; }
		/// <summary>
		///  The widget default name
		/// </summary>
		public string Name { get; set; }
		/// <summary>
		///  The widget description
		/// </summary>
		public string Description { get; set; }
		public string Author { get; set; }
		public string Version { get; set; }

		public Assembly Assembly { get; set; }
		public Type ComponentType { get; set; }
		//public Type Type { get; set; }
		public IWidget Instance { get; set; }
	}
}
