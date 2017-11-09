using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Services
{
    public class LocalFileService : IFileService
    {
        const string DefaultPath = "~/Uploads/";

        private readonly IHostingEnvironment _hostingEnvironment;

        public LocalFileService(IHostingEnvironment hostingEnvironment)
        {
            this._hostingEnvironment = hostingEnvironment;
        }

        public string Upload(string fileName, Stream stream)
        {
            if (stream == null)
                throw new ArgumentNullException(nameof(stream));

            if (string.IsNullOrWhiteSpace(fileName))
                throw new ArgumentNullException(nameof(fileName));

            var bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);

            var hashString = GetHash(bytes).ToLowerInvariant();
            var ext = Path.GetExtension(fileName);

            string newFile = hashString + ext.ToLower();

            string path = Path.Combine("Uploads", DateTime.Now.ToString("yyyyMMdd"), newFile);

            var serverPath = Path.Combine(_hostingEnvironment.WebRootPath, path);

            if (!Directory.Exists(Path.GetDirectoryName(serverPath)))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(serverPath));
            }

            File.WriteAllBytes(serverPath, bytes);

            // var applicationBase = PlatformServices.Default.Application.ApplicationBasePath;

            return path;
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
