namespace Passingwind.Blog.Models
{
	public interface IQueryOffsetInput
	{
		int Skip { get; set; }
		int Limit { get; set; }
	}
}
