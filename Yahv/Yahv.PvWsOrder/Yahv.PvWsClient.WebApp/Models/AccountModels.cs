using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Underly;

namespace Yahv.PvWsClient.WebApp.Models
{
    /// <summary>
    /// 账户信息数据模型
    /// </summary>
    public class MyInformationModel
    {
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
    /// 修改密码
    /// </summary>
    public class PasswordViewModel
    {
        public string Password { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
    /// <summary>
    /// 收货人
    /// </summary>
    public class ConsigneeViewModel
    {
        public string ID { get; set; }

        public string Title { get; set; }

        public string District { get; set; }

        public string Address { get; set; }

        public string Name { get; set; }

        public string Tel { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }

        public bool IsDefault { get; set; }


        public string DistrictOptions { get; set; }
    }

    /// <summary>
    /// 供应商信息
    /// </summary>
    public class SupplierInfoViewModel
    {
        //ID
        public string ID { get; set; }

        //中文名称
        public string ChineseName { get; set; }

        //英文名称
        public string EnglishName { get; set; }

        //备注
        public string Summary { get; set; }

        /// <summary>
        /// 管理员编码
        /// </summary>
        public string AdminCode { get; set; }

        /// <summary>
        /// 级别
        /// </summary>
        public int Grade { get; set; }

        /// <summary>
        /// 法人
        /// </summary>
        public string Corporation { get; set; }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegAddress { get; set; }

        /// <summary>
        /// 统一社会信用代码
        /// </summary>
        public string Uscc { get; set; }
    }

    /// <summary>
    /// 供应商银行账号
    /// </summary>
    public class BeneficiarieInfoViewModel
    {
        //ID
        public string ID { get; set; }

        public string SupplierID { get; set; }

        public string RealName { get; set; }

        public string Bank { get; set; }

        public string BankAddress { get; set; }

        public string Account { get; set; }

        public string SwiftCode { get; set; }

        public string Method { get; set; }

        public string Currency { get; set; }

        public string District { get; set; }

        public string InvoiceType { get; set; }

        public string Name { get; set; }

        public string Tel { get; set; }

        public string Mobile { get; set; }

        public string Email { get; set; }
    }

    /// <summary>
    /// 供应商提货地址
    /// </summary>
    public class SupplierAddressesViewModel
    {
        //ID
        public string ID { get; set; }

        //供应商编号
        public string SupplierID { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { get; set; }

        //联系人姓名
        public string Name { get; set; }

        //联系电话
        public string Tel { get; set; }

        //手机号码
        public string Mobile { get; set; }

        //邮箱
        public string Email { get; set; }

        //是否默认
        public bool IsDefault { get; set; }
    }

    public class MyConsigneesViewModel
    {
        //收货人列表
        public ConsigneeViewModel[] consignees { get; set; }

        //是否是主账号
        public bool IsMain { get; set; }
    }
}