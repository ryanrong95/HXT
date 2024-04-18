using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Enums
{
    /// <summary>
    /// 香港交货方式
    /// </summary>
    public enum HKDeliveryType
    {
        [Description("送货")]
        SentToHKWarehouse = 1,

        [Description("自提")]
        PickUp = 2,

        [Description("快递")]
        LocalExpress = 3,

        [Description("国际快递")]
        InternationalExpress = 4,
    }

    /// <summary>
    /// 深圳交货方式 
    /// </summary>
    public enum SZDeliveryType
    {
        [Description("自提")]
        PickUpInStore = 1,

        [Description("送货")]
        SentToClient = 2,

        [Description("代发货")]
        Shipping = 3
    }

    /// <summary>
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 内单
        /// </summary>
        [Description("A类")]
        Inside = 100,

        /// <summary>
        /// 外单
        /// </summary>
        [Description("B类")]
        Outside = 200,

        /// <summary>
        /// Icgoo
        /// </summary>
        [Description("Icgoo")]
        Icgoo = 300
    }

    /// <summary>
    /// 代理订单状态
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// 草稿
        /// </summary>
        [Description("草稿")]
        Draft = 0,

        /// <summary>
        /// 待归类（已下单）
        /// </summary>
        [Description("待归类")]
        Confirmed = 1,

        /// <summary>
        /// 待报价（已归类）
        /// </summary>
        [Description("待报价")]
        Classified = 2,

        /// <summary>
        /// 待客户确认（已报价）
        /// </summary>
        [Description("待客户确认")]
        Quoted = 3,

        /// <summary>
        /// 待报关（已客户确认）
        /// </summary>
        [Description("待报关")]
        QuoteConfirmed = 4,

        /// <summary>
        /// 待出库（已报关）
        /// </summary>
        [Description("待出库")]
        Declared = 5,

        /// <summary>
        /// 待收货（已出库）
        /// </summary>
        [Description("待收货")]
        WarehouseExited = 6,

        /// <summary>
        /// 完成
        /// </summary>
        [Description("完成")]
        Completed = 7,

        /// <summary>
        /// 已退回
        /// </summary>
        [Description("已退回")]
        Returned = 8,

        /// <summary>
        /// 已取消
        /// </summary>
        [Description("已取消")]
        Canceled = 9
    }

    /// <summary>
    /// 代理订单轨迹
    /// </summary>
    public enum OrderTraceStep
    {
        [Description("已下单")]
        Submitted = 1,

        [Description("订单处理中")]
        Processing = 2,

        [Description("香港仓库处理中")]
        HKProcessing = 3,

        [Description(" 报关中")]
        Declaring = 4,

        [Description("运输中")]
        InTransit = 5,

        [Description("深圳库房处理中")]
        SZProcessing = 6,

        [Description("派送中")]
        Delivering = 7,

        [Description("已提货")]
        PickUp = 8,

        [Description("订单已完成")]
        Completed = 9,

        [Description("订单异常")]
        Anomaly = 10,
    }

    /// <summary>
    /// 订单附件的审核状态
    /// </summary>
    public enum OrderFileStatus
    {
        /// <summary>
        /// 未上传
        /// </summary>
        [Description("未上传")]
        NotUpload = 0,

        /// <summary>
        /// 待审核
        /// </summary>
        [Description("待审核")]
        Auditing = 1,

        /// <summary>
        /// 已审核
        /// </summary>
        [Description("已审核")]
        Audited = 2,

        ///// <summary>
        ///// 待审核
        ///// </summary>
        //[Description("待审核")]
        //NewAuditing = 100,

        ///// <summary>
        ///// 已审核
        ///// </summary>
        //[Description("已审核")]
        //NewAudited = 300,
       
        ///// </summary>
        ///// <summary>
        ///// 中心订单状态 正常
        ///// </summary>
        //[Description("正常")]
        //Normal = 200,

        ///// <summary>
        ///// 删除
        ///// </summary>
        //[Description("删除")]
        //Delete = 400
    }

    /// <summary>
    /// 订单附加费用类型
    /// </summary>
    public enum OrderPremiumType
    {
        /// <summary>
        /// 代理费
        /// </summary>
        [Description("代理费")]
        AgencyFee,

        /// <summary>
        /// 商检费
        /// </summary>
        [Description("商检费")]
        InspectionFee,

        /// <summary>
        /// 送货费
        /// </summary>
        [Description("送货费")]
        DeliveryFee,

        /// <summary>
        /// 快递费
        /// </summary>
        [Description("快递费")]
        ExpressFee,

        /// <summary>
        /// 清关费
        /// </summary>
        [Description("清关费")]
        CustomClearanceFee,

        /// <summary>
        /// 提货费
        /// </summary>
        [Description("提货费")]
        PickUpFee,

        /// <summary>
        /// 停车费
        /// </summary>
        [Description("停车费")]
        ParkingFee,

        /// <summary>
        /// 入仓费
        /// </summary>
        [Description("入仓费")]
        EntryFee,

        /// <summary>
        /// 仓储费
        /// </summary>
        [Description("仓储费")]
        StorageFee,

        /// <summary>
        /// 收货异常费用
        /// </summary>
        [Description("收货异常费用")]
        UnNormalFee,

        /// <summary>
        /// 其他(的杂费)
        /// </summary>
        [Description("其他")]
        OtherFee
    }

    /// <summary>
    /// 订单费用(客户)付款状态
    /// </summary>
    public enum OrderPremiumStatus
    {
        /// <summary>
        /// 未付款
        /// </summary>
        [Description("未付款")]
        UnPay = 0,

        /// <summary>
        /// 部分付款
        /// </summary>
        [Description("部分付款")]
        PartPaid = 1,

        /// <summary>
        /// 已付款
        /// </summary>
        [Description("已付款")]
        Paid = 2
    }

    /// <summary>
    /// 订单管控类型、订单挂起原因
    /// 备注：因这个将来需要对外作为产品归类结果的代码提供出去，所以设计成100、200...
    /// </summary>
    public enum OrderControlType
    {
        /// <summary>
        /// 3C
        /// </summary>
        [Description("3C")]
        CCC = 100,

        /// <summary>
        /// 禁运
        /// </summary>
        [Description("禁运")]
        Forbid = 200,

        /// <summary>
        /// 原产地证明
        /// </summary>
        [Description("原产地证明")]
        OriginCertificate = 300,

        /// <summary>
        /// 超出垫款上限
        /// </summary>
        [Description("超出垫款上限")]
        ExceedLimit = 400,

        /// <summary>
        /// 归类异常
        /// </summary>
        [Description("归类异常")]
        ClassifyAnomaly = 500,

        /// <summary>
        /// 分拣异常
        /// </summary>
        [Description("分拣异常")]
        SortingAbnomaly = 600,

        /// <summary>
        /// 抽检异常
        /// </summary>
        [Description("抽检异常")]
        CheckingAbnomaly = 700,

        /// <summary>
        /// 产地变更
        /// </summary>
        [Description("产地变更")]
        OriginChange = 800,

        /// <summary>
        /// 删除型号
        /// </summary>
        [Description("删除型号")]
        DeleteModel = 900,

        /// <summary>
        /// 修改数量
        /// </summary>
        [Description("修改数量")]
        ChangeQuantity = 1000,

        /// <summary>
        /// (修改汇率引发的)重新生成对账单审批
        /// </summary>
        [Description("重新生成对账单")]
        GenerateBillApproval = 1100,

        /// <summary>
        /// 删除型号审批
        /// </summary>
        [Description("删除型号审批")]
        DeleteModelApproval = 1200,

        /// <summary>
        /// 修改数量审批
        /// </summary>
        [Description("修改数量审批")]
        ChangeQuantityApproval = 1300,

        /// <summary>
        /// 拆分订单审批
        /// </summary>
        [Description("拆分订单审批")]
        SplitOrderApproval = 1400,

        /// <summary>
        /// 垫款超期
        /// </summary>
        [Description("垫款超期")]
        OverdueAdvancePayment = 1500,
    }

    /// <summary>
    /// 订单管控状态
    /// </summary>
    public enum OrderControlStatus
    {
        /// <summary>
        /// 当前管控步骤待审核
        /// </summary>
        [Description("待审批")]
        Auditing = 100,

        /// <summary>
        /// 当前管控步骤审批通过
        /// </summary>
        [Description("通过")]
        Approved = 200,

        /// <summary>
        /// 当前管控步骤审批未通过
        /// </summary>
        [Description("拒绝")]
        Rejected = 300,

        /// <summary>
        /// 当前管控可以转第三方报关
        /// </summary>
        [Description("转第三方")]
        Turned = 400,

        /// <summary>
        /// 撤销
        /// </summary>
        [Description("撤销")]
        Cancel = 500,
    }

    /// <summary>
    /// 订单审核步骤/审核层级
    /// </summary>
    public enum OrderControlStep
    {
        /// <summary>
        /// 北京总部审核
        /// </summary>
        [Description("总部")]
        Headquarters = 1,

        /// <summary>
        /// 客户跟单员审核
        /// </summary>
        [Description("跟单员")]
        Merchandiser = 2,

        /// <summary>
        /// 客户
        /// </summary>
        [Description("客户")]
        Client = 3,
    }

    /// <summary>
    /// 审批类型
    /// </summary>
    public enum ApprovalType
    {
        /// <summary>
        /// (修改汇率引发的)重新生成对账单审批
        /// </summary>
        [Description("重新生成对账单")]
        GenerateBillApproval = 11,

        /// <summary>
        /// 删除型号审批
        /// </summary>
        [Description("删除型号审批")]
        DeleteModelApproval = 12,

        /// <summary>
        /// 修改数量审批
        /// </summary>
        [Description("修改数量审批")]
        ChangeQuantityApproval = 13,

        /// <summary>
        /// 拆分订单审批
        /// </summary>
        [Description("拆分订单审批")]
        SplitOrderApproval = 14,
    }

    /// <summary>
    /// 订单管控需要上传的文件类型
    /// </summary>
    [Obsolete("文件类型引用FileType枚举类")]
    public enum OrderControlFileType
    {
        /// <summary>
        /// 3C认证资料
        /// </summary>
        [Description("3C认证资料")]
        CCC = 1,

        /// <summary>
        /// 原产地证明
        /// </summary>
        [Description("原产地证明")]
        OriginCertificate = 2,
    }

    /// <summary>
    /// 订单产品项申报状态
    /// 1）DeclaredQuantity == null， 表示该订单项未申报
    /// 2）DeclaredQuantity < Quantity, 表示该订单项已经部分申报
    /// 3）DeclaredQuantity  == Quantity， 表示该订单项已申报
    /// </summary>
    public enum ProductDeclareStatus
    {
        /// <summary>
        /// 未申报
        /// </summary>
        [Description("未申报")]
        UnDeclare = 1,

        /// <summary>
        /// 部分申报
        /// </summary>
        [Description("部分申报")]
        PartDeclare = 2,

        /// <summary>
        /// 已申报
        /// </summary>
        [Description("已申报")]
        AllDeclare = 3
    }

    public enum ClientOrderType
    {
        /// <summary>
        /// 仓储收货
        /// </summary>
        [Description("仓储收货")]
        Recieved = 1,
        /// <summary>
        /// 即收即发
        /// </summary>
        [Description("即收即发")]
        Transport = 2,
        /// <summary>
        /// 代发货
        /// </summary>
        [Description("代发货")]
        Delivery = 3,
        /// <summary>
        /// 转报关
        /// </summary>
        [Description("转报关")]
        TransferDeclare = 4,
        /// <summary>
        /// 代报关
        /// </summary>
        [Description("代报关")]
        Declare = 5,
    }

}