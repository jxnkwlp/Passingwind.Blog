using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
	public interface IPluginDataStore
	{
		Task<IList<T>> GetListAsync<T>(string name, Guid pluginId) where T : IPluginData;
		Task InsertAsync<T>(string name, Guid pluginId, T data) where T : IPluginData;
		Task UpdateAsync<T>(string name, Guid pluginId, T data) where T : IPluginData;
		Task DeleteAsync<T>(string name, Guid pluginId, T data) where T : IPluginData;
		Task DeleteAsync<T>(string name, Guid pluginId, params Guid[] keys) where T : IPluginData;
		Task<T> GetByIdAsync<T>(string name, Guid pluginId, Guid key) where T : IPluginData;
	}

	public interface IPluginData
	{
		Guid Id { get; set; }
	}
}
