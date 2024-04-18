using Senparc.Weixin.MP.Entities.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeChat.Api.Model
{
    public class WxPostModel
    {
        //
        // 摘要:
        //     Signature
        public string Signature { get; set; }
        //
        // 摘要:
        //     Msg_Signature
        public string Msg_Signature { get; set; }
        //
        // 摘要:
        //     Timestamp
        public string Timestamp { get; set; }
        //
        // 摘要:
        //     Nonce
        public string Nonce { get; set; }
        //
        // 摘要:
        //     Token
        public string Token { get; set; }
        //
        // 摘要:
        //     EncodingAESKey
        public string EncodingAESKey { get; set; }
    }
}
