//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using Yahv.Payments;
//using Yahv.Underly;
//using Yahv.Utils.Serializers;
//using YaHv.Csrm.Services.Models.Origins;

//namespace Yahv.Csrm.WebApp
//{
//    /// <summary>
//    /// 客户协议
//    /// </summary>
//    public class ClientAgreement
//    {
//        /// <summary>
//        /// 企业
//        /// </summary>
//        public Enterprise Enterprise { get; set; }
//        /// <summary>
//        /// 合同协议开始时间
//        /// </summary>
//        public DateTime StartDate { get; set; }

//        /// <summary>
//        /// 合同协议结束日期
//        /// </summary>
//        public DateTime EndDate { get; set; }

//        /// <summary>
//        /// 代理费率
//        /// </summary>
//        public decimal AgencyRate { get; set; }

//        /// <summary>
//        /// 最低代理费
//        /// </summary>
//        public decimal MinAgencyFee { get; set; }

//        /// <summary>
//        /// 是否可以预换汇，否则不可以在报关前换汇
//        /// </summary>
//        public bool IsPrePayExchange { get; set; }

//        /// <summary>
//        /// 是否选定在90天内换汇，超过90天就不允许换汇，如果不限制，可以在90天后换汇
//        /// </summary>
//        public bool IsLimitNinetyDays { get; set; }

//        /// <summary>
//        /// 开票类型( 0 代表全额发票 1代表 服务费发票)
//        /// </summary>
//        public int InvoiceType { get; set; }

//        /// <summary>
//        /// 开票的税率 增值税 16%  服务费：3% 6% 固定不变，可以用系统常量表示
//        /// </summary>
//        public decimal InvoiceTaxRate { get; set; }

//        /// <summary>
//        /// 创建时间
//        /// </summary>
//        internal DateTime CreateDate { get; set; }

//        /// <summary>
//        ///  代理费的费用条款
//        /// </summary>

//        public ClientFeeSettlement AgencyFeeClause { get; set; }


//        /// <summary>
//        /// 货款的费用条款
//        /// </summary>
//        public ClientFeeSettlement ProductFeeClause { get; set; }

//        /// <summary>
//        /// 税费的费用条款
//        /// </summary>
//        public ClientFeeSettlement TaxFeeClause { get; set; }

//        /// <summary>
//        /// 杂费的费用条款
//        /// </summary>
//        public ClientFeeSettlement IncidentalFeeClause { get; set; }
//        /// <summary>
//        ///客户服务协议文件
//        /// </summary>

//        public AgreementFile ClientFile { get; set; }

//        /// <summary>
//        /// 操作人
//        /// </summary>
//        public string Creator { get; set; }


//        /// <summary>
//        ///备注
//        /// </summary>
//        public string Summary { get; set; }

//    }
//    /// <summary>
//    /// 
//    /// </summary>
//    public class ClientFeeSettlement
//    {
//        /// <summary>
//        /// 费用类型（1-货款, 2-税款,3-代理费,4-杂费）
//        /// </summary>
//        public int FeeType { get; set; }

//        /// <summary>
//        /// 账期类型（0,1,2）
//        /// </summary>
//        public int PeriodType { get; set; }

//        /// <summary>
//        /// 费用使用的汇率类型(0,1,2,3)
//        /// </summary>
//        public int ExchangeRateType { get; set; }

//        /// <summary>
//        /// 约定汇率的值
//        /// </summary>
//        public decimal? ExchangeRateValue { get; set; }

//        /// <summary>
//        /// /约定期限（天）不必填
//        /// </summary>
//        public int? DaysLimit { get; set; }

//        /// <summary>
//        /// 月结的日期 不必填
//        /// </summary>
//        public int? MonthlyDay { get; set; }

//        /// <summary>
//        /// 垫款上线 不必填
//        /// </summary>
//        public decimal? UpperLimit { get; set; }

