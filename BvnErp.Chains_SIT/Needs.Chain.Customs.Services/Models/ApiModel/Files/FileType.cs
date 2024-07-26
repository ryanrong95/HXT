using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models.ApiModels.Files
{
    /// <summary>
    /// 文件类型
    /// </summary>
    /// <remarks>
    /// 如果华芯通已经存在使用华芯通的数值
    /// 再增加的时候 从这里9000开始
    /// </remarks>
    public enum FileType : int
    {
        [Description("")]
        None = 0,

        #region 与华芯通文件类型值保持一致，勿改
        /// <summary>
        /// 营业执照
        /// </summary>
        [Description("营业执照")]
        BusinessLicense = 7,
        /// <summary>
        /// 香港营业执照
        /// </summary>
        [Description("香港营业执照")]
        HKBusinessLicense = 60,
        /// <summary>
        /// 授权委托书
        /// </summary>
        [Description("授权委托书")]
        PowerOfAttorney = 8,

        #endregion
        [Description("对账单")]
        OrderBill = 1,

        [Description("合同发票(PI)")]
        Invoice = 5,

        [Description("提货文件")]
        Delivery = 10,

        [Description("订单费用附件")]
        OrderFeeFile = 15,

        [Description("标签文件")]
        Label = 24,

        /// <summary>
        /// 付汇委托书
        /// </summary>
        [Description("付汇委托书")]
        PayExchange = 2,
        /// <summary>
        /// 付汇PI文件
        /// </summary>
        [Description("付汇PI文件")]
        PIFiles = 11,
        /// <summary>
        /// 代报关委托书
        /// </summary>
        [Description("代理报关委托书")]
        AgentTrustInstrument = 3,

        /// <summary>
        /// 服务协议
        /// </summary>
        [Description("服务协议")]
        ServiceAgreement = 22,

        [Description("随货文件")]
        FollowGoods = 25,

        [Description("代收货款委托书")]
        ReceiveEntrust = 26,

        [Description("代付货款委托书")]
        PaymentEntrust = 27,

        [Description("租赁合同")]
        Contract = 28,

        [Description("库存图片")]
        StoragesPic = 8000,

        [Description("发货文件")]
        DeliverGoods = 8010,

        [Description("送货确认文件")]
        SendGoods = 8020,

        [Description("报关委托书")]
        Declaration = 29,
        /// <summary>
        /// 代仓储协议
        /// </summary>
        [Description("代仓储协议")]
        StorageAgreement = 70,

        [Description("企业Logo")]
        EnterpriseLogo = 41,

        /// <summary>
        /// 华芯通境内销售合同（增值税开票时，税务要求）
        /// </summary>
        [Description("销售合同")]
        SalesContract = 50,


        //如果华芯通已经存在使用华芯通的数值
        //再增加的时候 从这里 9000 开始

        [Description("测试文件")]
        Test = 9000,

        [Description("产品导入文件")]
        ProductImportFile = 100001,

        /// <summary>
        /// 变更服务协议
        /// </summary>
        [Description("变更服务协议")]
        ChangeServiceAgreement = 222,

        /// <summary>
        /// 华芯通垫款保证协议
        /// </summary>
        [Description("华芯通垫款保证协议")]
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
    }
}
