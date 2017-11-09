using System.Xml.Serialization;

namespace BlogML
{
    public enum BlogPostTypes : short
    {
        [XmlEnum("normal")]
        Normal = 1,

        [XmlEnum("article")]
        Article = 2,
    }
}