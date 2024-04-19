using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Extends
{
    static public class ToLinqExtends
    {
        static public Layer.Data.Sqls.BvnVrs.Invoices ToLinq(this Models.Invoice entity)
        {
            return new Layer.Data.Sqls.BvnVrs.Invoices
            {
                ID = entity.ID,
                Required = entity.Required,
                Type = (int)entity.Type,
                CompanyID = entity.CompanyID,
                ContactID = entity.ContactID,
                Address = entity.Address,
                Postzip = entity.Postzip,
                Bank = entity.Bank,
                BankAddress = entity.BankAddress,
                Account = entity.Account,
                SwiftCode = entity.SwiftCode,
            };
        }

        static public Layer.Data.Sqls.BvnVrs.Companies ToLinq(this Models.Company entity)
        {
            return new Layer.Data.Sqls.BvnVrs.Companies
            {
                ID = entity.ID,
                Name = entity.Name,
                Type = (int)entity.Type,
                Code = entity.Code,
                Address = entity.Address,
                RegisteredCapital = entity.RegisteredCapital,
                CorporateRepresentative = entity.CorporateRepresentative,
                Summary = entity.Summary,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate

            };
        }
    }
}
