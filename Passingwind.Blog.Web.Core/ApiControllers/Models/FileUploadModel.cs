using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
	public class FileUploadResultModel
	{
		public string OriginalFileName { get; set; }
		public string FileName { get; set; }
		public long Size { get; set; }
		public string UriPath { get; set; }
		public Uri Uri { get; set; }
	}
}
