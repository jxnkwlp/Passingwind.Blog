namespace Passingwind.Blog
{
    public class PostTags
    {
        public string PostId { get; set; }

        public Post Post { get; set; }

        public string TagsId { get; set; }

        public Tags Tags { get; set; }
    }
}