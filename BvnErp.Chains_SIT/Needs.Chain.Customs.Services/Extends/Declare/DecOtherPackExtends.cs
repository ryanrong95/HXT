using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class DecOtherPackExtends
    {
        public static Layer.Data.Sqls.ScCustoms.DecOtherPacks ToLinq(this Models.DecOtherPack entity)
        {
            return new Layer.Data.Sqls.ScCustoms.DecOtherPacks
            {
                ID = entity.ID,
                DeclarationID = entity.DeclarationID,
                PackQty = entity.PackQty,
                PackType = entity.PackType
            };
        }
    }
}
