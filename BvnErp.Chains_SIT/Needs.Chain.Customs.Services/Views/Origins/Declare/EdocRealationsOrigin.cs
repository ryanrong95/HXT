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
    public class EdocRealationsOrigin : UniqueView<Models.EdocRealation, ScCustomsReponsitory>
    {
        public EdocRealationsOrigin()
        {
        }

        public EdocRealationsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<EdocRealation> GetIQueryable()
        {
            return from edocRealation in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EdocRealations>()
                   select new Models.EdocRealation
                   {
                       ID = edocRealation.ID,
                       DeclarationID = edocRealation.DeclarationID,
                       EdocID = edocRealation.EdocID,
                       EdocCode = edocRealation.EdocCode,
                       EdocFomatType = edocRealation.EdocFomatType,
                       OpNote = edocRealation.OpNote,
                       EdocCopId = edocRealation.EdocCopId,
                       SignTime = edocRealation.SignTime,
                       EdocOwnerCode = edocRealation.EdocOwnerCode,
                       EdocOwnerName = edocRealation.EdocOwnerName,
                       EdocSize = edocRealation.EdocSize,
                       FileUrl = edocRealation.FileUrl,
                   };
        }
    }
}
