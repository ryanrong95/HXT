using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Underly.Attributes;

namespace Yahv.Underly
{
    /// <summary>
    /// 通知类型
    /// </summary>
    public enum LogNoticeType
    {
        #region 报关部分

        /// <summary>
        /// 归类完成，等待报价
        /// </summary>
        [Description("归类完成，等待报价")]
        ClassifyDone = 101,

        /// <summary>
        /// 报关委托书待审核
        /// </summary>
        [Description("报关委托书待审核")]
        AgentProxyUploaded = 102,

        /// <summary>
        /// 对账单待审核
        /// </summary>
        [Description("对账单待审核")]
        OrderBillUploaded = 103,

        /// <summary>
        /// 库房费用待审批
        /// </summary>
        [Description("库房费用待审批")]
        WarehouseFee = 104,

        /// <summary>
        /// 报关通知，等待制单
        /// </summary>
        [Description("报关通知，等待制单")]
        DecNoticePending = 105,

        /// <summary>
        /// 税费异常，等待处理
        /// </summary>
        [Description("税费异常，等待处理")]
        TaxError = 106,

        /// <summary>
        /// 付汇申请，待审核
        /// </summary>
        [Description("付汇申请，待审核")]
        PayExchangeAudit = 107,

        /// <summary>
        /// 付汇申请，待审批
        /// </summary>
        [Description("付汇申请，待审批")]
        PayExChangeApprove = 108,

        /// <summary>
        /// 开票申请，待开票
        /// </summary>
        [Description("开票申请，待开票")]
        InvoicePending = 109,

        /// <summary>
        /// 收款通知，等待处理
        /// </summary>
        [Description("收款通知，等待处理")]
        ReceivingPending = 110,

        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 111,

        /// <summary>
        /// 禁运
        /// </summary>
        [Description("禁运")]
        Forbid = 112,

        /// <summary>
        /// 原产地证明
        /// </summary>
        [Description("原产地证明")]
        OriginCertificate = 113,

        /// <summary>
        /// 超出垫款上限
        /// </summary>
        [Description("超出垫款上限")]
        ExceedLimit = 114,

        /// <summary>
        /// 归类异常
        /// </summary>
        [Description("归类异常")]
        ClassifyAnomaly = 115,

        /// <summary>
        /// 分拣异常
        /// </summary>
        [Description("分拣异常")]
        SortingAbnomaly = 116,

        /// <summary>
        /// 抽检异常
        /// </summary>
        [Description("抽检异常")]
        CheckingAbnomaly = 117,

        /// <summary>
        /// 产地变更
        /// </summary>
        [Description("产地变更")]
        OriginChange = 118,

        /// <summary>
        /// 删除型号
        /// </summary>
        [Description("删除型号")]
        DeleteModel = 119,

        /// <summary>
        /// 修改数量
        /// </summary>
        [Description("修改数量")]
        ChangeQuantity = 120,

        /// <summary>
        /// HQ3C
        /// </summary>
        [Description("3C")]
        HQCCC = 121,

        /// <summary>
        /// 产地变更
        /// </summary>
        [Description("型号变更")]
        ModelChange = 122,

        /// <summary>
        /// 品牌变更
        /// </summary>
        [Description("型号变更")]
        ManufactureChange = 123,

        /// <summary>
        /// 总部认为是禁运，跟单退单
        /// </summary>
        [Description("禁运，退单处理")]
        ForbidRejected = 124,

        /// <summary>
        /// 张庆永付汇申请审批结束，通知财务付款
        /// </summary>
        [Description("付汇申请结束，付款")]
        PayPayExchange = 125,

        #endregion

        //代仓储统一从1000开始

        #region 库房部分

        /// <summary>
        /// 香港到货异常
        /// </summary>
        /// <remarks>
        /// 通知给：跟单员
        /// </remarks>
        [Description("香港到货异常")]
        HKDeliveryError = 1100,
        /// <summary>
        /// 香港发货通知跟单维护深圳库房出库
        /// </summary>
        /// <remarks>
        /// 通知给：跟单员
        /// </remarks>
        [Description("香港发货通知跟单维护深圳库房出库")]
        HKDeliveryToTracker = 1101,
        /// <summary>
        /// 香港装完箱
        /// </summary>
        /// <remarks>
        /// 通知给：报关员
        /// </remarks>
        [Description("香港装箱")]
        HKBoxed = 1102,
        /// <summary>
        /// 深圳发货
        /// </summary>
        /// <remarks>
        /// 通知给：客户，需要前端提供接口
        /// </remarks>
        [Description("深圳发货")]
        SZShiped = 1103,

        #endregion
    }
}
