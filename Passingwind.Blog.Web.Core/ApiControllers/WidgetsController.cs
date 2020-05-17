using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Utils;
using Passingwind.Blog.Web.Models;
using Passingwind.Blog.Web.Services;
using Passingwind.Blog.Widgets;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class WidgetsController : ApiControllerBase
	{
		private readonly IWidgetManager _widgetManager;
		private readonly IWidgetContainer _widgetContainer;

		public WidgetsController(IWidgetManager widgetManager, IWidgetContainer widgetContainer)
		{
			_widgetManager = widgetManager;
			_widgetContainer = widgetContainer;
		}

		[HttpGet]
		public IEnumerable<WidgetListModel> GetList()
		{
			var all = _widgetContainer.Widgets;

			//var installed = await _widgetManager.GetInstalledAsync();

			var result = new List<WidgetListModel>();

			foreach (var item in all)
			{
				//var installedItem = installed.Any(t => t == item.Id);

				result.Add(new WidgetListModel()
				{
					WidgetId = item.Id,
					WidgetName = item.Name,
					AdminConfigureUrl = item.Instance?.GetAdminConfigureUrl(),
				});
			}

			return result;
		}

		[HttpGet("zones")]
		public IEnumerable<object> GetZones()
		{
			var result = typeof(WidgetZones).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public).Where(t => t.IsLiteral).Select(t => new { t.Name, Value = t.GetRawConstantValue() });
			return result;
		}

		[HttpGet("zones/{zone}")]
		public async Task<IEnumerable<WidgetPositionConfigModel>> GetZoneConfigListAsync(string zone)
		{
			return (await _widgetManager.GetWidgetsByZoneAsync(zone)).Select(t => new WidgetPositionConfigModel()
			{
				Id = t.Id,
				Name = t.Title,
				WidgetId = t.WidgetId,
				Order = t.DisplayOrder,
			});
		}

		[HttpPost("zones")]
		public async Task SaveConfigListAsync([FromBody] WidgetZoneConfigItemModel model)
		{
			await _widgetManager.ClearFromZoneAsync(model.Zone);

			foreach (var item in model.Widgets)
			{
				await _widgetManager.AddToZoneAsync(model.Zone, item);
			}
		}

		//[HttpPost("[action]")]
		//public async Task InstallAsync(string id)
		//{
		//	await _widgetManager.InstallAsync(id);
		//}

		//[HttpPost("[action]")]
		//public async Task UnInstallAsync(string id)
		//{
		//	await _widgetManager.UninstallAsync(id);
		//}
	}
}
