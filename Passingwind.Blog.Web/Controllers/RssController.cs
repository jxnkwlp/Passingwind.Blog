using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace Passingwind.Blog.Web.Controllers
{
    public class RssController : Controller
    {
        private readonly PostManager _postManager;
        private readonly CategoryManager _categoryManager;
        private readonly TagsManager _tagManager;
        private readonly PageManager _pageManager;
        private readonly CommentManager _commentManager;

        private readonly BasicSettings _basicSettings;
        private readonly CommentsSettings _commentsSettings;

        public RssController(PostManager postManager, CategoryManager categoryManager, TagsManager tagManager, PageManager pageManager, CommentManager commentManager, BasicSettings basicSettings, CommentsSettings commentsSettings)
        {
            this._postManager = postManager;
            this._categoryManager = categoryManager;
            this._tagManager = tagManager;
            this._pageManager = pageManager;
            this._commentManager = commentManager;

            this._basicSettings = basicSettings;
            this._commentsSettings = commentsSettings;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Category(string name)
        {
            return View();
        }



    }
}
