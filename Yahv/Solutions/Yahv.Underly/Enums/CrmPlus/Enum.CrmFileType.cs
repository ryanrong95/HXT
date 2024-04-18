using Yahv.Underly.Attributes;

namespace Yahv.Underly
{

    public enum CrmFileType
    {
        /// <summary>
        /// 未知
        /// </summary>
        [Description("未知")]
        UnKnown = 0,
        /// <summary>
        /// 企业Logo
        /// </summary>
        [Description("企业Logo")]
        Logo = 1,
        /// <summary>
        /// 资质文件
        /// </summary>
        [Description("资质文件")]
        QualificationCertificate = 2,
        /// <summary>
        /// 企业证照
        /// </summary>
        [Description("企业证照")]
        License = 3,
        /// <summary>
        /// 型号的Pcn,产品变更通知？
        /// </summary>
        [Description("Pcn")]
        Pcn = 4,
        /// <summary>
        /// 型号的DataSheet,数据手册
        /// </summary>
        [Description("DataSheet")]
        DataSheet = 5,

        [Description("客户激活")]
        ClientActive = 6,

        [Description("客户交接")]
        ClientHandOver = 7,
        [Description("供应商激活")]
        SupplierActive = 8,
        /// <summary>
        /// 名片
        /// </summary>
        [Description("名片")]
        VisitingCard = 100,
        /// <summary>
        /// 特色品牌文件
        /// </summary>
        [Description("特色品牌文件")]
        SpecialBrands = 101,
        /// <summary>
        /// 定价规则（固定渠道）
        /// </summary>
        [Description("定价规则（固定渠道）")]
        PricingRules = 102,
        /// <summary>
        /// 企业关系
        /// </summary>
        [Description("企业关系")]
        EnterpriseRelation = 103,

        [Description("特殊要求")]
        Requirements = 104,

        /// <summary>
        /// 跟踪记录
        /// </summary>
        [Description("跟踪记录")]
        TraceRecords = 106,

        /// <summary>
        /// 授权文件
        /// </summary>
        [Description("授权文件")]
        Authorization = 110,

        /// <summary>
        /// 备案文件
        /// </summary>
        [Description("备案文件")]
        FilingDocument = 111,

      
    }
}