//    }
//    /// <summary>
//    /// 服务协议
//    /// </summary>
//    public class AgreementFile
//    {
//        /// <summary>
//        /// 类型
//        /// </summary>
//        public int Type { get; set; }
//        /// <summary>
//        /// 文件名
//        /// </summary>
//        public string Name { get; set; }
//        /// <summary>
//        /// 文件格式
//        /// </summary>
//        public string FileFormat { get; set; }
//        /// <summary>
//        /// 文件地址
//        /// </summary>
//        public string Url { get; set; }
//        /// <summary>
//        /// 创建时间
//        /// </summary>
//        public string CreateDate { get; set; }
//        /// <summary>
//        /// 备注
//        /// </summary>
//        public string Summary { get; set; }

//    }
//    public class XdtContract : ClientAgreement
//    {
//        public XdtContract(string clientid)
//        {
//            var wsclient = Erp.Current.Whs.WsClients[clientid];
//            Contract contract = wsclient.Contract;
//            #region 合同信息
//            this.Enterprise = contract.Enterprise;
//            this.StartDate = contract.StartDate;
//            this.EndDate = contract.EndDate;
//            this.AgencyRate = contract.AgencyRate;
//            this.MinAgencyFee = contract.MinAgencyFee;
//            this.IsPrePayExchange = contract.ExchangeMode == Underly.ExchangeMode.PrePayExchange;
//            this.IsLimitNinetyDays = contract.ExchangeMode == Underly.ExchangeMode.LimitNinetyDays;
//            this.InvoiceType = (int)contract.InvoiceType;
//            this.InvoiceTaxRate = contract.InvoiceTaxRate;
//            this.Creator = contract.Creator.ID;
//            this.Summary = contract.Summary;
//            this.CreateDate = contract.CreateDate;
//            this.ClientFile = new AgreementFile
//            {
//                Type = (int)FileType.ServiceAgreement,
//                Name = contract.ServiceAgreement.Name,
//                FileFormat = contract.ServiceAgreement.FileFormat,
//                Url = contract.ServiceAgreement.Url,
//                CreateDate = contract.ServiceAgreement.CreateDate.ToString(),
//                Summary = contract.ServiceAgreement.Summary
//            };
//            #endregion

