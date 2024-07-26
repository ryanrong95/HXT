using Senparc.NeuChar.Entities;
using Senparc.NeuChar.Helpers;
using Senparc.Weixin.MP.AppStore;
using Senparc.Weixin.MP.Entities;
using Senparc.Weixin.MP.Entities.Request;
using Senparc.Weixin.MP.Helpers;
using Senparc.Weixin.MP.MessageHandlers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using WeChat.Api.Model;

namespace WeChat.Api.CustomMessageHandler
{
    public class CustomMessageHandler : MessageHandler<CustomMessageContext>
    {
        public CustomMessageHandler(Stream inputStream, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(inputStream, postModel, maxRecordCount, developerInfo)
        {
            base.CurrentMessageContext.ExpireMinutes = 10;
        }

        public CustomMessageHandler(XDocument requestDocument, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(requestDocument, postModel, maxRecordCount, developerInfo)
        {
        }

        public CustomMessageHandler(RequestMessageBase requestMessageBase, PostModel postModel = null, int maxRecordCount = 0, DeveloperInfo developerInfo = null) : base(requestMessageBase, postModel, maxRecordCount, developerInfo)
        {
        }

        public override IResponseMessageBase OnEvent_ClickRequest(RequestMessageEvent_Click requestMessage)
        {
            if (requestMessage.EventKey == "CONTACT_US")
            {
                var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
                responseMessage.Content = "您好，欢迎关注华芯通一站式报关服务平台。\r\n报关热线：0755-83765188\r\n地址：深圳市龙岗区坂田区吉华路393号英达丰科技园";
                return responseMessage;
            }

            return new ResponseMessageNoResponse();
        }

        public override IResponseMessageBase DefaultResponseMessage(IRequestMessageBase requestMessage)
        {
            var responseMessage = requestMessage.CreateResponseMessage<ResponseMessageText>();
            responseMessage.Content = "您好，欢迎关注华芯通一站式报关服务平台。\r\n报关热线：0755-83765188\r\n地址：深圳市龙岗区坂田区吉华路393号英达丰科技园";
            return responseMessage;
        }
    }
}
