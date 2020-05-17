using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class ThemeAccessor : IThemeAccessor
	{
		public string GetCurrentThemeName()
		{
			return "Abc";
		}
	}
}
