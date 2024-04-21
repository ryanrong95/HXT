using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyeSDK.model
{
    public class AccessTokenKyeResponse
    {
        /// <summary>
        /// 响应代码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 响应数据
        /// </summary>
        public KyeAccessToken data { get; set; }
        /// <summary>
        /// 响应状态
        /// </summary>
        public bool success { get; set; }
    }
}
