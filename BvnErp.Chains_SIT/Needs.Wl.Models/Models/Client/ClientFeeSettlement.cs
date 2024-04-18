using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Model;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Wl.Models
{
    /// <summary>
    /// 费用的结算方式
    /// </summary>
    public class ClientFeeSettlement : ModelBase<Layer.Data.Sqls.ScCustoms.ClientAgreements, ScCustomsReponsitory>, IUnique, IPersist
    {
        string id;
        public new string ID
        {
            get
            {
                //结合所有有效数据，若有修改其中一项，则生成新的记录
                return this.id ?? string.Concat(this.AgreementID, this.FeeType, this.PeriodType, this.ExchangeRateType, this.DaysLimit, this.MonthlyDay, this.UpperLimit).MD5();
            }
            set
            {
                this.id = value;
            }
        }

        public string AgreementID { get; set; }

        /// <summary>
        /// 费用类型
        /// </summary>
        public Enums.FeeType FeeType { get; set; }

        /// <summary>
        /// 账期类型
        /// </summary>
        public Enums.PeriodType PeriodType { get; set; }

        /// <summary>
        /// 费用使用的汇率类型
        /// </summary>
        public Enums.ExchangeRateType ExchangeRateType { get; set; }

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

        public ClientFeeSettlement()
        {

        }

        public override void Enter()
        {
            int count = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>().Count(item => item.ID == this.ID);
            this.Reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>(new { Status = (int)Enums.Status.Delete }, item => item.AgreementID == this.AgreementID && item.FeeType == (int)this.FeeType);
            if (count == 0)
            {
                this.CreateDate = this.UpdateDate = DateTime.Now;
                this.Reponsitory.Insert(this.ToLinq());
            }
            else
            {
                this.UpdateDate = DateTime.Now;
                this.Reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
            }
        }
    }
}
