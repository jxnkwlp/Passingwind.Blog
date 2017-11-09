using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;


namespace Passingwind.Blog.Web.Services
{
    public interface IFileService
    {
        string Upload(string fileName, Stream stream);
    }
     
}
