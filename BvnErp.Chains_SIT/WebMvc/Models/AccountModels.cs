using System;

namespace WebMvc.Models
{
    /// <summary>
    /// 修改手机号码视图模型
    /// </summary>
    public class MobileViewModel
    {
        /// <summary>
        /// 原手机号码
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// 新手机号码
        /// </summary>
        public string NewPhone { get; set; }

        /// <summary>
        /// 短信验证码
        /// </summary>
        public string Code { get; set; }

        public bool IsMain { get; set; }
    }

    /// <summary>
    /// 修改邮箱视图模型
    /// </summary>
    public class EmailViewModel
    {
        public string Code { get; set; }
        public string Email { get; set; }

        public string NewEmail { get; set; }

        public bool IsMain { get; set; }
    }

    /// <summary>
    /// 会员中心显示页面
    /// </summary>
    public class IndexViewModel
    {
        /// <summary>
        /// 用户名
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 业务经理名称
        /// </summary>
        public string ServiceManagerName { get; set; }

        /// <summary>
        /// 业务经理电话
        /// </summary>
        public string ServiceManagerTel { get; set; }

        /// <summary>
        /// 业务经理邮箱
        /// </summary>
        public string ServiceManagerMail { get; set; }

        /// <summary>
        /// 跟单员名称
        /// </summary>
        public string MerchandiserName { get; set; }

        /// <summary>
        /// 跟单员电话
        /// </summary>
        public string MerchandiserTel { get; set; }

        /// <summary>
        /// 跟单员邮箱
        /// </summary>
        public string MerchandiserMail { get; set; }

        /// <summary>
        /// 应付款总金额
        /// </summary>
        public decimal TotalPayableAmount { get; set; }

        /// <summary>
        /// 应付货款
        /// </summary>
        public decimal ProductPayable { get; set; }

        /// <summary>
        /// 应付税款
        /// </summary>
        public decimal TaxPayable { get; set; }

        /// <summary>
        /// 应付代理费
        /// </summary>
        public decimal AgencyFeePayable { get; set; }

        /// <summary>
        /// 应付杂费
        /// </summary>
        public decimal IncidentalPayable { get; set; }

        /// <summary>
        /// 货款的垫款上限
        /// </summary>
        public decimal ProductUpperLimit { get; set; }

        /// <summary>
        /// 税款垫款上限
        /// </summary>
        public decimal TaxUpperLimit { get; set; }

        /// <summary>
        /// 代理费垫款上限
        /// </summary>
        public decimal AgencyUpperLimit { get; set; }

        /// <summary>
        /// 杂费垫款上限
        /// </summary>
        public decimal IncidentalUpperLimit { get; set; }

        /// <summary>
        /// 可用的货款垫款
        /// </summary>
        public decimal AvailableProductFee { get; set; }

        /// <summary>
        /// 可用的税款垫款
        /// </summary>
        public decimal AvailableTaxFee { get; set; }

        /// <summary>
        /// 可用的代理费垫款
        /// </summary>
        public decimal AvailableAgencyFee { get; set; }

        /// <summary>
        /// 可用的杂费垫款
        /// </summary>
        public decimal AvailableIncidentalFee { get; set; }

        /// <summary>
        /// 待确认订单数
        /// </summary>
        public int UnConfirmCount { get; set; }

        /// <summary>
        /// 代开票订单数
        /// </summary>
        public int UnInvoiceCount { get; set; }

        /// <summary>
        /// 挂起订单数
        /// </summary>
        public int HangUpCount { get; set; }

        /// <summary>
        /// 待付汇订单数
        /// </summary>
        public int UnPayExchangeCount { get; set; }

        /// <summary>
        /// 已完成订单数
        /// </summary>
        public int CompeletedCount { get; set; }

        /// <summary>
        /// 订单列表
        /// </summary>
        public Array Orderlist { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        public string NewPassword { get; set; }

        /// <summary>
        /// 确认密码
        /// </summary>
        public string ConfirmPassword { get; set; }

        /// <summary>
        /// 是否原始密码
        /// </summary>
        public bool IsOriginPassWord { get; set; }

        /// <summary>
        /// 验证码
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 入仓号
        /// </summary>
        public string ClientCode { get; set; }
    }

    /// <summary>
    /// 修改密码
    /// </summary>
    public class PasswordViewModel
    {
        public string Password { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }

    /// <summary>
    /// 客户公司
    /// </summary>
    public class ClientCompanyViewModel
    {
        //公司名称
        public string Company_Name { get; set; }

