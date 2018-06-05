using System.IO;


namespace Passingwind.Blog.Web.Services
{
    public interface IFileService
    {
        string Upload(string fileName, Stream stream);
    }
     
}
