/*
 * Microsoft.AspNetCore.Mvc.Core/Infrastructure/TypeActivatorCache.cs
 */
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;

namespace Passingwind.Blog.Widgets.Infrastructure
{
	internal class TypeActivatorCache : ITypeActivatorCache
	{
		private readonly Func<Type, ObjectFactory> _createFactory =
			(type) => ActivatorUtilities.CreateFactory(type, Type.EmptyTypes);
		private readonly ConcurrentDictionary<Type, ObjectFactory> _typeActivatorCache =
			   new ConcurrentDictionary<Type, ObjectFactory>();

		/// <inheritdoc/>
		public TInstance CreateInstance<TInstance>(
			IServiceProvider serviceProvider,
			Type implementationType)
		{
			if (serviceProvider == null)
			{
				throw new ArgumentNullException(nameof(serviceProvider));
			}

			if (implementationType == null)
			{
				throw new ArgumentNullException(nameof(implementationType));
			}

			var createFactory = _typeActivatorCache.GetOrAdd(implementationType, _createFactory);
			return (TInstance)createFactory(serviceProvider, arguments: null);
		}
	}
}
