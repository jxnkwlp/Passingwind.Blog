using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Themes
{
	public interface IThemeContainer
	{
		IReadOnlyCollection<ThemeDescriptor> Themes { get; }

		Task ReloadAsync();
	}
}
