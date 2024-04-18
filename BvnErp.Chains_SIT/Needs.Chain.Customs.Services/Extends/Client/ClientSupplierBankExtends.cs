using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class ClientSupplierBankExtends
    {
        public static Layer.Data.Sqls.ScCustoms.ClientSupplierBanks ToLinq(this Models.ClientSupplierBank entity)
        {
            return new Layer.Data.Sqls.ScCustoms.ClientSupplierBanks
            {
                ID = entity.ID,
                ClientSupplierID = entity.ClientSupplierID,
                BankAccount = entity.BankAccount,
                BankName = entity.BankName,
                BankAddress = entity.BankAddress,
                SwiftCode = entity.SwiftCode,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary,
                Place=entity.Place,
                Currency=(int)entity.Currency,
                Methord=(int)entity.Methord

            };
        }
    }
}