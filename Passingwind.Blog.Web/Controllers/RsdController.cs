using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Xml;

namespace Passingwind.Blog.Web.Controllers
{
	public class RsdController : BlogControllerBase
	{
		[ResponseCache(Duration = 3600 * 24)]
		[Route("/rsd.xml", Name = RouteNames.Rsd)]
		public IActionResult Index()
		{
			using (MemoryStream ms = new MemoryStream())
			{
				WriteXml(ms);

				string result = Encoding.UTF8.GetString(ms.ToArray());

				return Content(result, "text/xml");
			}
		}


		private void WriteXml(Stream stream)
		{
			using (XmlWriter writer = XmlWriter.Create(stream))
			{
				writer.WriteStartDocument();

				// Rsd tag
				writer.WriteStartElement("rsd");
				writer.WriteAttributeString("version", "1.0");

				// Service
				writer.WriteStartElement("service");
				writer.WriteElementString("engineName", "Passingwind");
				writer.WriteElementString("engineLink", "");
				writer.WriteElementString("homePageLink", Url.RouteUrl(RouteNames.Home, new { }, Request.Scheme));

				// APIs
				writer.WriteStartElement("apis");

				// MetaWeblog
				writer.WriteStartElement("api");
				writer.WriteAttributeString("name", "MetaWeblog");
				writer.WriteAttributeString("preferred", "true");
				writer.WriteAttributeString("apiLink", Url.RouteUrl(RouteNames.Metaweblog, new { }, Request.Scheme));
				writer.WriteAttributeString("blogID", Url.RouteUrl(RouteNames.Home, new { }, Request.Scheme));
				writer.WriteEndElement();

				// BlogML
				//writer.WriteStartElement("api");
				//writer.WriteAttributeString("name", "BlogML");
				//writer.WriteAttributeString("preferred", "false");
				//writer.WriteAttributeString("apiLink", $"{Utils.AbsoluteWebRoot}api/BlogImporter.asmx");
				//writer.WriteAttributeString("blogID", Utils.AbsoluteWebRoot.ToString());
				//writer.WriteEndElement();

				// End APIs
				writer.WriteEndElement();

				// End Service
				writer.WriteEndElement();

				// End Rsd
				writer.WriteEndElement();

				writer.WriteEndDocument();
			}
		}
	}
}
