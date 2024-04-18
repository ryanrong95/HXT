using System;
using System.Collections.Generic;
using Yahv.PvWsOrder.Services.ClientModels;
using Yahv.PvWsOrder.Services.Models;

namespace Yahv.PvWsClient.WebAppNew.Models
{
    /// <summary>
    /// 代收订单数据模型
    /// </summary>
    public class StorageOrdersModel
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
        /// 特殊货物处理要求
        /// </summary>
        public SpecialGoodsModel[] SpecialGoods { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        public string SettlementCurrency { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int? PackNo { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 是否代付货款
        /// </summary>
        public bool IsPay { get; set; }

        /// <summary>
        /// 是否代收货款
        /// </summary>
        public bool IsRecieve { get; set; }

        /// <summary>
        /// 代收货款
        /// </summary>
        public decimal RecievePrice { get; set; }

        /// <summary>
        /// 申请金额
        /// </summary>
        public decimal ApplyPrice { get; set; }

        /// <summary>
        /// 供应商银行账号ID
        /// </summary>
        public string BankID { get; set; }

        public string SupplierMethod { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string SupplierBankAccount { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string SupplierBankName { get; set; }

        /// <summary>
        /// 银行企业名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 客户账户余额
        /// </summary>
        public decimal WareHouseLeft { get; set; }

        /// <summary>
        /// 发货地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 代付货款付款人
        /// </summary>
        public string PayPayerID { get; set; }

        /// <summary>
        /// 代付货款付款方式
        /// </summary>
        public int PayPayerMethod { get; set; }
        public string PayPayerMethodName { get; set; }

        /// <summary>
        /// 代付货款付款币种
        /// </summary>
        public int PayPayerCurrency { get; set; }
        public string PayPayerCurrencyName { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public FileModel[] PIFiles { get; set; }

        /// <summary>
        /// 装箱单
        /// </summary>
        public FileModel[] PackingFiles { get; set; }

        /// <summary>
        /// 香港收货方式
        /// </summary>
        public string HKWaybillType { get; set; }

        /// <summary>
        /// 国内交货方式
        /// </summary>
        public string WaybillType { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? HKTakingDate { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public string HKTakingDateStr { get; set; }

        /// <summary>
        /// 提货地址
        /// </summary>
        public string HKSupplierAddress { get; set; }

        /// <summary>
        /// 提货文件
        /// </summary>
        public FileModel[] TakingFiles { get; set; }

        /// <summary>
        /// 是否即收即发
        /// </summary>
        public bool IsTransfer { get; set; }

        /// <summary>
        /// 是否垫付运费
        /// </summary>
        public bool HKFreight { get; set; }

        /// <summary>
        /// 是否垫付运费
        /// </summary>
        public bool Freight { get; set; }

        /// <summary>
        /// 香港库房英文名
        /// </summary>
        public string WareHouseEnglishName { get; set; }

        /// <summary>
        /// 香港仓库地址
        /// </summary>
        public string WareHouseAddress { get; set; }

        /// <summary>
        /// 香港仓库联系人
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 香港仓库联系人电话
        /// </summary>
        public string WareHouseTel { get; set; }

        /// <summary>
        /// 香港仓库信息(用于复制)
        /// </summary>
        public string WareHouseInfoForCopy { get; set; }

        /// <summary>
        /// 快递单号
        /// </summary>
        public string HKExpressNumber { get; set; }

        /// <summary>
        /// 子快递单号
        /// </summary>
        public string HKExpressSubNumber { get; set; }

        /// <summary>
        /// 快递公司
        /// </summary>
        public string HKExpressName { get; set; }

        /// <summary>
        /// 航次号
        /// </summary>
        public string HKAirCode { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string ClientPicker { get; set; }

        /// <summary>
        /// 提货人手机号码
        /// </summary>
        public string ClientPickerMobile { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? PickupTime { get; set; }

        //提货时间
        public string PickupTimeStr { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string IDType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// 公章内容
        /// </summary>
        public string SealContext { get; set; }

        /// <summary>
        /// 国内交货方式收货地址单位名称(ID)
        /// </summary>
        public string ClientConsignee { get; set; }

        /// <summary>
        /// 国内交货方式收货地址单位名称
        /// </summary>
        public string ClientConsigneeName { get; set; }

        /// <summary>
        /// 国内交货方式收货地址
        /// </summary>
        public string ClientConsigneeAddress { get; set; }

        /// <summary>
        /// 国内交货方式收货地址联系人手机号码
        /// </summary>
        public string ClientContactMobile { get; set; }

        /// <summary>
        /// 国内交货方式收货地址公司名称
        /// </summary>
        public string ClientConsigneeCompany { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string ExpressID { get; set; }

        /// <summary>
        /// 代收货款客户付款人
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public int? PayerMethod { get; set; }
        public string PayerMethodName { get; set; }

        /// <summary>
        /// 付款币种
        /// </summary>
        public int? PayerCurrency { get; set; }
        public string PayerCurrencyName { get; set; }

        /// <summary>
        /// 代付公司受益人
        /// </summary>
        public string PayCompanyBankID { get; set; }

        /// <summary>
        /// 代收公司受益人
        /// </summary>
        public string CompanyBankID { get; set; }

        /// <summary>
        /// 发货时机
        /// </summary>
        public string DelivaryOpportunity { get; set; }

        /// <summary>
        /// 是否入账
        /// </summary>
        public bool IsEntry { get; set; }

        /// <summary>
        /// 支票投送方式
        /// </summary>
        public string CheckDelivery { get; set; }

        /// <summary>
        /// 客户收款人
        /// </summary>
        public string CheckPayeeAccount { get; set; }

        /// <summary>
        /// 快递承运商
        /// </summary>
        public string CheckCarrier { get; set; }

        /// <summary>
        /// 收票人地址
        /// </summary>
        public string CheckConsignee { get; set; }

        /// <summary>
        /// 支票抬头
        /// </summary>
        public string CheckTitle { get; set; }

        /// <summary>
        /// 公司受益人
        /// </summary>
        public string CompanyBank { get; set; }

        /// <summary>
        /// 公司受益人
        /// </summary>
        public string CompanyBankAccount { get; set; }

        /// <summary>
        /// 是否编辑
        /// </summary>
        public bool isedit { get; set; }

        /// <summary>
        /// 暂存还是提交
        /// </summary>
        public bool IsSubmit { get; set; }

        /// <summary>
        /// 收款账户
        /// </summary>
        public string CompanyBankAccountName { get; set; }

        /// <summary>
        /// 收款银行
        /// </summary>
        public string CompanyBankName { get; set; }

    }


    /// <summary>
    /// 代发货数据模型
    /// </summary>
    public class DeliveryOrdersModel
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
        /// 特殊货物处理要求
        /// </summary>
        public SpecialGoodsModel[] SpecialGoods { get; set; }

        /// <summary>
        /// 货币
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        public string SettlementCurrency { get; set; }

        /// <summary>
        /// 件数
        /// </summary>
        public int? PackNo { get; set; }

        /// <summary>
        /// 总毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 是否代收货款
        /// </summary>
        public bool IsRecieve { get; set; }

        /// <summary>
        /// 代收货款
        /// </summary>
        public decimal RecievePrice { get; set; }

        /// <summary>
        /// 发货地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// PI文件
        /// </summary>
        public FileModel[] PIFiles { get; set; }

        /// <summary>
        /// 交货方式
        /// </summary>
        public string WaybillType { get; set; }

        /// <summary>
        /// 是否垫付运费
        /// </summary>
        public bool Freight { get; set; }

        /// <summary>
        /// 提货人
        /// </summary>
        public string ClientPicker { get; set; }

        /// <summary>
        /// 提货人手机号码
        /// </summary>
        public string ClientPickerMobile { get; set; }

        /// <summary>
        /// 提货时间
        /// </summary>
        public DateTime? PickupTime { get; set; }

        //提货时间
        public string PickupTimeStr { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public string IDType { get; set; }

        /// <summary>
        /// 证件号码
        /// </summary>
        public string IDNumber { get; set; }

        /// <summary>
        /// 公章内容
        /// </summary>
        public string SealContext { get; set; }

        /// <summary>
        /// 国内交货方式收货地址单位名称(ID)
        /// </summary>
        public string ClientConsignee { get; set; }

        /// <summary>
        /// 国内交货方式收货地址单位名称
        /// </summary>
        public string ClientConsigneeName { get; set; }

        /// <summary>
        /// 国内交货方式收货地址
        /// </summary>
        public string ClientConsigneeAddress { get; set; }

        /// <summary>
        /// 国内交货方式收货地址联系人手机号码
        /// </summary>
        public string ClientContactMobile { get; set; }

        /// <summary>
        /// 国内交货方式收货地址公司名称
        /// </summary>
        public string ClientConsigneeCompany { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string ZipCode { get; set; }

        /// <summary>
        /// 承运商
        /// </summary>
        public string ExpressID { get; set; }

        /// <summary>
        /// 客户付款人
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 付款方式
        /// </summary>
        public int? PayerMethod { get; set; }

        /// <summary>
        /// 付款币种
        /// </summary>
        public int? PayerCurrency { get; set; }

        /// <summary>
        /// 是否入账
        /// </summary>
        public bool IsEntry { get; set; }

        /// <summary>
        /// 发货时机
        /// </summary>
        public string DelivaryOpportunity { get; set; }

        /// <summary>
        /// 支票投送
        /// </summary>
        public string CheckDelivery { get; set; }

        /// <summary>
        /// 客户收款人
        /// </summary>
        public string CheckPayeeAccount { get; set; }

        /// <summary>
        /// 快递承运商
        /// </summary>
        public string CheckCarrier { get; set; }

        /// <summary>
        /// 收票人地址
        /// </summary>
        public string CheckConsignee { get; set; }

        /// <summary>
        /// 支票抬头
        /// </summary>
        public string CheckTitle { get; set; }

        /// <summary>
        /// 公司受益人
        /// </summary>
        public string CompanyBankID { get; set; }

        /// <summary>
        /// 公司受益人
        /// </summary>
        public string CompanyBank { get; set; }

        /// <summary>
        /// 公司受益人
        /// </summary>
        public string CompanyBankAccount { get; set; }

        /// <summary>
        /// 暂存还是提交
        /// </summary>
        public bool IsSubmit { get; set; }

        /// <summary>
        /// 收款账户
        /// </summary>
        public string CompanyBankAccountName { get; set; }

        /// <summary>
        /// 收款银行
        /// </summary>
        public string CompanyBankName { get; set; }
    }

    /// <summary>
    /// 付款人
    /// </summary>
    public class PayerModel
    {

        public string SupplierID { get; set; }

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
        public string Place { get; set; }

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

    /// <summary>
    /// 租赁订单数据模型
    /// </summary>
    public class LsOrderModel
    {
        /// <summary>
        /// 租赁产品项
        /// </summary>
        public LsProductModel[] Products { get; set; }

        /// <summary>
        /// 租赁订单项
        /// </summary>
        public LsOrderItemModel[] Items { get; set; }
    }

    /// <summary>
    /// 租赁订单项
    /// </summary>
    public class LsOrderItemModel
    {
        /// <summary>
        /// 订单主键
        /// </summary>
        public string OrderID { get; set; }

        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 库位号
        /// </summary>
        public string SpecID { get; set; }

        /// <summary>
        /// 承重
        /// </summary>
        public string Load { get; set; }

        /// <summary>
        /// 容积
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 开始日期
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 租期
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }

        /// <summary>
        /// 续租提交“续租数量”的字段
        /// </summary>
        public int Amount { get; set; }
    }

    /// <summary>
    /// 租赁产品
    /// </summary>
    public class LsProductModel
    {
        /// <summary>
        /// 产品主键
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 库位号
        /// </summary>
        public string SpecID { get; set; }

        /// <summary>
        /// 承重
        /// </summary>
        public int Load { get; set; }

        /// <summary>
        /// 容积
        /// </summary>
        public string Volume { get; set; }

        /// <summary>
        /// 产品价格表
        /// </summary>
        public LsProductPrice[] LsPrices { get; set; }

        /// <summary>
        /// 剩余数量
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public int Amount { get; set; }

        /// <summary>
        /// 租期
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { get; set; }
    }

    /// <summary>
    /// 产品价格表
    /// </summary>
    public class LsProductPrice
    {
        /// <summary>
        /// 产品ID
        /// </summary>
        public string ProductID { get; set; }

        /// <summary>
        /// 租期
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 价格
        /// </summary>
        public decimal Price { get; set; }
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

        /// <summary>
        /// 币种名称
        /// </summary>
        public string CurrencyName { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        /// 供应商
        /// </summary>
        public string SupplierID { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMethrod { get; set; }
    };

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
        /// 付款人
        /// </summary>
        public string PayerID { get; set; }

        /// <summary>
        /// 付款人银行
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 付款人银行账号
        /// </summary>
        public string PayerAccount { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// 币种
        /// </summary>
        public int CurrencyID { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        public string CompanyBankID { get; set; }

        /// <summary>
        /// 公司受益人
        /// </summary>
        public string CompanyBankAccount { get; set; }

        /// <summary>
        /// 收款账户
        /// </summary>
        public string CompanyBankAccountName { get; set; }

        /// <summary>
        /// 收款银行
        /// </summary>
        public string CompanyBankName { get; set; }
    }

    public class PayDetailInfo
    {
        /// <summary>
        /// 付款金额
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 账户名称
        /// </summary>
        public string AccountName { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string SupplierMethodDes { get; set; }

        /// <summary>
        /// 企业名称
        /// </summary>
        public string SupplierName { get; set; }

        /// <summary>
        /// 银行名称
        /// </summary>
        public string SupplierBankName { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 银行账号
        /// </summary>
        public string SupplierBankAccount { get; set; }

        /// <summary>
        /// 付款方式Int
        /// </summary>
        public int MethordInt { get; set; }

        /// <summary>
        /// 付款方式名称
        /// </summary>
        public string MethordDec { get; set; }

        /// <summary>
        /// 付款人币种
        /// </summary>
        public string CurrencyDec { get; set; }

        /// <summary>
        /// 收款账户
        /// </summary>
        public string CompanyBankAccountName { get; set; }

        /// <summary>
        /// 收款账号
        /// </summary>
        public string CompanyBankAccount { get; set; }

        /// <summary>
        /// 收款银行
        /// </summary>
        public string CompanyBankName { get; set; }
    }

    public class ReceiveDetailInfo
    {
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal ReceivePrice { get; set; }

        /// <summary>
        /// 付款人
        /// </summary>
        public string PayerName { get; set; }

        /// <summary>
        /// 付款方式Int
        /// </summary>
        public int PayerMethodInt { get; set; }

        /// <summary>
        /// 付款方式名称
        /// </summary>
        public string PayerMethodName { get; set; }

        /// <summary>
        /// 付款人币种
        /// </summary>
        public string PayerCurrencyName { get; set; }

        /// <summary>
        /// 发货时机显示
        /// </summary>
        public string DelivaryOpportunityDes { get; set; }

        /// <summary>
        /// 是否入账1/0
        /// </summary>
        public int IsEntryInt { get; set; }

        /// <summary>
        /// 是否入账显示
        /// </summary>
        public string IsEntryDes { get; set; }

        /// <summary>
        /// 支票投送方式显示
        /// </summary>
        public string CheckDeliveryDes { get; set; }
    }
}