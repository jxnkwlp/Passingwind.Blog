using System;
using System.Xml.Serialization;

namespace BlogML.Xml
{

    public sealed class BlogMLAttachment
    {
        private bool embedded = false;
        private string url;
        private byte[] data;
        private string path;
        private string mimeType;

        [XmlAttribute("embedded")]
        public bool Embedded
        {
            get { return this.embedded; }
            set { this.embedded = value; }
        }

        [XmlAttribute("url")]
        public string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }

        [XmlAttribute("path")]
        public string Path
        {
            get { return this.path; }
            set { this.path = value; }
        }

        [XmlAttribute("mime-type")]
        public string MimeType
        {
            get { return this.mimeType; }
            set { this.mimeType = value; }
        }

        [XmlText]
        public byte[] Data
        {
            get { return this.data; }
            set { this.data = value; }
        }
    }
}
