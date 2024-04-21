using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    public class GoldStoresOrigin : UniqueView<GoldStore, PvFinanceReponsitory>
    {
        internal GoldStoresOrigin() { }

        internal GoldStoresOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<GoldStore> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.GoldStores>()
                   select new GoldStore()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Summary = entity.Summary,
                       IsSpecial = entity.IsSpecial,
                       OwnerID = entity.OwnerID,
                       CreatorID = entity.CreatorID,
                       ModifierID = entity.ModifierID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (Underly.GeneralStatus)entity.Status,
                   };
        }
    }
}
