using Hishop.Alipay.OpenHome.Model;
using System;

namespace Hishop.Alipay.OpenHome.Request
{
    public class MessagePushRequest : IRequest
    {
        Message message;


        public string GetMethodName()
        {
            return "alipay.mobile.public.message.push";
        }


        public MessagePushRequest(string appid, string toUserId, Articles articles, int articleCount, string agreementId = null, string msgType = "image-text")
        {
            Message message = new Message()
            {
                AgreementId = agreementId,
                AppId = appid,
                Articles = articles,
                ArticleCount = articleCount,
                ToUserId = toUserId,
                CreateTime = Utility.TimeHelper.TransferToMilStartWith1970(DateTime.Now).ToString("F0"),
                MsgType = msgType,
            };

            this.message = message; 
        }


        public string GetBizContent()
        {
            return Utility.XmlSerialiseHelper.Serialise<Message>(message);
        }
    }
}
