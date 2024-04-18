using System;

namespace Yahv.Underly
{
    /// <summary>
    /// 消息体格式
    /// </summary>
    public class JMessage
    {
        /// <summary>
        /// 是否成功
        /// </summary>
        public bool success { get; set; }
        /// <summary>
        /// 标识值
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 信息
        /// </summary>
        public string data { get; set; }

        /// <summary>
        /// 成功
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public void IsSuccess(string message = "")
        {
            this.success = true;
            this.code = 200;
            this.data = message;
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="message"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public void IsFailed(string message = "", int code = 500)
        {
            this.success = false;
            this.code = code;
            this.data = message;
        }

        /// <summary>
        /// 失败
        /// </summary>
        /// <param name="ex"></param>
        /// <returns></returns>
        public void IsFailed(Exception ex)
        {
            this.success = false;
            this.code = 500;
            this.data = ex.InnerException?.StackTrace;
        }
    }
}
