using Passingwind.Blog.Web.Themes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers.Models
{
	public class ThemeListModel
	{
		public string Name { get; set; }

		public IEnumerable<ThemeDescription> Themes { get; set; }
	}
}
