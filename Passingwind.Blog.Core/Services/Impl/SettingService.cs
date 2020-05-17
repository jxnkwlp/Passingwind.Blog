using Microsoft.Extensions.Caching.Memory;
using Passingwind.Blog.Data;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Extensions;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services.Impl
{
	public class SettingService : Service<Setting>, ISettingService
	{
		private readonly IMemoryCache _memoryCache;

		public SettingService(IRepository<Setting, int> Repository, IMemoryCache memoryCache) : base(Repository)
		{
			_memoryCache = memoryCache;
		}

		public async Task<Setting> AddOrUpdateAsync(string key, string value)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			var source = await GetSettingsAsync(true);

			var entity = source.FirstOrDefault(t => t.Key.ToLower() == key.ToLower());

			if (entity != null)
			{
				entity.Value = value;

				await Repository.UpdateAsync(entity);
			}
			else
			{
				entity = new Setting(key, value);

				await Repository.InsertAsync(entity);
			}

			await ClearCacheAsync();

			return entity;
		}

		public async Task<Setting> AddOrUpdateAsync(string key, string value, string userId)
		{
			if (key == null)
				throw new ArgumentNullException(nameof(key));

			if (userId == null)
				throw new ArgumentNullException(nameof(userId));

			var source = await GetSettingsAsync(true);

			var entity = source.FirstOrDefault(t => t.UserId == userId && t.Key.ToLower() == key.ToLower());

			if (entity != null)
			{
				entity.Value = value;

				await Repository.UpdateAsync(entity);
			}
			else
			{
				entity = new Setting(userId, key, value);

				await Repository.InsertAsync(entity);
			}

			await ClearCacheAsync();

			return entity;
		}

		public async Task<IEnumerable<Setting>> GetSettingsAsync(bool allowCache)
		{
			if (allowCache)
			{
				return await _memoryCache.GetOrCreateAsync("settings", async (_) =>
				{
					_.SetAbsoluteExpiration(TimeSpan.FromHours(1));

					return await GetListAsync();
				});
			}
			else
			{
				return await GetListAsync();
			}
		}

		public Task ClearCacheAsync()
		{
			_memoryCache.Remove("settings");

			return Task.CompletedTask;
		}

		public Task<IPagedList<Setting>> GetPagedListAsync(ListBasicQueryInput input)
		{
			var query = Repository.GetQueryable();

			query = query.WhereIf(t => t.Key.Contains(input.SearchTerm), () => input.SearchTerm != null);

			return query.OrderBy(t => t.Key).ToPagedListAsync(input);
		}

		public async Task<string> GetValueAsync(string key)
		{
			return (await GetSettingsAsync(true)).FirstOrDefault(t => t.Key == key)?.Value;
		}

		public async Task<bool> HasKeyAsync(string key)
		{
			return (await GetSettingsAsync(true)).Any(t => t.Key == key);
		}
	}
}
