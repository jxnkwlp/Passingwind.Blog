namespace Passingwind.Blog.Data.Domains
{
    public class PostTags
    {
        public int PostId { get; set; }

        public Post Post { get; set; }

        public int TagsId { get; set; }

        public Tags Tags { get; set; }
    }
}