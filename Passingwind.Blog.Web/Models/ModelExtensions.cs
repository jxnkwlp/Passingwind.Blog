using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
    public static class ModelExtensions
    {
        public static PostViewModel ToModel(this Post entity)
        {
            return new PostViewModel()
            {
                Id = entity.Id,

                Content = entity.Content,
                PublishedTime = entity.PublishedTime,
                Description = entity.Description,
                EnableComment = entity.EnableComment,
                IsDraft = entity.IsDraft,
                ViewsCount = entity.ViewsCount,
                Slug = entity.Slug,
                Title = entity.Title,
                CommentsCount = entity.CommentsCount,

                User = entity.User?.ToModel(),
            };
        }

        public static PageViewModel ToModel(this Page entity)
        {
            return new PageViewModel()
            {
                Content = entity.Content,
                ParentId = entity.ParentId,
                Description = entity.Description,
                Keywords = entity.Keywords,
                IsShowInList = entity.IsShowInList,
                IsFrontPage = entity.IsFrontPage,
                Published = entity.Published,
                Slug = entity.Slug,
                Title = entity.Title,
                DisplayOrder = entity.DisplayOrder,
            };
        }

        public static CategoryViewModel ToModel(this Category entity)
        {
            return new CategoryViewModel()
            {
                Description = entity.Description,
                DisplayOrder = entity.DisplayOrder,
                Name = entity.Name,
                Slug = entity.Slug,
            };
        }

        public static CommentViewModel ToModel(this Comment entity, bool enableComment = true)
        {
            return new CommentViewModel()
            {
                Author = entity.Author,
                Content = entity.Content,
                Country = entity.Country,
                CreationTime = entity.CreationTime,
                Email = entity.Email,
                Id = entity.Id,
                IP = entity.IP,
                ParentId = entity.ParentId,
                PostId = entity.PostId,
                Website = entity.Website,
                CommentNestingEnabled = enableComment
            };
        }

        public static UserViewModel ToModel(this User entity)
        {
            return new UserViewModel()
            {
                Id = entity.Id,
                DisplayName = entity.DisplayName,
                UserDescription = entity.UserDescription,
                UserName = entity.UserName,
            };
        }
    }
}
