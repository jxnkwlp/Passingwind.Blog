using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog
{
    public class BasicSettings : ISettings
    {
        [Required]
        public string Title { get; set; } = "BlogName";

        public string Description { get; set; } = "Description";

        public int PageShowCount { get; set; } = 10;

        public bool ShowDescription { get; set; } = true;

        public int ShowDescriptionStringCount { get; set; } = 100;

        public string IcpNumber { get; set; }
    }
}