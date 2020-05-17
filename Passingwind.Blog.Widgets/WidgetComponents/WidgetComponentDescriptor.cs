using System;
using System.Collections.Generic;
using System.Reflection;

namespace Passingwind.Blog.Widgets
{
	/// <summary> 
	/// </summary>
	public class WidgetComponentDescriptor
	{
		public Guid Id { get; set; }

		public TypeInfo TypeInfo { get; set; }

		public MethodInfo MethodInfo { get; set; }

		public IReadOnlyList<ParameterInfo> Parameters { get; set; }

		public string RootName { get; set; }

		public WidgetComponentDescriptor()
		{
			Id = Guid.NewGuid();
		}
	}
}
