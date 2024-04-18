using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyeSDK.model
{
    public class AbstractKyeResponse
    {
        /// <summary>
        /// 响应代码
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 响应消息
        /// </summary>
        public string msg { get; set; }
        public string traceId { get; set; }
        /// <summary>
        /// 响应状态
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 返回参数格式
        /// </summary>
        public string responseFormat { get; set; }
        public string result { get; set; }
        /// <summary>
        /// 数据集
        /// </summary>
        public string data { get; set; }

    }
}
