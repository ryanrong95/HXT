using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.Payments.Models.Origins;
using Yahv.Underly.Enums;

namespace Yahv.Payments.Views.Origins
{
    /// <summary>
    /// 封账视图
    /// </summary>
    public class MonthSealedBillsOrigin : UniqueView<MonthSealedBill, PvbCrmReponsitory>
    {
        public MonthSealedBillsOrigin()
        {

        }

        public MonthSealedBillsOrigin(PvbCrmReponsitory reponsitory) : base(reponsitory)
        {

        }

        /// <summary>
        /// 是否封账
        /// </summary>
        /// <param name="payer">客户</param>
        /// <param name="conduct">业务</param>
        /// <param name="dateIndex">期号</param>
        /// <returns></returns>
        public bool this[string payer, string conduct, int dateIndex]
        {
            get
            {
                var status =
                    (int)(this.FirstOrDefault(
                        item => item.Payer == payer && item.Conduct == conduct && item.DateIndex == dateIndex)?.Sealed ?? 0);

                return status == (int)MonthSealed.Closed;
            }
        }

        protected override IQueryable<MonthSealedBill> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvbCrm.MonthSealedBills>()
                   select new MonthSealedBill()
                   {
                       ID = entity.ID,
                       Conduct = entity.Conduct,
                       Payer = entity.Payer,
                       CreateDate = entity.CreateDate,
                       DateIndex = entity.DateIndex,
                       ModifyDate = entity.ModifyDate,
                       OccurDate = entity.OccurDate,
                       Sealed = (MonthSealed)entity.Sealed,
                   };
        }

    }
}
