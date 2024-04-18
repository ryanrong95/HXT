using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 客户协议条款
    /// </summary>
    [Serializable]
    public class ClientAgreement : ModelBase<Layer.Data.Sqls.ScCustoms.ClientAgreements, ScCustomsReponsitory>, IUnique, IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                //结合有效数据，以及子项的ID（子项ID已做变更验证）验证是否需插入
                return this.id ?? string.Concat(this.ClientID, this.StartDate, this.EndDate, this.AgencyRate, this.MinAgencyFee, this.IsPrePayExchange, this.IsLimitNinetyDays, this.InvoiceType, this.InvoiceTaxRate).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string ClientID { get; set; }

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
        /// 是否可以预换汇，否则不可以在报关前换汇
        /// </summary>
        public bool IsPrePayExchange { get; set; }

        /// <summary>
        /// 是否选定在90天内换汇，超过90天就不允许换汇，如果不限制，可以在90天后换汇
        /// </summary>
        public bool IsLimitNinetyDays { get; set; }

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
                    return this.ClientFeeSettlementItems[Enums.FeeType.AgencyFee];
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
                    return this.ClientFeeSettlementItems[Enums.FeeType.Product];
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
                    return this.ClientFeeSettlementItems[Enums.FeeType.Tax];
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
                    return this.ClientFeeSettlementItems[Enums.FeeType.Incidental];
                }

                return incidentalFeeClause;
            }
            set
            {
                this.incidentalFeeClause = value;
            }
        }

        internal ClientFeeSettlementItems ClientFeeSettlementItems { get; set; }

        /// <summary>
        /// 操作人
        /// </summary>
        public string AdminID { get; set; }

        public ClientAgreement()
        {
            this.UpdateDate = this.CreateDate = DateTime.Now;
            this.Status = (int)Enums.Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientAgreements>().Count(item => item.ID == this.ID);

            //失效协议
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientAgreements>(new { Status = (int)Enums.Status.Delete }, item => item.ClientID == this.ClientID);

            if (count == 0)
            {
                this.Reponsitory.Insert(this.ToLinq());
            }
            else
            {
                this.Reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
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
    }
}