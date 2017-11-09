namespace Passingwind.Blog
{
    public class PostCategory
    {
        public string PostId { get; set; }

        public Post Post { get; set; }

        public string CategoryId { get; set; }

        public Category Category { get; set; }
    }
}