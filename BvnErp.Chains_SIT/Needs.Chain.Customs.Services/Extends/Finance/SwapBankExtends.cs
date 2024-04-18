using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class SwapBankExtends
    {
        public static Layer.Data.Sqls.ScCustoms.SwapBanks ToLinq(this Models.SwapBank entity)
        {
            return new Layer.Data.Sqls.ScCustoms.SwapBanks
            {
                ID = entity.ID,
                Code=entity.Code,
                Name = entity.Name,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }
}
