using System;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
    /// <summary>
    /// Async Helper
    /// </summary>
    public class AsyncHelper
    {
        private static readonly TaskFactory _factory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        /// <summary>
        /// run 
        /// </summary> 
        public static void RunSync(Func<Task> func)
        {
            _factory.StartNew<Task>(() =>
            {
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }

        /// <summary>
        /// run 
        /// </summary> 
        public static TResult RunSync<TResult>(Func<Task<TResult>> func)
        {
            return _factory.StartNew<Task<TResult>>(() =>
            {
                return func();
            }).Unwrap<TResult>().GetAwaiter().GetResult();
        }
    }
}
