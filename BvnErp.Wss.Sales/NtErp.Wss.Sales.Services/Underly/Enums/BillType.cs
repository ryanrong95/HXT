using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
    /// <summary>
    /// 票据类型
    /// </summary>
    [Obsolete("废弃，可能有用")]
    public enum BillType
    {
        /// <summary>
        /// 银行汇票
        /// </summary>
        Bank = 1,
        /// <summary>
        /// 商业承兑汇票
        /// </summary>
        TradeAcceptance = 2,
        /// <summary>
        /// 银行承兑汇票
        /// </summary>
        BankAcceptance = 3,
        /// <summary>
        /// 转账支票
        /// </summary>
        TransferCheque = 4,
        /// <summary>
        /// 现金支票
        /// </summary>
        CashCheque = 5,
        /// <summary>
        /// 普通支票
        /// </summary>
        CommonCheque = 6
    }
}
