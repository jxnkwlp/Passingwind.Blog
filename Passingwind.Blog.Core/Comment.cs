using System;
using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
{
    /// <summary>
    ///
    /// </summary>
    public class Comment : CreationEntity
    {
        public string PostId { get; set; }

        public Post Post { get; set; }

        [MaxLength(250)]
        public string Author { get; set; }

        [MaxLength(250)]
        public string Email { get; set; }

        public string Website { get; set; }

        public string Content { get; set; }

        public string Country { get; set; }

        /// <summary>
        ///  已审核
        /// </summary>
        public bool IsApproved { get; set; } = true;

        /// <summary>
        ///  垃圾
        /// </summary>
        public bool IsSpam { get; set; }

        /// <summary>
        ///  删除
        /// </summary>
        public bool IsDeleted { get; set; }

        public string ParentId { get; set; }

        [MaxLength(250)]
        public string IP { get; set; }
    }
}