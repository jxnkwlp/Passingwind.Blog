using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using Passingwind.Blog.Services;
using Passingwind.Blog.Services.Models;
using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Services
{
	public class LocalFileService : IFileService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;
		private readonly BlogOptions _blogOptions;
		private readonly IWebHostEnvironment _webHostEnvironment;

		private readonly string _serverFullFolder;
		private readonly string _serverBaseFolder;

		public LocalFileService(IOptions<BlogOptions> blogOptions, IHttpContextAccessor httpContextAccessor, IWebHostEnvironment webHostEnvironment)
		{
			_blogOptions = blogOptions.Value;
			_webHostEnvironment = webHostEnvironment;
			_httpContextAccessor = httpContextAccessor;

			if (_blogOptions.Upload.Provider != BlogUploadProvider.Local && _blogOptions.Upload.Provider != BlogUploadProvider.Unknow)
			{
				throw new Exception("The blog upload option provider is not 'local'.");
			}

			var folder = _blogOptions.Upload.Value ?? "uploads";

			_serverBaseFolder = _webHostEnvironment.WebRootPath;
			var fullFolder = Path.Combine(_serverBaseFolder, folder, DateTime.Now.ToString("yyyyMMdd"));

			if (!Directory.Exists(fullFolder))
				Directory.CreateDirectory(fullFolder);

			_serverFullFolder = fullFolder;
		}

		public Task DeleteAsync(string path)
		{
			throw new NotImplementedException();
		}

		public async Task<FileWriteOutputModel> WriteAsync(FileWriteInputModel input)
		{
			if (input == null)
				throw new ArgumentNullException(nameof(input));

			var hashString = GetHash(input.Data).ToLowerInvariant();
			var ext = Path.GetExtension(input.FileName);

			if (!string.IsNullOrEmpty(_blogOptions.Upload.AllowExtensions) && !_blogOptions.Upload.AllowExtensions.Split(';').Contains(ext))
			{
				throw new Exception($"The fil extension '{ext}' not allowed.");
			}

			string newFileName = hashString + ext.ToLower();

			var serverFilePath = Path.Combine(_serverFullFolder, newFileName);

			if (!Directory.Exists(Path.GetDirectoryName(serverFilePath)))
			{
				Directory.CreateDirectory(Path.GetDirectoryName(serverFilePath));
			}

			await File.WriteAllBytesAsync(serverFilePath, input.Data);

			var filePath = serverFilePath.Substring(_serverBaseFolder.Length);
			var uriPath = filePath.Replace("\\", "/");

			return new FileWriteOutputModel()
			{
				FileName = newFileName,
				OriginalFileName = input.FileName,
				Size = input.Data.Length,
				UriPath = uriPath,
				Uri = new Uri(_blogOptions.HostUri, uriPath),
			};
		}

		private string GetHash(byte[] bytes)
		{
			var hash = MD5.Create();
			var result = hash.ComputeHash(bytes);

			string resultString = null;
			result.ToList().ForEach(b => { resultString = string.Concat(resultString, b.ToString("X2")); });

			return resultString;
		}
	}
}
