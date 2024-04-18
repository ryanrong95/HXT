using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    public enum FileType
    {
        [Description("")]
        None = 0,

        /// <summary>
        /// 对账单
        /// </summary>
        [Description("对账单")]
        OrderBill = 1,

        /// <summary>
        /// 付汇委托书
        /// </summary>
        [Description("付汇委托书")]
        PayExchange = 2,

        /// <summary>
        /// 代报关委托书
        /// </summary>
        [Description("代理报关委托书")]
        AgentTrustInstrument = 3,

        /// <summary>
        /// 原始采购单
        /// </summary>
        [Description("原始采购单")]
        OriginalPR = 4,

        /// <summary>
        /// 合同发票
        /// </summary>
        [Description("合同发票")]
        OriginalInvoice = 5,

        /// <summary>
        /// 原始装箱单
        /// </summary>
        [Description("原始装箱单")]
        OriginalPackingList = 6,

        /// <summary>
        /// 营业执照
        /// </summary>
        [Description("营业执照")]
        BusinessLicense = 7,

        /// <summary>
        /// 授权委托书
        /// </summary>
        [Description("授权委托书")]
        PowerOfAttorney = 8,

        /// <summary>
        /// 暂存图片
        /// </summary>
        [Description("暂存图片")]
        TemporaryPicture = 9,

        /// <summary>
        /// 提货文件
        /// </summary>
        [Description("提货文件")]
        DeliveryFiles = 10,

        /// <summary>
        /// 付汇PI文件
        /// </summary>
        [Description("付汇PI文件")]
        PIFiles = 11,

        /// <summary>
        /// 3C证书
        /// </summary>
        [Description("3C证书")]
        CCC = 12,

        /// <summary>
        /// 原产地证明
        /// </summary>
        [Description("原产地证明")]
        OriginCertificate = 13,

        /// <summary>
        /// 仓库费用附件
        /// </summary>
        [Description("仓库费用附件")]
        WarehousFeeFile = 14,

        /// <summary>
        /// 订单费用附件
        /// </summary>
        [Description("订单费用附件")]
        OrderFeeFile = 15,

        /// <summary>
        /// 报关单文件
        /// </summary>
        [Description("报关单文件")]
        DecHeadFile = 16,

        /// <summary>
        /// 报关单进口关税发票
        /// </summary>
        [Description("报关单关税发票")]
        DecHeadTariffFile = 17,

        /// <summary>
        /// 报关单进口增值税发票
        /// </summary>
        [Description("报关单增值税发票")]
        DecHeadVatFile = 18,

        /// <summary>
        /// 付款凭证
        /// </summary>
        [Description("付款凭证")]
        PaymentVoucher = 19,

        /// <summary>
        /// 提货委托书
        /// </summary>
        [Description("提货委托书")]
        DeliveryAgentFile = 20,

        /// <summary>
        /// 六联单
        /// </summary>
        [Description("六联单")]
        LiuLianDan = 21,

        /// <summary>
        /// 服务协议
        /// </summary>
        [Description("服务协议")]
        ServiceAgreement = 22,
        /// <summary>
        /// 代仓储协议
        /// </summary>
        [Description("代仓储协议")]
        StorageAgreement = 70,

        /// <summary>
        /// 客户收货确认单
        /// </summary>
        [Description("客户收货确认单")]
        ReceiptConfirmationFile = 23,

        /// <summary>
        /// 标签文件
        /// </summary>
        [Description("标签文件")]
        Label = 24,

        /// <summary>
        /// 随货文件
        /// </summary>
        [Description("随货文件")]
        FollowGoods = 25,

        /// <summary>
        /// 装箱单
        /// </summary>
        [Description("装箱单")]
        PackingList = 30,

        /// <summary>
        /// 报关单进口消费税发票
        /// </summary>
        [Description("报关单消费税发票")]
        DecHeadExciseTaxFile = 40,

        /// <summary>
        /// 用户头像
        /// </summary>
        [Description("用户头像")]
        Avatar = 42,

        /// <summary>
        /// 芯达通境内销售合同（增值税开票时，税务要求）
        /// </summary>
        [Description("销售合同")]
        SalesContract = 50,

        /// <summary>
        /// 3C目录外鉴定结果单
        /// </summary>
        [Description("3C目录外鉴定结果单")]
        AppraiseReuslt = 51,

        /// <summary>
        /// 未列明单据1
        /// </summary>
        [Description("未列明单据1")]
        Unlist1 = 52,

        /// <summary>
        /// 未列明单据2
        /// </summary>
        [Description("未列明单据2")]
        Unlist2 = 53,

        /// <summary>
        /// 未列明单据3
        /// </summary>
        [Description("未列明单据3")]
        Unlist3 = 54,

        [Description("库存图片")]
        StoragesPic = 8000,

        [Description("发货文件")]
        DeliverGoods = 8010,

        [Description("送货确认文件")]
        SendGoods = 8020,

        [Description("登记证")]
        HKBusinessLicense = 60,

        /// <summary>
        /// 变更服务协议
        /// </summary>
        [Description("变更服务协议")]
        ChangeServiceAgreement = 222,

        /// <summary>
        /// 芯达通垫款保证协议
        /// </summary>
        [Description("芯达通垫款保证协议")]
        AdvanceMoneyApplyAgreement = 223,

        /// <summary>
        /// 签署视频
        /// </summary>
        [Description("签署视频")]
        SignVideo = 301,

        /// <summary>
        /// 照片
        /// </summary>
        [Description("照片")]
        Photos = 302,

        /// <summary>
        /// 报关协议
        /// </summary>
        [Description("报关协议")]
        DeclareAgreement = 303,

        /// <summary>
        /// 补充协议
        /// </summary>
        [Description("补充协议")]
        SupplementAgreement = 304,

        [Description("评估资料")]
        EvaluationInfo = 305,

        [Description("付汇承诺书")]
        PECommitment = 310,

        [Description("担保文件")]
        SecurityDoc = 315,

        [Description("其它")]
        Others = 320,
    }
}
