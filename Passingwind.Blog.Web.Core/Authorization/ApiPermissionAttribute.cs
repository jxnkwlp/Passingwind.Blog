using System;

namespace Passingwind.Blog.Web.Authorization
{
	[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
	public sealed class ApiPermissionAttribute : Attribute
	{
		public ApiPermissionAttribute(params string[] keys)
		{
			Keys = keys;
		}

		public string[] Keys { get; }

		public ApiPermissionMultipleCondition Condition { get; set; }
	}

	public enum ApiPermissionMultipleCondition
	{
		And = 0,
		Or
	}
}
