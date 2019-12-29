using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Plugins;
using Passingwind.Blog.Plugins.Widgets;
using Passingwind.Blog.Web.Areas.Admin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
			var widgetList = _pluginManager.GetAllPlugins().Where(t => string.Equals(t.Group, "widget", StringComparison.InvariantCultureIgnoreCase)).Select(t =>
			{
				var m = new PluginDescriptorModel()
				{
					Author = t.Author,
					Description = t.Description,
					Group = t.Group,
					Name = t.Name,
					Version = t.Version,
				};
				if (typeof(IPluginConfigure).GetTypeInfo().IsAssignableFrom(t.PluginType))
				{
					var pluginInstance = Activator.CreateInstance(t.PluginType) as IPluginConfigure;
					m.CanConfigration = true;

					pluginInstance.GetConfigureRouteData(out var controller, out var action);

					m.ConfigrationPath = Url.Action(action, controller);
					//m.ConfigrationPath = Url.Action(nameof(WidgetConfig), new { name = m.Name });
				}
				return m;
			}).ToList();

			var positions = typeof(WidgetPositionsConsts).GetFields(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public);

			var cp = position ?? positions[0].Name;

			var configPlugins = await GetWidgetsPluginsByPositionAsync(cp);

			var plugins = configPlugins.Select(t =>
			  {
				  var widget = widgetList.FirstOrDefault(w => w.Name == t.Name);
				  if (widget != null)
					  return new ConfigPluginDescriptorModel(widget)
					  {
						  Id = t.Id,
						  Order = t.Order,
					  };
				  else
					  return new ConfigPluginDescriptorModel()
					  {
						  Id = t.Id,
						  Order = t.Order,
					  };
			  }).ToList();

			var model = new WidgetConfigViewModel()
			{
				Position = cp,
				AllPlugins = widgetList,
				Positions = positions.ToDictionary(t => t.Name, t => (string)t.GetValue(null)),
				Plugins = plugins,
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

		//public async Task<IActionResult> WidgetConfig(string name)
		//{
		//	var plugin = _pluginManager.GetPluginInstance(name);

		//	if (typeof(IPluginConfigration<>).IsAssignableFrom(plugin.GetType()))
		//	{
		//		var pluginConfigration = plugin as IPluginConfigration<PluginConfigurationModel>;

		//		var model = await pluginConfigration.GetConfigDataAsync();

		//		return View(new WidgetSettingViewModel() { Model = model });
		//	}

		//	return View(new WidgetSettingViewModel());
		//}

		private async Task<List<WidgetConfigInfo>> GetWidgetsPluginsByPositionAsync(string position)
		{
			var list = await _widgetConfigService.GetByPositionAsync(position);

			if (list == null)
				return new List<WidgetConfigInfo>();

			return list.ToList();
		} 
	}
}
