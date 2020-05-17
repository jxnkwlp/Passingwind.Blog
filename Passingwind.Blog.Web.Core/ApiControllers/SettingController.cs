using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.Data.Settings;
using Passingwind.Blog.Json;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Models;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class SettingController : ApiControllerBase
	{
		private readonly ISettingService _settingService;
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly IJsonSerializer _jsonSerializer;

		public SettingController(ISettingService settingService, IHttpContextAccessor httpContextAccessor, IJsonSerializer jsonSerializer)
		{
			_settingService = settingService;
			_httpContextAccessor = httpContextAccessor;
			_jsonSerializer = jsonSerializer;
		}


		[ApiPermission("settings.list")]
		[HttpGet]
		public async Task<ApiPagedListOutput<Setting>> GetListAsync([FromQuery] ApiListQueryModel model)
		{
			var list = await _settingService.GetPagedListAsync(new ListBasicQueryInput()
			{
				SearchTerm = model.SearchTerm,
				Limit = model.Limit,
				Skip = model.Skip,
			});

			return new ApiPagedListOutput<Setting>(list);
		}

		[ApiPermission("settings.update")]
		[HttpPost]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task CreateOrUpdateAsync([FromBody] Setting model)
		{
			await _settingService.AddOrUpdateAsync(model.Key, model.Value, model.UserId);
		}

		[ApiPermission("settings.delete")]
		[HttpDelete]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task DeleteAsync([FromBody] int[] ids)
		{
			if (ids != null && ids.Any())
				await _settingService.DeleteByAsync(t => ids.Contains(t.Id));
		}

		[ApiPermission("setting.load")]
		[HttpGet("load")]
		public async Task<ISettings> LoadSettingsAsync([Required] string name)
		{
			if (string.IsNullOrWhiteSpace(name))
			{
				throw new ArgumentNullException(nameof(name));
			}

			ISettings settings = null;

			switch (name.ToLower())
			{
				case "basic":
					settings = await _settingService.LoadAsync<BasicSettings>();
					break;
				case "advanced":
					settings = await _settingService.LoadAsync<AdvancedSettings>();
					break;
				case "comments":
					settings = await _settingService.LoadAsync<CommentsSettings>();
					break;
				case "email":
					settings = await _settingService.LoadAsync<EmailSettings>();
					break;
				case "feed":
					settings = await _settingService.LoadAsync<FeedSettings>();
					break;
			}

			return settings;
		}

		[ApiPermission("setting.update")]
		[HttpPost("save/{name}")]
		[Consumes(MediaTypeNames.Application.Json)]
		public async Task SaveSettingsAsync(string name)
		{
			byte[] bytes;
			using (var ms = new MemoryStream())
			{
				await _httpContextAccessor.HttpContext.Request.Body.CopyToAsync(ms);
				bytes = ms.ToArray();
			}

			var body = Encoding.UTF8.GetString(bytes);

			if (string.IsNullOrEmpty(body))
				return;

			ISettings settings = null;
			switch (name.ToLower())
			{
				case "basic":
					settings = _jsonSerializer.Deserialize<BasicSettings>(body);
					break;
				case "advanced":
					settings = _jsonSerializer.Deserialize<AdvancedSettings>(body);
					break;
				case "comments":
					settings = _jsonSerializer.Deserialize<CommentsSettings>(body);
					break;
				case "email":
					settings = _jsonSerializer.Deserialize<EmailSettings>(body);
					break;
				case "feed":
					settings = _jsonSerializer.Deserialize<FeedSettings>(body);
					break;
			}

			if (settings != null)
				await _settingService.SaveAsync(settings);
		}
	}
}
