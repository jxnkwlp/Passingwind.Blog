using System.ComponentModel.DataAnnotations;

namespace Passingwind.Blog.Data.Settings
{
    public class AdvancedSettings : ISettings
    {
        [DataType(DataType.MultilineText)]
        public string HeaderHtml { get; set; }

        [DataType(DataType.MultilineText)]
        public string FooterHtml { get; set; }

        public bool EnableOpenSearch { get; set; } = true;

        public bool EnableRegister { get; set; } = false;
		 
        public string EditorName { get; set; }
    }
}
