using System;
using System.Xml.Serialization;

namespace BlogML.Xml
{

    public sealed class BlogMLAuthor : BlogMLNode
    {
        private string email;

        [XmlAttribute("email")]
        public string Email {
            get { return this.email; }
            set { this.email = value; }
        }
    }
}
