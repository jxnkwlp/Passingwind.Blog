using System;

namespace Passingwind.Blog
{
	public class BlogOptions
	{
		public Uri HostUri { get; set; }

		public Uri AdminDevUri { get; set; }

		public BlogUploadOptions Upload { get; set; } = new BlogUploadOptions();
		public BlogAccountOptions Account { get; set; } = new BlogAccountOptions();

		public bool EnableMiniProfiler { get; set; }
	}

	public class BlogUploadOptions
	{
		public BlogUploadProvider Provider { get; set; }
		public string Value { get; set; }
		public string AllowExtensions { get; set; }
	}

	public enum BlogUploadProvider
	{
		Unknow = 0,
		Local,
		Azure,
		Aliyun,
		Tencent
	}

	public class BlogAccountOptions
	{
		public bool RequireConfirmedAccount { get; set; }
		public bool LockoutOnFailure { get; set; }
	}
}
