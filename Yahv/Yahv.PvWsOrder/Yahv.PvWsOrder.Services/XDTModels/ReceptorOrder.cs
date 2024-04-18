using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.XDTClientView;
using Yahv.Underly.Attributes;

namespace Yahv.PvWsOrder.Services.XDTModels
{
    public class ReceptorOrder
    {
        public ReceptorOrder()
        {
        }

        public string ID { get; set; }

        public OrderType Type { get; set; }

        /// <summary>
        /// 下单时的跟单员
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 平台用户
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string ClientName { get; set; }

        /// <summary>
        /// 客户代码（入仓号）
        /// </summary>
        public string ClientCode { get; set; }

        /// <summary>
        /// 交易币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 是否包车：Yes/No
        /// </summary>
        public bool IsFullVehicle { get; set; }

        /// <summary>
        /// 是否垫款：Yes/No
        /// </summary>
        public bool IsLoan { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int? PackNo { get; set; }

        /// <summary>
        /// 包装种类
        /// </summary>
        public string WarpType { get; set; }

        public Underly.OrderInvoiceStatus InvoiceStatus { get; set; }

        public decimal PaidExchangeAmount { get; set; }

        public bool IsHangUp { get; set; }

        /// <summary>
        /// 是否编辑后提交
        /// </summary>
        public bool IsReturned { get; set; }

        public Status Status { get; set; }

        public OrderBillType OrderBillType { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public List<OrderItems> Items { get; set; }

        /// <summary>
        /// 订单的香港接货方式
        /// </summary>
        public OrderConsignee OrderConsignee { get; set; }

        /// <summary>
        /// 订单的深圳交货方式
        /// </summary>
        public OrderConsignor OrderConsignor { get; set; }

        /// <summary>
        /// 付汇供应商
        /// </summary>
        public List<OrderPayExchangeSuppliers> PayExchangeSuppliers { get; set; }
    }

    public class OrderItems
    {
        public OrderItems()
        {
            Name = "";
        }

        /// <summary>
        /// 订单项ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 小订单号
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 预归类产品ID
        /// </summary>
        public string PreProductID { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        public bool IsSampllingCheck { get; set; }

        public ClassifyStatus ClassifyStatus { get; set; }

        /// <summary>
        /// 产品唯一编码
        /// </summary>
        public string ProductUniqueCode { get; set; }

        public int Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public string Batch { get; set; }
    }

    public class OrderConsignee
    {
        //此处根据Name和ClientID取得供应商ID
        public string ClientSupplierName { get; set; }

        /// <summary>
        /// 香港接货方式：供应商送货、自提
        /// </summary>
        public HKDeliveryType Type { get; set; }

        /// <summary>
        /// 联系人, 交货方式为自提时填写
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机号码, 交货方式为自提时填写
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 联系人电话, 交货方式为自提时填写（可空）
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 交货地址, 交货方式为自提时填写
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 提货时间，交货方式为自提时填写
        /// </summary>
        public DateTime? PickUpTime { get; set; }

        /// <summary>
        /// 物流单号，供应商送货时填写
        /// </summary>
        public string WayBillNo { get; set; }
    }

    public class OrderConsignor
    {
        /// <summary>
        /// 订单交货方式：自提、送货上门、快递
        /// </summary>
        public SZDeliveryType Type { get; set; }

        /// <summary>
        /// 收件单位名称（送货上门/快递时填写）
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系人（送货上门/快递时），提货人（客户自提时）
        /// </summary>
        public string Contact { get; set; }

        /// <summary>
        /// 联系人手机号码（送货上门/快递时），提货人手机号码（客户自提时）
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 联系人电话（送货上门/快递时），提货人电话（客户自提时），可空
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 收件地址（送货上门/快递时填写）
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 提货人证件类型：身份证、驾驶证（客户自提时填写）
        /// </summary>
        public string IDType { get; set; }

        /// <summary>
        /// 提货人证件号码（客户自提时填写）
        /// </summary>
        public string IDNumber { get; set; }
    }

    public class OrderPayExchangeSuppliers
    {
        /// <summary>
        /// 付汇供应商
        /// </summary>
        public string ClientSupplierName { get; set; }
    }


    #region 芯达通对应枚举类型

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
    /// 订单类型
    /// </summary>
    public enum OrderType
    {
        /// <summary>
        /// 内单
        /// </summary>
        [Description("内单")]
        Inside = 100,

        /// <summary>
        /// 外单
        /// </summary>
        [Description("外单")]
        Outside = 200,

        /// <summary>
        /// Icgoo
        /// </summary>
        [Description("Icgoo")]
        Icgoo = 300
    }

    /// <summary>
    /// 代理订单开票状态
    /// </summary>
    public enum InvoiceStatus
    {
        [Description("未开票")]
        UnInvoiced = 1,

        [Description("已申请")]
        Applied = 2,

        [Description("已开票")]
        Invoiced = 3
    }

    /// <summary>
    /// 付汇状态
    /// </summary>
    public enum PayExchangeStatus
    {
        [Description("未付汇")]
        UnPay = 1,

        [Description("部分付汇")]
        Partial = 2,

        [Description("已付汇")]
        All = 4,
    }

    /// <summary>
    /// 数据状态
    /// </summary>
    public enum Status
    {
        /// <summary>
        /// 待审批
        /// </summary>
        [Description("待审批")]
        Auditing = 100,

        /// <summary>
        /// 正常
        /// </summary>
        [Description("正常")]
        Normal = 200,

        /// <summary>
        /// 删除
        /// </summary>
        [Description("删除")]
        Delete = 400
    }

    public enum OrderBillType
    {
        /// <summary>
        /// 代理费小于最小代理费，按照最小代理费收，否则是多少就收多少
        /// </summary>
        [Description("正常收取")]
        Normal = 1,

        /// <summary>
        /// 按最小代理费收
        /// </summary>
        [Description("最低代理费")]
        MinAgencyFee = 2,

        /// <summary>
        /// 按固定金额收
        /// </summary>
        [Description("指定代理费")]
        Pointed = 3
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
    /// 香港交货方式
    /// </summary>
    public enum HKDeliveryType
    {
        [Description("送货")]
        SentToHKWarehouse = 1,

        [Description("自提")]
        PickUp = 2
    }

    /// <summary>
    /// 归类状态
    /// </summary>
    public enum ClassifyStatus
    {
        /// <summary>
        /// 未归类
        /// </summary>
        [Description("未归类")]
        Unclassified,

        /// <summary>
        /// 首次归类
        /// </summary>
        [Description("首次归类")]
        First,

        /// <summary>
        /// 归类异常
        /// </summary>
        [Description("归类异常")]
        Anomaly,

        /// <summary>
        /// 归类完成
        /// </summary>
        [Description("归类完成")]
        Done
    }

    public enum IDType
    {
        [Description("身份证")]
        IDCard = 1,

        [Description("驾驶证")]
        IDDriver = 2
    }

    #endregion
}
