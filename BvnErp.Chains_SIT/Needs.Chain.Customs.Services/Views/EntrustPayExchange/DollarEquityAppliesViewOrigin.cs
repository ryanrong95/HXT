using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DollarEquityAppliesViewOrigin : UniqueView<Models.DollarEquityApply, ScCustomsReponsitory>
    {
        public DollarEquityAppliesViewOrigin()
        {
        }

        internal DollarEquityAppliesViewOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DollarEquityApply> GetIQueryable()
        {
            return from apply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquityApplies>()

                   select new Models.DollarEquityApply
                   {
                       ID = apply.ID,
                       ApplyID = apply.ApplyID,
                       ClientID = apply.ClientID,
                       SupplierChnName = apply.SupplierChnName,
                       SupplierEngName = apply.SupplierEngName,
                       BankName = apply.BankName,
                       BankAddress = apply.BankAddress,
                       BankAccount = apply.BankAccount,
                       SwiftCode = apply.SwiftCode,
                       Amount = apply.Amount,
                       Currency = apply.Currency,
                       IsPaid = apply.IsPaid,                      
                       ExpectDate = apply.ExpectDate,
                       Status = (Enums.Status)apply.Status,
                       CreateDate = apply.CreateDate,
                       UpdateDate = apply.UpdateDate,
                       Summary = apply.Summary,
                   };
        }
    }
}
