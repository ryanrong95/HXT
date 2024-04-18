using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeChat.Api.Model;

namespace WeChat.Api.CustomMessageHandler
{
    public class WxCustomMessageHandler
    {
        private CustomMessageHandler messageHandler;

        public WxCustomMessageHandler(Stream inputStream, WxPostModel wxPostModel)
        {
            PostModel postModel = new PostModel()
            {
                Signature = wxPostModel.Signature,
                Msg_Signature = wxPostModel.Msg_Signature,
                Timestamp = wxPostModel.Timestamp,
                Nonce = wxPostModel.Nonce,
                Token = wxPostModel.Token,
                EncodingAESKey = wxPostModel.EncodingAESKey,
            };

            this.messageHandler = new CustomMessageHandler(inputStream, postModel);
            //去重
            messageHandler.OmitRepeatedMessage = true;
            messageHandler.Execute();
        }

        public string FinalResponseString
        {
            get
            {
                if (messageHandler.FinalResponseDocument == null)
                {
                    return NoResponseXml();
                }

                return messageHandler.FinalResponseDocument.ToString();
            }
        }

        private string NoResponseXml()
        {
            return @"<xml>
              <ToUserName><![CDATA[]]></ToUserName>
              <FromUserName><![CDATA[]]></FromUserName>
              <CreateTime>-62135596800</CreateTime>
              <MsgType><![CDATA[text]]></MsgType>
            </xml>";
        }
    }
}
