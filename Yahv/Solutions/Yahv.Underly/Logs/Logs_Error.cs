using System;

namespace Yahv.Underly.Logs
{
    /// <summary>
    /// 错误日志
    /// </summary>
    public class Logs_Error
    {
        /// <summary>
        /// 构造函数
        /// </summary>
        public Logs_Error()
        {
            this.CreateDate = DateTime.Now;
        }

        /// <summary>
        /// 主键
        /// </summary>
        public string ID { get; }
        /// <summary>
        /// 操作管理员
        /// </summary>
        public string AdminID { get; }
        /// <summary>
        /// 页面地址
        /// </summary>
        public string Page { get; set; }
        /// <summary>
        /// 错误编码
        /// </summary>
        public string Codes { get; set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }
        /// <summary>
        /// 错误源
        /// </summary>
        public string Source { get; set; }
        /// <summary>
        /// 错误堆栈
        /// </summary>
        public string Stack { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }

    }
}
