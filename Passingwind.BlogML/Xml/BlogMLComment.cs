using System;
using System.Xml.Serialization;

namespace BlogML.Xml
{

    public sealed class BlogMLComment : BlogMLNode
    {
        private string userName;
        private string userEmail;
        private string userUrl;
        private BlogMLContent content = new BlogMLContent();

        [XmlAttribute("user-name")]
        public string UserName
        {
            get { return this.userName; }
            set { this.userName = value; }
        }

        [XmlAttribute("user-url")]
        public string UserUrl
        {
            get { return this.userUrl; }
            set { this.userUrl = value; }
        }

        [XmlAttribute("user-email")]
        public string UserEMail
        {
            get { return this.userEmail; }
            set { this.userEmail = value; }
        }

        [XmlElement("content")]
        public BlogMLContent Content
        {
            get { return this.content; }
            set { this.content = value; }
        }
    }
}
