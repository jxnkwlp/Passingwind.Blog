using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Themes
{
	public interface IThemeAccessor
	{
		Task<string> GetCurrentThemeNameAsync();
	}
}
