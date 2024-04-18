using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class SwapLimitCountryExtends
    {
        public static Layer.Data.Sqls.ScCustoms.SwapLimitCountries ToLinq(this Models.SwapLimitCountry entity)
        {
            return new Layer.Data.Sqls.ScCustoms.SwapLimitCountries
            {
                ID = entity.ID,
                BankID=entity.BankID,
                Code = entity.Code,
                Name = entity.Name,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }
}
