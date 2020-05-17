using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Json;
using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.LinkList.Models;
using System;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.LinkList.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Area("widget")]
	[Route("admin/Widgets/linklist/[action]")]
	[Authorize]
	public class LinkListWidgetController : Controller
	{
		public IActionResult Configure(Guid id)
		{
			var model = new ConfigureViewModel() { Id = id };
			return View("/Passingwind.Blog.Widget.LinkList/Views/Configure.cshtml", model);
		}

	}

	[Authorize]
	[Route("/admin/widgets/linklist")]
	public class LinkListWidgetApiController : ControllerBase
	{
		private readonly IWidgetDynamicContentService _widgetContentService;
		private readonly IJsonSerializer _jsonSerializer;

		public LinkListWidgetApiController(IWidgetDynamicContentService widgetSettingsService, IJsonSerializer jsonSerializer)
		{
			_widgetContentService = widgetSettingsService;
			_jsonSerializer = jsonSerializer;
		}

		[HttpGet("list")]
		public async Task<IActionResult> GetList(Guid id)
		{
			var links = await _widgetContentService.GetListAsync<LinkModel>(id, string.Empty);

			return Ok(links);
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Add(Guid widgetId, [FromBody] LinkContent model)
		{
			await _widgetContentService.InsertAsync(new LinkModel()
			{
				Title = model.Title,
				Url = model.Url,
				WidgetId = widgetId,
			});

			return Ok();
		}

		[HttpPost("[action]")]
		public async Task<IActionResult> Delete(int id)
		{
			await _widgetContentService.DeleteByIdAsync(id);

			return Ok();
		}

	}
}
