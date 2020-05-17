using AutoMapper;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Web.Models;

namespace Passingwind.Blog.Web.Factory
{
	public class RoleFactory : IRoleFactory
	{
		private readonly IMapper _mapper;

		public RoleFactory(IMapper mapper)
		{
			_mapper = mapper;
		}

		public Role ToEntity(RoleModel model, Role entity)
		{
			return _mapper.Map(model, entity);
		}

		public Role ToEntity(RoleModel model)
		{
			return ToEntity(model, new Role());
		}

		public RoleModel ToModel(Role entity, RoleModel model)
		{
			return _mapper.Map(entity, model);
		}
	}
}
