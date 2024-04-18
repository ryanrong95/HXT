using NtErp.Crm.Services.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Crm.Services.Models
{
    public class ClientExtends
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string ID
        {
            get;set;
        }
        /// <summary>
        /// 客户名称
        /// </summary>
        public string Name
        {
            get; set;
        }

        /// <summary>
        /// 企业性质
        /// </summary>
        public CustomerNature EnterpriseProperty
        {
            get; set;
        }
        /// <summary>
        /// 国别
        /// </summary>
        public CustomerArea Area
        {
            get; set;
        }

        /// <summary>
        /// 注册资本
        /// </summary>
        public string RegisteredCapital
        {
            get; set;
        }

        /// <summary>
        /// 币种
        /// </summary>
        public CurrencyType Currency
        {
            get; set;
        }

        /// <summary>
        /// 成立日期
        /// </summary>
        public string EstablishmentDate
        {
            get; set;
        }

        /// <summary>
        /// 经营期限
        /// </summary>
        public string OperatingPeriod
        {
            get; set;
        }

        /// <summary>
        /// 注册地址
        /// </summary>
        public string RegisteredAddress
        {
            get;set;
        }

        /// <summary>
        /// 办公地址
        /// </summary>
        public string OfficeAddress
        {
            get;set;
        }

        /// <summary>
        /// 网址
        /// </summary>
        public string Site
        {
            get; set;
        }

        /// <summary>
        /// 经营范围
        /// </summary>
        public string BusinessScope
        {
            get; set;
        }

        /// <summary>
        /// 社会统一信用代码
        /// </summary>
        public string CUSCC
        {
            get;set;
        }

        /// <summary>
        /// 客户类型
        /// </summary>
        public CustomerType CustomerType
        {
            get; set;
        }

        /// <summary>
        /// 客户级别
        /// </summary>
        public CustomerLevel? CustomerLevel
        {
            get; set;
        }

        /// <summary>
        /// 代理品牌
        /// </summary>
        public string AgentBrand
        {
            get; set;
        }

        /// <summary>
        /// 我方合作公司
        /// </summary>
        public string CompanyID
        {
            get;set;
        }

        /// <summary>
        /// 业务类型
        /// </summary>
        public BusinessType BusinessType
        {
            get; set;
        }

        /// <summary>
        /// 所属行业
        /// </summary>
        public string ReIndustry
        {
            get; set;
        }

        /// <summary>
        /// 主要产品
        /// </summary>
        public string IndustryInvolved
        {
            get; set;
        }
        
        /// <summary>
        /// 保护级别
        /// </summary>
        public ProtectLevel? ProtectLevel
        {
            get; set;
        }

        /// <summary>
        /// 保护范围
        /// </summary>
        public string ProtectionScope
        {
            get; set;
        }

        /// <summary>
        /// 授信额度
        /// </summary>
        public string CreditLimit
        {
            get;set;
        }

        /// <summary>
        /// 授信账期
        /// </summary>
        public string CreditPayment
        {
            get; set;
        }

        /// <summary>
        /// 客户状态
        /// </summary>
        public CustomerStatus CustomerStatus
        {
            get; set;
        }
        
        /// <summary>
        /// 特殊包装
        /// </summary>
        public string ExtraPacking
        {
            get; set;
        }

        /// <summary>
        /// 特殊供应商
        /// </summary>
        public string SpecialSupplier
        {
            get; set;
        }

        /// <summary>
        /// 信息来源
        /// </summary>
        public string InformationSource
        {
            get; set;
        }

        /// <summary>
        /// 自定义客户编号
        /// </summary>
        public string AdminCode
        {
            get; set;
        }

        /// <summary>
        /// 其他备注信息
        /// </summary>
        public string Summary
        {
            get;set;
        }

        /// <summary>
        /// 区域
        /// </summary>
        public object AreaID
        {
            get;set;
        }

        public ImportantLevel? ImportantLevel
        {
            get;set;
        }
    }
}
