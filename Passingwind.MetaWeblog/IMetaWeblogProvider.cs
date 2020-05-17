using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.MetaWeblog
{
	public interface IMetaWeblogProvider
	{
		/// <summary>
		///  metaWeblog.newPost 
		/// </summary> 
		Task NewPostAsync();

		/// <summary>
		///  metaWeblog.editPost
		/// </summary> 
		Task EditPostAsync();
		/// <summary>
		///  metaWeblog.getPost 
		/// </summary> 
		Task GetPostAsync();
		/// <summary>
		///  metaWeblog.newMediaObject 
		/// </summary> 
		Task NewMediaObjectAsync();
		/// <summary>
		///  metaWeblog.getCategories 
		/// </summary> 
		Task GetCategoriesAsync();
		/// <summary>
		///  wp.getAuthors 
		/// </summary> 
		Task getAuthorsAsync();
		/// <summary>
		///  wp.getPageList
		/// </summary> 
		Task getPageListAsync();
		/// <summary>
		///  wp.getPages
		/// </summary> 
		Task getPagesAsync();
		/// <summary>
		///  wp.getTags
		/// </summary> 
		Task getTags();

		// metaWeblog.getRecentPosts
		// blogger.getUsersBlogs
		//metaWeblog.getUsersBlogs
		//blogger.deletePost
		// blogger.getUserInfo
		// wp.newPage
		// wp.getPage
		// wp.editPage
		// wp.deletePage
	}
}
