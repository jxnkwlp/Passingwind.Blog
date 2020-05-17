using System;
using System.Net;
using System.Xml.Serialization;

namespace BlogML.Xml
{

    public sealed class BlogMLContent
    {
        private bool Base64Encoded
        {
            get { return ContentType == ContentTypes.Base64; }
        }

        private bool HtmlEncoded
        {
            get { return ContentType == ContentTypes.Html || ContentType == ContentTypes.Xhtml; }
        }

        [XmlAttribute("type")]
        public ContentTypes ContentType
        {
            get;
            set;
        }

        // Encoded Text
        [XmlText]
        public string Text
        {
            get;
            set;
        }

        [XmlIgnore]
        public string UncodedText
        {
            get
            {
                if (Base64Encoded)
                {
                    byte[] byteArray = Convert.FromBase64String(Text);
                    return System.Text.Encoding.UTF8.GetString(byteArray);
                }
                if (HtmlEncoded)
                {
                    return WebUtility.HtmlDecode(Text);
                }
                return Text;
            }
        }

        public static BlogMLContent Create(string unencodedText, ContentTypes contentType)
        {
            var content = new BlogMLContent { ContentType = contentType };
            if (content.Base64Encoded)
            {
                byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(unencodedText);
                content.Text = Convert.ToBase64String(byteArray);
            }
            else if (content.HtmlEncoded)
            {
                content.Text = WebUtility.HtmlEncode(unencodedText);
            }
            else
            {
                content.Text = unencodedText;
            }

            return content;
        }
    }
}