        //公司法人
        public string Corporat { get; set; }

        //公司地址(省市区)
        public string[] Address { get; set; }

        //详细地址
        public string DetailAddress { get; set; }

        //地址
        public string AllAddress { get; set; }

        //海关编码
        public string CustomsCode { get; set; }

        //统一社会信用编码
        public string Code { get; set; }

        //联系人
        public string Contacts { get; set; }


        //联系人手机号码
        public string Contacts_Moblie { get; set; }

        //电话
        public string Phone { get; set; }

        //传真
        public string Fax { get; set; }

        //QQ
        public string QQ { get; set; }

        public bool IsMain { get; set; }
    }

    public class ClientViewModel
    {
        //客户ID
        public string ID { get; set; }

        //登录名
        public string User_Name { get; set; }

        //公司
        public string Company_Name { get; set; }


        //登陆密码
        public string Password { get; set; }

        //手机号
        public string Mobile { get; set; }

        //邮箱
        public string Mail { get; set; }

        public bool IsMain { get; set; }

        /// <summary>
        /// 营业执照图片url
        /// </summary>
        public string PicURL { get; set; }

        public FileModel LisencePic { get; set; }

    }

    /// <summary>
    /// 条款协议视图模型
    /// </summary>
    public class AgreementViewModel
    {
        //开始日期
        public string StartDate { get; set; }

        //结束日期
        public string EndDate { get; set; }

        //代理费率
        public string AgencyRate { get; set; }

        //最低代理费
        public string MinAgencyFee { get; set; }

        //货款换汇条款
        public string IsPrePayExchange { get; set; }

        //货款结算方式
        public string GoodsPeriodType { get; set; }

        //货款结算类型
        public string GoodsExchangeRateType { get; set; }

        /// <summary>
        /// 货款是否预付款
        /// </summary>
        public bool isGoodsPrePaid { get; set; }

        /// <summary>
        /// 货款是否为约定期限
        /// </summary>
        public bool isGoodsAgreedPeriod { get; set; }

        /// <summary>
        /// 货款约定期限(天)
        /// </summary>
        public string GoodsDaysLimit { get; set; }

        /// <summary>
        /// 货款是否为月结
        /// </summary>
        public bool isGoodsMonthly { get; set; }

        /// <summary>
        /// 月结日期(天)
        /// </summary>
        public string GoodsMonthlyDay { get; set; }

        /// <summary>
        /// 货款是否为约定期限
        /// </summary>
        public bool isGoodsAgreed { get; set; }

        /// <summary>
        /// 货款约定汇率
        /// </summary>
        public string GoodsExchangeRateValue { get; set; }

        //货款垫款上限
        public string GoodsUpperLimit { get; set; }

        //税款结算方式
        public string TaxPeriodType { get; set; }

        //税款汇率类型
        public string TaxExchangeRateType { get; set; }

        //税款垫款上限
        public string TaxUpperLimit { get; set; }

        /// <summary>
        /// 税款是否预付款
        /// </summary>
        public bool isTaxPrePaid { get; set; }

        /// <summary>
        /// 税款是否为约定期限
        /// </summary>
        public bool isTaxAgreedPeriod { get; set; }

        /// <summary>
        /// 税款约定期限(天)
        /// </summary>
        public string TaxDaysLimit { get; set; }

        /// <summary>
        /// 税款是否为月结
        /// </summary>
        public bool isTaxMonthly { get; set; }

        /// <summary>
        /// 月结日期(天)
        /// </summary>
        public string TaxMonthlyDay { get; set; }

        /// <summary>
        /// 税款是否为约定期限
        /// </summary>
        public bool isTaxAgreed { get; set; }

        /// <summary>
        /// 税款约定汇率
        /// </summary>
        public string TaxExchangeRateValue { get; set; }

        //代理费结算方式
        public string AgencyFeePeriodType { get; set; }

        //代理费汇率类型
        public string AgencyFeeExchangeRateType { get; set; }

        //代理费垫款上限
        public string AgencyFeeUpperLimit { get; set; }
        /// <summary>
        /// 代理费是否预付款
        /// </summary>
        public bool isAgencyPrePaid { get; set; }

        /// <summary>
        /// 代理费是否为约定期限
        /// </summary>
        public bool isAgencyAgreedPeriod { get; set; }

        /// <summary>
        /// 代理费约定期限(天)
        /// </summary>
        public string AgencyDaysLimit { get; set; }

