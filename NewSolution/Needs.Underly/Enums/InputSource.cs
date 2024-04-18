using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    /// <summary>
    /// 账户收入来源
    /// </summary>
    public enum InputSource
    {
        /// <summary>
        /// 现金充值
        /// </summary>
        Cach = 1,
        /// <summary>
        /// 后台充值
        /// </summary>
        BackCach = 2,
        /// <summary>
        /// 信用申请
        /// </summary>
        Credit = 3,
        /// <summary>
        /// 信用提额申请
        /// </summary>
        AddCredit = 4,
        /// <summary>
        /// 后台信用
        /// </summary>
        BackCredit = 5,
        /// <summary>
        /// 后台信用提额
        /// </summary>
        BackAddCredit = 6,
        /// <summary>
        /// 信用还款账单
        /// </summary>
        CreditBill = 7,
        /// <summary>
        /// 信用还款
        /// </summary>
        CachToCredit = 8
    }
}
