using Passingwind.Blog.Models;

namespace Passingwind.Blog.Web.Models
{
	public class ListOffsetQueryModel : IQueryOffsetInput
	{
		public int Skip { get; set; }
		public int Limit { get; set; } = 1;
	}

	public class ApiListQueryModel : ListOffsetQueryModel, IQueryBasicInput
	{
		public string SearchTerm { get; set; }
	}
}
