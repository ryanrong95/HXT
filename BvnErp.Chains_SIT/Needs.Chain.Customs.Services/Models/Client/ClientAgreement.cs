using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Npoi;
using NPOI.XWPF.UserModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 客户协议条款
    /// </summary>
    [Serializable]
    public class ClientAgreement : IUnique, IPersist
    {
        string id;
        public string ID
        {
            get
            {
                //结合有效数据，以及子项的ID（子项ID已做变更验证）验证是否需插入
                return this.id ?? string.Concat(this.ClientID, this.StartDate, this.EndDate, this.AgencyRate, this.MinAgencyFee, this.IsPrePayExchange, this.IsLimitNinetyDays, this.IsTen, this.InvoiceType, this.InvoiceTaxRate).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientID { get; set; }

        /// <summary>
        /// 协议编号
        /// </summary>
        public string AgreementCode { get; set; }

        /// <summary>
        /// 合同协议开始时间
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 合同协议结束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 基础收费
        /// </summary>
        public decimal? PreAgency {  get; set; }

        /// <summary>
        /// 代理费率
        /// </summary>
        public decimal AgencyRate { get; set; }

        /// <summary>
        /// 最低代理费
        /// </summary>
        public decimal MinAgencyFee { get; set; }

        /// <summary>
        /// 是否可以预换汇，否则不可以在报关前换汇
        /// </summary>
        public bool IsPrePayExchange { get; set; }

        /// <summary>
        /// 是否选定在90天内换汇，超过90天就不允许换汇，如果不限制，可以在90天后换汇
        /// </summary>
        public bool IsLimitNinetyDays { get; set; }

        /// <summary>
        /// 是否使用十点汇率 否则 用九点半
        /// </summary>
        public PEIsTen IsTen { get; set; }

        /// <summary>
        /// 开票类型
        /// </summary>
        public Enums.InvoiceType InvoiceType { get; set; }

        /// <summary>
        /// 开票的税率 增值税 16%  服务费：3% 6% 固定不变，可以用系统常量表示
        /// </summary>
        public decimal InvoiceTaxRate { get; set; }

        private ClientFeeSettlement agencyFeeClause;

        /// <summary>
        /// 代理费的费用条款
        /// </summary>
        public ClientFeeSettlement AgencyFeeClause
        {
            get
            {
                if (agencyFeeClause == null)
                {
                    return this.ClientFeeSettlementItems.Where(item => item.FeeType == FeeType.AgencyFee && item.Status == Status.Normal).SingleOrDefault();
                }
                return agencyFeeClause;
            }
            set
            {
                this.agencyFeeClause = value;
            }
        }

        private ClientFeeSettlement productFeeClause;

        /// <summary>
        /// 货款的费用条款
        /// </summary>
        public ClientFeeSettlement ProductFeeClause
        {
            get
            {
                if (productFeeClause == null)
                {
                    return this.ClientFeeSettlementItems.Where(item => item.FeeType == FeeType.Product && item.Status == Status.Normal).SingleOrDefault();
                }
                return productFeeClause;
            }
            set
            {
                this.productFeeClause = value;
            }
        }

        private ClientFeeSettlement taxFeeClause;

        /// <summary>
        /// 税费的费用条款
        /// </summary>
        public ClientFeeSettlement TaxFeeClause
        {
            get
            {
                if (taxFeeClause == null)
                {
                    return this.ClientFeeSettlementItems.Where(item => item.FeeType == FeeType.Tax && item.Status == Status.Normal).SingleOrDefault();
                }
                return taxFeeClause;
            }
            set
            {
                this.taxFeeClause = value;
            }
        }

        private ClientFeeSettlement incidentalFeeClause;

        /// <summary>
        /// 杂费的费用条款
        /// </summary>
        public ClientFeeSettlement IncidentalFeeClause
        {
            get
            {
                if (incidentalFeeClause == null)
                {
                    return this.ClientFeeSettlementItems.Where(item => item.FeeType == FeeType.Incidental && item.Status == Status.Normal).SingleOrDefault();
                }
                return incidentalFeeClause;
            }
            set
            {
                this.incidentalFeeClause = value;
            }
        }

        internal IEnumerable<ClientFeeSettlement> ClientFeeSettlementItems { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string AdminID { get; set; }

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public string NewValue { get; set; }
        public string OldValue { get; set; }
        public string ChangeType { get; set; }

        /// <summary>
        /// 当导出协议文档后发生
        /// </summary>
        public event AgreementExportedEventHanlder AgreementExported;

        public ClientAgreement()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
            this.AgreementExported += ClientAgreement_AgreementExported;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Count(item => item.ID == this.ID);

                //失效协议
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientAgreements>(new { Status = (int)Enums.Status.Delete }, item => item.ClientID == this.ClientID);

                if (count == 0)
                {
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }

            this.AgencyFeeClause.Enter();
            this.ProductFeeClause.Enter();
            this.TaxFeeClause.Enter();
            this.IncidentalFeeClause.Enter();

            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

        #region  代报关协议
        /// <summary>
        /// 代报关协议
        /// </summary>
        /// <returns></returns>
        public XWPFDocument ToWord()
        {
            var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\templates\\服务协议模板.docx");
            var npoi = new NPOIHelper(tempPath);
            var replaceText = new Dictionary<string, string>();

            var client = new Views.ClientsView()[this.ClientID];

            #region 组装内容

            replaceText.Add("{CreateTime}", this.StartDate.ToString("yyyy年MM月dd日"));
            replaceText.Add("{CompanyName}", client.Company.Name);
            replaceText.Add("{RegisteredAddress}", client.Company.Address);
            replaceText.Add("{LegalPerson}", client.Company.Corporate);
            replaceText.Add("{ServiceChargePoint}", (this.AgencyRate * 100).ToRound(2).ToString());
            replaceText.Add("{ServiceChargePointPC}", (this.AgencyRate * 0.01M).ToString());
            replaceText.Add("{MinimumAgent}", this.MinAgencyFee.ToRound(2).ToString());
            replaceText.Add("{AgreementStartDate}", this.StartDate.ToString("yyyy年MM月dd日"));
            replaceText.Add("{AgreementEndDate}", this.EndDate.ToString("yyyy年MM月dd日"));

            replaceText.Add("{CreditLevel}", client.ClientRank.GetDescription());

            replaceText.Add("{Date}", this.StartDate.ToString("yyyyMMdd"));
            replaceText.Add("{Year}", this.StartDate.Year.ToString());
            replaceText.Add("{Month}", this.StartDate.Month.ToString());
            replaceText.Add("{Day}", this.StartDate.Day.ToString());
            replaceText.Add("{AgencyRate}", (this.AgencyRate * 100M).ToRound(2).ToString() + "%");

            //开票点位
            replaceText.Add("{InvoicePoint}", (this.InvoiceTaxRate + 1).ToRound(2).ToString());

            //换汇
            replaceText.Add("{GoodsPayExchange90}", this.IsLimitNinetyDays ? "☑" : "□");
            replaceText.Add("{GoodsPayExchangePre}", this.IsPrePayExchange ? "☑" : "□");

            //换汇汇率
            replaceText.Add("{IsTen}", this.IsTen  == PEIsTen.Ten ? "10:00" : "09:30" );

            #region 货款

            //货款
            replaceText.Add("{GoodsPaymentTypePre}", this.ProductFeeClause.PeriodType == PeriodType.PrePaid ? "☑" : "□");
            replaceText.Add("{GoodsPaymentTypeLimit}", this.ProductFeeClause.PeriodType == PeriodType.AgreedPeriod ? "☑" : "□");
            replaceText.Add("{GoodsPaymentTypeMon}", this.ProductFeeClause.PeriodType == PeriodType.Monthly ? "☑" : "□");

            if (this.ProductFeeClause.PeriodType == PeriodType.PrePaid)
            {
                replaceText.Add("{GoodsPaymentMonthDay}", "");
                replaceText.Add("{GoodsPaymentMax}", "");
                replaceText.Add("{GoodsPaymentMonMax}", "");
                replaceText.Add("{GoodspaymentLimitDay}", "");
            }
            else if (this.ProductFeeClause.PeriodType == PeriodType.AgreedPeriod)
            {
                replaceText.Add("{GoodsPaymentMonthDay}", "");
                replaceText.Add("{GoodsPaymentMonMax}", "");
                replaceText.Add("{GoodsPaymentMax}", this.ProductFeeClause.UpperLimit.HasValue ? this.ProductFeeClause.UpperLimit.Value.ToRound(2).ToString() : "");
                replaceText.Add("{GoodspaymentLimitDay}", this.ProductFeeClause.DaysLimit.HasValue ? this.ProductFeeClause.DaysLimit.Value.ToString() : "");
            }
            else if (this.ProductFeeClause.PeriodType == PeriodType.Monthly)
            {
                replaceText.Add("{GoodsPaymentMax}", "");
                replaceText.Add("{GoodspaymentLimitDay}", "");
                replaceText.Add("{GoodsPaymentMonthDay}", this.ProductFeeClause.MonthlyDay.HasValue ? this.ProductFeeClause.MonthlyDay.Value.ToString() : "");
                replaceText.Add("{GoodsPaymentMonMax}", this.ProductFeeClause.UpperLimit.HasValue ? this.ProductFeeClause.UpperLimit.Value.ToRound(2).ToString() : "");
            }

            //货款汇率
            if (this.ProductFeeClause.ExchangeRateType == ExchangeRateType.RealTime)
            {
                replaceText.Add("{GoodsPaymentRateTypeReal}", "☑");
                replaceText.Add("{GoodsPaymentRateTypeAgree}", "□");
                replaceText.Add("{GoodsPaymentRate}", "");
            }
            else
            {
                replaceText.Add("{GoodsPaymentRateTypeReal}", "□");
                replaceText.Add("{GoodsPaymentRateTypeAgree}", "☑");
                replaceText.Add("{GoodsPaymentRate}", this.ProductFeeClause.ExchangeRateValue.HasValue ? this.ProductFeeClause.ExchangeRateValue.Value.ToString() : "");
            }

            #endregion

            #region 税款

            //税款
            replaceText.Add("{TaxPaymentTypePre}", this.TaxFeeClause.PeriodType == PeriodType.PrePaid ? "☑" : "□");
            replaceText.Add("{TaxPaymentTypeLimit}", this.TaxFeeClause.PeriodType == PeriodType.AgreedPeriod ? "☑" : "□");
            replaceText.Add("{TaxPaymentTypeMon}", this.TaxFeeClause.PeriodType == PeriodType.Monthly ? "☑" : "□");
            if (this.TaxFeeClause.PeriodType == PeriodType.PrePaid)
            {
                replaceText.Add("{TaxPaymentMonthDay}", "");
                replaceText.Add("{TaxPaymentMax}", "");
                replaceText.Add("{TaxPaymentMonMax}", "");
                replaceText.Add("{TaxpaymentLimitDay}", "");
            }
            else if (this.TaxFeeClause.PeriodType == PeriodType.AgreedPeriod)
            {
                replaceText.Add("{TaxPaymentMonthDay}", "");
                replaceText.Add("{TaxPaymentMonMax}", "");
                replaceText.Add("{TaxPaymentMax}", this.TaxFeeClause.UpperLimit.HasValue ? this.TaxFeeClause.UpperLimit.Value.ToRound(2).ToString() : "");
                replaceText.Add("{TaxpaymentLimitDay}", this.TaxFeeClause.DaysLimit.HasValue ? this.TaxFeeClause.DaysLimit.Value.ToString() : "");
            }
            else if (this.TaxFeeClause.PeriodType == PeriodType.Monthly)
            {
                replaceText.Add("{TaxPaymentMax}", "");
                replaceText.Add("{TaxpaymentLimitDay}", "");
                replaceText.Add("{TaxPaymentMonthDay}", this.TaxFeeClause.MonthlyDay.HasValue ? this.TaxFeeClause.MonthlyDay.Value.ToString() : "");
                replaceText.Add("{TaxPaymentMonMax}", this.TaxFeeClause.UpperLimit.HasValue ? this.TaxFeeClause.UpperLimit.Value.ToRound(2).ToString() : "");
            }
            //税款汇率
            if (this.TaxFeeClause.ExchangeRateType == ExchangeRateType.Custom)
            {
                replaceText.Add("{TaxPaymentRateTypeReal}", "□");
                replaceText.Add("{TaxPaymentRateTypeCustoms}", "☑");
                replaceText.Add("{TaxPaymentRateTypeAgree}", "□");
                replaceText.Add("{TaxPaymentRate}", "");
                replaceText.Add("{TaxRateTypeCN}", "海关汇率");
            }
            else if (this.TaxFeeClause.ExchangeRateType == ExchangeRateType.RealTime)
            {
                replaceText.Add("{TaxPaymentRateTypeReal}", "☑");
                replaceText.Add("{TaxPaymentRateTypeCustoms}", "□");
                replaceText.Add("{TaxPaymentRateTypeAgree}", "□");
                replaceText.Add("{TaxPaymentRate}", "");
                replaceText.Add("{TaxRateTypeCN}", "实时汇率");
            }
            else
            {
                replaceText.Add("{TaxPaymentRateTypeReal}", "□");
                replaceText.Add("{TaxPaymentRateTypeCustoms}", "□");
                replaceText.Add("{TaxPaymentRateTypeAgree}", "☑");
                replaceText.Add("{TaxPaymentRate}", this.TaxFeeClause.ExchangeRateValue.HasValue ? this.TaxFeeClause.ExchangeRateValue.Value.ToString() : "");
                replaceText.Add("{TaxRateTypeCN}", "约定汇率");
            }

            #endregion

            #region 代理费

            //代理费
            replaceText.Add("{AgentPaymentTypePre}", this.AgencyFeeClause.PeriodType == PeriodType.PrePaid ? "☑" : "□");
            replaceText.Add("{AgentPaymentTypeLimit}", this.AgencyFeeClause.PeriodType == PeriodType.AgreedPeriod ? "☑" : "□");
            replaceText.Add("{AgentPaymentTypeMon}", this.AgencyFeeClause.PeriodType == PeriodType.Monthly ? "☑" : "□");
            if (this.AgencyFeeClause.PeriodType == PeriodType.PrePaid)
            {
                replaceText.Add("{AgentPaymentMonthDay}", "");
                replaceText.Add("{AgentPaymentMax}", "");
                replaceText.Add("{AgentPaymentMonMax}", "");
                replaceText.Add("{AgentpaymentLimitDay}", "");
            }
            else if (this.AgencyFeeClause.PeriodType == PeriodType.AgreedPeriod)
            {
                replaceText.Add("{AgentPaymentMonthDay}", "");
                replaceText.Add("{AgentPaymentMonMax}", "");
                replaceText.Add("{AgentPaymentMax}", this.AgencyFeeClause.UpperLimit.HasValue ? this.AgencyFeeClause.UpperLimit.Value.ToRound(2).ToString() : "");
                replaceText.Add("{AgentpaymentLimitDay}", this.AgencyFeeClause.DaysLimit.HasValue ? this.AgencyFeeClause.DaysLimit.Value.ToString() : "");
            }
            else if (this.AgencyFeeClause.PeriodType == PeriodType.Monthly)
            {
                replaceText.Add("{AgentPaymentMax}", "");
                replaceText.Add("{AgentpaymentLimitDay}", "");
                replaceText.Add("{AgentPaymentMonthDay}", this.AgencyFeeClause.MonthlyDay.HasValue ? this.AgencyFeeClause.MonthlyDay.Value.ToString() : "");
                replaceText.Add("{AgentPaymentMonMax}", this.AgencyFeeClause.UpperLimit.HasValue ? this.AgencyFeeClause.UpperLimit.Value.ToRound(2).ToString() : "");
            }

            //代理费汇率
            if (this.AgencyFeeClause.ExchangeRateType == ExchangeRateType.RealTime)
            {
                replaceText.Add("{AgentPaymentRateTypeReal}", "☑");
                replaceText.Add("{AgentPaymentRateTypeCustoms}", "□");
                replaceText.Add("{AgentPaymentRateTypeAgree}", "□");
                replaceText.Add("{AgentPaymentRate}", "");
                replaceText.Add("{AgentRateTypeCN}", "实时");
            }
            else if (this.AgencyFeeClause.ExchangeRateType == ExchangeRateType.Custom)
            {
                replaceText.Add("{AgentPaymentRateTypeReal}", "□");
                replaceText.Add("{AgentPaymentRateTypeCustoms}", "☑");
                replaceText.Add("{AgentPaymentRateTypeAgree}", "□");
                replaceText.Add("{AgentPaymentRate}", "");
                replaceText.Add("{AgentRateTypeCN}", "海关");
            }
            else
            {
                replaceText.Add("{AgentPaymentRateTypeReal}", "□");
                replaceText.Add("{AgentPaymentRateTypeCustoms}", "□");
                replaceText.Add("{AgentPaymentRateTypeAgree}", "☑");
                replaceText.Add("{AgentPaymentRate}", this.AgencyFeeClause.ExchangeRateValue.HasValue ? this.AgencyFeeClause.ExchangeRateValue.Value.ToString() : "");
                replaceText.Add("{AgentRateTypeCN}", "约定");
            }

            #endregion

            #region 杂费

            //杂费
            replaceText.Add("{OtherPaymentTypePre}", this.IncidentalFeeClause.PeriodType == PeriodType.PrePaid ? "☑" : "□");
            replaceText.Add("{OtherPaymentTypeLimit}", this.IncidentalFeeClause.PeriodType == PeriodType.AgreedPeriod ? "☑" : "□");
            replaceText.Add("{OtherPaymentTypeMon}", this.IncidentalFeeClause.PeriodType == PeriodType.Monthly ? "☑" : "□");
            if (this.IncidentalFeeClause.PeriodType == PeriodType.PrePaid)
            {
                replaceText.Add("{OtherPaymentMonthDay}", "");
                replaceText.Add("{OtherPaymentMax}", "");
                replaceText.Add("{OtherPaymentMonMax}", "");
                replaceText.Add("{OtherpaymentLimitDay}", "");
            }
            else if (this.IncidentalFeeClause.PeriodType == PeriodType.AgreedPeriod)
            {
                replaceText.Add("{OtherPaymentMonthDay}", "");
                replaceText.Add("{OtherPaymentMonMax}", "");
                replaceText.Add("{OtherPaymentMax}", this.IncidentalFeeClause.UpperLimit.HasValue ? this.IncidentalFeeClause.UpperLimit.Value.ToRound(2).ToString() : "");
                replaceText.Add("{OtherpaymentLimitDay}", this.IncidentalFeeClause.DaysLimit.HasValue ? this.IncidentalFeeClause.DaysLimit.Value.ToString() : "");
            }
            else if (this.IncidentalFeeClause.PeriodType == PeriodType.Monthly)
            {
                replaceText.Add("{OtherPaymentMax}", "");
                replaceText.Add("{OtherpaymentLimitDay}", "");
                replaceText.Add("{OtherPaymentMonthDay}", this.IncidentalFeeClause.MonthlyDay.HasValue ? this.IncidentalFeeClause.MonthlyDay.Value.ToString() : "");
                replaceText.Add("{OtherPaymentMonMax}", this.IncidentalFeeClause.UpperLimit.HasValue ? this.IncidentalFeeClause.UpperLimit.Value.ToRound(2).ToString() : "");
            }

            //杂费汇率
            if (this.IncidentalFeeClause.ExchangeRateType == ExchangeRateType.RealTime)
            {
                replaceText.Add("{OtherPaymentRateTypeReal}", "☑");
                replaceText.Add("{OtherPaymentRateTypeCustoms}", "□");
                replaceText.Add("{OtherPaymentRateTypeAgree}", "□");
                replaceText.Add("{OtherPaymentRateTypeNone}", "□");
                replaceText.Add("{OtherPaymentRate}", "");
            }
            else if (this.IncidentalFeeClause.ExchangeRateType == ExchangeRateType.Custom)
            {
                replaceText.Add("{OtherPaymentRateTypeReal}", "□");
                replaceText.Add("{OtherPaymentRateTypeCustoms}", "☑");
                replaceText.Add("{OtherPaymentRateTypeAgree}", "□");
                replaceText.Add("{OtherPaymentRateTypeNone}", "□");
                replaceText.Add("{OtherPaymentRate}", "");
            }
            else if (this.IncidentalFeeClause.ExchangeRateType == ExchangeRateType.Agreed)
            {
                replaceText.Add("{OtherPaymentRateTypeReal}", "□");
                replaceText.Add("{OtherPaymentRateTypeCustoms}", "□");
                replaceText.Add("{OtherPaymentRateTypeAgree}", "☑");
                replaceText.Add("{OtherPaymentRateTypeNone}", "□");
                replaceText.Add("{OtherPaymentRate}", this.IncidentalFeeClause.ExchangeRateValue.HasValue ? this.IncidentalFeeClause.ExchangeRateValue.Value.ToString() : "");
            }
            else
            {
                replaceText.Add("{OtherPaymentRateTypeReal}", "□");
                replaceText.Add("{OtherPaymentRateTypeCustoms}", "□");
                replaceText.Add("{OtherPaymentRateTypeAgree}", "□");
                replaceText.Add("{OtherPaymentRateTypeNone}", "☑");
                replaceText.Add("{OtherPaymentRate}", "");
            }

            #endregion

            //开票
            replaceText.Add("{InvoiceType}", this.InvoiceType == InvoiceType.Full ? "☑" : "□");
            replaceText.Add("{InvoiceTypeService}", this.InvoiceType == InvoiceType.Service ? "☑" : "□");

            #region 计算年份

            var years = this.EndDate.Year - this.StartDate.Year;
            var y = "";
            //1=壹，2=贰，3=叁，4=肆，5=伍，6=陆，7=柒，8=捌，9=玖，10=拾。
            switch (years)
            {
                case 1:
                    y = "壹";
                    break;
                case 2:
                    y = "贰";
                    break;
                case 3:
                    y = "叁";
                    break;
                case 4:
                    y = "肆";
                    break;
                case 5:
                    y = "伍";
                    break;
                case 6:
                    y = "陆";
                    break;
                case 7:
                    y = "柒";
                    break;
                case 8:
                    y = "捌";
                    break;
                case 9:
                    y = "玖";
                    break;
                case 10:
                    y = "拾";
                    break;
                default: break;
            }
            replaceText.Add("{Years}", y);

            #endregion

            #endregion

            return npoi.GenerateWordByTemplete(replaceText);
        }

        /// <summary>
        /// 保存文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void SaveAs(string filePath)
        {
            XWPFDocument doc = this.ToWord();
            FileStream file = new FileStream(filePath, FileMode.Create);
            doc.Write(file);
            file.Close();
        }



        /// <summary>
        /// 进口服务协议--新
        /// </summary>
        /// <returns></returns>
        public XWPFDocument ToWordImport(string agreementCode)
        {
            var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\templates\\华芯通供应链进口服务协议.docx");
            var npoi = new NPOIHelper(tempPath);
            var replaceText = new Dictionary<string, string>();

            var client = new Views.ClientsView()[this.ClientID];

            #region 组装内容

            //基础信息
            replaceText.Add("{CreateTime}", this.StartDate.ToString("yyyy年MM月dd日"));
            replaceText.Add("{AgreementCode}", agreementCode);
            replaceText.Add("{CompanyName}", client.Company.Name);
            replaceText.Add("{RegisteredAddress}", client.Company.Address);
            replaceText.Add("{LegalPerson}", client.Company.Corporate);

            replaceText.Add("{AgreementStartDate}", this.StartDate.ToString("yyyy年MM月dd日"));
            replaceText.Add("{AgreementEndDate}", this.EndDate.ToString("yyyy年MM月dd日"));

            //换汇汇率
            replaceText.Add("{IsTen}", this.IsTen == PEIsTen.Ten ? "10:00" : "09:30");
            // 进口当月海关汇率
            replaceText.Add("{TaxRateType}", this.TaxFeeClause.ExchangeRateType == ExchangeRateType.Custom ? "进口当月海关汇率" : (this.TaxFeeClause.ExchangeRateType == ExchangeRateType.RealTime ? "付款当天中国银行10:00之后第一个外汇卖出价" : "双方约定汇率"));

            //代理费率
            var preAgency = (this.PreAgency.HasValue && this.PreAgency.Value > 0) ? (this.PreAgency.Value.ToRound(2).ToString() + "元 + ") : "";
            replaceText.Add("{AgencyRate}", preAgency + (this.AgencyRate * 100M).ToRound(2).ToString() + "%");
            replaceText.Add("{MinimumAgent}", this.MinAgencyFee.ToRound(2).ToString());
            //代理费汇率 
            replaceText.Add("{AgencyRateType}", this.AgencyFeeClause.ExchangeRateType == ExchangeRateType.Custom ? "进口当月海关汇率" : (this.AgencyFeeClause.ExchangeRateType == ExchangeRateType.RealTime ? "付款当天中国银行10:00之后第一个外汇卖出价" : "双方约定汇率"));


            //开票点位
            replaceText.Add("{InvoicePoint}", (this.InvoiceTaxRate + 1).ToRound(2).ToString());
            //开票类别
            replaceText.Add("{InvoiceTypeDescription}", this.InvoiceType == InvoiceType.Full ? "签署内贸合同，受托方以进口货物价款、关税、增值税、消费税、服务费向委托方开具税率为13%的增值税专用发票" : "受托方向委托方提供海关进口关税专用缴款书、海关进口增值税专用缴款书和报关单，受托方以所收服务费开具税率为6%的增值税专用发票");


            //税款
            if (this.ProductFeeClause.PeriodType == PeriodType.PrePaid)
            {
                //replaceText.Add("{GoodsPaymentPre}", "☑无信用额度");
                replaceText.Add("{GoodsPaymentPeriod}", "无信用额度");
            }
            else if (this.ProductFeeClause.PeriodType == PeriodType.AgreedPeriod)
            {
                //replaceText.Add("{GoodsPaymentPre}", "□无信用额度");
                replaceText.Add("{GoodsPaymentPeriod}", "信用额度：约定期限，进口" + this.ProductFeeClause.DaysLimit.Value.ToString() + "天；额度：" + this.ProductFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元");
            }
            else
            {
                //replaceText.Add("{GoodsPaymentPre}", "□无信用额度");
                replaceText.Add("{GoodsPaymentPeriod}", "信用额度：月结，次月" + this.ProductFeeClause.MonthlyDay.Value.ToString() + "日；额度：" + this.ProductFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元");
            }

            //税款
            if (this.TaxFeeClause.PeriodType == PeriodType.PrePaid)
            {
                //replaceText.Add("{TaxPaymentPre}", "☑无信用额度");
                replaceText.Add("{TaxPaymentPeriod}", "无信用额度");
            }
            else if (this.TaxFeeClause.PeriodType == PeriodType.AgreedPeriod)
            {
                //replaceText.Add("{TaxPaymentPre}", "□无信用额度");
                replaceText.Add("{TaxPaymentPeriod}", "信用额度：约定期限，进口" + this.TaxFeeClause.DaysLimit.Value.ToString() + "天；额度：" + this.TaxFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元");
            }
            else
            {
                //replaceText.Add("{TaxPaymentPre}", "□无信用额度");
                replaceText.Add("{TaxPaymentPeriod}", "信用额度：月结，次月" + this.TaxFeeClause.MonthlyDay.Value.ToString() + "日；额度：" + this.TaxFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元");
            }

            //服务费
            if (this.AgencyFeeClause.PeriodType == PeriodType.PrePaid)
            {
                //replaceText.Add("{AgentPaymentPre}", "☑无信用额度");
                replaceText.Add("{AgentPaymentPeriod}", "无信用额度");
            }
            else if (this.AgencyFeeClause.PeriodType == PeriodType.AgreedPeriod)
            {
                //replaceText.Add("{AgentPaymentPre}", "□无信用额度");
                replaceText.Add("{AgentPaymentPeriod}", "信用额度：约定期限，进口" + this.AgencyFeeClause.DaysLimit.Value.ToString() + "天；额度：" + this.AgencyFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元");
            }
            else
            {
                //replaceText.Add("{AgentPaymentPre}", "□无信用额度");
                replaceText.Add("{AgentPaymentPeriod}", "信用额度：月结，次月" + this.AgencyFeeClause.MonthlyDay.Value.ToString() + "日；额度：" + this.AgencyFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元");
            }

            //杂费
            if (this.IncidentalFeeClause.PeriodType == PeriodType.PrePaid)
            {
                //replaceText.Add("{OtherPaymentPre}", "☑无信用额度");
                replaceText.Add("{OtherPaymentPeriod}", "无信用额度");
            }
            else if (this.IncidentalFeeClause.PeriodType == PeriodType.AgreedPeriod)
            {
                //replaceText.Add("{OtherPaymentPre}", "□无信用额度");
                replaceText.Add("{OtherPaymentPeriod}", "信用额度：约定期限，进口" + this.IncidentalFeeClause.DaysLimit.Value.ToString() + "天；额度：" + this.IncidentalFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元");
            }
            else
            {
                //replaceText.Add("{OtherPaymentPre}", "□无信用额度");
                replaceText.Add("{OtherPaymentPeriod}", "信用额度：月结，次月" + this.IncidentalFeeClause.MonthlyDay.Value.ToString() + "日；额度：" + this.IncidentalFeeClause.UpperLimit.Value.ToRound(2).ToString() + "元");
            }


            #region 计算年份

            var years = this.EndDate.Year - this.StartDate.Year;
            var y = "";
            //1=壹，2=贰，3=叁，4=肆，5=伍，6=陆，7=柒，8=捌，9=玖，10=拾。
            switch (years)
            {
                case 1:
                    y = "壹";
                    break;
                case 2:
                    y = "贰";
                    break;
                case 3:
                    y = "叁";
                    break;
                case 4:
                    y = "肆";
                    break;
                case 5:
                    y = "伍";
                    break;
                case 6:
                    y = "陆";
                    break;
                case 7:
                    y = "柒";
                    break;
                case 8:
                    y = "捌";
                    break;
                case 9:
                    y = "玖";
                    break;
                case 10:
                    y = "拾";
                    break;
                default: break;
            }
            replaceText.Add("{Years}", y);

            #endregion

            #endregion

            return npoi.GenerateWordByTemplete(replaceText);
        }


        /// <summary>
        /// 保存文件-新
        /// </summary>
        /// <param name="filePath"></param>
        public void SaveAsImport(string filePath)
        {
            XWPFDocument doc = this.ToWordImport(this.AgreementCode);
            FileStream file = new FileStream(filePath, FileMode.Create);
            doc.Write(file);
            file.Close();
        }


        #endregion

        #region 华芯通垫款保证协议模板导出
        public XWPFDocument ToXDTWord()
        {
            var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\templates\\华芯通垫款保证协议模板.docx");
            var npoi = new NPOIHelper(tempPath);
            var replaceText = new Dictionary<string, string>();

            var client = new Views.ClientsView()[this.ClientID];

            #region 组装内容
            replaceText.Add("{CompanyName}", client.Company.Name);
            replaceText.Add("{RegisteredAddress}", client.Company.Address);
            replaceText.Add("{LegalPerson}", client.Company.Corporate);

            replaceText.Add("{Date}", this.StartDate.ToString("yyyyMMdd"));
            replaceText.Add("{Year}", this.StartDate.Year.ToString());
            replaceText.Add("{Month}", this.StartDate.Month.ToString());
            replaceText.Add("{Day}", this.StartDate.Day.ToString());
            #endregion

            return npoi.GenerateWordByTemplete(replaceText);
        }
        /// <summary>
        /// 保存华芯通垫款保证协议模板
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void XDTSaveAs(string filePath)
        {
            XWPFDocument doc = this.ToXDTWord();
            FileStream file = new FileStream(filePath, FileMode.Create);
            doc.Write(file);
            file.Close();
        }
        #endregion

        #region 协议变更模板导出
        public XWPFDocument ToChangeWord(string applyId)
        {
            var tempPath = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Content\\templates\\华芯通进口补充协议.docx");
            var npoi = new NPOIHelper(tempPath);
            var replaceText = new Dictionary<string, string>();

            //var clientApplyList = new Needs.Ccs.Services.Views.AgreementChangeApplyListView().Where(t => t.ID == applyId).ToArray();
            var client = new Views.ClientsView()[this.ClientID];

            List<AgreementChangeDateil> list = new List<AgreementChangeDateil>();
            var AgreementApplyItem = new Needs.Ccs.Services.Views.AgreementChangeApplyView().Where(t => t.ID == applyId).ToArray();
            if (AgreementApplyItem.Length != 0)
            {
                //生效日期
                var Date = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.StartDate || t.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.EndDate).ToList();
                foreach (var item in Date)
                {
                    if (item.AgreementChangeType == AgreementChangeType.StartDate)
                    {
                        ChangeType = "协议生效日期";
                        OldValue = item.OldValue;
                        NewValue = item.NewValue;
                    }
                    if (item.AgreementChangeType == AgreementChangeType.EndDate)
                    {
                        if (OldValue == "" && NewValue == "")
                        {
                            ChangeType = "协议生效日期";
                            OldValue = item.OldValue;
                            NewValue = item.NewValue;
                        }
                        else
                        {
                            ChangeType = "协议生效日期";
                            OldValue = OldValue + " - " + item.OldValue;
                            NewValue = NewValue + " - " + item.NewValue;
                        }

                    }

                }
                if (ChangeType == "协议生效日期")
                {
                    //string str = Getstring(ChangeType, OldValue, NewValue);

                    //replaceText.Add("{ChangeType}", str);
                    replaceText.Add("{ChangeType}", ChangeType != "" ? ChangeType : "");
                    replaceText.Add("{OldValue}", OldValue != "" ? " 变更前为：" + OldValue : "");
                    replaceText.Add("{NewValue}", NewValue != "" ? " 变更后为：" + NewValue : "");
                }
                else
                {
                    replaceText.Add("{ChangeType}", "");
                    replaceText.Add("{OldValue}", "");
                    replaceText.Add("{NewValue}", "");
                }
                //代理费
                var AgencyFee = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.AgencyRate 
                || t.AgreementChangeType == AgreementChangeType.MinAgencyFee
                || t.AgreementChangeType == AgreementChangeType.PreAgency).ToList();
                foreach (var item in AgencyFee)
                {
                    if (item.AgreementChangeType == AgreementChangeType.AgencyRate)
                    {
                        ChangeType = "服务费";
                        OldValue = OldValue + "服务费率：" + Convert.ToDouble(item.OldValue).ToString() + "; ";
                        NewValue = NewValue + "服务费率：" + Convert.ToDouble(item.NewValue).ToString() + "; ";
                    }
                    if (item.AgreementChangeType == AgreementChangeType.PreAgency)
                    {
                        if (ChangeType == "服务费")
                        {
                            if (!string.IsNullOrEmpty(item.OldValue))
                            {
                                OldValue = OldValue + "基础服务费：" + Convert.ToDouble(item.OldValue).ToString() + "; ";
                            }
                            if (!string.IsNullOrEmpty(item.NewValue))
                            {
                                NewValue = NewValue + "基础服务费：" + Convert.ToDouble(item.NewValue).ToString() + "; ";
                            }
                        }
                        else
                        {
                            ChangeType = "服务费";
                            OldValue = OldValue + "基础服务费：" + Convert.ToDouble(item.OldValue).ToString() + "; ";
                            NewValue = NewValue + "基础服务费：" + Convert.ToDouble(item.NewValue).ToString() + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.MinAgencyFee)
                    {
                        if (ChangeType == "服务费")
                        {
                            if (!string.IsNullOrEmpty(item.OldValue))
                            {
                                OldValue = OldValue + "最低服务费：" + Convert.ToDouble(item.OldValue).ToString() + "; ";
                            }
                            if (!string.IsNullOrEmpty(item.NewValue))
                            {
                                NewValue = NewValue + "最低服务费：" + Convert.ToDouble(item.NewValue).ToString() + "; ";
                            }
                        }
                        else
                        {
                            ChangeType = "服务费";
                            OldValue = OldValue + "最低服务费：" + Convert.ToDouble(item.OldValue).ToString() + "; ";
                            NewValue = NewValue + "最低服务费：" + Convert.ToDouble(item.NewValue).ToString() + "; ";
                        }
                    }
                }
                if (ChangeType == "服务费")
                {
                    //string str = Getstring(ChangeType, OldValue, NewValue);
                    //replaceText.Add("{ChangeType1}", str);
                    replaceText.Add("{ChangeType1}", ChangeType != "" ? ChangeType : "");
                    replaceText.Add("{OldValue1}", OldValue != "" ? " 变更前为：" + OldValue : "");
                    replaceText.Add("{NewValue1}", NewValue != "" ? " 变更后为：" + NewValue : "");
                }
                else
                {
                    replaceText.Add("{ChangeType1}", "");
                    replaceText.Add("{OldValue1}", "");
                    replaceText.Add("{NewValue1}", "");
                }
                //换汇方式
                var IsPrePayExchange = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.IsPrePayExchange).ToList();
                foreach (var item in IsPrePayExchange)
                {
                    if (item.AgreementChangeType == AgreementChangeType.IsPrePayExchange)
                    {
                        if (item.OldValue == "True")
                        {
                            ChangeType = "换汇方式";
                            OldValue = "预换汇";
                            NewValue = "90天内换汇";
                        }
                        else
                        {
                            ChangeType = "换汇方式";
                            OldValue = "90天内换汇";
                            NewValue = "预换汇";
                        }
                    }
                }
                if (ChangeType == "换汇方式")
                {
                    //string str = Getstring(ChangeType, OldValue, NewValue);
                    //replaceText.Add("{ChangeType2}", str);
                    replaceText.Add("{ChangeType2}", ChangeType != "" ? ChangeType : "");
                    replaceText.Add("{OldValue2}", OldValue != "" ? " 变更前为：" + OldValue : "");
                    replaceText.Add("{NewValue2}", NewValue != "" ? " 变更后为：" + NewValue : "");
                }
                else
                {
                    replaceText.Add("{ChangeType2}", "");
                    replaceText.Add("{OldValue2}", "");
                    replaceText.Add("{NewValue2}", "");
                }


                //换汇汇率类型
                var IsTenType = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.IsTenType).ToList();
                foreach (var item in IsTenType)
                {
                        if (item.OldValue == "1")
                        {
                            ChangeType = "换汇汇率";
                            OldValue = "中国银行上午10:00";
                            NewValue = "中国银行上午09:30";
                        }
                        else
                        {
                            ChangeType = "换汇汇率";
                            OldValue = "中国银行上午09:30";
                            NewValue = "中国银行上午10:00";
                        }
                    
                }
                if (ChangeType == "换汇汇率")
                {
                    //string str = Getstring(ChangeType, OldValue, NewValue);
                    //replaceText.Add("{ChangeType2}", str);
                    replaceText.Add("{ChangeType7}", ChangeType != "" ? ChangeType : "");
                    replaceText.Add("{OldValue7}", OldValue != "" ? " 变更前为：" + OldValue : "");
                    replaceText.Add("{NewValue7}", NewValue != "" ? " 变更后为：" + NewValue : "");
                }
                else
                {
                    replaceText.Add("{ChangeType7}", "");
                    replaceText.Add("{OldValue7}", "");
                    replaceText.Add("{NewValue7}", "");
                }

                //税款条款
                var Tax = AgreementApplyItem.Where(t => t.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.TaxPeriodType
                || t.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.TaxDaysLimit
                || t.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.TaxMonthlyDay
                || t.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.TaxUpperLimit
                || t.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.TaxExchangeRateType
                || t.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.TaxExchangeRateValue).ToList();

                OldValue = "";
                NewValue = "";
                foreach (var item in Tax)
                {
                    ChangeType = "税款条款";
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.TaxPeriodType)
                    {
                        if (item.OldValue == PeriodType.PrePaid.ToString())
                        {
                            OldValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            OldValue = item.OldValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            OldValue = "结算方式：" + OldValue + "; ";
                        }
                        if (item.NewValue == Needs.Ccs.Services.Enums.PeriodType.PrePaid.ToString())
                        {
                            NewValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            NewValue = item.NewValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : PeriodType.Monthly.GetDescription();
                            NewValue = "结算方式：" + NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxDaysLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "约定期限(天)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "约定期限(天)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "约定期限(天)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "约定期限(天)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxMonthlyDay)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "结算日期(次月)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "结算日期(次月)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "结算日期(次月)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "结算日期(次月)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxUpperLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "垫款上限(元)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "垫款上限(元)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "垫款上限(元)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "垫款上限(元)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxExchangeRateType)
                    {
                        if (item.OldValue == ExchangeRateType.Agreed.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.Custom.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.RealTime.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                        if (item.NewValue == ExchangeRateType.Agreed.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.Custom.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.RealTime.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxExchangeRateValue)
                    {
                        if (OldValue.Contains("约定汇率"))
                        {
                            OldValue = OldValue + ";约定汇率：" + item.OldValue;
                        }
                        if (NewValue.Contains("约定汇率"))
                        {
                            NewValue = NewValue + ";约定汇率：" + item.NewValue;
                        }
                    }
                }
                if (ChangeType == "税款条款")
                {
                    replaceText.Add("{ChangeType3}", ChangeType != "" ? ChangeType : "");
                    replaceText.Add("{OldValue3}", OldValue != "" ? " 变更前为：" + OldValue : "");
                    replaceText.Add("{NewValue3}", NewValue != "" ? " 变更后为：" + NewValue : "");
                }
                else
                {
                    replaceText.Add("{ChangeType3}", "");
                    replaceText.Add("{OldValue3}", "");
                    replaceText.Add("{NewValue3}", "");
                }
                //代理费条款
                var Agency = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.AgencyPeriodType
                || t.AgreementChangeType == AgreementChangeType.AgencyDaysLimit
                || t.AgreementChangeType == AgreementChangeType.AgencyMonthlyDay
                || t.AgreementChangeType == AgreementChangeType.AgencyUpperLimit
                || t.AgreementChangeType == AgreementChangeType.AgencyExchangeRateType
                || t.AgreementChangeType == AgreementChangeType.AgencyExchangeRateValue).ToList();

                OldValue = "";
                NewValue = "";
                foreach (var item in Agency)
                {
                    ChangeType = "服务费条款";
                    if (item.AgreementChangeType == AgreementChangeType.AgencyPeriodType)
                    {

                        if (item.OldValue == PeriodType.PrePaid.ToString())
                        {
                            OldValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            OldValue = item.OldValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            OldValue = "结算方式：" + OldValue + "; ";
                        }
                        if (item.NewValue == PeriodType.PrePaid.ToString())
                        {
                            NewValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            NewValue = item.NewValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            NewValue = "结算方式：" + NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyDaysLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "约定期限(天)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "约定期限(天)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "约定期限(天)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "约定期限(天)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyMonthlyDay)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "结算日期(次月)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "结算日期(次月)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "结算日期(次月)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "结算日期(次月)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyUpperLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "垫款上限(元)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "垫款上限(元)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "垫款上限(元)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "垫款上限(元)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyExchangeRateType)
                    {
                        if (item.OldValue == ExchangeRateType.Agreed.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.Custom.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.RealTime.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                        if (item.NewValue == ExchangeRateType.Agreed.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.Custom.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.RealTime.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyExchangeRateValue)
                    {

                        if (OldValue.Contains("约定汇率"))
                        {
                            OldValue = OldValue + ";约定汇率：" + item.OldValue;
                        }
                        if (NewValue.Contains("约定汇率"))
                        {
                            NewValue = NewValue + ";约定汇率：" + item.NewValue;
                        }
                    }
                }
                if (ChangeType == "服务费条款")
                {
                    //string str = Getstring(ChangeType, OldValue, NewValue);
                    //replaceText.Add("{ChangeType4}", str);
                    replaceText.Add("{ChangeType4}", ChangeType != "" ? ChangeType : "");
                    replaceText.Add("{OldValue4}", OldValue != "" ? " 变更前为：" + OldValue : "");
                    replaceText.Add("{NewValue4}", NewValue != "" ? " 变更后为：" + NewValue : "");
                }
                else
                {
                    replaceText.Add("{ChangeType4}", "");
                    replaceText.Add("{OldValue4}", "");
                    replaceText.Add("{NewValue4}", "");
                }
                //杂费条款
                var Other = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.OtherPeriodType
                || t.AgreementChangeType == AgreementChangeType.OtherDaysLimit
                || t.AgreementChangeType == AgreementChangeType.OtherMonthlyDay
                || t.AgreementChangeType == AgreementChangeType.OtherUpperLimit
                || t.AgreementChangeType == AgreementChangeType.OtherExchangeRateType
                || t.AgreementChangeType == AgreementChangeType.OtherExchangeRateValue).ToList();

                OldValue = "";
                NewValue = "";
                foreach (var item in Other)
                {
                    ChangeType = "杂费条款";
                    if (item.AgreementChangeType == AgreementChangeType.OtherPeriodType)
                    {
                        if (item.OldValue == PeriodType.PrePaid.ToString())
                        {
                            OldValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            OldValue = item.OldValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            OldValue = "结算方式：" + OldValue + "; ";
                        }
                        if (item.NewValue == PeriodType.PrePaid.ToString())
                        {
                            NewValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            NewValue = item.NewValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            NewValue = "结算方式：" + NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherDaysLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "约定期限(天)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "约定期限(天)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "约定期限(天)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "约定期限(天)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherMonthlyDay)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "结算日期(次月)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "结算日期(次月)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "结算日期(次月)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "结算日期(次月)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherUpperLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "垫款上限(元)：" + item.OldValue + "; ";
                        }
                        //else
                        //{
                        //    OldValue = "";
                        //    OldValue = "垫款上限(元)：" + item.OldValue + "; ";
                        //}
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "垫款上限(元)：" + item.NewValue + "; ";
                        }
                        //else
                        //{
                        //    NewValue = "";
                        //    NewValue = "垫款上限(元)：" + item.NewValue + "; ";
                        //}
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherExchangeRateType)
                    {
                        if (item.OldValue == ExchangeRateType.Agreed.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.Custom.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.RealTime.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                        if (item.NewValue == ExchangeRateType.Agreed.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.Custom.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.RealTime.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherExchangeRateValue)
                    {
                        if (OldValue.Contains("约定汇率"))
                        {
                            OldValue = OldValue + ";约定汇率：" + item.OldValue;
                        }
                        if (NewValue.Contains("约定汇率"))
                        {
                            NewValue = NewValue + ";约定汇率：" + item.NewValue;
                        }
                    }
                }
                if (ChangeType == "杂费条款")
                {
                    //string str = Getstring(ChangeType, OldValue, NewValue);
                    //replaceText.Add("{ChangeType5}", str);
                    replaceText.Add("{ChangeType5}", ChangeType != "" ? ChangeType : "");
                    replaceText.Add("{OldValue5}", OldValue != "" ? " 变更前为：" + OldValue : "");
                    replaceText.Add("{NewValue5}", NewValue != "" ? " 变更后为：" + NewValue : "");
                }
                else
                {
                    replaceText.Add("{ChangeType5}", "");
                    replaceText.Add("{OldValue5}", "");
                    replaceText.Add("{NewValue5}", "");
                }
                //开票类型
                var Invoice = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.InvoiceType || t.AgreementChangeType == AgreementChangeType.InvoiceTaxRate).ToList();
                foreach (var item in Invoice)
                {
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.InvoiceType)
                    {
                        ChangeType = "开票类型";
                        if (item.OldValue == InvoiceType.Full.ToString())
                        {
                            OldValue = InvoiceType.Full.GetDescription();
                        }
                        else
                        {
                            OldValue = InvoiceType.Service.GetDescription();
                        }
                        if (item.NewValue == InvoiceType.Full.ToString())
                        {
                            NewValue = InvoiceType.Full.GetDescription();
                        }
                        else
                        {
                            NewValue = InvoiceType.Service.GetDescription();
                        }
                    }
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.InvoiceTaxRate)
                    {
                        if (ChangeType == "开票类型")
                        {
                            OldValue = OldValue + ";" + (Convert.ToDouble(item.OldValue) * 100).ToString() + "%";
                            NewValue = NewValue + ";" + (Convert.ToDouble(item.NewValue) * 100).ToString() + "%";
                        }
                        else
                        {
                            ChangeType = "开票类型";
                            OldValue = (Convert.ToDouble(item.OldValue) * 100).ToString() + "%";
                            NewValue = (Convert.ToDouble(item.NewValue) * 100).ToString() + "%";
                        }

                    }
                }
                if (ChangeType == "开票类型")
                {
                    //string str = Getstring(ChangeType, OldValue, NewValue);
                    replaceText.Add("{ChangeType6}", ChangeType != "" ? ChangeType : "");
                    replaceText.Add("{OldValue6}", OldValue != "" ? " 变更前为：" + OldValue : "");
                    replaceText.Add("{NewValue6}", NewValue != "" ? " 变更后为：" + NewValue : "");
                }
                else
                {
                    replaceText.Add("{ChangeType6}", "");
                    replaceText.Add("{OldValue6}", "");
                    replaceText.Add("{NewValue6}", "");
                }
            }
            #region 组装内容

            //replaceText.Add("{CompanyName}", clientApplyList[0].ClientName);
            //replaceText.Add("{RegisteredAddress}", clientApplyList[0].Address == "" ? clientApplyList[0].Address : "");
            // replaceText.Add("{LegalPerson}", clientApplyList[0].Corporate == "" ? clientApplyList[0].Corporate : "");
            replaceText.Add("{CompanyName}", client.Company.Name);
            replaceText.Add("{RegisteredAddress}", client.Company.Address);
            replaceText.Add("{LegalPerson}", client.Company.Corporate);
            replaceText.Add("{AgreementStartDate}", this.StartDate.ToString("yyyy年MM月dd日"));

            #endregion

            return npoi.ChangeAgreementWordByTemplete(replaceText);
            //return npoi.CreateWord(list);
        }
        string Getstring(string ChangeType, string OldValue, string NewValue)
        {
            string str = "";
            if (!string.IsNullOrEmpty(ChangeType))
            {
                str = ChangeType;
            }
            if (!string.IsNullOrEmpty(OldValue))
            {
                str = str + " 变更前为：" + OldValue;
            }
            if (!string.IsNullOrEmpty(NewValue))
            {
                str = str + " 变更后为：" + NewValue;
            }
            return str;
        }
        /// <summary>
        /// 保存协议变更文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        public void ChangeSaveAs(string filePath, string applyId)
        {
            XWPFDocument doc = this.ToChangeWord(applyId);
            FileStream file = new FileStream(filePath, FileMode.Create);
            doc.Write(file);
            file.Close();
        }
        #endregion

        void OnAgreementExported()
        {
            if (this.AgreementExported != null)
            {
                this.AgreementExported(this, new AgreementExportedEventArgs(this));
            }
        }

        private void ClientAgreement_AgreementExported(object sender, AgreementExportedEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
