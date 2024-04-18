using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Yahv.Plats.Services.Models
{
    /// <summary>
    /// 用于获取用户信息的access_token
    /// </summary>
    public class Access_Token_U
    {
        //  {
        //   "access_token":"ACCESS_TOKEN",
        //   "expires_in":7200,
        //   "refresh_token":"REFRESH_TOKEN",
        //   "openid":"OPENID",
        //   "scope":"SCOPE",
        //   "unionid": "o6_bmasdasdsad6_2sgVt7hMZOPfL"
        //  }

        /// <summary>
        /// 网页授权接口调用凭证
        /// </summary>
        public string access_token { set; get; }
        /// <summary>
        /// 有效时间
        /// </summary>
        public int expires_in { set; get; }
        /// <summary>
        /// 用户刷新
        /// </summary>
        public string refresh_token { set; get; }
        /// <summary>
        /// 用户刷新
        /// </summary>
        public string openid { set; get; }
        /// <summary>
        /// 用户授权的作用域,使用逗号（,）分隔 
        /// </summary>
        public string scope { set; get; }
        /// <summary>
        /// 只有在用户将公众号绑定到微信开放平台帐号后，才会出现该字段
        /// </summary>
        public string unionid { set; get; }
    }
}
