using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ClientSupplierBanksView : UniqueView<Models.ClientSupplierBank, ScCustomsReponsitory>
    {
        public ClientSupplierBanksView()
        {
        }

        internal ClientSupplierBanksView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ClientSupplierBank> GetIQueryable()
        {
            return from account in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierBanks>()
                   orderby account.CreateDate descending
                   select new Models.ClientSupplierBank
                   {
                       ID = account.ID,
                       ClientSupplierID = account.ClientSupplierID,
                       BankAccount = account.BankAccount,
                       BankAddress = account.BankAddress,
                       BankName = account.BankName,
                       SwiftCode = account.SwiftCode,
                       Status = (Enums.Status)account.Status,
                       CreateDate = account.CreateDate,
                       Summary = account.Summary,
                       Place=account.Place,
                       Currency=(Needs.Underly.CRMCurrency)account.Currency,
                       Methord=(Methord)account.Methord
                   };
        }
    }


    public class ClientSupplierBanksByNameView : UniqueView<ClientSupplierBankForQ, ScCustomsReponsitory>
    {
        public ClientSupplierBanksByNameView()
        {
        }

        internal ClientSupplierBanksByNameView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<ClientSupplierBankForQ> GetIQueryable()
        {
            return from account in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSupplierBanks>()
                   join supplier in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>() on account.ClientSupplierID equals supplier.ID
                   orderby account.CreateDate descending
                   select new ClientSupplierBankForQ
                   {
                       ID = account.ID,
                       ClientSupplierID = account.ClientSupplierID,
                       BankAccount = account.BankAccount,
                       BankAddress = account.BankAddress,
                       BankName = account.BankName,
                       SwiftCode = account.SwiftCode,
                       Status = (Enums.Status)account.Status,
                       CreateDate = account.CreateDate,
                       Summary = account.Summary,
                       Place=account.Place,
                       Currency=(Needs.Underly.CRMCurrency)account.Currency,
                       Methord=(Methord)account.Methord,
                       SupplierName = supplier.Name
                   };
        }
    }

    public class ClientSupplierBankForQ : ClientSupplierBank
    {
        public string SupplierName { get; set; }
    }
}