//        }
//        public object Unify()
//        {
//            string apiurl = System.Configuration.ConfigurationManager.AppSettings["UnifyApiUrl"];
//            if (!string.IsNullOrWhiteSpace(apiurl))
//            {
//                #region 条款信息
//                var catalog = PaymentManager.Npc["深圳市芯达通供应链管理有限公司", this.Enterprise.ID]["代仓储"];
//                var agencyFeeClause = catalog.DebtTerm["代理费"];
//                var productFeeClause = catalog.DebtTerm["货款"];
//                var taxFeeClause = catalog.DebtTerm["税款"];
//                var incidentalFeeClause = catalog.DebtTerm["杂费"];
//                if (agencyFeeClause.SettlementType != 0 && productFeeClause.SettlementType != 0 && taxFeeClause.SettlementType != 0 && incidentalFeeClause.SettlementType != 0)
//                {
//                    this.ProductFeeClause = new ClientFeeSettlement
//                    {
//                        FeeType = 1,
//                        PeriodType = (int)productFeeClause.SettlementType,
//                        ExchangeRateType = ToExchangeRateType(productFeeClause.ExchangeType),
//                        ExchangeRateValue = 0,
//                        DaysLimit = GetDays(productFeeClause.SettlementType, SettlementType.DueTime, productFeeClause.Months, productFeeClause.Days, productFeeClause.CreateDate),
//                        MonthlyDay = GetDays(productFeeClause.SettlementType, SettlementType.Month, productFeeClause.Months, productFeeClause.Days, productFeeClause.CreateDate),
//                        UpperLimit = catalog.Credit["货款"][Currency.CNY].Total
//                    };
//                    this.AgencyFeeClause = new ClientFeeSettlement
//                    {
//                        FeeType = 3,
//                        PeriodType = (int)agencyFeeClause.SettlementType,
//                        ExchangeRateType = ToExchangeRateType(agencyFeeClause.ExchangeType),
//                        ExchangeRateValue = 0,
//                        DaysLimit = GetDays(agencyFeeClause.SettlementType, SettlementType.DueTime, agencyFeeClause.Months, agencyFeeClause.Days, agencyFeeClause.CreateDate),
//                        MonthlyDay = GetDays(agencyFeeClause.SettlementType, SettlementType.Month, agencyFeeClause.Months, agencyFeeClause.Days, agencyFeeClause.CreateDate),
//                        UpperLimit = catalog.Credit["代理费"][Currency.CNY].Total
//                    };
//                    this.TaxFeeClause = new ClientFeeSettlement
//                    {
//                        FeeType = 2,
//                        PeriodType = (int)taxFeeClause.SettlementType,
//                        ExchangeRateType = ToExchangeRateType(taxFeeClause.ExchangeType),
//                        ExchangeRateValue = 0,
//                        DaysLimit = GetDays(taxFeeClause.SettlementType, SettlementType.DueTime, taxFeeClause.Months, taxFeeClause.Days, taxFeeClause.CreateDate),
//                        MonthlyDay = GetDays(taxFeeClause.SettlementType, SettlementType.Month, taxFeeClause.Months, taxFeeClause.Days, taxFeeClause.CreateDate),
//                        UpperLimit = catalog.Credit["税款"][Currency.CNY].Total
//                    };
//                    this.IncidentalFeeClause = new ClientFeeSettlement
//                    {
//                        FeeType = 4,
//                        PeriodType = (int)incidentalFeeClause.SettlementType,
//                        ExchangeRateType = ToExchangeRateType(incidentalFeeClause.ExchangeType),
//                        ExchangeRateValue = 0,
//                        DaysLimit = GetDays(incidentalFeeClause.SettlementType, SettlementType.DueTime, incidentalFeeClause.Months, incidentalFeeClause.Days, incidentalFeeClause.CreateDate),
//                        MonthlyDay = GetDays(incidentalFeeClause.SettlementType, SettlementType.Month, incidentalFeeClause.Months, incidentalFeeClause.Days, incidentalFeeClause.CreateDate),
//                        UpperLimit = catalog.Credit["杂费"][Currency.CNY].Total
//                    };
//                    var response = HttpClientHelp.HttpPostRaw(apiurl + "/agreement", this.Json());
//                    return response;
//                }
//                #endregion

//            }
//            return null;
//        }

//        /// <summary>
//        /// Crm汇率枚举转换为芯达通
//        /// </summary>
//        /// <param name="exchange"></param>
//        /// <returns></returns>
//        private int ToExchangeRateType(ExchangeType exchange)
//        {
//            switch (exchange)
//            {
//                //海关汇率
//                case ExchangeType.Customs:
//                    return 1;
//                //实时汇率
//                case ExchangeType.Floating:
//                    return 2;
//                //预设汇率
//                case ExchangeType.Preset:
//                    return 3;
//                default:
//                    return 0;
//            }
//        }

//        /// <summary>
//        /// 获取天数
//        /// </summary>
//        /// <param name="type">汇率类型</param>
//        /// <param name="currType">当前汇率</param>
//        /// <param name="month">月数</param>
//        /// <param name="day">还款日</param>
//        /// <param name="createDate">合同创建时间</param>
//        /// <returns></returns>
//        private int? GetDays(SettlementType type, SettlementType currType, int month, int day, DateTime createDate)
//        {
//            if (type != currType) return null;

//            //约定期限
//            if (type == SettlementType.DueTime)
//            {
//                return (createDate.AddMonths(month).AddDays(day) - createDate).Days;
//            }
//            //月结
//            else if (type == SettlementType.Month)
//            {
//                return day;
//            }

//            return null;
//        }
//    }
//}