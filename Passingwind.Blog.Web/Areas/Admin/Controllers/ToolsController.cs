using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.BlogML;
using System.Text;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
	public class ToolsController : AdminControllerBase
	{
		private readonly BlogMLImporter _blogMLImporter;
		private readonly BlogMLExporter _blogMLExporter;
		private readonly UserManager _userManager;
		private readonly IApplicationRestart _applicationRestart;

		public ToolsController(BlogMLImporter blogMLImporter, BlogMLExporter blogMLExporter, UserManager userManager, IApplicationRestart applicationRestart)
		{
			this._blogMLImporter = blogMLImporter;
			this._blogMLExporter = blogMLExporter;

			this._userManager = userManager;
			_applicationRestart = applicationRestart;
		}

		public IActionResult Index()
		{
			return View();
		}


		[HttpPost]
		public async Task<IActionResult> Import(IFormFile file)
		{
			if (file == null || file.Length == 0)
				AlertError("文件错误");

			// full path to file in temp location
			var filePath = Path.GetTempFileName();

			using (var stream = new FileStream(filePath, FileMode.Create))
			{
				await file.CopyToAsync(stream);
			}

			using (var stream = new FileStream(filePath, FileMode.Open))
			{
				using (var sr = new StreamReader(stream))
				{
					_blogMLImporter.XmlData = await sr.ReadToEndAsync();

					var user = await _userManager.GetUserAsync(User);

					if (await _blogMLImporter.ImportAsync(user))
					{
						AlertSuccess(_blogMLImporter.Message);
					}
					else
					{
						AlertError(_blogMLImporter.Message);
					}
				}
			}

			return RedirectToAction(nameof(Index));
		}

		[HttpPost]
		public IActionResult Export()
		{
			var xml = _blogMLExporter.GetExportXml();

			return File(Encoding.UTF8.GetBytes(xml), "text/xml", "BlogML.xml");
		}

		[HttpPost]
		public IActionResult AppRestart()
		{
			// just work on iis or others web host.
			_applicationRestart.Restart();

			return RedirectToAction(nameof(Index));
		}
	}
}
