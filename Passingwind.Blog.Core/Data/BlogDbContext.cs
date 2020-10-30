using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Passingwind.Blog.Data.Domains;

namespace Passingwind.Blog.Data
{
	public class BlogDbContext : IdentityDbContext<User, Role, string, IdentityUserClaim<string>, UserRole, IdentityUserLogin<string>, IdentityRoleClaim<string>, IdentityUserToken<string>>
	{
		#region entity

		public DbSet<Post> Posts { get; set; }
		public DbSet<Category> Categories { get; set; }
		public DbSet<Tags> Tags { get; set; }
		public DbSet<PostCategory> PostCategories { get; set; }
		public DbSet<PostTags> PostTags { get; set; }
		public DbSet<Page> Pages { get; set; }
		public DbSet<Comment> Comments { get; set; }
		public DbSet<Setting> Settings { get; set; }

		public DbSet<WidgetDynamicContent> WidgetDynamicContents { get; set; }

		#endregion

		public BlogDbContext(DbContextOptions<BlogDbContext> options) : base(options)
		{
		}

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Post>().HasIndex(t => t.Slug);
			builder.Entity<Post>().HasMany(t => t.Categories).WithOne(t => t.Post);
			builder.Entity<Post>().HasMany(t => t.Tags).WithOne(t => t.Post);
			builder.Entity<Post>().HasMany(t => t.Comments).WithOne(t => t.Post);

			builder.Entity<Category>().HasIndex(t => t.Slug);
			builder.Entity<Tags>().HasIndex(t => t.Name);

			builder.Entity<PostCategory>().HasKey(t => new { t.PostId, t.CategoryId });
			builder.Entity<PostTags>().HasKey(t => new { t.PostId, t.TagsId });

			builder.Entity<Page>().HasIndex(t => t.Slug);

			builder.Entity<Comment>().HasIndex(t => t.Author);
			builder.Entity<Comment>().HasIndex(t => t.Email);

			builder.Entity<Setting>().Property(t => t.Key).IsRequired().HasMaxLength(128);
			builder.Entity<Setting>().HasIndex(t => t.Key);


			builder.Entity<Role>().HasMany(t => t.Permissions).WithOne(t => t.Role);
			builder.Entity<RolePermission>().HasKey(t => new { t.RoleId, t.Key });

			builder.Entity<UserRole>().HasOne(t => t.User).WithMany(t => t.UserRoles).HasForeignKey(t => t.UserId).IsRequired();
			builder.Entity<UserRole>().HasOne(t => t.Role).WithMany(t => t.UserRoles).HasForeignKey(t => t.RoleId).IsRequired();


			builder.Entity<WidgetDynamicContent>().HasMany(t => t.Properties).WithOne(t => t.WidgetDynamicContent).HasForeignKey(t => t.WidgetDynamicContentId);

			builder.Entity<WidgetDynamicContentProperty>().HasKey(t => new { t.WidgetDynamicContentId, t.Name });

			//var customModelBuilderTypes = AssemblyLoadContext.Default.Assemblies.SelectMany(t => t.ExportedTypes).Where(t => typeof(ICustomModelBuilder).IsAssignableFrom(t));

			//foreach (var type in customModelBuilderTypes)
			//{
			//	(Activator.CreateInstance(type) as ICustomModelBuilder)?.CustomConfig(builder);
			//}
		}
	}
}
