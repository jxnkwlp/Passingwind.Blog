using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Passingwind.Blog.Web.Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;

namespace Passingwind.Blog.Web.Areas.Admin.Controllers
{
    //[Authorize(Policy = "Admin")]
    public class DefaultController : AdminControllerBase
    {
        private readonly PostManager _postManager;
        private readonly PageManager _pageManager;
        private readonly CategoryManager _categoryManager;
        private readonly TagsManager _tagsManager;
        private readonly CommentManager _commentManager;

        public DefaultController(PostManager postManager, PageManager pageManager, CategoryManager categoryManager, TagsManager tagsManager, CommentManager commentManager)
        {
            this._postManager = postManager;
            this._pageManager = pageManager;
            this._categoryManager = categoryManager;
            this._tagsManager = tagsManager;
            this._commentManager = commentManager;

        }

        public IActionResult Index()
        {
            //var model = new DashboardViewModel()
            //{
            //    TotalPostsCount = Task.FromResult(_postManager.GetQueryable().Count()).Result,
            //    TotalPagesCount = Task.FromResult(_pageManager.GetQueryable().Count()).Result,
            //    TotalCategoriesCount = Task.FromResult(_categoryManager.GetQueryable().Count()).Result,
            //    TotalTagsCount = Task.FromResult(_tagsManager.GetQueryable().Count()).Result,
            //};

            var model = new DashboardViewModel()
            {
                TotalPostsCount = _postManager.GetQueryable().Count(),
                TotalPagesCount = _pageManager.GetQueryable().Count(),
                TotalCategoriesCount = _categoryManager.GetQueryable().Count(),
                TotalTagsCount = _tagsManager.GetQueryable().Count(),
                TotalCommentsCount = _commentManager.GetQueryable().Count(t => !t.IsDeleted),
            };

            return View(model);
        }

        [AllowAnonymous]
        public IActionResult UnAuthorized()
        {
            return View();
        }
    }
}
