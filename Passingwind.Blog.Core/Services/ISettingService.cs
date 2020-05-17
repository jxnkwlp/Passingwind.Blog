using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Services.Models;
using Passingwind.PagedList;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog.Services
{
	public interface ISettingService : IService<Setting>
	{
		Task<Setting> AddOrUpdateAsync(string key, string value);
		Task<Setting> AddOrUpdateAsync(string key, string value, string userId);
		Task<IEnumerable<Setting>> GetSettingsAsync(bool allowCache);
		Task<string> GetValueAsync(string key);
		Task<bool> HasKeyAsync(string key);
		Task ClearCacheAsync();

		Task<IPagedList<Setting>> GetPagedListAsync(ListBasicQueryInput input);
	}
}
