using System;
using System.Xml.Serialization;

namespace BlogML.Xml
{

    public sealed class BlogMLTrackback : BlogMLNode
    {
        private string url;

        [XmlAttribute("url")]
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }
    }
}
