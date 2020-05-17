using System;
using System.Xml.Serialization;

namespace BlogML.Xml
{

    public abstract class BlogMLNode
    {
        private string id;
        private string title;
        private DateTime dateCreated = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private bool approved = true;

        [XmlAttribute("id")]
        public string ID
        {
            get { return this.id; }
            set { this.id = value; }
        }

        [XmlElement("title")]
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        [XmlAttribute("date-created", DataType = "dateTime")]
        public DateTime DateCreated
        {
            get { return this.dateCreated; }
            set { this.dateCreated = value; }
        }

        [XmlAttribute("date-modified", DataType = "dateTime")]
        public DateTime DateModified
        {
            get { return this.dateModified; }
            set { this.dateModified = value; }
        }

        [XmlAttribute("approved")]
        public bool Approved
        {
            get { return this.approved; }
            set { this.approved = value; }
        }
    }
}
