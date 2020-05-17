using AutoMapper;
using Passingwind.Blog.Data.Domains;
using System.Linq;

namespace Passingwind.Blog.Web.Models
{
	public class MapperProfile : Profile
	{
		public MapperProfile()
		{
			CreateMap<Category, CategoryModel>();
			CreateMap<CategoryModel, Category>();
			CreateMap<Category, CategoryListItemModel>()
					.ForMember(t => t.PostCount, t => t.MapFrom((s, d) => d.PostCount = s.Posts.Count));

			CreateMap<Page, PageModel>();
			CreateMap<PageModel, Page>();

			CreateMap<Post, PostModel>()
				.ForMember(t => t.Categories, t => t.Ignore());
			CreateMap<PostEditModel, Post>()
				.ForMember(t => t.Categories, t => t.Ignore())
				.ForMember(t => t.Tags, t => t.Ignore())
				;
			CreateMap<Post, PostEditModel>();

			CreateMap<Role, RoleModel>()
				.ForMember(t => t.PermissionKeys, t => t.MapFrom((d) => d.Permissions.Select(t => t.Key)));
			CreateMap<RoleModel, Role>();

			CreateMap<Tags, TagsModel>()
				.ForMember(t => t.PostCount, t => t.MapFrom((d) => d.Posts.Count)); ;

			CreateMap<User, UserModel>();
			CreateMap<UserEditModel, User>();
			CreateMap<UserProfileUpdateModel, User>();

			CreateMap<Comment, CommentModel>();
		}
	}
}
