using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Passingwind.Blog.Web.Areas.Admin.Models
{
    public abstract class BaseModel
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

    }

    public abstract class BaseCreationModel : BaseModel
    {
        public DateTime CreationTime { get; set; } = DateTime.Now;
    }

    public abstract class BaseAuditedModel : BaseCreationModel
    {
        public DateTime? LastModificationTime { get; set; }
    }




}
