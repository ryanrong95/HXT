using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum TTPaymentNature
    {
        /// <summary>
        /// 保税区
        /// </summary>
        [Description("A")]
        A = 1,

        /// <summary>
        /// 出口加工区
        /// </summary>
        [Description("B")]
        B = 2,

        /// <summary>
        /// 钻石交易所
        /// </summary>
        [Description("C")]
        C = 3,

        /// <summary>
        /// 其他特殊经济区域
        /// </summary>
        [Description("D")]
        D = 4,

        /// <summary>
        /// 深加工结转
        /// </summary>
        [Description("E")]
        E = 5,

        /// <summary>
        /// 其他
        /// </summary>
        [Description("F")]
        F = 6,
    }

    public enum TTPaymentCategory
    {
        /// <summary>
        /// Trade Related
        /// </summary>
        [Description("TR")]
        TR = 1,

        /// <summary>
        /// Non Trade Related
        /// </summary>
        [Description("NT")]
        NT = 2,

        /// <summary>
        /// Capital
        /// </summary>
        [Description("C")]
        C = 3

    }

    public enum TTPaymentPurpose
    {
        /// <summary>
        /// 预付货款 Advance Payment
        /// </summary>
        [Description("AP")]
        AP = 1,

        /// <summary>
        /// 进料加工 Processing Trade with Imported Material
        /// </summary>
        [Description("PD")]
        PD = 2,

        /// <summary>
        /// 边境贸易 Border Trade
        /// </summary>
        [Description("RF")]
        RF = 3,

        /// <summary>
        /// 其他贸易 Other Trade
        /// </summary>
        [Description("OT")]
        OT = 4
    }

    public enum TTBOPCode
    {
        /// <summary>
        /// Trade Related
        /// </summary>
        [Description("121010")]
        TT121010 = 1,

        /// <summary>
        /// Non Trade Related
        /// </summary>
        [Description("121020")]
        TT121020 = 2,

        /// <summary>
        /// Capital
        /// </summary>
        [Description("121030")]
        TT121030 = 3
    }

    public enum TTTaxFree
    {
        /// <summary>
        /// 免税相关
        /// </summary>
        [Description("Y")]
        Y = 1,

        /// <summary>
        /// 免税无关
        /// </summary>
        [Description("N")]
        N = 2,
    }

    public enum TTChargeBearer
    {
        /// <summary>
        /// 收款方承担
        /// </summary>
        [Description("收款方承担")]
        CRED = 1,

        /// <summary>
        /// 汇款人承担
        /// </summary>
        [Description("汇款人承担")]
        DEBT = 2,

        /// <summary>
        /// 共同承担
        /// </summary>
        [Description("共同承担")]
        SHAR = 3,
    }
}
