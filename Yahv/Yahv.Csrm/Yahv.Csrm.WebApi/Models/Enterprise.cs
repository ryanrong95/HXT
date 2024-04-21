using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Yahv.Underly;
using Yahv.Underly.Enums;
using YaHv.Csrm.Services;
using YaHv.Csrm.Services.Models.Origins;

namespace Yahv.Csrm.WebApi.Models
{
    /// <summary>
    /// 代仓储客户
    /// </summary>
    public class Client
    {
        /// <summary>
        /// 企业信息
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.Enterprise Enterprise { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public ClientGrade Grade { set; get; }
        /// <summary>
        /// 是否Vip
        /// </summary>
        public bool Vip { set; get; }
        /// <summary>
        /// 入仓号
        /// </summary>
        public string EnterCode { set; get; }
        /// <summary>
        /// 海关编码
        /// </summary>
        public string CustomsCode { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { set; get; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string Creator { set; get; }
        /// <summary>
        /// 客户性质
        /// </summary>
        public ClientType ClientNature { set; get; }
        /// <summary>
        /// 业务
        /// </summary>
        public ServiceType ServiceType { set; get; }
        public WsIdentity StorageType { set; get; }
        /// <summary>
        /// 是否代报关
        /// </summary>
        public bool IsDeclaretion { set; get; }
        /// <summary>
        /// 是否仓储审批
        /// </summary>
        public bool IsStorageService { set; get; }

        /// <summary>
        //是否收入入仓费
        /// </summary>
        public ChargeWHType ChargeWH { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.Contact Contact { set; get; }
        /// <summary>
        /// 联系人
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.WsInvoice Invoice { set; get; }
        /// <summary>
        /// 到货地址，提货地址
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.WsConsignee Consignee { set; get; }
        /// <summary>
        /// 营业执照
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.FileDescription BusinessLicense { set; get; }
        /// <summary>
        /// 登记证
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.FileDescription HKBusinessLicense { set; get; }


    }
    /// <summary>
    /// 代仓储供应商
    /// </summary>
    public class Supplier
    {
        /// <summary>
        /// 企业信息
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.Enterprise Enterprise { set; get; }
        /// <summary>
        /// 英文名称
        /// </summary>
        public string EnglishName { set; get; }
        /// <summary>
        /// 中文名称
        /// </summary>
        public string ChineseName { set; get; }
        /// <summary>
        /// 等级
        /// </summary>
        public SupplierGrade Grade { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string Creator { set; get; }
    }
    /// <summary>
    /// 客户与供应商
    /// </summary>
    public class ClientSuppler
    {
        /// <summary>
        /// 客户
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.Enterprise Client { set; get; }
        /// <summary>
        /// 供应商
        /// </summary>
        public Models.Supplier Supplier { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string Creator { set; get; }
    }
    /// <summary>
    /// 分配业务员跟单员
    /// </summary>
    public class AssinAdmin
    {
        /// <summary>
        /// 客户
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.Enterprise Client { set; get; }
        /// <summary>
        /// 业务员真实姓名
        /// </summary>
        public string ServiceManager { set; get; }
        /// <summary>
        /// 跟单员真实姓名
        /// </summary>
        public string Merchandiser { set; get; }
        /// <summary>
        /// 引荐人真实姓名
        /// </summary>
        public string Referrer { set; get; }
        /// <summary>
        /// 是否已报关审批
        /// </summary>
        public bool IsDeclaretion { set; get; }
        /// <summary>
        /// 是否已仓储审批
        /// </summary>
        public bool IsStorageService { set; get; }
    }

    public class ApiContract
    {
        public YaHv.Csrm.Services.Models.Origins.Enterprise Enterprise { set; get; }
        public string Creator { set; get; }

        //协议信息
        public Agreement Agreement { set; get; }
        /// <summary>
        /// 营业执照
        /// </summary>
        public YaHv.Csrm.Services.Models.Origins.FileDescription ServiceAgreement { set; get; }
    }

    public class Agreement
    {
        /// <summary>
        /// 合同协议开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 合同协议结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 代理费率
        /// </summary>
        public decimal AgencyRate { get; set; }

        /// <summary>
        /// 最低代理费
        /// </summary>
        public decimal MinAgencyFee { get; set; }
        /// <summary>
        /// 换汇方式：预换汇，90天内换汇
        /// </summary>
        public ExchangeMode ExchangeMode { get; set; }

        /// <summary>
        /// 开票类型:服务费发票，全额发票
        /// </summary>
        public BillingType InvoiceType { get; set; }

        /// <summary>
        /// 开票的税率 增值税 16%  服务费：3% 6% 固定不变，可以用系统常量表示
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 修改时间
        /// </summary>
        public DateTime UpdateDate { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public GeneralStatus Status { set; get; }

        /// <summary>
        ///  代理费的费用条款  add on 20200720
        /// </summary>
        public ClientFeeSettlement AgencyFeeClause { get; set; }

        /// <summary>
        /// 货款的费用条款 add on 20200717
        /// </summary>
        public ClientFeeSettlement ProductFeeClause { get; set; }

        /// <summary>
        /// 税费的费用条款 add on 20200717
        /// </summary>
        public ClientFeeSettlement TaxFeeClause { get; set; }

        /// <summary>
        /// 杂费的费用条款 add on 20200717
        /// </summary>
        public ClientFeeSettlement IncidentalFeeClause { get; set; }


    }


    public class ClientFeeSettlement
    {

        /// <summary>
        /// 费用类型
        /// </summary>
        public Underly.Enums.FeeType FeeType { get; set; }

        /// <summary>
        /// 账期类型
        /// </summary>
        public Underly.Enums.PeriodType PeriodType { get; set; }

        /// <summary>
        /// 费用使用的汇率类型
        /// </summary>
        public Underly.Enums.ExchangeRateType ExchangeRateType { get; set; }

        /// <summary>
        /// 约定汇率的值
        /// </summary>
        public decimal? ExchangeRateValue { get; set; }

        //约定期限（天）
        public int? DaysLimit { get; set; }

        /// <summary>
        /// 月结的日期
        /// </summary>
        public int? MonthlyDay { get; set; }

        /// <summary>
        /// 垫款上线
        /// </summary>
        public decimal? UpperLimit { get; set; }

        public string AdminID { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }


    }
    //受益人
    //public class Beneficiary
    //{
    //    public YaHv.Csrm.Services.Models.Origins.Enterprise WsClient { set; get; }
    //    #region 属性

    //    /// <summary>
    //    /// 是否可以带票采购
    //    /// </summary>
    //    public InvoiceType? InvoiceType { set; get; }
    //    /// <summary>
    //    /// 真实姓名
    //    /// </summary>
    //    public string RealName { get; set; }

    //    /// <summary>
    //    /// 开户银行
    //    /// </summary>
    //    public string Bank { get; set; }
    //    /// <summary>
    //    /// 开户行地址
    //    /// </summary>
    //    public string BankAddress { get; set; }
    //    /// <summary>
    //    /// 银行账户
    //    /// </summary>
    //    public string Account { get; set; }
    //    /// <summary>
    //    /// 银行编码 (国际)
    //    /// </summary>
    //    public string SwiftCode { get; set; }
    //    /// <summary>
    //    /// 汇款方式
    //    /// </summary>
    //    public Methord Methord { get; set; }
    //    /// <summary>
    //    /// 币种
    //    /// </summary>
    //    public Currency Currency { get; set; }
    //    /// <summary>
    //    /// 联系人姓名
    //    /// </summary>
    //    public string Name { set; get; }
    //    /// <summary>
    //    /// 联系电话
    //    /// </summary>
    //    public string Tel { get; set; }
    //    /// <summary>
    //    /// 联系人手机号
    //    /// </summary>
    //    public string Mobile { get; set; }
    //    /// <summary>
    //    /// 联系人邮箱
    //    /// </summary>
    //    public string Email { get; set; }
    //    /// <summary>
    //    /// 状态
    //    /// </summary>
    //    public ApprovalStatus Status { get; set; }
    //    /// <summary>
    //    /// 公司基本信息
    //    /// </summary>
    //    public YaHv.Csrm.Services.Models.Origins.Enterprise Enterprise { set; get; }
    //    /// <summary>
    //    /// 添加人
    //    /// </summary>
    //    public string Creator { get; set; }
    //    /// <summary>
    //    /// 默认受益人
    //    /// </summary>
    //    public bool IsDefault { set; get; }
    //    /// <summary>
    //    /// 创建日期
    //    /// </summary>
    //    public DateTime CreateDate { get; internal set; }
    //    /// <summary>
    //    /// 更新时间
    //    /// </summary>
    //    public DateTime UpdateDate { get; internal set; }
    //    #endregion
    //}

    public class Consignee
    {
        #region 属性
        /// <summary>
        /// 名称，主要用于库房的子库房名称
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 大赢家Code
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { set; get; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 地
        /// </summary>
        public string Land { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
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
        /// 状态
        /// </summary>
        public ApprovalStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
        /// <summary>
        /// 公司基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        #endregion
    }
    public class Consignor
    {
        public Enterprise WsClient { set; get; }
        #region 属性
        /// <summary>
        /// 名称，主要用于库房的子库房名称
        /// </summary>
        public string Title { set; get; }
        /// <summary>
        /// 大赢家Code
        /// </summary>
        public string DyjCode { set; get; }
        /// <summary>
        /// 地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { set; get; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 地
        /// </summary>
        public string Land { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
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
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
        /// <summary>
        /// 公司基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 国家或地区
        /// </summary>
        public string Place { set; get; }
        #endregion
    }
    public class Invoice
    {
        #region 属性
        /// <summary>
        /// 企业电话
        /// </summary>
        public string CompanyTel { set; get; }
        /// <summary>
        /// 发票类型 1 普通发票 2 增值税发票 3 海关发票
        /// </summary>
        public InvoiceType Type { get; set; }
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
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
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
        /// 收货地区
        /// </summary>
        public District District { set; get; }
        /// <summary>
        /// 省
        /// </summary>
        public string Province { set; get; }
        /// <summary>
        /// 市
        /// </summary>
        public string City { set; get; }
        /// <summary>
        /// 地
        /// </summary>
        public string Land { set; get; }
        /// <summary>
        /// 收货地址
        /// </summary>
        public string Address { set; get; }
        /// <summary>
        /// 邮编
        /// </summary>
        public string Postzip { set; get; }
        /// <summary>
        /// 交付方式
        /// </summary>
        public DeliveryType DeliveryType { set; get; }

        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { get; set; }
        /// <summary>
        /// 创建日期
        /// </summary>
        public DateTime CreateDate { get; internal set; }
        /// <summary>
        /// 更新时间
        /// </summary>
        public DateTime UpdateDate { get; internal set; }
        /// <summary>
        /// 公司基本信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        /// <summary>
        /// 添加人
        /// </summary>
        public string Creator { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 发票地址
        /// </summary>
        public string InvoiceAddrss { set; get; }
        #endregion
    }
    public class FileDesc
    {
        #region 属性
        /// <summary>
        /// 文件名称
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 文件类型：营业执照、委托合同
        /// </summary>
        public FileType Type { set; get; }
        /// <summary>
        /// 文件格式
        /// </summary>
        public string FileFormat { set; get; }
        /// <summary>
        /// 路径
        /// </summary>
        public string Url { set; get; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { set; get; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Summary { set; get; }
        /// <summary>
        /// 状态
        /// </summary>
        public ApprovalStatus Status { set; get; }
        /// <summary>
        /// 录入人
        /// </summary>
        public string Creator { set; get; }
        /// <summary>
        /// 企业信息
        /// </summary>
        public Enterprise Enterprise { set; get; }
        #endregion
    }

    public class nPayee
    {
        public Enterprise WsClient { set; get; }
        public Enterprise Enterprise { set; get; }
        #region 属性
        /// <summary>
        /// 唯一标识
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 私有供应商ID
        /// </summary>
        public string nSupplierID { set; get; }

        /// <summary>
        /// 开户银行
        /// </summary>
        public string Bank { get; set; }
        /// <summary>
        /// 开户行地址
        /// </summary>
        public string BankAddress { get; set; }
        /// <summary>
        /// 银行账户
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 银行编码 (国际)
        /// </summary>
        public string SwiftCode { get; set; }
        /// <summary>
        /// 汇款方式
        /// </summary>
        public Methord Methord { get; set; }
        /// <summary>
        /// 币种
        /// </summary>
        public Currency Currency { get; set; }
        /// <summary>
        /// 联系人姓名
        /// </summary>
        public string Name { set; get; }
        /// <summary>
        /// 联系电话
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
        /// 状态
        /// </summary>
        public GeneralStatus Status { get; set; }
        /// <summary>
        /// 录入人
        /// </summary>
        public string Creator { set; get; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { set; get; }
        /// <summary>
        /// 国家/地区
        /// </summary>
        public string Place { set; get; }

        #endregion
    }


}