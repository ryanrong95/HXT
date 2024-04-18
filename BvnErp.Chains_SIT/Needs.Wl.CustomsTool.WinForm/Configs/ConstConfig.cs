using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.CustomsTool
{
    public class ConstConfig
    {
        /// <summary>
        /// 海关回执文件夹
        /// </summary>
        public const string InBox = "InBox";

        /// <summary>
        /// 回执备份
        /// </summary>
        public const string InBox_BK = "InBox_BK";

        /// <summary>
        /// 待海关读取文件夹
        /// </summary>
        public const string OutBox = "OutBox";

        /// <summary>
        /// 发送失败文件夹
        /// </summary>
        public const string FailBox = "FailBox";

        /// <summary>
        /// 发送失败 BK 文件夹
        /// </summary>
        public const string FailBox_BK = "FailBox_BK";

        /// <summary>
        /// 发送成功文件夹
        /// </summary>
        public const string SentBox = "SentBox";

        /// <summary>
        /// 电子随附单据临时文件夹
        /// </summary>
        public const string Edoc = "Edoc";

        /// <summary>
        /// 报文临时文件夹
        /// </summary>
        public const string Message = "Message";

        /// <summary>
        /// 成功/业务回执待处理
        /// </summary>
        public const string WaitReceipt = "WaitReceipt";

        /// <summary>
        /// 失败回执待处理
        /// </summary>
        public const string WaitFail = "WaitFail";

        /// <summary>
        /// 海关订阅报文
        /// </summary>
        public const string DecSub = "DECSUB";
        
    }
}
