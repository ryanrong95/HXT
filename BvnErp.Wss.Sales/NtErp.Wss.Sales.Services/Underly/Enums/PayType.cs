using NtErp.Wss.Sales.Services.Utils.Structures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Underly
{
  
    [Obsolete("废弃")]
    public enum PayType
    {        

        /// <summary>
        /// 网银支付
        /// </summary>
        [Naming("网银支付")]
        NetBank,
        /// <summary>
        /// 支付宝
        /// </summary>
        [Naming("支付宝")]
        Zhifubo,
        /// <summary>
        /// 银行转账
        /// </summary>
        [Naming("银行转账")]
        Transfer,
        /// <summary>
        /// 余额支付
        /// </summary>
        [Naming("余额支付")]
        Balance,
        /// <summary>
        /// 代支付
        /// </summary>
        [Naming("余额代支付")]
        Agent,
        /// <summary>
        /// 信用支付
        /// </summary>
        [Naming("信用支付")]
        Credit,
        /// <summary>
        /// 信用支付
        /// </summary>
        [Naming("信用代支付")]
        AgentCredit,


        [Naming("退款")]
        Returns = 100000,
    }
}
