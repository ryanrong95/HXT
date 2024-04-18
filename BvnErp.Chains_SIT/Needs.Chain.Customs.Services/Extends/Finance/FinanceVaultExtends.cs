using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 金库扩展方法
    /// </summary>
    public static class FinanceVaultExtends
    {
        public static Layer.Data.Sqls.ScCustoms.FinanceVaults ToLinq(this Models.FinanceVault entity)
        {
            return new Layer.Data.Sqls.ScCustoms.FinanceVaults
            {
                ID = entity.ID,
                AdminID = entity.Admin.ID,
                Leader = entity.Leader,
                Name = entity.Name,
                BigWinVaultID = entity.BigWinVaultID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }
}
