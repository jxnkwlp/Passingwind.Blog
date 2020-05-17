using System;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
	/// <summary>
	/// Async Helper
	/// </summary>
	public static class AsyncHelper
	{
		/// <summary>
		/// run 
		/// </summary> 
		public static void RunSync(Func<Task> func)
		{
			Nito.AsyncEx.AsyncContext.Run(func);
		}

		/// <summary>
		/// run 
		/// </summary> 
		public static TResult RunSync<TResult>(Func<Task<TResult>> func)
		{
			return Nito.AsyncEx.AsyncContext.Run<TResult>(func);
		}
	}
}
