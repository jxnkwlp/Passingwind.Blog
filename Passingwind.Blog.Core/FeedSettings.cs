namespace Passingwind.Blog
{
    public class FeedSettings : ISettings
    {
        public bool Enabled { get; set; } = true;

        public string Author { get; set; }

        public int ShowCount { get; set; } = 50;
    }
}