        /// <summary>
        /// 代理费是否为月结
        /// </summary>
        public bool isAgencyMonthly { get; set; }

        /// <summary>
        /// 月结日期(天)
        /// </summary>
        public string AgencyMonthlyDay { get; set; }

        /// <summary>
        /// 代理费是否为约定期限
        /// </summary>
        public bool isAgencyAgreed { get; set; }

        /// <summary>
        /// 代理费约定汇率
        /// </summary>
        public string AgencyExchangeRateValue { get; set; }
        //杂费结算方式
        public string IncidentalPeriodType { get; set; }

        //杂费汇率类型
        public string IncidentalExchangeRateType { get; set; }

        //杂费垫款上限
        public string IncidentalUpperLimit { get; set; }
        /// <summary>
        /// 杂费是否预付款
        /// </summary>
        public bool isIncidentalPrePaid { get; set; }

        /// <summary>
        /// 杂费是否为约定期限
        /// </summary>
        public bool isIncidentalAgreedPeriod { get; set; }

        /// <summary>
        /// 杂费约定期限(天)
        /// </summary>
        public string IncidentalDaysLimit { get; set; }

        /// <summary>
        /// 杂费是否为月结
        /// </summary>
        public bool isIncidentalMonthly { get; set; }

        /// <summary>
        /// 月结日期(天)
        /// </summary>
        public string IncidentalMonthlyDay { get; set; }

        /// <summary>
        /// 杂费是否为约定期限
        /// </summary>
        public bool isIncidentalAgreed { get; set; }

        /// <summary>
        /// 杂费约定汇率
        /// </summary>
        public string IncidentalExchangeRateValue { get; set; }
        //开票类型
        public string InvoiceType { get; set; }

        //税率
        public string InvoiceRate { get; set; }
    }

    public class MyConsigneesViewModel
    {
        //收货人列表
        public ConsigneeViewModel[] consignees { get; set; }

        //是否是主账号
        public bool IsMain { get; set; }
    }

    /// <summary>
    /// 收货人
    /// </summary>
    public class ConsigneeViewModel
    {
        public string ID { get; set; }

        //收件人
        public string Consignee { get; set; }

        //手机
        public string Mobile { get; set; }

        //收货单位
        public string Name { get; set; }

        //公司地址(省市区)
        public string[] Address { get; set; }

        //详细地址
        public string DetailAddress { get; set; }

        //地址
        public string AllAddress { get; set; }

        //邮箱
        public string Mail { get; set; }

        //是否默认地址
        public bool IsDefault { get; set; }

        public DateTime CreateDate { get; set; }
    }

    public class InvoiceAndConsigneeModel
    {
        public InvoiceViewModel invoice { get; set; }
        public InvoiceConsigneeViewModel consignee { get; set; }
    }
    /// <summary>
    /// 客户发票
    /// </summary>
    public class InvoiceViewModel
    {
        //名称
        public string Title { get; set; }

        //纳税人识别号
        public string TaxCode { get; set; }

        //地址
        public string Address { get; set; }

        // 电话
        public string Tel { get; set; }

        // 银行名称
        public string BankName { get; set; }

        // 银行账号
        public string BankAccount { get; set; }

        // 电话
        public string InvoiceDeliveryType { get; set; }

        //邮寄方式
        public string InvoiceDeliveryTypeOptions { get; set; }

        //邮寄方式（名称）
        public string InvoiceDeliveryTypeName { get; set; }

        //是否是主账号
        public bool IsMain { get; set; }
    }

    /// <summary>
    /// 发票联系人
    /// </summary>
    public class InvoiceConsigneeViewModel
    {
        //联系人
        public string ConsigneeName { get; set; }

        //联系人手机
        public string ConsigneeMobile { get; set; }

        //联系人电话
        public string ConsigneeTel { get; set; }

        //联系人邮箱
        public string ConsigneeEmail { get; set; }

        //联系人地址(省市区)
        public string[] ConsigneeAddress { get; set; }

        //联系人详细地址
        public string ConsigneeDetailAddress { get; set; }

        //联系人地址
        public string ConsigneeAllAddress { get; set; }
    }


    /// <summary>
    /// 投诉建议
    /// </summary>
    public class SuggestionViewModel
    {
        /// <summary>
        /// 申请人
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// 电话
        /// </summary>
        public string phone { get; set; }

        /// <summary>
        /// 建议
        /// </summary>
        public string summary { get; set; }
    }
}