using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace kyeSDK.model
{
     public class KyeRequest
    {
        /// <summary>
        /// 请求方法
        /// </summary>
        public string methodCode { get; set; }
        /// <summary>
        /// 请求参数
        /// </summary>
        public string paramInfo { get; set; }
        /// <summary>
        /// 编码类型
        /// </summary>
        public string contentType { get; set; }
        /// <summary>
        /// 返回参数格式
        /// </summary>
        public string responseFormat { get; set; }

    }
}
