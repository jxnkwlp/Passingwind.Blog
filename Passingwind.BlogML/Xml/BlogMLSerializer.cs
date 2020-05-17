using System;
using System.IO;
using System.Xml.Serialization;
using System.Xml;

namespace BlogML.Xml
{
    public class BlogMLSerializer
    {
        private static readonly object syncRoot = new object();
        private static XmlSerializer serializer;

        public static XmlSerializer Serializer
        {
            get
            {
                lock (syncRoot)
                {
                    if (serializer == null)
                        serializer = new XmlSerializer(typeof(BlogMLBlog));
                    return serializer;
                }
            }
        }

        public static BlogMLBlog Deserialize(Stream stream)
        {
            return Serializer.Deserialize(stream) as BlogMLBlog;
        }

        public static BlogMLBlog Deserialize(TextReader reader)
        {
            return Serializer.Deserialize(reader) as BlogMLBlog;
        }

        public static BlogMLBlog Deserialize(XmlReader reader)
        {
            return Serializer.Deserialize(reader) as BlogMLBlog;
        }

        public static void Serialize(Stream stream, BlogMLBlog blog)
        {
            Serializer.Serialize(stream, blog);
        }

        public static void Serialize(TextWriter writer, BlogMLBlog blog)
        {
            Serializer.Serialize(writer, blog);
        }

        public static void Serialize(XmlWriter writer, BlogMLBlog blog)
        {
            Serializer.Serialize(writer, blog);
        }
    }
}
