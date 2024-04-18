using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebApi.Models.ReceptorOrder
{
    public class ReceptorOrder
    {
        public ReceptorOrder()
        {
            Type = Needs.Ccs.Services.Enums.OrderType.Outside;
            InvoiceStatus = Needs.Ccs.Services.Enums.InvoiceStatus.UnInvoiced;
            PaidExchangeAmount = 0;
            IsHangUp = false;
            Status = Needs.Ccs.Services.Enums.Status.Normal;
            UpdateDate = CreateDate = DateTime.Now;
            OrderBillType = Needs.Ccs.Services.Enums.OrderBillType.Normal;
        }

        public string ID { get; set; }
        
        /// <summary>
        /// 订单类型：内单、外单、Icgoo
        /// </summary>
        public Needs.Ccs.Services.Enums.OrderType Type { get; set; }

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

        /// <summary>
        /// 订单的开票状态
        /// </summary>
        public Needs.Ccs.Services.Enums.InvoiceStatus InvoiceStatus { get; set; }

        /// <summary>
        /// 已申请付汇金额
        /// </summary>
        public decimal PaidExchangeAmount { get; set; }

        /// <summary>
        /// 是否挂起：Yes/No
        /// </summary>
        public bool IsHangUp { get; set; }

        /// <summary>
        /// 是否退回重新提交订单？
        /// </summary>
        public bool IsReturned { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Needs.Ccs.Services.Enums.Status Status { get; set; }

        /// <summary>
        /// 代理费收取方式
        /// </summary>
        public Needs.Ccs.Services.Enums.OrderBillType OrderBillType { get; set; }

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
       
        /// <summary>
        /// 付汇委托书，对账单，PI
        /// </summary>
        //public List<MainOrderFiles> MainOrderFiles { get; set; }

    }

    public class OrderItems
    {
        /// <summary>
        /// 订单项ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 小订单ID，订单退回客户编辑后，提交时赋值
        /// </summary>
        public string TinyOrderID { get; set; }

        /// <summary>
        /// 供前端快捷下单使用
        /// 正常下单情况，禁止使用
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

        /// <summary>
        /// 对于四级客户，报价时需要选择是否抽检
        /// </summary>
        public bool IsSampllingCheck { get; set; }

        /// <summary>
        /// 归类状态：未归类、首次归类、归类完成，归类异常
        /// </summary>
        public Needs.Ccs.Services.Enums.ClassifyStatus ClassifyStatus { get; set; }

        /// <summary>
        /// 产品唯一编码
        /// </summary>
        public string ProductUniqueCode { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Needs.Ccs.Services.Enums.Status Status { get; set; }

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
        public Needs.Ccs.Services.Enums.HKDeliveryType Type { get; set; }

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
        public Needs.Ccs.Services.Enums.SZDeliveryType Type { get; set; }

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

    //public class MainOrderFiles
    //{
    //    /// <summary>
    //    /// 文件名称
    //    /// </summary>
    //    public string Name { get; set; }

    //    /// <summary>
    //    /// 文件格式
    //    /// </summary>
    //    public string FileFormat { get; set; }

    //    /// <summary>
    //    /// 文件地址
    //    /// </summary>
    //    public string Url { get; set; }

    //    /// <summary>
    //    /// 文件类型
    //    /// </summary>
    //    public Needs.Ccs.Services.Enums.FileType FileType { get; set; }
    //}
}