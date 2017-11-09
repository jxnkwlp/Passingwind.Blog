using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Web;
using System;
using System.Collections.Generic;

namespace Passingwind.Blog.Data
{
    public class DbInitializer : ISingletonService
    {
        private readonly BlogDbContext _dbContext;
        private readonly UserManager _userManager;
        private readonly RoleManager _roleManager;
        private readonly PostManager _postManager;

        public DbInitializer(BlogDbContext context, UserManager userManager, RoleManager roleManager, PostManager postManager)
        {
            this._dbContext = context;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._postManager = postManager;
        }

        public void Initialize()
        {
            var result = _dbContext.Database.EnsureCreated();

            if (result)
            {
                var role = new Role()
                {
                    Name = "Administrator",
                };
                _roleManager.CreateAsync(role).Wait();

                _roleManager.CreateAsync(new Role() { Name = "Editor" }).Wait();
                _roleManager.CreateAsync(new Role() { Name = "Anonymous" }).Wait();

                var user = new User()
                {
                    UserName = "admin",
                    Email = "admin@wuliping.cn",
                    DisplayName = "admin",

                    SecurityStamp = Guid.NewGuid().ToString(),
                };

                _userManager.CreateAsync(user, "123456").Wait();

                _userManager.AddToRolesAsync(user, new string[] { role.Name }).Wait();


                // add simple post 
                AddSimpleData(_dbContext, user);

            }

        }

        private void AddSimpleData(BlogDbContext dbContext, User user)
        {
            var category = new Category()
            {
                Name = "General",
                Slug = "General",
            };

            dbContext.Add(category);

            var post = new Post()
            {
                Title = "This website build by asp.net core and entity framework core.",
                Content = "## This website build by asp.net core and entity framework core.",
                IsDraft = false,
                CreationTime = DateTime.Now,
                EnableComment = true,
                Description = "This website build by asp.net core and entity framework core.",
                UserId = user.Id,
            };

            post.Categories.Add(new PostCategory() { CategoryId = category.Id, });

            post.Slug = post.GetSeName();

            dbContext.Add(post);
             
            // save 
            dbContext.SaveChanges();

        }

    }
}
