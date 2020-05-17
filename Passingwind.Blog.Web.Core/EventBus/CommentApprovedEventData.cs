using Passingwind.Blog.Data.Domains;
using Passingwind.Blog.EventBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.EventBus
{
	public class CommentApprovedEventData : IEventBusData
	{
		public Comment SourceComment { get; set; }
		public Comment Replay { get; set; }

		public string CommentUrl { get; set; }

		public string PostAuthor { get; set; }
		public string PostAuthorEmail { get; set; }

		public Post Post { get; set; }
	}
}
