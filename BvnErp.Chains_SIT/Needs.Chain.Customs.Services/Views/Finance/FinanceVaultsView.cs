using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 金库的视图
    /// </summary>
    public class FinanceVaultsView : UniqueView<Models.FinanceVault, ScCustomsReponsitory>
    {
        public FinanceVaultsView()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public FinanceVaultsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<FinanceVault> GetIQueryable()
        {
            var adminsView = new Views.AdminsTopView(this.Reponsitory);
            var result = from financeVault in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.FinanceVaults>()
                         join admin in adminsView on financeVault.Leader equals admin.ID
                         where financeVault.Status == (int)Enums.Status.Normal
                         select new Models.FinanceVault
                         {
                             ID = financeVault.ID,
                             Leader = financeVault.Leader,
                             Name = financeVault.Name,
                             BigWinVaultID = financeVault.BigWinVaultID,
                             Status = (Enums.Status)financeVault.Status,
                             Summary = financeVault.Summary,
                             Admin = admin,
                             UpdateDate = financeVault.UpdateDate,
                             CreateDate = financeVault.CreateDate,
                         };
            return result;
        }
    }
}
