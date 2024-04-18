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
    public class DollarEquityAppliesOrigin : UniqueView<Models.DollarEquityApply, ScCustomsReponsitory>
    {
        public DollarEquityAppliesOrigin()
        {
        }

        public DollarEquityAppliesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<DollarEquityApply> GetIQueryable()
        {
            return from dollarEquityApply in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DollarEquityApplies>()
                   select new Models.DollarEquityApply
                   {
                       ID = dollarEquityApply.ID,
                       ApplyID = dollarEquityApply.ApplyID,
                       ClientID = dollarEquityApply.ClientID,
                       SupplierChnName = dollarEquityApply.SupplierChnName,
                       SupplierEngName = dollarEquityApply.SupplierEngName,
                       BankName = dollarEquityApply.BankName,
                       BankAddress = dollarEquityApply.BankAddress,
                       BankAccount = dollarEquityApply.BankAccount,
                       SwiftCode = dollarEquityApply.SwiftCode,
                       Amount = dollarEquityApply.Amount,
                       Currency = dollarEquityApply.Currency,
                       IsPaid = dollarEquityApply.IsPaid,                       
                       ExpectDate = dollarEquityApply.ExpectDate,
                       Status = (Enums.Status)dollarEquityApply.Status,
                       CreateDate = dollarEquityApply.CreateDate,
                       UpdateDate = dollarEquityApply.UpdateDate,
                       Summary = dollarEquityApply.Summary,
                   };
        }
    }
}
