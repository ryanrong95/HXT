using Needs.Ccs.Services.Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebMvc.Models
{
    /// <summary>
    /// 新增订单
    /// </summary>
    public class AddViewModel
    {
        //ID
        public string ID { get; set; }

        /// <summary>
        /// 传递的多个id
        /// </summary>
        public string QueryIDs { get; set; }
        //产品
        public OrderProductModel[] OrderProducts { get; set; }

        /// <summary>
        /// 提货文件
        /// </summary>
        public FileModel[] HKFiles { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public FileModel[] PIFiles { get; set; }

        //货币数据源
        public string CurrencyOptions { get; set; }

        //货币
        public string Currency { get; set; }

        //产地数据源
        public string OriginOptions { get; set; }

        //单位
        public string UnitOptions { get; set; }

        //包装类型
        public string WrapType { get; set; }

        //包装类型数据源
        public string WrapOptions { get; set; }

        //香港接货方式：供应商送货、自提
        public string HKDeliveryType { get; set; }

        //香港接货方式：供应商送货、自提
        public string HKDeliveryTypeOptions { get; set; }

        //物流单号
        public string WayBillNo { get; set; }

        //供应商（香港交货方式）
        public string Supplier { get; set; }

        //供应商数据源
        public string SupplierOptions { get; set; }

        //供应商提货地址(id)
        public string SupplierAddress { get; set; }

        //供应商提货地址
        public string SupplierAddressName { get; set; }
        //供应商联系人
        public string supplierContact { get; set; }

        //供应商联系人手机
        public string supplierContactMobile { get; set; }

        //供应商提货地址数据源
        public string SupplierAddressOptions { get; set; }

        //提货时间
        public DateTime? PickupTime { get; set; }

        //提货时间
        public string PickupTimeStr { get; set; }

        //国内交货方式
        public string SZDeliveryType { get; set; }

        //国内交货方式数据源
        public string SZDeliveryTypeOptions { get; set; }

        //提货人
        public string ClientPicker { get; set; }

        //提货人手机号码
        public string ClientPickerMobile { get; set; }

        //证件类型

        public string IDType { get; set; }

        //证件类型数据源
        public string IdTypeOptions { get; set; }

        //证件号码
        public string IDNumber { get; set; }

        //国内交货方式收货地址单位名称(ID)
        public string ClientConsignee { get; set; }

        //国内交货方式收货地址单位名称
        public string ClientConsigneeName { get; set; }

        //国内交货方式收货地址
        public string clientConsigneeAddress { get; set; }

        //国内交货方式收货地址联系人
        public string clientContact { get; set; }

        //国内交货方式收货地址联系人手机号码
        public string clientContactMobile { get; set; }

        //收货地址数据源
        public string ClientConsigneeOptions { get; set; }

        //付汇供应商
        public string[] PayExchangeSupplier { get; set; }

        //付汇供应商数据源
        public string PayExchangeSupplierOptions { get; set; }

        //是否包车
        public bool IsFullVehicle { get; set; }

        //是否垫款
        public bool IsLoan { get; set; }

        //件数
        public string PackNo { get; set; }

        //备注
        public string Summary { get; set; }

        //订单状态（提交，草稿）
        public bool isComfirmed { get; set; }

        //是否预付款
        public bool IsPrePaid { get; set; }

        //是否是草稿订单修改
        public bool IsDraft { get; set; }

        /// <summary>
        /// 是否已归类
        /// </summary>
        public bool IsClssified { get; set; }

    }

    /// <summary>
    /// 订单的产品
    /// </summary>
    public class OrderProductModel
    {
        //批次
        public string Batch { get; set; }

        //品名
        public string Name { get; set; }

        //品牌
        public string Manufacturer { get; set; }

        //型号
        public string Model { get; set; }

        //产地
        public string Origin { get; set; }

        //产地名称
        public string OriginLabel { get; set; }

        //数量
        public decimal Quantity { get; set; }

        //单位
        public string Unit { get; set; }

        //单位名称
        public string UnitLabel { get; set; }

        //单价
        public decimal UnitPrice { get; set; }

        //总价
        public decimal TotalPrice { get; set; }

        //毛重
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 产品归类：普通，商检，3C，原产地证明、禁运、检疫等
        /// </summary>
        public ItemCategoryType? Type { get; set; }

        /// <summary>
        /// 税务编码
        /// </summary>
        public string TaxCode { get; set; }

        /// <summary>
        /// 税务名称
        /// </summary>
        public string TaxName { get; set; }

        /// <summary>
        /// 海关编码\商品编码
        /// </summary>
        public string HSCode { get; set; }

        /// <summary>
        /// 申报要素
        /// </summary>
        public string Elements { get; set; }

        /// <summary>
        /// 法定第一单位
        /// </summary>
        public string Unit1 { get; set; }

        /// <summary>
        /// 法定第二单位（可空）
        /// </summary>
        public string Unit2 { get; set; }

        /// <summary>
        /// 检验检疫编码
        /// </summary>
        public string CIQCode { get; set; }

        /// <summary>
        /// 关税率
        /// </summary>
        public decimal? TariffRate { get; set; }

        /// <summary>
        /// 增值税率
        /// </summary>
        public decimal? AddedValueRate { get; set; }

        /// <summary>
        /// 消费税率
        /// </summary>
        public decimal? ExciseTaxRate { get; set; }

        ///// <summary>
        ///// 报关人
        ///// </summary>
        //public string Declarant { get; set; }

        /// <summary>
        /// 预处理一人员
        /// </summary>
        public string ClassifyFirstOperatorID { get; set; }

        /// <summary>
        /// 预处理二人员
        /// </summary>
        public string ClassifySecondOperatorID { get; set; }

        /// <summary>
        /// 是否3C
        /// </summary>
        public bool IsCCC { get; set; }

        /// <summary>
        /// 是否原产地证明
        /// </summary>
        public bool IsOriginProof { get; set; }

        /// <summary>
        /// 是否商检
        /// </summary>
        public bool IsInsp { get; set; }

        /// <summary>
        /// 系统判定是否3C
        /// </summary>
        public bool IsSysCCC { get; set; }

        /// <summary>
        /// 系统判定是否禁运
        /// </summary>
        public bool IsSysForbid { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        /// <summary>
        /// 唯一标识、物料号、单据号
        /// </summary>
        public string ProductUnionCode { get; set; }
    }

    public class InfoViewModel
    {

    }

    /// <summary>
    /// 我的订单
    /// </summary>
    public class MyOrdersViewModel
    {
        //订单状态数据源
        public string OrderStatusOptions { get; set; }

        //开票类型数据源
        public string InvoiceStatusOptions { get; set; }

        //付汇状态数据源
        public string PayExchangeStatusOptions { get; set; }

        public int Total { get; set; }

        //订单列表
        public IEnumerable<PreConfirmViewModel> OrderList { get; set; }
    }

    /// <summary>
    /// 代理报关委托书
    /// </summary>
    public class ProxyViewModel
    {

    }

    /// <summary>
    /// 待确认订单列表
    /// </summary>
    public class PreConfirmsViewModel
    {
        //订单列表
        public PreConfirmViewModel[] preconfirms { get; set; }
    }

    /// <summary>
    /// 待确认订单
    /// </summary>
    public class PreConfirmViewModel
    {
        //订单编号
        public string ID { get; set; }

        //订单日期
        public string CreateDate { get; set; }

        //交易币种
        public string Currency { get; set; }

        //报关总货值（外币）
        public decimal DeclarePrice { get; set; }

        //供应商
        public string Suppliers { get; set; }

        //供应商
        public string[] PaySuppliers { get; set; }

        //供应商
        public string PaySuppliersName { get; set; }

        //收件人
        public string Contact { get; set; }

        //下单人
        public string OrderMaker { get; set; }

        //订单状态
        public string OrderStatus { get; set; }

        //开票状态
        public string InvoiceStatus { get; set; }

        //付汇状态
        public string PayExchangeStatus { get; set; }

        //付汇方式
        public string PayExchangeType { get; set; }

        //可付汇
        public string Remittance { get; set; }

        //已付汇
        public string Remittanced { get; set; }

        //备注
        public string Summary { get; set; }

        //是否显示删除按钮
        public bool isDelete { get; set; }

        //挂起原因
        public string HangUpReason { get; set; }

        //是否选择（用于待付汇多选）
        public bool IsCheck { get; set; }

        //是否90天内
        public bool IsPrePayExchange { get; set; }

        /// <summary>
        /// 是否显示对账单
        /// </summary>
        public bool isShowBill { get; set; }

        /// <summary>
        /// 报关日期
        /// </summary>
        public DateTime? DeclareDate { get; set; }

        /// <summary>
        /// 日志
        /// </summary>
        public string[] Logs { get; set; }

        /// <summary>
        /// 日志查询结果
        /// </summary>
        public bool NoData { get; set; }

        /// <summary>
        /// 最新日志是否加载中
        /// </summary>
        public bool isLoading { get; set; }
    }

    /// <summary>
    /// 订单确认
    /// </summary>
    public class ConfirmViewModel
    {
        //订单编号
        public string ID { get; set; }

        //产品明细
        public ComfirmProducts[] Products { get; set; }

        public List<ComfirmProducts> PartProducts { get; set; }

        public FileModel[] AgentProxyFiles { get; set; }

        /// <summary>
        /// 代理报关委任书url
        /// </summary>
        public string AgentProxyURL { get; set; }

        //产品数量合计
        public decimal Products_Num { get; set; }

        //产品总价
        public string Products_TotalPrice { get; set; }

        //报关货值合计
        public string Products_DeclareValue { get; set; }

        //关税合计
        public string Products_Traiff { get; set; }

        //增值税合计
        public string Products_AddTax { get; set; }

        //代理费合计
        public string Products_AgencyFee { get; set; }

        //商检费合计
        public string Products_InspectionFee { get; set; }

        //税费合计
        public string Products_TotalTaxFee { get; set; }

        //报关总金额合计
        public string Products_TotalDeclareValue { get; set; }

        //供应商（香港交货方式）
        public string Supplier { get; set; }

        //物流单号
        public string WayBillNo { get; set; }

        //香港交货是否自提
        public bool isPickUp { get; set; }

        //国内交货是否自提
        public bool isSZPickUp { get; set; }

        //交易币种
        public string Currency { get; set; }

        //交易币种
        public string CurrencyCode { get; set; }

        //身份证号
        public string IDNumber { get; set; }

        //下单人
        public string OrderMaker { get; set; }

        //订单日期
        public string CreateDate { get; set; }

        //是否包车
        public string IsFullVehicle { get; set; }

        //是否代垫货款
        public string IsAdvanceMoneny { get; set; }

        //件数
        public string PackNo { get; set; }

        //包装类型
        public string WrapType { get; set; }

        //备注
        public string Summary { get; set; }

        //香港接货方式：供应商送货、自提
        public string HKDeliveryType { get; set; }

        //供应商提货地址
        public string SupplierAddress { get; set; }

        //提货文件url
        public string DeliveryFiles { get; set; }

        //供应商联系人
        public string supplierContact { get; set; }

        //供应商联系人手机
        public string supplierContactMobile { get; set; }

        //提货时间
        public string PickupTime { get; set; }

        //国内交货方式
        public string SZDeliveryType { get; set; }

        //国内交货方式收货地址
        public string clientConsigneeAddress { get; set; }

        //国内交货方式收货地址联系人
        public string clientContact { get; set; }

        //国内交货方式收货地址联系人/收件单位名称
        public string clientContactName { get; set; }

        //国内交货方式收货地址联系人手机号码
        public string clientContactMobile { get; set; }

        //付汇供应商
        public Array PayExchangeSupplier { get; set; }

        //发票信息
        public InvoiceModel invoice { get; set; }

        //PI文件
        public Array PIFiles { get; set; }

        //是否是待客户确认订单
        public bool isPreConfirm { get; set; }

        /// <summary>
        /// 代理报关委托书url
        /// </summary>
        public string AgentFileUrl { get; set; }

        /// <summary>
        /// 代理报关文件
        /// </summary>
        public string AgentProxyName { get; set; }

        /// <summary>
        /// 代理报关文件是否通过审核
        /// </summary>
        public bool AgentProxyStatus { get; set; }

        /// <summary>
        /// 是否显示报关委托书
        /// </summary>
        public bool IsShowAgentProxy { get; set; }

        /// <summary>
        /// 提货单
        /// </summary>
        public string Tihuo_file { get; set; }

        /// <summary>
        /// 是否是因为修改型号（删除型号/修改数量）引起的确认订单
        /// </summary>
        public bool IsBecauseModified { get; set; }

        /// <summary>
        /// 型号变更信息
        /// </summary>
        public string[] ModelModifiedInfo { get; set; } = new string[] { };
        public decimal DeclarePrice { get; set; }
       
        public string OrderStatusName { get; set; }
        public Needs.Wl.Models.Enums.OrderStatus OrderStatus { get; set; }
    }

    //待确认订单产品明细
    public class ComfirmProducts
    {
        //产地
        public string Origin { get; set; }

        //单位
        public string Unit { get; set; }

        //毛重
        public decimal? GrossWeight { get; set; }

        //批次
        public string Batch { get; set; }
        //型号
        public string Model { get; set; }

        /// <summary>
        /// 特殊类型
        /// 归类类型
        /// </summary>
        public string[] ItemCategoryTypes { get; set; }

        //品名
        public string Name { get; set; }

        //品牌
        public string Manufacturer { get; set; }

        //数量
        public decimal Quantity { get; set; }

        //单价
        public decimal UnitPrice { get; set; }

        //总价
        public decimal TotalPrice { get; set; }

        //报关货值
        public decimal DeclareValue { get; set; }

        //关税率
        public decimal TraiffRate { get; set; }

        //关税
        public decimal Traiff { get; set; }

        //增值税率
        public decimal AddTaxRate { get; set; }

        //增值税
        public decimal AddTax { get; set; }

        //代理费
        public decimal AgencyFee { get; set; }

        //商检费
        public decimal InspectionFee { get; set; }

        //税费合计
        public decimal TotalTaxFee { get; set; }

        //报关总金额
        public decimal TotalDeclareValue { get; set; }


        /// <summary>
        /// 报关总金额(不包含关税和增值税)
        /// </summary>
        public decimal TotalDeclareValue_Except_TraAndAdd { get; set; }

        /// <summary>
        /// 税费总金额(不包含关税和增值税)
        /// </summary>
        public decimal TotalTaxFee_Except_TraAndAdd { get; set; }
    }



    //发票详情
    public class OrdersInvoiceViewModel
    {
        //发票
        public InvoiceModel invoice { get; set; }

        //ID
        public string ID { get; set; }

        /// <summary>
        /// 是否是海关发票
        /// </summary>
        public bool IsCustomsInvoice { get; set; }

        /// <summary>
        /// 关税发票
        /// </summary>
        public string TariffInvoice { get; set; }

        /// <summary>
        /// 是否有关税发票
        /// </summary>
        public bool IsTariffInvoice { get; set; }

        /// <summary>
        /// 是否有消费税发票
        /// </summary>
        public bool IsConsumptionTax { get; set; }

        /// <summary>
        /// 增值税发票
        /// </summary>
        public string VATInvoice { get; set; }

        /// <summary>
        /// 是否显示海关增值税发票
        /// </summary>
        public bool IsAddInvoice { get; set; }

        /// <summary>
        /// 消费税
        /// </summary>
        public string ConsumptionTaxInvoice { get; set; }
    }

    //发票模型
    public class InvoiceModel
    {
        //开票类型
        public string invoiceType { get; set; }

        //发票交付方式
        public string invoiceDeliveryType { get; set; }

        //发票名称
        public string invoiceTitle { get; set; }

        //发票纳税人识别号
        public string invoiceTaxCode { get; set; }

        //发票地址
        public string invoiceAddress { get; set; }

        //发票电话
        public string invoiceTel { get; set; }

        //开户行
        public string invoiceBank { get; set; }

        //银行账号
        public string invoiceAccount { get; set; }

        //开票人
        public string contact { get; set; }

        //开票人手机
        public string mobile { get; set; }

        //开票人地址
        public string contactAddress { get; set; }
        //联系人名称
        public string contactName { get; set; }
        //联系人手机
        public string contactMobile { get; set; }
        //联系人电话
        public string contactTel { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public string ExpressCompany { get; set; }

        /// <summary>
        /// 运单号
        /// </summary>
        public string ExpressCode { get; set; }

        /// <summary>
        /// 寄出日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 发票号
        /// </summary>
        public string InvoiceNo { get; set; }
    }
    /// <summary>
    /// 对账单
    /// </summary>
    public class OrderBillsViewModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 合同号
        /// </summary>
        public string ContractNO { get; set; }

        /// <summary>
        /// 客户名称
        /// </summary>
        public string User_name { get; set; }

        /// <summary>
        /// 客户电话
        /// </summary>
        public string User_tel { get; set; }

        /// <summary>
        /// 被委托方公司
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        /// 被委托方电话
        /// </summary>
        public string CompanyTel { get; set; }

        /// <summary>
        /// 报关明细
        /// </summary>
        public Array Productlist { get; set; }

        /// <summary>
        /// 费用明细
        /// </summary>
        public Array Feelist { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 货款名称
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 货款汇率类型
        /// </summary>
        public string RateType { get; set; }

        /// <summary>
        /// 货款汇率
        /// </summary>
        public string Rate { get; set; }

        /// <summary>
        /// 货款记账日期
        /// </summary>
        public string GoodsDate { get; set; }

        /// <summary>
        ///是否代垫货款
        /// </summary>
        public bool IsLoan { get; set; }

        /// <summary>
        /// 对账单的url
        /// </summary>
        public string BillFileUrl { get; set; }

        /// <summary>
        /// 对账单的Name
        /// </summary>
        public string BillFileName { get; set; }

        /// <summary>
        /// 对账单是否已审核
        /// </summary>
        public bool BillFileStatus { get; set; }

        /// <summary>
        /// 报关总价
        /// </summary>
        public string TotalDeclareValue { get; set; }

        /// <summary>
        /// 下单日期
        /// </summary>
        public string CreateDate { get; set; }

        /// <summary>
        /// 传真
        /// </summary>
        public string AgentFax { get; set; }

        /// <summary>
        /// 公司地址
        /// </summary>
        public string AgentAddress { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string AgentName { get; set; }

        /// <summary>
        /// 公司电话
        /// </summary>
        public string AgentTel { get; set; }

        /// <summary>
        /// 开户名
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string AccountID { get; set; }

        /// <summary>
        /// 印章
        /// </summary>
        public string SealUrl { get; set; }

        /// <summary>
        /// 实时汇率
        /// </summary>
        public decimal RealExchangeRate { get; set; }

        /// <summary>
        /// 海关汇率
        /// </summary>
        public decimal CustomsExchangeRate { get; set; }

        /// <summary>
        /// 结算日期
        /// </summary>
        public string DueDate { get; set; }

        /// <summary>
        /// 数量合计
        /// </summary>
        public string SumQuantity { get; set; }

        /// <summary>
        /// 报关货值合计
        /// </summary>
        public string SumDeclarePrice { get; set; }

        /// <summary>
        /// 报关总金额合计
        /// </summary>
        public string SumTotalCNYPrice { get; set; }

        /// <summary>
        /// 关税合计
        /// </summary>
        public string SumTraiff { get; set; }

        /// <summary>
        /// 增值税合计
        /// </summary>
        public string SumAddedValueTax { get; set; }

        /// <summary>
        /// 代理费合计
        /// </summary>
        public string SumAgencyFee { get; set; }

        /// <summary>
        /// 杂费合计
        /// </summary>
        public string SumIncidentalFee { get; set; }

        /// <summary>
        /// 税费合计
        /// </summary>
        public string SumTotalTax { get; set; }

        /// <summary>
        /// 报关总金额合计
        /// </summary>
        public string SumTotalDeclarePrice { get; set; }


    }

    /// <summary>
    /// 文件对象
    /// </summary>
    public class FileModel
    {

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
    /// 付款记录
    /// </summary>
    public class PaymentViewModel
    {
        /// <summary>
        /// 已付款总计
        /// </summary>
        public string TotalAmount { get; set; }

        /// <summary>
        /// 已入账总计
        /// </summary>
        public string ClearAmount { get; set; }

        /// <summary>
        /// 未入账总计
        /// </summary>
        public string UnClearAmount { get; set; }

        /// <summary>
        /// 总记录
        /// </summary>
        public int Total { get; set; }
    }

    /// <summary>
    /// 付款明细
    /// </summary>
    public class OrderRecievedViewModel
    {
        public string ID { get; set; }

        /// <summary>
        /// 已付款总计
        /// </summary>
        public string Amount { get; set; }

        /// <summary>
        /// 已入账总计
        /// </summary>
        public string ClearAmount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 总记录
        /// </summary>
        public int Total { get; set; }

        /// <summary>
        /// 交易流水号
        /// </summary>
        public string SeqNo { get; set; }

        /// <summary>
        /// 收款人
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 收款银行
        /// </summary>
        public string AccountBankName { get; set; }

        /// <summary>
        /// 收款银行账号
        /// </summary>
        public string AccountBankAccount { get; set; }

        public string CreateTime { get; set; }
    }

    /// <summary>
    /// 请求响应数据
    /// </summary>
    public class ResponseData
    {
        public string success { get; set; }

        public string message { get; set; }

        public string url { get; set; }
    }

    public class ConfirmViewModels
    {
        public string MainOrderID { get; set; }
        public Array PayExchangeSupplier { get; set; }
        public Array PIFiles { get; set; }
        public string DeliveryFiles { get; set; }
        /// <summary>
        /// 代理报关文件
        /// </summary>
        public string AgentProxyName { get; set; }
        public string OrderBillName { get; set; }

        /// <summary>
        /// 代理报关文件是否通过审核
        /// </summary>
        public bool AgentProxyStatus { get; set; }

        public bool OrderBillStatus { get; set; }
        /// <summary>
        /// 是否显示报关委托书
        /// </summary>
        public bool IsShowAgentProxy { get; set; }
        /// <summary>
        /// 代理报关委任书url
        /// </summary>
        public string AgentProxyURL { get; set; }
        public bool IsShowOrderBill { get; set; }
        public string OrderBillURL { get; set; }

        public List<ConfirmViewModel> Confirms { get; set; }

        public decimal DeclarePrice { get; set; }
        public string CreateDate { get; set; }

        public List<ComfirmProducts> ProductsForIcgoo { get; set; }
        public List<ComfirmProducts> PartProductsForIcgoo { get; set; }
        
    }
}