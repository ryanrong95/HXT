using Needs.Utils.Descriptions;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 报关/转关关系标志
    /// </summary>
    public enum DeclTrnRel
    {
        [Description("一般报关单")]
        Normal = 0,

        [Description("转关提前报关单")]
        TrnAhead = 1
    }

    /// <summary>
    /// 报关标志
    /// </summary>
    public enum EdiId
    {
        [MultiDescription("1", "普通报关")]
        Normal,

        [MultiDescription("3", "北方转关提前")]
        NorthTrn,

        [MultiDescription("5", "南方转关提前")]
        SouthTrn,

        [MultiDescription("6", "普通报关")]
        SouthH2000
    }


    /// <summary>
    /// 报关单类型
    /// </summary>
    public enum EntryType
    {
        [MultiDescription("0", "普通报关单")]
        Normal,

        [MultiDescription("L", "带报关清单的报关单")]
        WithList,

        [MultiDescription("W", "无纸报关类型")]
        Paperless,

        [MultiDescription("D", "既是清单又是无纸报关的情况")]
        ListPaperless,

        [MultiDescription("M", "无纸化通关")]
        PaperlessCustomsClearance
    }

    /// <summary>
    /// 备案清单类型
    /// </summary>
    public enum BillType
    {
        [MultiDescription("1", "")]
        Normal,

        [MultiDescription("2", "先进区后报关")]
        FirstInLastDeclaration,

        [MultiDescription("3", "分送集报备案清单")]
        CollectionList,

        [MultiDescription("4", "分送集报报关单")]
        CollectionDeclaration,
    }

    /// <summary>
    /// 查验分流标志
    /// </summary>
    public enum CheckFlow
    {
        [Description("否")]
        No = 0,

        [Description("是")]
        Yes = 1
    }

    /// <summary>
    /// 担保验放标志
    /// </summary>
    public enum ChkSurety
    {
        [Description("是")]
        Yes = 0,

        [Description("否")]
        No = 1
    }

    /// <summary>
    /// 签名人分类
    /// </summary>
    public enum DomainId
    {
        [Description("录入人")]
        InputUser = 1,

        [Description("申报人")]
        Applicant = 2,
    }

    /// <summary>
    /// 进出口标志
    /// </summary>
    public enum IEFlag
    {
        [MultiDescription("I", "进口")]
        InPort,

        [MultiDescription("E", "出口")]
        Export
    }

    /// <summary>
    /// 拆箱标识
    /// </summary>
    public enum LclFlag
    {
        [Description("否")]
        No = 0,

        [Description("是")]
        Yes = 1
    }

    /// <summary>
    /// 操作类型
    /// </summary>
    public enum OperType
    {
        [MultiDescription("A", "报关单上载")]
        DeclarationUpload,

        [MultiDescription("B", "报关单、转关单上载")]
        DeclarationTransferUpload,

        [MultiDescription("C", "报关单申报")]
        CustomsDeclaration,

        [MultiDescription("D", "报关单、转关单申报")]
        CustomsDeclarationTransfer,

        [MultiDescription("E", "电子手册报关单上载")]
        ManualDeclarationUpload,

        [MultiDescription("G", "报关单暂存（转关单提前报关单暂存)")]
        TemporaryStorage
    }

    /// <summary>
    /// 原集装箱标识（入境原集装箱装载直接到目的机构）
    /// </summary>
    public enum OrigBoxFlag
    {
        [Description("否")]
        No = 0,

        [Description("是")]
        Yes = 1
    }

    /// <summary>
    /// 税收征管标记
    /// </summary>
    public enum TaxAaminMark
    {
        [Description("无")]
        No = 0,

        [Description("有")]
        Yes = 1
    }

    /// <summary>
    /// 转关类型
    /// </summary>
    public enum TransFlag
    {
        [MultiDescription("1", "转关提前")]
        TransferAdvance,

        [MultiDescription("AA", "出口二次转关")]
        SecondaryExportTransfer,

        [MultiDescription("1G", "提前/工厂验放")]
        AdvanceOrFactoryInspection,

        [MultiDescription("1T", "提前/暂时进出口")]
        AdvanceOrTemporarily,

        [MultiDescription("1E", "提前/中欧班列")]
        AdvanceOrCentralEurope,

        [MultiDescription("1P", "提前/市场采购出口")]
        AdvanceOrMarketPurchasingAndExport,

        [MultiDescription("1K", "提前/出口空运联程")]
        AdvanceOrExportAirLink
    }

    /// <summary>
    /// 转关单类型
    /// </summary>
    public enum TrnType
    {
        [Description("普通有纸申报")]
        Normal = 0,

        [Description("无纸申报")]
        PaperLess = 1
    }

    /// <summary>
    /// 报关单-单据类型
    /// </summary>
    public enum DeclarationType
    {
        [MultiDescription("", "一般报关")]
        Normal,

        [MultiDescription("ML", "保税区进出境备案清单")]
        TypeML,

        [MultiDescription("SD", "“属地申报，口岸验放”报关单")]
        TypeSD,

        [MultiDescription("LY", "两单一审备案清单")]
        TypeLY,

        [MultiDescription("CL", "汇总征税报关单")]
        TypeCL,

        [MultiDescription("SS", "”属地申报，属地验放”报关单")]
        TypeSS,

        [MultiDescription("ZB", "自主报税")]
        TypeZB,

        [MultiDescription("SW", "税单无纸化")]
        TypeSW,

        [MultiDescription("SZ", "水运中转普通报关单")]
        TypeSZ,

        [MultiDescription("SM", "水运中转保税区进出境备案清单")]
        TypeSM,

        [MultiDescription("SL", "水运中转两单一审备案清单")]
        TypeSL,

        [MultiDescription("Z", "自报自缴")]
        TypeZ,

        [MultiDescription("MF", "公路舱单跨境快速通关报关单")]
        TypeMF,

        [MultiDescription("MK", "公路舱单跨境快速通关备案清单")]
        TypeMK,

        [MultiDescription("ZY", "自报自缴，两单一审备案清单")]
        TypeZY,

        [MultiDescription("ZC", "自报自缴，汇总征税报关单")]
        TypeZC,

        [MultiDescription("ZW", "自报自缴，税单无纸化")]
        TypeZW,

        [MultiDescription("ZF", "自报自缴，公路舱单跨境快速通关")]
        TypeZF,

        [MultiDescription("ZX", "自报自缴，多项式联运")]
        TypeZX,
    }

    /// <summary>
    /// 报关单状态（包含回执）
    /// </summary>
    public enum CusDecStatus
    {
        [MultiDescription("01", "草稿")]
        Draft,
        [MultiDescription("02", "已制单")]
        Make,
        [MultiDescription("03", "已申报")]
        Declare,
        [MultiDescription("04", "已取消")]
        Cancel,
        [MultiDescription("05", "待复核")]
        PendingDoubleCheck,
        [MultiDescription("06", "已复核")]
        DoubleChecked,
        [MultiDescription("7", "申报成功")]
        Succeed,
        [MultiDescription("a", "签证，已被检务收单，领取证单")]
        a,
        [MultiDescription("A", "海关放行前删除或者异常处理（手工申报通知）")]
        A,
        [MultiDescription("b", "已发检验检疫审核")]
        b,
        [MultiDescription("B", "担保放行")]
        B,
        [MultiDescription("C", "出口查验通知")]
        C,
        [MultiDescription("c", "检验检疫撤单")]
        c,
        [MultiDescription("D", "海关删单")]
        D,
        [MultiDescription("d", "申请检验检疫成功")]
        d,
        [MultiDescription("E", "海关退单/不受理回执")]
        E,
        [MultiDescription("F", "放行交单")]
        F,
        [MultiDescription("G", "报关单已审结")]
        G,
        [MultiDescription("H", "海关挂起，需手工申报")]
        H,
        [MultiDescription("I", "海关无纸放行通知（放行）")]
        I,
        [MultiDescription("J", "通关无纸化审结回执")]
        J,
        [MultiDescription("K", "通关无纸化担保放行")]
        K,
        [MultiDescription("L", "海关已接收")]
        L,
        [MultiDescription("M", "报关单重审")]
        M,
        [MultiDescription("N", "重传文件")]
        N,
        [MultiDescription("P", "海关已放行")]
        P,
        [MultiDescription("R", "已结关")]
        R,
        [MultiDescription("S", "施检")]
        S,
        [MultiDescription("T", "需交税费")]
        T,
        [MultiDescription("W", "海关无纸验放通知（审结）")]
        W,
        [MultiDescription("X", "海关准予进港回执（上海洋山保税港区专用）")]
        X,
        [MultiDescription("Y", "申报失败")]
        Y,
        [MultiDescription("Z", "退回修改")]
        Z,
        [MultiDescription("f0", "客户端错误")]
        f0,
        [MultiDescription("F1", "暂存失败")]
        F1,
        [MultiDescription("S0", "暂存成功")]
        S0,
        [MultiDescription("E0", "表格已申报")]
        E0,
        [MultiDescription("E1", "表格申报成功")]
        E1,
        //以下属于两步申报
        [MultiDescription("SpL", "概要申报海关入库")]
        SpL,
        [MultiDescription("SpE", "概要申报海关退单")]
        SpE,
        [MultiDescription("SpJ", "概要申报海关审结")]
        SpJ,
        [MultiDescription("Sp1", "要求补充申报通知")]
        Sp1,
        [MultiDescription("SpC", "查验通知")]
        SpC,
        [MultiDescription("SpK", "提货放行")]
        SpK
    }

    /// <summary>
    /// 报关单项状态
    /// </summary>
    public enum CusItemDecStatus
    {
        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Cancel = 0,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 1,

    }

    /// <summary>
    /// 舱单/回执状态
    /// </summary>
    public enum CusMftStatus
    {
        /// <summary>
        /// xdt自定义拓展状态
        /// </summary>
        [MultiDescription("xdt01", "草稿")]
        Draft,
        [MultiDescription("xdt02", "已制单")]
        Make,
        [MultiDescription("xdt03", "已申报")]
        Declare,
        [MultiDescription("xdt04", "已取消")]
        Cancel,
        [MultiDescription("xdt05", "已复核")]
        DoubleChecked,
        [MultiDescription("xdt99", "错误")]
        Error,
        [MultiDescription("01", "海关接受申报")]
        Receipt,
        [MultiDescription("02", "传输待人工审核")]
        WaitAudit,
        [MultiDescription("03", "不允许传输，退单")]
        Refuse,
        [MultiDescription("11", "提运单放行")]
        Pass,
        [MultiDescription("12", "提（运）单或集装箱（器）拒装")]
        RefuseLoad,
        [MultiDescription("13", "提（运）单或集装箱（器）禁卸")]
        RefuseUninstall,

        [MultiDescription("0", "调用成功")]
        CallSuccess,
        [MultiDescription("E3", "发往海关成功")]
        TransCustomsSucceed,
        [MultiDescription("D0", "删除中")]
        Deleting,
        [MultiDescription("D1", "已删除")]
        Deleted,

    }

    /// <summary>
    /// 报关员（制单员、发单员）候选类型
    /// </summary>
    public enum DeclarantCandidateType
    {
        /// <summary>
        /// 可选的
        /// </summary>
        [Description("可选的")]
        Optional = 0,

        /// <summary>
        /// 制单员
        /// </summary>
        [Description("制单员")]
        DeclareCreator = 1,

        /// <summary>
        /// 发单员
        /// </summary>
        [Description("发单员")]
        CustomSubmiter = 2,

        /// <summary>
        /// 核对人
        /// </summary>
        [Description("核对人")]
        Checker = 3,

        /// <summary>
        /// 报关单复核人
        /// </summary>
        [Description("报关单复核员")]
        DoubleChecker = 4,

        /// <summary>
        /// 舱单复核人
        /// </summary>
        [Description("舱单复核员")]
        ManifestDoubleChecker = 5,
    }

    /// <summary>
    /// 境外发货人/供应商
    /// </summary>
    public enum ConsignorCode
    {
        /// <summary>
        /// 香港畅运国际物流有限公司
        /// </summary>
        [MultiDescription("CY", "香港畅运国际物流有限公司")]
        CY,

        /// <summary>
        /// 香港万路通国际物流有限公司
        /// </summary>
        [MultiDescription("WLT", "香港万路通国际物流有限公司")]
        WLT,

    }

}