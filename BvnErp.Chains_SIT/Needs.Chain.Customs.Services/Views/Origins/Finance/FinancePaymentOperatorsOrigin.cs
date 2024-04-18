using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    public class FinancePaymentOperatorsOrigin : UniqueView<Models.FinancePaymentOperator, ScCustomsReponsitory>
    {
        public FinancePaymentOperatorsOrigin()
        {
        }

        public FinancePaymentOperatorsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<FinancePaymentOperator> GetIQueryable()
        {
            return from financePaymentOperator in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinancePaymentOperators>()
                   select new FinancePaymentOperator
                   {
                       ID = financePaymentOperator.ID,
                       AdminID = financePaymentOperator.AdminID,
                       Type = (Enums.PaymentOperatorType)financePaymentOperator.Type,
                       Status = (Enums.Status)financePaymentOperator.Status,
                       CreateDate = financePaymentOperator.CreateDate,
                       UpdateDate = financePaymentOperator.UpdateDate,
                       Summary = financePaymentOperator.Summary,
                   };
        }
    }
}
