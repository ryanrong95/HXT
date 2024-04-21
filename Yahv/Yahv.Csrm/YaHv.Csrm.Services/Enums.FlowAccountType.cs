using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace YaHv.Csrm.Services
{
    /// <summary>
    /// 流水账类型
    /// </summary>
    public enum FlowAccountType
    {
        [Description("信用")]
        CreditRecharge = 10,
        [Description("信用花费")]
        CreditCost = 20,
        [Description("现金")]
        Cash = 30,
        [Description("银行")]
        Bank = 40,
        [Description("信用总账")]
        CreditTotal = 50,
    }
}
