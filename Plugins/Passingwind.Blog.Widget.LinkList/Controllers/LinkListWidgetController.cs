using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Widget.LinkList.Models;
using System;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.LinkList.Controllers
{
	[Area("admin")]
	[Authorize]
	public class LinkListWidgetController : Controller
	{
		private readonly IPluginDataStore _pluginDataStore;

		public LinkListWidgetController(IPluginDataStore pluginDataStore)
		{
			_pluginDataStore = pluginDataStore;
		}

		public async Task<IActionResult> Configure(Guid id)
		{
			var list = await _pluginDataStore.GetListAsync<LinkModel>(Consts.Name, id);

			var model = new ConfigureViewModel()
			{
				Id = id,
				Links = list,
			};

			return View("/Passingwind.Blog.Widget.LinkList/Views/Configure.cshtml", model);
		}

		[HttpPost]
		public async Task<IActionResult> Add(Guid id, string title, string url)
		{
			if (title == null)
				throw new System.ArgumentNullException(nameof(title));

			if (url == null)
				throw new System.ArgumentNullException(nameof(url));

			await _pluginDataStore.InsertAsync(Consts.Name, id, new LinkModel
			{
				Title = title,
				Url = url,
			});

			return RedirectToAction(nameof(Configure), new { id });
		}

		[HttpPost]
		public async Task<IActionResult> Delete(Guid id, Guid[] itemId)
		{
			if (itemId != null && itemId.Length > 0)
				await _pluginDataStore.DeleteAsync<LinkModel>(Consts.Name, id, itemId);

			return RedirectToAction(nameof(Configure), new { id });
		}
	}
}
