using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.InvoiceService.Services.InvVeri
{
    /// <summary>
    /// 用来表示方法或属性的返回值，返回的数据类型有<b>T</b>决定
    /// </summary>
    /// <typeparam name="T">数据的类型</typeparam>
    public class ReturnValue<T>
    {
        /// <summary>
        /// true表示成功，否则失败
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// 消息代码
        /// </summary>
        public string MessageCode { get; set; }

        /// <summary>
        /// 消息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 数据
        /// </summary>
        public T Data { get; set; }
    }

    /// <summary>
    /// 用来表示方法或属性的返回值
    /// </summary>
    public class ReturnValue : ReturnValue<object>
    {
    }
}
