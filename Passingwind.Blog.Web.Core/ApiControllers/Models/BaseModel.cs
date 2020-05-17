using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Models
{
	public abstract class BaseModel<TKey>
	{
		public TKey Id { get; set; }
	}

	public abstract class BaseModel : BaseModel<int> { }

	public abstract class BaseAuditTimeModel<TKey> : BaseModel<TKey>
	{
		public DateTime CreationTime { get; set; } = DateTime.Now;
		public DateTime? LastModificationTime { get; set; }
	}

	public abstract class BaseAuditTimeModel : BaseAuditTimeModel<int> { }
}
