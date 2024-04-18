using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 库房费用类型
    /// </summary>
    public enum WarehousePremiumType
    {
        /// <summary>
        /// 入仓费
        /// </summary>
        [Description("入仓费")]
        EntryFee = 1,

        /// <summary>
        /// 仓储费
        /// </summary>
        [Description("仓储费")]
        StorageFee = 2,

        /// <summary>
        /// 收货异常费用
        /// </summary>
        [Description("收货异常费用")]
        UnNormalFee = 3,

        [Description("处理标签费")]
        ProcessLabelFee = 4,

        [Description("换箱费")]
        ChangeBoxFee = 5,

        [Description("垫付快递费")]
        ExpressFee = 6,

        [Description("提货费")]
        DeliverFee = 7,

        [Description("垫付登记费")]
        RegisterFee = 8,

        [Description("垫付隧道费")]
        TunnelFee = 9,

        [Description("垫付车场费")]
        parkingFee = 10,

        [Description("超重费")]
        OverweightFee = 11,
        
        [Description("包车费（单独一车）")]
        CharterFee = 12,

        /// <summary>
        /// 其他费用（库房）
        /// </summary>
        [Description("其他")]
        Other = 13,

        [Description("大陆来货清关费")]
        MainlandClearance = 14,

    }
}
