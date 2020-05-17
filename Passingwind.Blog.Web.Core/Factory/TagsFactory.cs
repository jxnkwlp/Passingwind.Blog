using AutoMapper;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public class TagsFactory : ITagsFactory
	{
		private readonly IMapper _mapper;

		public TagsFactory(IMapper mapper)
		{
			_mapper = mapper;
		}

		public TagsModel ToModel(Tags tags, TagsModel model)
		{
			return _mapper.Map(tags, model);
		}
	}
}
