using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Models;
using Passingwind.Blog.Web.Models;
using System.Threading;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.ApiControllers
{
	public class FileController : ApiControllerBase
	{
		private readonly IFileService _fileService;

		public FileController(IFileService fileService)
		{
			_fileService = fileService;
		}

		[HttpPost]
		public async Task<FileUploadResultModel> UploadAsync(IFormFile file, CancellationToken cancellationToken = default)
		{
			if (file.Length == 0)
				return null;

			using (var stream = file.OpenReadStream())
			{
				var bytes = new byte[file.Length];
				await stream.ReadAsync(bytes, 0, (int)file.Length, cancellationToken);

				var result = await _fileService.WriteAsync(new FileWriteInputModel() { Data = bytes, FileName = file.FileName });

				return new FileUploadResultModel()
				{
					FileName = result.FileName,
					OriginalFileName = result.OriginalFileName,
					Size = result.Size,
					Uri = result.Uri,
					UriPath = result.UriPath,
				};
			}
		}

		[HttpPost("editor/tinymce")]
		public async Task<IActionResult> TinymceEditorFileUploadAsync(IFormFile file, CancellationToken cancellationToken = default)
		{
			if (file == null || file.Length == 0)
			{
				return new JsonResult(new { location = "" });
			}

			var result = await UploadAsync(file, cancellationToken);

			return new JsonResult(new { location = result.UriPath });
		}
		 
	}
}
