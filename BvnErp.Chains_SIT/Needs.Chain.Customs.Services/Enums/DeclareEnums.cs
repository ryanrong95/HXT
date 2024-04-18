using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 报关通知状态
    /// </summary>
    public enum DeclareNoticeStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未制单")]
        UnDec = 0,

        /// <summary>
        /// 已处理
        /// </summary>
        [Description("已制单")]
        AllDec = 1,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancel = 2,
    }

    /// <summary>
    /// 报关通知项状态
    /// </summary>
    public enum DeclareNoticeItemStatus
    {
        /// <summary>
        /// 未制单
        /// </summary>
        [Description("未制单")]
        UnMake = 0,

        /// <已制单>
        /// 已制单
        /// </summary>
        [Description("已制单")]
        Make = 2,

        /// <已制单>
        /// 已制单
        /// </summary>
        [Description("已取消")]
        Cancel = 3
    }

    /// <summary>
    /// 香港仓库标记已报关，未报关
    /// </summary>
    public enum SortingDecStatus
    {
        /// <summary>
        /// 未报关
        /// </summary>
        [Description("未报关")]
        No = 0,

        /// <已制单>
        /// 已报关
        /// </summary>
        [Description("已报关")]
        Yes = 1
    }

    /// <summary>
    /// 报关单税费类型
    /// </summary>
    public enum DecTaxType
    {
        [Description("进口关税")]
        Tariff = 1,

        [Description("进口增值税")]
        AddedValueTax = 2,

        [Description("进口消费税")]
        ExciseTax = 3,
    }

    /// <summary>
    /// 报关单缴税状态
    /// </summary>
    public enum DecTaxStatus
    {
        [Description("未缴税")]
        Unpaid = 1,

        [Description("已缴税")]
        Paid = 2,

        [Description("已抵扣")]
        Deducted = 3,

        [Description("无需缴税")]
        NoTax = 4,

    }
    /// <summary>
    /// 是否上传流水
    /// </summary>
    public enum UploadStatus
    {
        [Description("未上传")]
        NotUpload = 0,

        [Description("已上传")]
        Uploaded = 1,

    }

    /// <summary>
    /// 处理类型(复合形式)
    /// </summary>
    public enum HandledType
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        NoHandled = 0,

        /// <summary>
        /// 关税已处理
        /// </summary>
        [Description("关税")]
        Tariff = 1,

        /// <summary>
        /// 增值税已处理
        /// </summary>
        [Description("增值税")]
        AddedValueTax = 2,

        /// <summary>
        /// 消费税已处理
        /// </summary>
        [Description("消费税")]
        ExciseTax = 4,
    }

    /// <summary>
    /// 缴费状态 
    /// </summary>
    public enum TaxStatus
    {
        [Description("未缴税")]
        Unpaid = 0,

        [Description("已缴税")]
        Paid = 1,
    }
    /// <summary>
    /// 报关单特殊类型
    /// </summary>
    public enum DecHeadSpecialTypeEnum
    {
        /// <summary>
        /// 包车
        /// </summary>
        [Description("包车")]
        CharterBus = 1,

        /// <summary>
        /// 高价值
        /// </summary>
        [Description("高价值")]
        HighValue = 2,

        /// <summary>
        /// 商检
        /// </summary>
        [Description("商检")]
        Inspection = 3,

        /// <summary>
        /// 检疫
        /// </summary>
        [Description("检疫")]
        Quarantine = 4,

        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 5,

        /// <summary>
        /// 原产地加征
        /// </summary>
        [Description("加征")]
        OriginATRate = 6,

        /// <summary>
        /// 敏感产地
        /// </summary>
        [Description("敏感产地")]
        SenOrigin = 7,
    }

    /// <summary>
    /// 订单是否报关标志
    /// 跟单，订单和到货匹配的时候用
    /// </summary>
    public enum DeclareFlagEnums
    {
        /// <summary>
        /// 不可报关
        /// </summary>
        Unable = 1,
        /// <summary>
        /// 可报关
        /// </summary>
        Able = 2,
        /// <summary>
        /// 已报关
        /// </summary>
        Done = 3
    }
}
