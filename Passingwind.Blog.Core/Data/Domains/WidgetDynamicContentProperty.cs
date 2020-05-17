using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Data.Domains
{
	public class WidgetDynamicContentProperty : Entity<long>
	{
		[MaxLength(64)]
		public string Name { get; set; }
		[MaxLength(16)]
		public string ValueType { get; set; }

		public bool IsArray { get; set; }

		public string Value { get; set; }
	}
}
