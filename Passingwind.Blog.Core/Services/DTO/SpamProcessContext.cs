using Passingwind.Blog.Data.Domains;

namespace Passingwind.Blog.Services.Models
{
	public class SpamProcessContext
	{
		public bool Passed { get; set; }

		public string Category { get; set; }

		public Comment Comment { get; set; }
	}
}
