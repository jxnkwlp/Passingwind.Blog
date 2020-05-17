using System;
using System.Xml.Serialization;

namespace BlogML.Xml
{

    public sealed class BlogMLCategory : BlogMLNode
    {
        private string description;
        private string parentRef;

        [XmlAttribute("description")]
        public string Description
        {
            get { return this.description; }
            set { this.description = value; }
        }

        [XmlAttribute("parentref")]
        public string ParentRef
        {
            get { return this.parentRef; }
            set { this.parentRef = value; }
        }
    }
}
