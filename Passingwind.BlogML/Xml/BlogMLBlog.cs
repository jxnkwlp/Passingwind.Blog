using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml.Serialization;
using System.Collections.Generic;

namespace BlogML.Xml
{

    [XmlRoot(ElementName = "blog", Namespace = "http://www.blogml.com/2006/09/BlogML")]
    public sealed class BlogMLBlog
    {
        private string rootUrl;
        private string title;
        private string subTitle;
        private DateTime dateCreated = DateTime.Now;
        private AuthorCollection authors = new AuthorCollection();
        private PostCollection posts = new PostCollection();
        private CategoryCollection categories = new CategoryCollection();
        private ExtendedPropertiesCollection extendedproperties = new ExtendedPropertiesCollection();


        [XmlAttribute("root-url")]
        public string RootUrl
        {
            get { return this.rootUrl; }
            set { this.rootUrl = value; }
        }

        [XmlElement("title")]
        public string Title
        {
            get { return this.title; }
            set { this.title = value; }
        }

        [XmlElement("sub-title")]
        public string SubTitle
        {
            get { return this.subTitle; }
            set { this.subTitle = value; }
        }

        [XmlAttribute("date-created", DataType = "dateTime")]
        public DateTime DateCreated
        {
            get { return this.dateCreated; }
            set { this.dateCreated = value; }
        }

        [XmlArray("extended-properties")]
        [XmlArrayItem("property", typeof(KeyValuePair<string, string>))]
        public ExtendedPropertiesCollection ExtendedProperties
        {
            get
            {
                return this.extendedproperties;
            }
        }

        [XmlArray("authors")]
        [XmlArrayItem("author", typeof(BlogMLAuthor))]
        public AuthorCollection Authors
        {
            get
            {
                return this.authors;
            }
        }


        [XmlArray("posts")]
        [XmlArrayItem("post", typeof(BlogMLPost))]
        public PostCollection Posts
        {
            get
            {
                return this.posts;
            }
        }

        [XmlArray("categories")]
        [XmlArrayItem("category", typeof(BlogMLCategory))]
        public CategoryCollection Categories
        {
            get
            {
                return this.categories;
            }
        }


        public sealed class AuthorCollection : List<BlogMLAuthor> { }

        public sealed class PostCollection : List<BlogMLPost> { }

        public sealed class CategoryCollection : List<BlogMLCategory> { }

        public sealed class ExtendedPropertiesCollection : List<KeyValuePair<string, string>> { }

    }
}
