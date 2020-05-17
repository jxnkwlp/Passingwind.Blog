using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Services;
using Passingwind.Blog.Web.Authorization;
using Passingwind.Blog.Web.Extensions;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class BlogMLController : ApiControllerBase
	{
		private readonly IBlogMLImporter _blogMLImporter;
		private readonly IBlogMLExporter _blogMLExporter;
		private readonly BlogUserManager _blogUserManager;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public BlogMLController(IBlogMLImporter blogMLImporter, BlogUserManager blogUserManager, IBlogMLExporter blogMLExporter, IHttpContextAccessor httpContextAccessor)
		{
			_blogMLImporter = blogMLImporter;
			_blogUserManager = blogUserManager;
			_blogMLExporter = blogMLExporter;
			_httpContextAccessor = httpContextAccessor;
		}

		[ApiPermission("blogml")]
		[HttpPost("import")]
		public async Task<BlogMLImporterResult> ImportAsync(IFormFile file, CancellationToken cancellationToken = default
		)
		{
			if (file.Length == 0)
				return null;

			using (var stream = file.OpenReadStream())
			{
				var bytes = new byte[file.Length];
				await stream.ReadAsync(bytes, 0, (int)file.Length, cancellationToken);

				var userId = _httpContextAccessor.HttpContext.User.GetUserId();

				var user = await _blogUserManager.FindByIdAsync(userId);

				if (user == null)
					throw new Exception("The current user not found.");

				try
				{
					var result = await _blogMLImporter.ImportAsync(user, Encoding.UTF8.GetString(bytes));

					return result;
				}
				catch (Exception)
				{
					throw;
				}
			}
		}

		[ApiPermission("blogml")]
		[HttpPost("export")]
		public async Task<IActionResult> ExportAsync(CancellationToken cancellationToken = default
		)
		{
			var userId = _httpContextAccessor.HttpContext.User.GetUserId();

			var user = await _blogUserManager.FindByNameAsync(userId);

			var resultData = await _blogMLExporter.ExportAsync(user, new BlogMLExportOptions()
			{
				ExportAllUser = true,
				ExportComments = true,
				ExportPages = true,
			});

			return new FileContentResult(resultData, "application/stream");
		}
	}
}
