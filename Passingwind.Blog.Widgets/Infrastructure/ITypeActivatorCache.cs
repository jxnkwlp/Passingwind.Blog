/*
 * Microsoft.AspNetCore.Mvc.Core/Infrastructure/ITypeActivatorCache.cs
 */
using System;

namespace Passingwind.Blog.Widgets.Infrastructure
{
	public interface ITypeActivatorCache
	{
		TInstance CreateInstance<TInstance>(
			IServiceProvider serviceProvider,
			Type implementationType);

	}
}
