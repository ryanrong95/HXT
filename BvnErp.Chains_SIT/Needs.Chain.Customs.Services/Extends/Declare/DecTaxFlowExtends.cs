using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class DecTaxFlowExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DecTaxFlows ToLinq(this Models.DecTaxFlow entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DecTaxFlows
            {
                ID = entity.ID,
                DecTaxID = entity.DecheadID,
                TaxNumber = entity.TaxNumber,
                TaxType = (int)entity.TaxType,
                Amount = entity.Amount,
                PayDate = entity.PayDate,
                BankName = entity.BankName,
                DeductionTime = entity.DeductionTime,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
                // Add by JS ,on 2019-07-08 
                //IsUpload = (int)entity.IsUpload,
            };
        }
    }
}
