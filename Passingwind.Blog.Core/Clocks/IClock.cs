using Passingwind.Blog.DependencyInjection;
using System;

namespace Passingwind.Blog.Clocks
{
	public interface IClock: ISingletonDependency
	{
		DateTime Now { get; }

		DateTimeKind Kind { get; }
	}
}
