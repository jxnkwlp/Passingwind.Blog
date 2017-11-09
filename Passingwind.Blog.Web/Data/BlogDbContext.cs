using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Passingwind.Blog.Data
{
    public class BlogDbContext : IdentityDbContext<User, Role, string>
    {
        #region entity

        public DbSet<Post> Posts { get; set; }
        public DbSet<Page> Pages { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<PostCategory> PostCategories { get; set; }
        public DbSet<PostTags> PostTags { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Setting> Settings { get; set; }

        #endregion

        public BlogDbContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<PostCategory>().HasKey(t => new { t.PostId, t.CategoryId });
            builder.Entity<PostTags>().HasKey(t => new { t.PostId, t.TagsId });

            //builder.Entity<Post>().HasMany(t => t.Categories).WithOne(t => t.Post);
            //builder.Entity<Post>().HasMany(t => t.Tags).WithOne(t => t.Post);

            builder.Entity<PostCategory>().HasOne(t => t.Post).WithMany(t => t.Categories);
            builder.Entity<PostCategory>().HasOne(t => t.Category).WithMany(t => t.Posts);

            builder.Entity<PostTags>().HasOne(t => t.Post).WithMany(t => t.Tags);
            builder.Entity<PostTags>().HasOne(t => t.Tags).WithMany(t => t.Posts);

            builder.Entity<Comment>().HasOne(t => t.Post).WithMany(t => t.Comments).OnDelete(DeleteBehavior.SetNull);

            builder.Entity<Setting>().Property(t => t.Key).IsRequired().HasMaxLength(300);

            builder.Entity<RolePermission>().HasKey(t => new { t.RoleId, t.PermissionKey });

            builder.Entity<Role>().HasMany(t => t.Permissions).WithOne(t => t.Role);

        }
    }
}
