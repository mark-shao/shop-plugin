using System.Collections.Generic;


namespace Hishop.Weixin.MP.Domain
{
    public class TemplateMessage
    {
        public TemplateMessage() {

            Topcolor = "#00FF00";
        }


        public string Touser { get; set; }

        public string TemplateId { get; set; }

        public string Url { get; set; }
        /// <summary>
        /// 小程序内部链接
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FormId { get; set; }
        /// <summary>
        /// 顶部线条颜色，默认#FF0000
        /// </summary>
        public string Topcolor { get; set; }

        public IEnumerable<MessagePart> Data { get; set; }


        public class MessagePart
        {
            public MessagePart() {
                Color = "#000099";
            
            }

            public string Name { get; set; }

            public string Value { get; set; }

            /// <summary>
            /// 文字颜色，默认#CCCCCC
            /// </summary>
            public string Color { get; set; }
        }
        /// <summary>
        /// 模板需要放大的关键词，不填则默认无放大
        /// </summary>
        public string EmphasisKeyword { get; set; }
    }
}
