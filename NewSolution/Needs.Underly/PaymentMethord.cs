using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Underly
{
    /// <summary>
    /// 付款方式
    /// </summary>
    public enum PaymentMethord
    {
        [Description("信用证")]
        LetterOfCredit,
        [Description("电汇")]
        TelegraphicTransfer,
        [Description("付款交单")]
        DocumentAgainstPayment,
        [Description("承兑交单")]
        DocumentsAgainstAcceptance,
        [Description("微信")]
        Wiki,
        [Description("支付宝")]
        Alipay,
        [Description("Paypal")]
        Paypal
    }
}
