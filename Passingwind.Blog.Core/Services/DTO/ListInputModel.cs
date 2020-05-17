using Passingwind.Blog.Models;

namespace Passingwind.Blog.Services.Models
{
	public class ListOffsetInput : IQueryOffsetInput
	{
		public int Skip { get; set; }
		public int Limit { get; set; } = 1;
	}

	public class ListBasicQueryInput : ListOffsetInput, IQueryBasicInput
	{
		public string SearchTerm { get; set; }
	}
}
