using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Widget.CustomHtml.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.CustomHtml.Controllers
{
	[Area("admin")]
	[Authorize]
	public class CustomHtmlWidgetController : Controller
	{
		private readonly IPluginDataStore _pluginDataStore;

		public CustomHtmlWidgetController(IPluginDataStore pluginDataStore)
		{
			_pluginDataStore = pluginDataStore;
		}

		public async Task<IActionResult> Configure(Guid id)
		{
			var data = (await _pluginDataStore.GetListAsync<CustomHtmlModel>(Consts.Name, id)).FirstOrDefault();

			var model = new ConfigureViewModel()
			{
				Id = id,
				Html = data?.Title,
				Title = data?.Title,
			};

			return View("/Passingwind.Blog.Widget.CustomHtml/Views/CustomHtmlWidget/Configure.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> Update(CustomHtmlModel model)
		{
			if (model == null)
				throw new ArgumentNullException(nameof(model));

			var data = (await _pluginDataStore.GetListAsync<CustomHtmlModel>(Consts.Name, model.Id)).FirstOrDefault();

			if (data == null)
				await _pluginDataStore.InsertAsync(Consts.Name, model.Id, model);
			else
				await _pluginDataStore.UpdateAsync(Consts.Name, model.Id, model);

			return RedirectToAction(nameof(Configure), new { model.Id });
		}

	}
}
