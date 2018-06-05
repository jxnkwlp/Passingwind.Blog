using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Services;
using System.IO;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    public class FileController : AdminControllerBase
    {
        private IFileService _fileService;

        public FileController(IFileService fileService)
        {
            this._fileService = fileService;
        }

        [HttpPost]
        public IActionResult Upload(IFormFile file, string editor)
        {
            if (editor == "tinymce")
            {
                return TinymceUpload(file);
            }
            else if (editor == "editormd")
            {
                return EditormdUpload();
            }


            return View();
        }

        private IActionResult TinymceUpload(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return Json(new { location = "" });
            }

            var path = _fileService.Upload(file.FileName, file.OpenReadStream());

            path = Path.Combine((Request.PathBase.HasValue ? Request.PathBase.Value : "/"), path).Replace(@"\", "/");

            return Json(new { location = path });
        }

        private IActionResult EditormdUpload()
        {
            if (Request.Form.Files.Count == 0 || Request.Form.Files[0].Length == 0)
            {
                return Json(new { success = 0, message = "File error " });
            }

            var file = Request.Form.Files[0];

            var path = _fileService.Upload(file.FileName, file.OpenReadStream());

            path = Path.Combine((Request.PathBase.HasValue ? Request.PathBase.Value : "/"), path).Replace(@"\", "/");

            return Json(new { success = 1, message = "upload success", url = path });
        }
    }
}