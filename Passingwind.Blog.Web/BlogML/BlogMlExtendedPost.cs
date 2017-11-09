using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogML.Xml;

namespace Passingwind.Blog.BlogML
{
    /// <summary>
    /// Extended BlogML post
    /// </summary>
    public class BlogMlExtendedPost
    {
        /// <summary>
        /// Gets or sets blog post
        /// </summary>
        public BlogMLPost BlogPost { get; set; }

        /// <summary>
        /// Gets or sets post URL
        /// </summary>
        public string PostUrl { get; set; }

        /// <summary>
        /// Gets or sets post tags
        /// </summary>
        public List<string> Tags { get; set; }

        /// <summary>
        /// Gets or sets post categories
        /// </summary>
        public List<Category> Categories { get; set; }

        /// <summary>
        /// Post comments
        /// </summary>
        public List<Comment> Comments { get; set; }
    }
}
