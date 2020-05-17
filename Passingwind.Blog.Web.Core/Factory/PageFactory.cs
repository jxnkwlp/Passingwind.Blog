using AutoMapper;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public class PageFactory : IPageFactory
	{
		private readonly IMapper _mapper;

		public PageFactory(IMapper mapper)
		{
			_mapper = mapper;
		}

		public Page ToEntity(PageModel model, Page page)
		{
			return _mapper.Map(model, page);
		}

		public PageModel ToModel(Page page, PageModel model)
		{
			return _mapper.Map(page, model);
		}
	}
}
