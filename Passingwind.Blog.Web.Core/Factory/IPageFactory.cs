using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public interface IPageFactory
	{
		PageModel ToModel(Page page, PageModel model);
		Page ToEntity(PageModel model, Page page);
	}
}
