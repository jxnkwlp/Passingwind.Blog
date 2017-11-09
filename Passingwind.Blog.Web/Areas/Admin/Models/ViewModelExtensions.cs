using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public static class ViewModelExtensions
    {
        #region entity to model

        public static CategoryViewModel ToModel(this Category entity)
        {
            return new CategoryViewModel()
            {
                Description = entity.Description,
                DisplayOrder = entity.DisplayOrder,
                Name = entity.Name,
                Slug = entity.Slug,
                Id = entity.Id,

                Count = entity.Posts.Count, 
            };
        }

        public static TagsViewModel ToModel(this Tags entity)
        {
            return new TagsViewModel()
            {
                Name = entity.Name,
                Id = entity.Id,

                Count = entity.Posts.Count,
            };
        }

        public static PageViewModel ToModel(this Page entity)
        {
            return new PageViewModel()
            {
                Id = entity.Id,

                Content = entity.Content,
                Description = entity.Description,
                Keywords = entity.Keywords,
                Published = entity.Published,
                Slug = entity.Slug,
                Title = entity.Title,
                DisplayOrder = entity.DisplayOrder,

                CreationTime = entity.CreationTime,
                LastModificationTime = entity.LastModificationTime,
            };
        }

        public static PostViewModel ToModel(this Post entity)
        {
            return new PostViewModel()
            {
                Id = entity.Id,

                Content = entity.Content,
                Description = entity.Description,
                EnableCommented = entity.EnableComment,
                IsDraft = entity.IsDraft,
                Slug = entity.Slug,
                Title = entity.Title,
                CommentsCount = entity.CommentsCount,

                PublishedTime = entity.PublishedTime,
                EnableComment = entity.EnableComment,
                ViewsCount = entity.ViewsCount,

                CreationTime = entity.CreationTime,
                LastModificationTime = entity.LastModificationTime,

                User = entity.User?.ToModel(),
            };
        }

        public static CommentViewModel ToModel(this Comment entity)
        {
            return new CommentViewModel()
            {
                Id = entity.Id,

                Content = entity.Content,
                Author = entity.Author,
                Country = entity.Country,

                Email = entity.Email,
                Website = entity.Website,

                IP = entity.IP,
                IsApproved = entity.IsApproved,
                IsDeleted = entity.IsDeleted,
                IsSpam = entity.IsSpam,

                ParentId = entity.ParentId,

                PostId = entity.PostId,

                CreationTime = entity.CreationTime,
            };
        }


        public static RoleViewModel ToModel(this Role entity)
        {
            return new RoleViewModel()
            {
                Id = entity.Id,
                Name = entity.Name,
            };
        }

        public static UserViewModel ToModel(this User entity)
        {
            return new UserViewModel()
            {
                Id = entity.Id,
                Email = entity.Email,
                UserName = entity.UserName,
                PhoneNumber = entity.PhoneNumber,
                EmailConfirmed = entity.EmailConfirmed,
                PhoneNumberConfirmed = entity.PhoneNumberConfirmed,
                DisplayName = entity.DisplayName,
                UserDescription = entity.UserDescription,

                Lockouted = entity.IsLockouted
            };
        }

        #endregion

        #region model to entity

        public static Category ToEntity(this CategoryViewModel model)
        {
            return new Category()
            {
                Description = model.Description,
                DisplayOrder = model.DisplayOrder,
                Name = model.Name,
                //ParentId = entity.ParentId,
                Slug = model.Slug,
                Id = model.Id,
            };
        }

        public static Category ToEntity(this CategoryViewModel model, Category entity)
        {
            entity.Description = model.Description;
            entity.DisplayOrder = model.DisplayOrder;
            entity.Name = model.Name;
            entity.Slug = model.Slug;
            entity.Id = model.Id;

            return entity;
        }


        public static Tags ToEntity(this TagsViewModel model)
        {
            return new Tags()
            {
                Name = model.Name,
                Id = model.Id,
            };
        }

        public static Tags ToEntity(this TagsViewModel model, Tags entity)
        {
            entity.Name = model.Name;
            entity.Id = model.Id;

            return entity;
        }


        public static Page ToEntity(this PageViewModel model)
        {
            return new Page()
            {
                Id = model.Id,
                Content = model.Content,
                Description = model.Description,
                Keywords = model.Keywords,
                Published = model.Published,
                Slug = model.Slug,
                Title = model.Title,
                DisplayOrder = model.DisplayOrder,
            };
        }

        public static Page ToEntity(this PageViewModel model, Page entity)
        {
            entity.Id = model.Id;
            entity.Content = model.Content;
            entity.Description = model.Description;
            entity.Keywords = model.Keywords;
            entity.Published = model.Published;
            entity.Slug = model.Slug;
            entity.Title = model.Title;
            entity.DisplayOrder = model.DisplayOrder;

            return entity;
        }


        public static Post ToEntity(this PostViewModel model)
        {
            return new Post()
            {
                Id = model.Id,
                Content = model.Content,
                Description = model.Description,
                PublishedTime = model.PublishedTime,
                IsDraft = model.IsDraft,
                Slug = model.Slug,
                Title = model.Title,
                EnableComment = model.EnableCommented,
            };
        }

        public static Post ToEntity(this PostViewModel model, Post entity)
        {
            entity.Id = model.Id;
            entity.Content = model.Content;
            entity.Description = model.Description;
            entity.PublishedTime = model.PublishedTime;
            entity.Slug = model.Slug;
            entity.Title = model.Title;
            entity.EnableComment = model.EnableCommented;
            entity.IsDraft = model.IsDraft;

            return entity;
        }


        public static Comment ToEntity(this CommentViewModel model)
        {
            return new Comment()
            {
                Id = model.Id,

                Content = model.Content,
                Author = model.Author,
                Country = model.Country,

                Email = model.Email,
                Website = model.Website,

                IP = model.IP,
                IsApproved = model.IsApproved,
                IsDeleted = model.IsDeleted,
                IsSpam = model.IsSpam,

                ParentId = model.ParentId,

                PostId = model.PostId,
            };
        }

        public static Comment ToEntity(this CommentViewModel model, Comment entity)
        {
            entity.Id = model.Id;
            entity.Content = model.Content;
            entity.Author = model.Author;
            entity.Country = model.Country;
            entity.Email = model.Email;
            entity.Website = model.Website;
            entity.IP = model.IP;
            entity.IsApproved = model.IsApproved;
            entity.IsDeleted = model.IsDeleted;
            entity.IsSpam = model.IsSpam;
            entity.ParentId = model.ParentId;
            entity.PostId = model.PostId;

            return entity;
        }


        public static Role ToEntity(this RoleViewModel model)
        {
            return new Role()
            {
                Id = model.Id,
                Name = model.Name,
            };
        }

        public static Role ToEntity(this RoleViewModel model, Role entity)
        {
            entity.Id = model.Id;
            entity.Name = model.Name;

            return entity;
        }

        public static User ToEntity(this UserViewModel model)
        {
            return new User()
            {
                Id = model.Id,
                UserName = model.UserName,
                Email = model.Email,

                UserDescription = model.UserDescription,
                DisplayName = model.DisplayName,
            };
        }

        public static User ToEntity(this UserViewModel model, User entity)
        {
            entity.Id = model.Id;
            entity.UserName = model.UserName;
            entity.Email = model.Email;
            entity.UserDescription = model.UserDescription;
            entity.DisplayName = model.DisplayName;

            return entity;
        }

        #endregion
    }
}
