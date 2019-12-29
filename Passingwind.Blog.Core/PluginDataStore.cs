using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Passingwind.Blog
{
	public class PluginDataStore : IPluginDataStore
	{
		const string DataFolderName = "store";

		private readonly string _dataFolder;
		private readonly IWebHostEnvironment _webHostEnvironment;

		public PluginDataStore(IWebHostEnvironment webHostEnvironment)
		{
			_webHostEnvironment = webHostEnvironment;

			_dataFolder = Path.Combine(webHostEnvironment.ContentRootPath, "App_Data", DataFolderName);

			if (!Directory.Exists(_dataFolder))
				Directory.CreateDirectory(_dataFolder);
		}

		public Task DeleteAsync<T>(string name, Guid pluginId, T data) where T : IPluginData
		{
			var list = GetDataFromFile<T>(pluginId);

			var item = list.FirstOrDefault(t => t.Id == data.Id);
			if (item != null)
			{
				list.Remove(item);

				SaveDataToFile<T>(pluginId, list);
			}

			return Task.CompletedTask;
		}

		public Task DeleteAsync<T>(string name, Guid pluginId, params Guid[] keys) where T : IPluginData
		{
			var list = GetDataFromFile<T>(pluginId);

			var removed = list.Where(t => keys.Contains(t.Id));

			var savedList = list.Except(removed);

			SaveDataToFile<T>(pluginId, savedList);

			return Task.CompletedTask;
		}

		public Task<T> GetByIdAsync<T>(string name, Guid pluginId, Guid key) where T : IPluginData
		{
			var list = GetDataFromFile<T>(pluginId);

			return Task.FromResult(list.FirstOrDefault(t => t.Id == key));
		}

		public Task<IList<T>> GetListAsync<T>(string name, Guid pluginId) where T : IPluginData
		{
			var list = GetDataFromFile<T>(pluginId);

			return Task.FromResult(list);
		}

		public Task InsertAsync<T>(string name, Guid pluginId, T data) where T : IPluginData
		{
			var list = GetDataFromFile<T>(pluginId);
			list.Add(data);

			SaveDataToFile<T>(pluginId, list);

			return Task.CompletedTask;
		}

		public Task UpdateAsync<T>(string name, Guid pluginId, T data) where T : IPluginData
		{
			var list = GetDataFromFile<T>(pluginId);

			var removed = list.FirstOrDefault(t => t.Id == data.Id);

			if (removed != null)
				list.Remove(removed);

			list.Add(data);

			SaveDataToFile<T>(pluginId, list);

			return Task.CompletedTask;
		}

		protected string GetDataJsonFile<T>(Guid pluginId) where T : IPluginData
		{
			var file = Path.Combine(_dataFolder, typeof(T).FullName, pluginId.ToString("N") + ".json");

			if (!Directory.Exists(Path.GetDirectoryName(file)))
				Directory.CreateDirectory(Path.GetDirectoryName(file));

			return file;
		}

		protected IList<T> GetDataFromFile<T>(Guid pluginId) where T : IPluginData
		{
			var file = GetDataJsonFile<T>(pluginId);
			if (!File.Exists(file))
				return new List<T>();

			try
			{
				return JsonSerializer.Deserialize<IList<T>>(File.ReadAllText(file));
			}
			catch (Exception)
			{
			}

			return new List<T>();
		}

		protected void SaveDataToFile<T>(Guid pluginId, T instance) where T : IPluginData
		{
			var list = GetDataFromFile<T>(pluginId);

			list.Add(instance);

			SaveDataToFile(pluginId, list);
		}

		protected void SaveDataToFile<T>(Guid pluginId, IEnumerable<T> list) where T : IPluginData
		{
			try
			{
				var jsonString = JsonSerializer.Serialize(list);

				var file = GetDataJsonFile<T>(pluginId);
				File.WriteAllTextAsync(file, jsonString);
			}
			catch
			{
			}
		}
	}
}
