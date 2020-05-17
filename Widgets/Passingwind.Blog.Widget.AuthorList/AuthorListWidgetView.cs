using Passingwind.Blog.Services;
using Passingwind.Blog.Widgets;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.AuthorList
{
	public class AuthorListWidgetView : WidgetComponent
	{
		private readonly BlogUserManager _userManager;

		public AuthorListWidgetView(BlogUserManager blogUserManager)
		{
			_userManager = blogUserManager;
		}

		public async Task<IWidgetComponentViewResult> InvokeAsync()
		{
			var list = await _userManager.GetUserListAsync(new Services.Models.UserPagedListInputModel() { Limit = 10 });

			return View();
		}
	}
}
