using System;

namespace Passingwind.Blog.Services.Models
{
	public class FileWriteInputModel
	{
		public byte[] Data { get; set; }
		public string FileName { get; set; }
	}

	public class FileWriteOutputModel
	{
		public string OriginalFileName { get; set; }
		public string FileName { get; set; }
		public long Size { get; set; }
		public string UriPath { get; set; }
		public Uri Uri { get; set; }
	}
}
