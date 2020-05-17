namespace Passingwind.Blog.Data.Settings
{
    public class FeedSettings : ISettings
    {
        public bool Enabled { get; set; } = true;

        //public string Author { get; set; }

        public int Limit { get; set; } = 50;
    }
}
