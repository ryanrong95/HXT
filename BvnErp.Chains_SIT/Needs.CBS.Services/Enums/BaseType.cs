using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Cbs.Services.Enums
{
    /// <summary>
    /// 海关基础数据类型
    /// </summary>
    public enum BaseType
    {
        [Description("随附单证类型")]
        AppCertCode = 0,

        [Description("集装箱规格")]
        Container = 1,

        [Description("关联理由")]
        CorrelationReason = 2,

        [Description("国家地区")]
        Country = 3,

        [Description("币种")]
        Currency = 4,

        [Description("海关回执代码")]
        CusReceiptCode = 5,

        [Description("关区代码")]
        CustomMaster = 6,

        [Description("征免性质")]
        CutMode = 7,

        [Description("国内行政区划")]
        DestCode = 8,

        [Description("国内行政区划/境内目的地代码")]
        DistrictCode = 9,

        [Description("监管证件")]
        DocuCode = 10,

        [Description("正减免税方式")]
        DutyMode = 11,

        [Description("电子随附单据类型代码")]
        EdocCode = 12,

        [Description("国内口岸")]
        EntryPort = 13,

        [Description("货物属性")]
        GoodsAttr = 14,

        [Description("海关通关")]
        GovProcedure = 15,

        [Description("检验检疫机关代码")]
        OrgCode = 16,

        [Description("原产地区")]
        OriginArea = 17,

        [Description("包装类型(舱单)")]
        PackType = 18,

        [Description("港口")]
        Port = 19,

        [Description("用途")]
        Purpose = 20,

        [Description("监管方式")]
        TradeMode = 21,

        [Description("运输方式")]
        TrafMode = 22,

        [Description("成交方式")]
        TransMode = 23,

        [Description("计量单位代码")]
        Units = 24,

        [Description("包装类型")]
        WrapType = 25,
    }
}
