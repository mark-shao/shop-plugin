using Hishop.Alipay.OpenHome.Utility;
using System.Xml.Serialization;

namespace Hishop.Alipay.OpenHome.Model
{
    [XmlRoot("Item")]
    public class Item
    {

        private string title;
        private string description;
        private string imageUrl;
        private string url;



        [XmlElement("Title",typeof(CData))]
        public CData Title { get { return title; } set { title = value; } }

        [XmlElement("Desc", typeof(CData))]
        public CData Description { get { return description; } set { description = value; } }

        [XmlElement("ImageUrl", typeof(CData))]
        public CData ImageUrl { get { return imageUrl; } set { imageUrl = value; } }

        [XmlElement("Url", typeof(CData))]
        public CData Url { get { return url; } set { url = value; } }
    }
}
