using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    public class SupplierBanksView : View<Models.ClientSupplierBank, ScCustomsReponsitory>
    {
        private string SupplierID;

        public SupplierBanksView(string supplierID)
        {
            this.SupplierID = supplierID;
        }

        internal SupplierBanksView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientSupplierBank> GetIQueryable()
        {
            return from bank in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierBanks>()
                   where bank.ClientSupplierID == this.SupplierID && bank.Status == (int)Enums.Status.Normal
                   orderby bank.CreateDate descending
                   select new Models.ClientSupplierBank
                   {
                       ID = bank.ID,
                       ClientSupplierID = bank.ClientSupplierID,
                       BankAccount = bank.BankAccount,
                       BankAddress = bank.BankAddress,
                       BankName = bank.BankName,
                       SwiftCode = bank.SwiftCode,
                       Status = bank.Status,
                       CreateDate = bank.CreateDate,
                       Summary = bank.Summary
                   };
        }
    }
}