using AutoMapper;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public class CommentFactory : ICommentFactory
	{
		private readonly IMapper _mapper;

		public CommentFactory(IMapper mapper)
		{
			_mapper = mapper;
		}

		public CommentModel ToModel(Comment entity, CommentModel model)
		{
			return _mapper.Map(entity, model);
		}
	}
}
