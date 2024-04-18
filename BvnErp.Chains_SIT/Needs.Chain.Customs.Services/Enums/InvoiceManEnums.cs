using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 发票认证状态
    /// </summary>
    public enum InvoiceVaildStatus
    {
        /// <summary>
        /// 待查验
        /// </summary>
        [Description("待查验")]
        UnChecked = 0,

        /// <summary>
        /// 查验成功
        /// </summary>
        [Description("有")]
        Vailded = 1,

        /// <summary>
        /// 查验失败
        /// </summary>
        [Description("无")]
        Invailded = 2,
    }
}
