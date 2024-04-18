using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// Post到接口的变量数据定义
    /// </summary>
    public class ApiModel
    {
        /// <summary>
        /// 保存客户信息传入参数
        /// </summary>
        public class ClientModel
        {
            /// <summary>
            /// 企业信息
            /// </summary>
            public EnterpriseObj Enterprise { get; set; }
            /// <summary>
            /// 客户编号
            /// </summary>
            public string EnterCode { get; set; }

            /// <summary>
            /// 海关编码(不必填)
            /// </summary>

            public string CustomsCode { get; set; }

            /// <summary>
            /// 会员等级
            /// </summary>
            public int Grade { get; set; }

            /// <summary>
            /// 会员等级
            /// </summary>
            public bool Vip { get; set; }

            /// <summary>
            /// 备注
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 创建人(业务员的realName)
            /// </summary>
            public string Creator { get; set; }
            /// <summary>
            /// 客户状态
            /// </summary>
            public int Status { get; set; }
            public string CreateDate { get; set; }
            public string UpdateDate { get; set; }

            /// <summary>
            /// 联系人
            /// </summary>
            public ApiContact Contact { get; set; }

            public BusinessLicense BusinessLicense { get; set; }

            /// <summary>
            /// 登记证
            /// </summary>
            public BusinessLicense HKBusinessLicense { get; set; }
            public int? ClientNature { get; set; }
            /// <summary>
            /// 业务类型
            /// </summary>
            public int ServiceType { get; set; }
            /// <summary>
            /// 仓储类型
            /// </summary>
            public int StorageType { get; set; }

            public bool? IsDeclaretion { get; set; }

            public bool? IsStorageService { get; set; }

            /// <summary>
            /// 是否收取入仓费
            /// </summary>
            public int ChargeWH { get; set; }
        }

        public class BusinessLicense
        {

            /// <summary>
            /// 类型
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// 文件名
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 文件格式
            /// </summary>
            public string FileFormat { get; set; }
            /// <summary>
            /// 文件地址
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public string CreateDate { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 状态
            /// </summary>
            public int Status { get; set; }
        }
        /// <summary>
        /// 客户附件
        /// </summary>
        public class ApiClientFile
        {
            /// <summary>
            ///客户ID
            /// </summary>
            public string ClientID { get; set; }
            /// <summary>
            /// 文件名
            /// </summary>
            public string FileName { get; set; }
            /// <summary>
            /// /文件URL地址
            /// </summary>
            public string FileURL { get; set; }

        }

        /// <summary>
        /// 供应商
        /// </summary>
        public class ClientSupplier
        {

            /// <summary>
            /// 企业
            /// </summary>
            public EnterpriseObj client { get; set; }
            /// <summary>
            /// 供应商
            /// </summary>
            public Supplier supplier { get; set; }
            /// <summary>
            ///操作人
            /// </summary>
            public string Creator { get; set; }
        }

        public class Supplier
        {
            /// <summary>
            /// 企业
            /// </summary>
            public EnterpriseObj Enterprise { get; set; }

            /// <summary>
            /// 英文名称
            /// </summary>
            public string EnglishName { get; set; }

            /// <summary>
            /// 中文名称
            /// </summary>
            public string ChineseName { get; set; }
            /// <summary>
            /// 备注（不必填）
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            ///级别
            /// </summary>
            public int Grade { get; set; }

            public int Status { get; set; }

        }
        /// <summary>
        ///供应商账户
        /// </summary>
        public class SupplierBank
        {
            public EnterpriseObj Enterprise { get; set; }
            /// <summary>
            /// 供应商名称
            /// </summary>
            public string SupplierName { get; set; }

            /// <summary>
            /// 3海关、2增票、1普票、0无法开票（Yahv.Underly.InvoiceType）
            /// </summary>
            public int InvoiceType { get; set; }
            /// <summary>
            /// 企业名称
            /// </summary>
            public string RealName { get; set; }

            /// <summary>
            /// 开户行
            /// </summary>
            public string Bank { get; set; }
            /// <summary>
            /// 开户行地址
            /// </summary>
            public string BankAddress { get; set; }
            /// <summary>
            /// 账号
            /// </summary>

            public string Account { get; set; }
            /// <summary>
            /// 银行编码 (国际)
            /// </summary>
            public string SwiftCode { get; set; }
            /// <summary>
            /// 汇款方式(TT,支付宝)
            /// </summary>
            public int Methord { get; set; }
            /// <summary>
            /// 币种
            /// </summary>
            public int Currency { get; set; }
            /// <summary>
            /// 地区
            /// </summary>
            public int District { get; set; }
            /// <summary>
            /// 姓名(不必填)
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 电话(不必填)
            /// </summary>
            public string Tel { get; set; }
            /// <summary>
            /// 手机号(不必填)
            /// </summary>
            public string Mobile { get; set; }
            /// <summary>
            /// 邮箱
            /// </summary>
            public string Email { get; set; }
            ///// <summary>
            ///// 状态 200  400 
            ///// </summary>
            //public string Status { get; set; }
            ///// <summary>
            ///// 
            ///// </summary>
            //public string CreateDate { get; set; }
            ///// <summary>
            ///// 
            ///// </summary>
            //public string UpdateDate { get; set; }

            ///// <summary>
            ///// 银行名称(英文)
            ///// </summary>
            //public string BankName { get; set; }
            ///// <summary>
            ///// 银行地址(英文)
            ///// </summary>
            //public string BankAddress { get; set; }

            ///// <summary>
            ///// 银行账号
            ///// </summary>
            //public string BankAccount { get; set; }
            ///// <summary>
            ///// 银行国际代码
            ///// </summary>
            //public string SwiftCode { get; set; }
            ///// <summary>
            ///// 备注（不必填）
            ///// </summary>
            //public string Summary { get; set; }
        }

        /// <summary>
        /// 供应商提货地址
        /// </summary>
        public class SupplierAddress
        {
            /// <summary>
            /// 供应商
            /// </summary>
            public EnterpriseObj Enterprise { get; set; }
            /// <summary>
            /// 客户信息
            /// </summary>
            public EnterpriseObj WsClient { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string Title { get; set; }
            public string DyjCode { get; set; }

            public int District { get; set; }
            /// <summary>
            /// 省
            /// </summary>
            public string Province { get; set; }

            /// <summary>
            /// 市
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// 区
            /// </summary>
            public string Land { get; set; }
            /// <summary>
            /// 收件单位
            /// </summary>
            public string Receiver { get; set; }
            /// <summary>
            /// 收件人
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 电话
            /// </summary>
            public string Mobile { get; set; }

            /// <summary>
            /// 地址
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// 是否默认地址
            /// </summary>
            public bool IsDefault { get; set; }

            /// <summary>
            /// 电子邮箱(不必填)
            /// </summary>
            public string Email { get; set; }
            /// <summary>
            /// 备注(不必填)
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Status { get; set; }

            public string CreateDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UpdateDate { get; set; }

            public string Postzip { get; set; }
            /// <summary>
            /// 操作人
            /// </summary>
            public string Creator { get; set; }
            /// <summary>
            /// 国家/ 地区
            /// </summary>
            public string Place { get; set; }

        }


        /// <summary>
        /// 客户收件地址请求参数
        /// </summary>
        public class ClientConsigin
        {

            public EnterpriseObj Enterprise { get; set; }
            /// <summary>
            /// 收件单位
            /// </summary>
            public string Title { get; set; }
            public string DyjCode { get; set; }

            public int District { get; set; }
            /// <summary>
            /// 省
            /// </summary>
            public string Province { get; set; }

            /// <summary>
            /// 市
            /// </summary>
            public string City { get; set; }

            /// <summary>
            /// 区
            /// </summary>
            public string Land { get; set; }
            ///// <summary>
            ///// 
            ///// </summary>
            public string Creator { get; set; }
            /// <summary>
            /// 收件人
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 手机号
            /// </summary>
            public string Mobile { get; set; }
            /// <summary>
            /// 电话
            /// </summary>
            public string Tel { get; set; }
            /// <summary>
            /// 电子邮箱(不必填)
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// 地址
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// 是否默认地址
            /// </summary>
            public bool IsDefault { get; set; }


            /// <summary>
            /// 备注(不必填)
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Status { get; set; }

            public string CreateDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UpdateDate { get; set; }

            public string Postzip { get; set; }
            /// <summary>
            ///国家/地区
            /// </summary>
            public string Place { get; set; }

        }

        /// <summary>
        /// 用户账号
        /// </summary>
        public class ClientAccount
        {
            /// <summary>
            /// 企业
            /// </summary>
            public EnterpriseObj Enterprise { get; set; }
            /// <summary>
            /// 用户名/账号/登录账号
            /// </summary>
            public string UserName { get; set; }

            /// <summary>
            /// 登录密码(无加密的密码)
            /// </summary>
            public string Password { get; set; }

            /// <summary>
            /// 手机号码
            /// </summary>
            public string Mobile { get; set; }

            /// <summary>
            /// 邮箱(可为空)
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// 是否主账号
            /// </summary>
            public bool IsMain { get; set; }

            /// <summary>
            ///真实姓名
            /// </summary>
            public string RealName { get; set; }
            /// <summary>
            /// 备注(不必填)
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 添加人
            /// </summary>
            public string CreateDate { get; set; }
            public string UpdateDate { get; set; }
            public int Status { get; set; }

            public string QQ { get; set; }
            public string Wx { get; set; }

        }

        /// <summary>
        /// 客户附件
        /// </summary>
        public class ClientFile
        {
            /// <summary>
            /// 企业
            /// </summary>
            public EnterpriseObj Enterprise { get; set; }

            public string Name { set; get; }
            /// <summary>
            /// 文件类型：营业执照、委托合同
            /// </summary>
            public int Type { set; get; }
            /// <summary>
            /// 文件格式
            /// </summary>
            public string FileFormat { set; get; }
            /// <summary>
            /// 路径
            /// </summary>
            public string Url { set; get; }


            /// <summary>
            /// 状态
            /// </summary>
            public int Status { set; get; }


            /// <summary>
            ///摘要(不必填)
            /// </summary>
            public string Summary { get; set; }

            /// <summary>
            /// 操作人
            /// </summary>
            public string Creator { get; set; }

        }

        /// <summary>
        /// 新增或修改客户发票
        /// </summary>
        public class ClientInvoice
        {
            /// <summary>
            ///企业
            /// </summary>
            public EnterpriseObj Enterprise { get; set; }

            public string InvoiceAddrss { get; set; }
            /// <summary>
            /// 开户行
            /// </summary>
            public string Bank { get; set; }

            /// <summary>
            /// 开户行地址
            /// </summary>
            public string BankAddress { get; set; }

            /// <summary>
            /// 纳税人识别号
            /// </summary>
            public string TaxperNumber { get; set; }

            /// <summary>
            /// 银行账号
            /// </summary>
            public string Account { get; set; }
            /// <summary>
            /// 发票交付方式（默认值为1 ，邮寄）
            /// </summary>
            public int DeliveryType { get; set; }

            /// <summary>
            /// 收件地址(省市区之间要有空格 ex:黑龙江 齐齐哈尔 龙沙区 稻花村)
            /// </summary>
            public string Address { get; set; }

            /// <summary>
            /// 联系人
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 电话
            /// </summary>

            public string Tel { get; set; }
            /// <summary>
            /// 邮箱
            /// </summary>
            public string Email { get; set; }

            /// <summary>
            /// 手机号
            /// </summary>
            public string Mobile { get; set; }
            /// <summary>
            /// 邮编
            /// </summary>

            public string Postzip { get; set; }
            /// <summary>
            /// 备注(不必填)
            /// </summary>
            public string Summary { get; set; } = string.Empty;
            /// <summary>
            /// 发票信息的电话
            /// </summary>
            public string CompanyTel { get; set; }

            /// <summary>
            /// Type
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// 省
            /// </summary>
            public string Province { get; set; } = string.Empty;

            /// <summary>
            /// 市
            /// </summary>
            public string City { get; set; } = string.Empty;

            /// <summary>
            /// 区
            /// </summary>
            public string Land { get; set; } = string.Empty;

            /// <summary>
            /// 区域
            /// </summary>
            public int District { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Status { get; set; }

            public string CreateDate { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public string UpdateDate { get; set; }


            /// <summary>
            /// 操作人
            /// </summary>
            public string Creator { get; set; }

            ///// <summary>
            ///// 电话
            ///// </summary>
            //public string Tel { get; set; } = string.Empty;

            ///// <summary>
            ///// 银行名称
            ///// </summary>
            //public string BankName { get; set; } = string.Empty;

            ///// <summary>
            ///// 银行账号
            ///// </summary>
            //public string BankAccount { get; set; } = string.Empty;
            ///// <summary>
            ///// 发票交付方式
            ///// </summary>
            //public int DeliveryType { get; set; }
            ///// <summary>
            ///// 备注(不必填)
            ///// </summary>
            //public string Summary { get; set; } = string.Empty;

            ///// <summary>
            ///// 收件人
            ///// </summary>
            //public string ConsigneeName { get; set; }
            ///// <summary>
            ///// 收件地址
            ///// </summary>
            //public string ConsigneeAddress { get; set; }

            ///// <summary>
            ///// 手机
            ///// </summary>
            //public string ConsigneeMobile { get; set; }

            ///// <summary>
            ///// 电话
            ///// </summary>
            //public string ConsigneeTel { get; set; }
            ///// <summary>
            ///// 邮箱
            ///// </summary>
            //public string ConsigneeEmail { get; set; }

        }

        /// <summary>
        /// 客户分配人员
        /// </summary>
        public class ClientAssign
        {
            public EnterpriseObj Client { get; set; }
            /// <summary>
            /// 业务员名称
            /// </summary>
            public string ServiceManager { get; set; }
            /// <summary>
            /// 跟单员名称
            /// </summary>
            public string Merchandiser { get; set; }

            public string Referrer { get; set; }
            /// <summary>
            /// 备注(不必填)
            /// </summary>
            public string Summary { get; set; }
            public bool IsDeclaretion { set; get; }
            public bool IsStorageService { set; get; }

        }

        /// <summary>
        /// 客户协议
        /// </summary>
        public class ClientAgreement
        {
            /// <summary>
            /// 企业
            /// </summary>
            public EnterpriseObj Enterprise { get; set; }

            public Agreement Agreement { get; set; }
            /// <summary>
            ///客户服务协议文件
            /// </summary>

            public AgreementFile ServiceAgreement { get; set; }

            /// <summary>
            /// 操作人
            /// </summary>
            public string Creator { get; set; }
        }

        /// <summary>
        /// 合同内容
        /// </summary>
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
            /// 1 预换汇 ,2  90天内换汇
            /// </summary>
            public int ExchangeMode { get; set; }

            /// <summary>
            /// 开票类型( 0 代表全额发票 1代表 服务费发票)
            /// </summary>
            public int InvoiceType { get; set; }

            /// <summary>
            /// 开票的税率 增值税 16%  服务费：3% 6% 固定不变，可以用系统常量表示
            /// </summary>
            public decimal InvoiceTaxRate { get; set; }


            /// <summary>
            ///  代理费的费用条款  add on 20200717
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


            /// <summary>
            ///备注
            /// </summary>
            public string Summary { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public string CreateDate { get; set; }

            /// <summary>
            /// 更新时间
            /// </summary>
            public string UpdateDate { get; set; }

        }
        /// <summary>
        /// 服务协议
        /// </summary>
        public class AgreementFile
        {
            /// <summary>
            /// 类型
            /// </summary>
            public int Type { get; set; }
            /// <summary>
            /// 文件名
            /// </summary>
            public string Name { get; set; }
            /// <summary>
            /// 文件格式
            /// </summary>
            public string FileFormat { get; set; }
            /// <summary>
            /// 文件地址
            /// </summary>
            public string Url { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public string CreateDate { get; set; }
            /// <summary>
            /// 备注
            /// </summary>
            public string Summary { get; set; }

        }

        /// <summary>
        /// 联系人
        /// </summary>
        public class ApiContact
        {
            /// <summary>
            /// 联系人类型(1 Online线上、2 Offline线下、3 Sales销售、4 Pruchaser 采购)
            /// </summary>
            public int Type { get; set; }
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
            /// 传真
            /// </summary>
            public string Fax { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public int Status { get; set; }
            /// <summary>
            /// 
            /// </summary>
            public string CreateDate { get; set; }
            public string UpdateDate { get; set; }

        }
    }
}