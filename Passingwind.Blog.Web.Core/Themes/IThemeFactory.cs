using Microsoft.AspNetCore.Builder;

namespace Passingwind.Blog.Web.Themes
{
	public interface IThemeFactory : IThemeContainer
	{
		void Initialize(IApplicationBuilder applicationBuilder);
	}
}
