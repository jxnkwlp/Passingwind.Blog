using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Text;
using System.Xml;

namespace Passingwind.Blog.Web.Controllers
{
    public class OpenSearchController : Controller
    {
        private readonly BasicSettings _basicSettings;

        public OpenSearchController(BasicSettings basicSettings)
        {
            this._basicSettings = basicSettings;
        }

        [ResponseCache(Duration = 3600 * 24)]
        [Route("opensearch.xml" ,Name =RouteNames.OpenSearch)]
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
            using (XmlWriter xw = XmlWriter.Create(stream))
            {
                xw.WriteStartDocument();

                xw.WriteStartElement("OpenSearchDescription", "http://a9.com/-/spec/opensearch/1.1/");

                // required element
                xw.WriteElementString("ShortName", _basicSettings.Title);
                xw.WriteElementString("Description", _basicSettings.Description);

                xw.WriteStartElement("Url");
                xw.WriteAttributeString("type", "text/html");
                string url = Url.RouteUrl(RouteNames.Search, new { q = "" }, Request.Scheme) + "?q={searchTerms}";
                xw.WriteAttributeString("template", url);
                xw.WriteEndElement();

                // Optionals
                xw.WriteStartElement("Image");
                xw.WriteAttributeString("width", "16");
                xw.WriteAttributeString("height", "16");
                xw.WriteAttributeString("type", "image/x-icon");
                xw.WriteString(Url.Content("~/favicon.ico"));
                xw.WriteEndElement();


                xw.WriteEndElement();

                xw.WriteEndDocument();
            }
        }
    }
}
