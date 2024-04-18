using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Enums
{
    public enum FileType
    {
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
        /// 3C认证资料
        /// </summary>
        [Description("3C认证资料")]
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
        /// 客户收货确认单
        /// </summary>
        [Description("客户收货确认单")]
        ReceiptConfirmationFile = 23,

        /// <summary>
        /// 报关单消费税发票
        /// </summary>
        [Description("报关单消费税发票")]
        ConsumptionTaxFile = 24,
    }
}
