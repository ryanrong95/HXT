using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyeSDK.model
{
    public class KyeAccessToken
    {
        /// <summary>
        /// token
        /// </summary>
        public string token { get; set; }
        /// <summary>
        /// 刷新TOKEN
        /// </summary>
        public string refresh_token { get; set; }
        /// <summary>
        /// 时间戳
        /// </summary>
        public long expire_time { get; set; }

    }
}
