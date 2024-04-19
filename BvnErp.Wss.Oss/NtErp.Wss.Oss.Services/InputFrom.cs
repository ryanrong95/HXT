using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services
{
    /// <summary>
    /// 账户收入来源
    /// </summary>
    public enum InputFrom
    {
        /// <summary>
        /// 现金充值
        /// </summary>
        [Description("充值")]
        Cach = 1,
        /// <summary>
        /// 后台充值
        /// </summary>
        [Description("后台充值")]
        BackCach = 2,
        /// <summary>
        /// 信用申请
        /// </summary>
        [Description("信用申请")]
        Credit = 3,
        /// <summary>
        /// 信用提额申请
        /// </summary>
        [Description("信用提额申请")]
        AddCredit = 4,
        /// <summary>
        /// 后台信用
        /// </summary>
        [Description("后台信用添加")]
        BackCredit = 5,
        /// <summary>
        /// 后台信用提额
        /// </summary>
        [Description("后台信用提额")]
        BackAddCredit = 6
    }
}
