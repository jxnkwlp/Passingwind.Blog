using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Json;
using Passingwind.Blog.Services;
using Passingwind.Blog.Widget.CustomHtml.Models;
using System;
using System.Threading.Tasks;

namespace Passingwind.Blog.Widget.CustomHtml.Controllers
{
	[ApiExplorerSettings(IgnoreApi = true)]
	[Area("widget")]
	[Route("admin/Widgets/customhtml/[action]")]
	[Authorize]
	public class CustomHtmlWidgetController : Controller
	{
		public IActionResult Configure(Guid id)
		{
			var model = new ConfigureViewModel() { Id = id };
			return View("/Passingwind.Blog.Widget.CustomHtml/Views/CustomHtmlWidget/Configure.cshtml", model);
		}

	}

	[Authorize]
	[Route("/admin/widgets/customhtml")]
	public partial class CustomHtmlWidgetApiController : ControllerBase
	{
		private readonly IWidgetDynamicContentService _widgetContentService;
		private readonly IJsonSerializer _jsonSerializer;

		public CustomHtmlWidgetApiController(IWidgetDynamicContentService widgetSettingsService, IJsonSerializer jsonSerializer)
		{
			_widgetContentService = widgetSettingsService;
			_jsonSerializer = jsonSerializer;
		}

		[HttpGet]
		public async Task<IActionResult> GetAsync(Guid widgetId)
		{
			var model = await _widgetContentService.GetAsync<CustomHtmlModel>(widgetId, string.Empty);

			return Ok(model);
		}

		[HttpPost]
		public async Task<IActionResult> UpdateAsync(Guid widgetId, [FromBody] UpdateContentModel model)
		{
			var entity = await _widgetContentService.GetAsync<CustomHtmlModel>(widgetId, string.Empty);

			if (entity == null)
			{
				await _widgetContentService.InsertAsync(new CustomHtmlModel()
				{
					UserId = string.Empty,
					WidgetId = widgetId,
					Content = model.Content,
				});
			}
			else
			{
				entity.Content = model.Content;

				await _widgetContentService.UpdateAsync(entity);
			}

			return Ok();
		}

		public class UpdateContentModel
		{
			public string Content { get; set; }
		}
	}
}
