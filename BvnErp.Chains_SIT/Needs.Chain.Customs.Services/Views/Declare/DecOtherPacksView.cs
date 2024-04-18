using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecOtherPacksView : UniqueView<Models.DecOtherPack, ScCustomsReponsitory>
    {
        public DecOtherPacksView()
        {
        }
        internal DecOtherPacksView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecOtherPack> GetIQueryable()
        {
            return from otherpack in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecOtherPacks>()                  
                   select new Models.DecOtherPack
                   {
                       ID = otherpack.ID,
                       DeclarationID = otherpack.DeclarationID,
                       PackQty = otherpack.PackQty,
                       PackType = otherpack.PackType
                   };
        }
    }
}
