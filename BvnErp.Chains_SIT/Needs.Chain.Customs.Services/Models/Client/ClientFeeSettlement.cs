using Needs.Ccs.Services.Enums;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Linq;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 费用的结算方式
    /// </summary>
    public class ClientFeeSettlement : IUnique, IPersist
    {
        string id;
        public string ID
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

        /// <summary>
        /// 数据状态
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        public ClientFeeSettlement()
        {
            this.Status = Status.Normal;
        }

        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>().Count(item => item.ID == this.ID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.ClientFeeSettlements>(new { Status = (int)Enums.Status.Delete }, item => item.AgreementID == this.AgreementID && item.FeeType == (int)this.FeeType);
                if (count == 0)
                {
                    this.CreateDate = this.UpdateDate = DateTime.Now;
                    reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.UpdateDate = DateTime.Now;
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
            }
        }
    }
}
