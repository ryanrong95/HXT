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
    public class AccountCatalogsOrigin : UniqueView<Models.AccountCatalog, ScCustomsReponsitory>
    {
        public AccountCatalogsOrigin()
        {
        }

        public AccountCatalogsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.AccountCatalog> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PvFinanceAccountCatalogs>()
                   select new Models.AccountCatalog
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       ModifierID = entity.ModifierID,
                       ModifyDate = entity.ModifyDate,
                       Status = (GeneralStatus)entity.Status,
                       CreateDate = entity.CreateDate,
                       CreatorID = entity.CreatorID,
                       FatherID = entity.FatherID,
                   };
        }
    }
}
