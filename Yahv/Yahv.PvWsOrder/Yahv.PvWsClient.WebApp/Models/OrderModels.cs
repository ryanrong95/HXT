using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Underly;

namespace Yahv.PvWsClient.WebApp.Models
{
    /// <summary>
    /// 代仓储新增订单
    /// </summary>
    public class StorageAddModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 发货地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 是否代付货款
        /// </summary>
        public bool IsPrePaid { get; set; }

        /// <summary>
        /// 交货方式
        /// </summary>
        public string WaybillType { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? TakingDate { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public string TakingDateStr { get; set; }

        /// <summary>
        /// 提货地址
        /// </summary>
        public string[] TakingAddress { get; set; }

        /// <summary>
        /// 提货详细地址
        /// </summary>
        public string TakingDetailAddress { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string TakingContact { get; set; }

        /// <summary>
        /// 提货人联系电话
        /// </summary>
        public string TakingPhone { get; set; }

        /// <summary>
        /// 提货文件
        /// </summary>
        public FileModel[] TakingFiles { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public FileModel[] PIFiles { get; set; }

        /// <summary>
        /// 是否垫付运费
        /// </summary>
        public bool IsPayFreight { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public int? PartsTotal { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 子运单号
        /// </summary>
        public string Subcodes { get; set; }

        /// <summary>
        /// 是否拆箱验货
        /// </summary>
        public bool IsUnpacking { get; set; }

        /// <summary>
        /// 支付币种
        /// </summary>
        public string PayCurrency { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        public string SettlementCurrency { get; set; }

        /// <summary>
        /// 是否代检测
        /// </summary>
        public bool IsTesting { get; set; }

        /// <summary>
        /// 提货ID
        /// </summary>
        public string LoadingID { get; set; }

        /// <summary>
        /// 货物条款ID
        /// </summary>
        public string ChargeID { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 航次号
        /// </summary>
        public string VoyageNumber { get; set; }

        ///// <summary>
        ///// 供应商账户
        ///// </summary>
        //public string SupplierBeneficiaryID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }

        ///// <summary>
        ///// 支付方式
        ///// </summary>
        //public string PayType { get; set; }


    }


    /// <summary>
    /// 待仓储订单项
    /// </summary>
    public class OrderItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 归类产品ID
        /// </summary>
        public string PreProcuctID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string OriginLabel { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public int Unit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string UnitLabel { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

        public string InputID { get; set; }

        public string OutputID { get; set; }

    }

    /// <summary>
    /// 文件对象
    /// </summary>
    public class FileModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 文件名称
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 文件后缀
        /// </summary>
        public string fileFormat { get; set; }

        /// <summary>
        /// 文件URL
        /// </summary>
        public string URL { get; set; }

        /// <summary>
        /// 文件的网络地址
        /// </summary>
        public string fullURL { get; set; }

    }

    /// <summary>
    /// 订单详情列表
    /// </summary>
    public class StorageDetailModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单状态
        /// </summary>
        public string MainStatus { get; set; }

        /// <summary>
        /// 发票
        /// </summary>
        public InvoiceViewModel Invoice { get; set; }

        /// <summary>
        /// 创建订单日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 支付币种
        /// </summary>
        public string PayCurrency { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        public string SettlementCurrency { get; set; }

        /// <summary>
        /// 订单总金额
        /// </summary>
        public string TotalMoney { get; set; }

        /// <summary>
        /// 发货地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 是否拆箱验货
        /// </summary>
        public string IsUnpacking { get; set; }

        /// <summary>
        /// 是否代收货款
        /// </summary>
        public string IsBringPay { get; set; }

        /// <summary>
        /// 是否代检测
        /// </summary>
        public string IsTesting { get; set; }

        /// <summary>
        /// 是否代付货款
        /// </summary>
        public string IsPrePaid { get; set; }

        /// <summary>
        /// 交货方式
        /// </summary>
        public string DeliveryType { get; set; }

        /// <summary>
        /// 交货方式
        /// </summary>
        public string DeliveryTypeName { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public string TakingDate { get; set; }

        /// <summary>
        /// 提货详细地址
        /// </summary>
        public string TakingDetailAddress { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string TakingContact { get; set; }

        /// <summary>
        /// 提货人联系电话
        /// </summary>
        public string TakingPhone { get; set; }

        /// <summary>
        /// 是否代付运费
        /// </summary>
        public string IsPayFreight { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public int? PartsTotal { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 子运单号
        /// </summary>
        public string Subcodes { get; set; }

        /// <summary>
        /// 航次号
        /// </summary>
        public string VoyageNumber { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }

        ///// <summary>
        ///// 银行账户
        ///// </summary>
        //public string BeneficiaryName { get; set; }

        ///// <summary>
        ///// 支付方式
        ///// </summary>
        //public string PayTypeName { get; set; }

        public bool IsPrePay { get; set; }

        /// <summary>
        /// 提货文件
        /// </summary>
        public string DeliveryFiles { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public Array PIFiles { get; set; }

        /// <summary>
        /// 随货文件
        /// </summary>
        public Array AccompanyFiles { get; set; }

        /// <summary>
        /// 代收货款委托书
        /// </summary>
        public string ReceiveEntrust { get; set; }

        /// <summary>
        /// 代理报关委托书
        /// </summary>
        public Array DeclareFile { get; set; }

        /// <summary>
        /// 销售总金额
        /// </summary>
        public string TotalSalePrice { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMethod { get; set; }

        /// <summary>
        /// 是否自定义标签
        /// </summary>
        public string IsCustomLabel { get; set; }

        /// <summary>
        /// 是否重新包装
        /// </summary>
        public string IsRepackaging { get; set; }

        /// <summary>
        /// 是否真空包装
        /// </summary>
        public string IsVacuumPackaging { get; set; }

        /// <summary>
        /// 是否防水包装
        /// </summary>
        public string IsWaterproofPackaging { get; set; }

        /// <summary>
        /// 是否代检测
        /// </summary>
        public string IsDetection { get; set; }

        /// <summary>
        /// 是否拆箱验货
        /// </summary>
        public string IsUnBoxed { get; set; }

        /// <summary>
        /// 是否包车
        /// </summary>
        public string IsCharterBus { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ReceivedContact { get; set; }

        /// <summary>
        /// 收货人号码
        /// </summary>
        public string ReceivedPhone { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string ReceivedAddress { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string Zipcode { get; set; }

        /// <summary>
        /// 发货类型
        /// </summary>
        public string DeliveryGoodType { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string Certificate { get; set; }
    }

    /// <summary>
    /// 代发货数据模型
    /// </summary>
    public class DeliveryAddModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 产品项
        /// </summary>
        public DeliveryItem[] DeliveryItems { get; set; }

        /// <summary>
        /// 是否代收货款
        /// </summary>
        public bool IsBringPay { get; set; }

        /// <summary>
        /// 发货类型
        /// </summary>
        public string DeliveryType { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public string PayType { get; set; }

        /// <summary>
        /// 送货方式
        /// </summary>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string TakingContact { get; set; }

        /// <summary>
        /// 提货人联系电话
        /// </summary>
        public string TakingPhone { get; set; }

        /// <summary>
        /// 证件
        /// </summary>
        public string Certificate { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ReceivedContact { get; set; }

        /// <summary>
        /// 收货人联系电话
        /// </summary>
        public string ReceivedPhone { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string[] ReceivedAddress { get; set; }

        /// <summary>
        /// 收货人详细地址
        /// </summary>
        public string ReceivedDetailAddress { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? TakingDate { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public string TakingDateStr { get; set; }

        /// <summary>
        /// 国际发货区域
        /// </summary>
        public string InternationalAddress { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// 快递支付方式
        /// </summary>
        public string ExpressPaymentType { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 支付币种
        /// </summary>
        public string Currency { get; set; }


        /// <summary>
        /// 结算币种
        /// </summary>
        public string SettlementCurrency { get; set; }

        /// <summary>
        /// 委托书
        /// </summary>
        public FileModel[] EntrustFile { get; set; }

        /// <summary>
        /// 合同发票
        /// </summary>
        public FileModel[] PIFile { get; set; }

        /// <summary>
        /// 随货文件
        /// </summary>
        public FileModel[] AccompanyingFile { get; set; }

        /// <summary>
        /// 提货ID
        /// </summary>
        public string LoadingID { get; set; }

        /// <summary>
        /// 货物条款ID
        /// </summary>
        public string ChargeID { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string WaybillID { get; set; }

        /// <summary>
        /// 是否自定义标签
        /// </summary>
        public bool IsCustomLabel { get; set; }

        /// <summary>
        /// 是否重新包装
        /// </summary>
        public bool IsRepackaging { get; set; }

        /// <summary>
        /// 是否真空包装
        /// </summary>
        public bool IsVacuumPackaging { get; set; }

        /// <summary>
        /// 是否防水包装
        /// </summary>
        public bool IsWaterproofPackaging { get; set; }

        /// <summary>
        /// 是否代检测
        /// </summary>
        public bool IsDetection { get; set; }

        /// <summary>
        /// 是否拆箱验货
        /// </summary>
        public bool IsUnBoxed { get; set; }

    }

    /// <summary>
    /// 代发货订单项
    /// </summary>
    public class DeliveryItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string OriginName { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public decimal StockNum { get; set; }

        /// <summary>
        /// 发货数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 销售单价
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// 销售总价
        /// </summary>
        public decimal? SaleTotalPrice { get; set; }

        /// <summary>
        /// InputID
        /// </summary>
        public string InputID { get; set; }

        /// <summary>
        /// OutputID
        /// </summary>
        public string OutputID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 库房编号
        /// </summary>
        public string WareHouseID { get; set; }
    }

    /// <summary>
    /// 代收代发新增订单
    /// </summary>
    public class TransportAddModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public TransportOrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 发货地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 是否代付货款
        /// </summary>
        public bool IsPrePaid { get; set; }

        /// <summary>
        /// 交货方式
        /// </summary>
        public string WaybillType { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? TakingDate { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public string TakingDateStr { get; set; }

        /// <summary>
        /// 提货地址
        /// </summary>
        public string[] TakingAddress { get; set; }

        /// <summary>
        /// 提货详细地址
        /// </summary>
        public string TakingDetailAddress { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string TakingContact { get; set; }

        /// <summary>
        /// 提货人联系电话
        /// </summary>
        public string TakingPhone { get; set; }

        /// <summary>
        /// 提货文件
        /// </summary>
        public FileModel[] TakingFiles { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public FileModel[] PIFiles { get; set; }

        /// <summary>
        /// 是否垫付运费
        /// </summary>
        public bool IsPayFreight { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 总件数
        /// </summary>
        public int? PartsTotal { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string CarrierID { get; set; }

        /// <summary>
        /// 子运单号
        /// </summary>
        public string Subcodes { get; set; }

        /// <summary>
        /// 是否拆箱验货
        /// </summary>
        public bool IsUnpacking { get; set; }

        /// <summary>
        /// 支付币种
        /// </summary>
        public string PayCurrency { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        public string SettlementCurrency { get; set; }

        /// <summary>
        /// 是否代检测
        /// </summary>
        public bool IsTesting { get; set; }

        /// <summary>
        /// 提货ID
        /// </summary>
        public string InLoadingID { get; set; }

        /// <summary>
        /// 提货ID
        /// </summary>
        public string OutLoadingID { get; set; }

        /// <summary>
        /// 货物条款ID
        /// </summary>
        public string InChargeID { get; set; }

        /// <summary>
        /// 货物条款ID
        /// </summary>
        public string OutChargeID { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string InWaybillID { get; set; }

        /// <summary>
        /// 运单ID
        /// </summary>
        public string OutWaybillID { get; set; }

        /// <summary>
        /// 航次号
        /// </summary>
        public string VoyageNumber { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 银行账户
        /// </summary>
        public string BeneficiaryName { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 发货类型
        /// </summary>
        public string DeliveryType { get; set; }

        /// <summary>
        /// 是否代收货款
        /// </summary>
        public bool IsCollectMoney { get; set; }

        /// <summary>
        /// 收款委托书
        /// </summary>
        public FileModel[] EntrustFile { get; set; }

        /// <summary>
        /// 送货方式
        /// </summary>
        public string ShippingMethod { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string ReceiveTakingContact { get; set; }

        /// <summary>
        /// 提货人联系电话
        /// </summary>
        public string ReceiveTakingPhone { get; set; }

        /// <summary>
        /// 证件
        /// </summary>
        public string Certificate { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string CertificateType { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        public string ReceivedContact { get; set; }

        /// <summary>
        /// 收货人联系电话
        /// </summary>
        public string ReceivedPhone { get; set; }

        /// <summary>
        /// 收货人地址
        /// </summary>
        public string[] ReceivedAddress { get; set; }

        /// <summary>
        /// 收货人详细地址
        /// </summary>
        public string ReceivedDetailAddress { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        public string PostCode { get; set; }

        /// <summary>
        /// 快递支付方式
        /// </summary>
        public string ExpressPaymentType { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string ReceiveCarrierID { get; set; }

        /// <summary>
        /// 随货文件
        /// </summary>
        public FileModel[] AccompanyingFile { get; set; }

        /// <summary>
        /// 是否自定义标签
        /// </summary>
        public bool IsCustomLabel { get; set; }

        /// <summary>
        /// 是否重新包装
        /// </summary>
        public bool IsRepackaging { get; set; }

        /// <summary>
        /// 是否真空包装
        /// </summary>
        public bool IsVacuumPackaging { get; set; }

        /// <summary>
        /// 是否防水包装
        /// </summary>
        public bool IsWaterproofPackaging { get; set; }

        /// <summary>
        /// 是否拆箱验货
        /// </summary>
        public bool IsUnBoxed { get; set; }

        /// <summary>
        /// 国际区域
        /// </summary>
        public string InternationalAddress { get; set; }

        /// <summary>
        /// 收货提货时间
        /// </summary>
        public DateTime? ReceiveTakingDate { get; set; }

        /// <summary>
        /// 收货提货时间
        /// </summary>
        public string ReceiveTakingDateStr { get; set; }

    }

    /// <summary>
    /// 待仓储订单项
    /// </summary>
    public class TransportOrderItem
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string OriginLabel { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public int Unit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string UnitLabel { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        public string InputID { get; set; }

        public string OutputID { get; set; }

    }

    /// <summary>
    /// 库存列表视图
    /// </summary>
    public class StorageListViewModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 销售数量
        /// </summary>
        public decimal SaleQuantity { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 批号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Orgin { get; set; }
    }

    /// <summary>
    /// 发票对象
    /// </summary>
    public class InvoiceViewModel
    {

        /// <summary>
        /// 唯一码
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 交付类型
        /// </summary>
        public string DeliveryType { set; get; }


        public string DeliveryTypeName { get; set; }

        /// <summary>
        /// 企业电话
        /// </summary>
        public string CompanyTel { set; get; }
        /// <summary>
        /// 企业名称
        /// </summary>
        public string CompanyName { set; get; }

        /// <summary>
        /// 发票类型 1 普通发票 2 增值税发票 3 海关发票
        /// </summary>
        public string Type { get; set; }

        public string TypeName { get; set; }
        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { set; get; }
        /// <summary>
        /// 纳税人识别号
        /// </summary>
        public string TaxperNumber { get; set; }

        /// <summary>
        /// 企业注册地址
        /// </summary>
        public string RegAddress { set; get; }

        /// <summary>
        /// 收票地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 联系人电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 联系人邮箱
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// 开票地址
        /// </summary>
        public string Address { get; set; }

        public string InvoiceDeliveryTypeOptions { get; set; }

        public string InvoiceTypeOptions { get; set; }

        /// <summary>
        /// 是否主账号
        /// </summary>
        public bool IsMain { get; set; }
    }

    /// <summary>
    /// 新增租赁数据模型
    /// </summary>
    public class LeaseModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// FatherID
        /// </summary>
        public string FatherID { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime ? StartDate { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public string StartDateStr { get; set; }

        /// <summary>
        /// 月数
        /// </summary>
        public int MonthNum { get; set; }

        /// <summary>
        /// 租赁信息
        /// </summary>
        public LeaseDataModel[] LeaseData { get; set; }

        /// <summary>
        /// 合同文件
        /// </summary>
        public FileModel[] ContractFile { get; set; }
    }

    /// <summary>
    /// 租赁信息
    /// </summary>
    public class LeaseDataModel
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 个数
        /// </summary>
        public int Num { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }
    }

    /// <summary>
    /// 转报关新增数据模型
    /// </summary>
    public class TransferDeclareAddModel:DeclareAddViewModel
    {
        /// <summary>
        /// 订单项
        /// </summary>
        public TransferDeclare[] OrderItem { get; set; }

    }

    /// <summary>
    /// 转报关订单项
    /// </summary>
    public class TransferDeclare
    {
        /// <summary>
        /// ID
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 库存ID
        /// </summary>
        public string StorageID { get; set; }

        /// <summary>
        /// 品名
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 品牌
        /// </summary>
        public string Manufacturer { get; set; }

        /// <summary>
        /// 型号
        /// </summary>
        public string PartNumber { get; set; }

        /// <summary>
        /// 批次号
        /// </summary>
        public string DateCode { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string OriginName { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string UnitLabel { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public decimal StockNum { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal? UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal? TotalPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 体积
        /// </summary>
        public decimal? Volume { get; set; }

    }

    /// <summary>
    /// 代付货款申请
    /// </summary>
    public class PrePayApplyModel
    {
        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        public string BankID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierEnglishName { get; set; }

        /// <summary>
        /// 代付货款委托书
        /// </summary>
        public FileModel[] PrePayFile { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public string TotalMoney { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal ApplyMoney { get; set; }

        /// <summary>
        /// 已申请金额
        /// </summary>
        public decimal AppliedMoney { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }
    }

    /// <summary>
    /// 代收货款申请
    /// </summary>
    public class ReceiveApplyModel
    {
        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 付款人ID
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        public string BankID { get; set; }

        /// <summary>
        /// 代付货款委托书
        /// </summary>
        public FileModel[] ReceiveFile { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public string TotalMoney { get; set; }

        /// <summary>
        /// 订单项
        /// </summary>
        public OrderItem[] OrderItems { get; set; }

        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal ApplyMoney { get; set; }

        /// <summary>
        /// 已申请金额
        /// </summary>
        public decimal AppliedMoney { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        public string CurrencyID { get; set; }
    }

    /// <summary>
    /// 付款人
    /// </summary>
    public class PayerModel
    {
        /// <summary>
        /// 开户行
        /// </summary>
        public string Bank { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 银行地址
        /// </summary>
        public string BankAddress { get; set; }
 
        /// <summary>
        /// 银行编码
        /// </summary>
        public string SwiftCode { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string Methord { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 地区
        /// </summary>
        public string District { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 邮箱
        /// </summary>
        public string Email { get; set; }

    }
}