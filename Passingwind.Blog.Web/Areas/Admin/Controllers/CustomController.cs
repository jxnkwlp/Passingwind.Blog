using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
	public class CustomController : AdminControllerBase
	{
		private readonly IPluginManager _pluginManager;
		private readonly IWidgetsManager _widgetsManager;
		private readonly IWidgetConfigService _widgetConfigService;

		public CustomController(IPluginManager pluginManager, IWidgetsManager widgetsManager, IWidgetConfigService widgetConfigService)
		{
			_pluginManager = pluginManager;
			_widgetsManager = widgetsManager;
			_widgetConfigService = widgetConfigService;
		}

		public IActionResult Themes()
		{
			return View();
		}

		public async Task<IActionResult> Widgets(string position)
		{
			var widgetList = _pluginManager.GetAllPlugins().Where(t => string.Equals(t.Group, "widget", StringComparison.InvariantCultureIgnoreCase)).ToList();

			var positions = typeof(WidgetPositionsConsts).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

			var cp = position ?? (string)positions[0].Name;

			var pluginNames = await GetWidgetsPluginsByPositionAsync(cp);

			var model = new WidgetConfigViewModel()
			{
				Position = cp,
				AllPlugins = widgetList,
				Positions = positions.ToDictionary(t => t.Name, t => (string)t.GetValue(null)),
				PluginNames = pluginNames,
			};

			return View(model);
		}

		[HttpPost]
		public async Task<IActionResult> Widgets(WidgetConfigPostViewModel model)
		{
			if (model.Config != null)
			{
				var list = await _widgetConfigService.GetByPositionAsync(model.Position);

				var addedList = model.Config.Where(t => t.Id == Guid.Empty).ToList();

				var removedList = list.Select(t => t.Id).Except(model.Config.Where(t => t.Id != Guid.Empty).Select(t => t.Id)).ToArray();

				foreach (var item in removedList)
				{
					await _widgetConfigService.RemoveAsync(model.Position, item);
				}

				foreach (var item in addedList)
				{
					await _widgetConfigService.AddAsync(model.Position, new WidgetConfigInfo()
					{
						Order = item.Index,
						Id = Guid.NewGuid(),
						Name = item.Name,
					});
				}

				var existList = model.Config.Where(t => t.Id != Guid.Empty).Select(t => t.Id).Intersect(list.Select(t => t.Id)).ToArray();

				foreach (var item in existList)
				{
					var modelItem = model.Config.FirstOrDefault(t => t.Id == item);
					if (modelItem != null)
					{
						await _widgetConfigService.UpdateOrderAsync(model.Position, item, modelItem.Index);
					}
				}
			}
			else
			{
				await _widgetConfigService.ClearAsync(model.Position);
			}

			AlertSuccess("Saved.");

			return RedirectToAction(nameof(Widgets), new { position = model.Position });
		}

		public async Task<IActionResult> WidgetsPlugins(string position)
		{
			var list = await GetWidgetsPluginsByPositionAsync(position);

			return Json(list);
		}

		private async Task<List<WidgetConfigInfo>> GetWidgetsPluginsByPositionAsync(string position)
		{
			var list = await _widgetsManager.GetWidgetsAsync(position);

			if (list == null)
				return new List<WidgetConfigInfo>();

			return list.ToList();
		}
	}
}
