using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class EdocRealationsView : UniqueView<Models.EdocRealation, ScCustomsReponsitory>
    {
        public EdocRealationsView()
        {
        }
        internal EdocRealationsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.EdocRealation> GetIQueryable()
        {
            var edocView = new BaseEdocCodesView(this.Reponsitory);

            return from edoc in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EdocRealations>()
                   join edcodetail in edocView on edoc.EdocCode equals edcodetail.Code
                   select new Models.EdocRealation
                   {
                       ID = edoc.ID,
                       DeclarationID = edoc.DeclarationID,
                       EdocID = edoc.EdocID,
                       Edoc = edcodetail,
                       EdocCode = edoc.EdocCode,
                       EdocFomatType = edoc.EdocFomatType,
                       OpNote = edoc.OpNote,
                       EdocCopId = edoc.EdocCopId,
                       SignTime = edoc.SignTime,
                       EdocSize = edoc.EdocSize,
                       EdocOwnerCode = edoc.EdocOwnerCode,
                       EdocOwnerName = edoc.EdocOwnerName,
                       FileUrl = edoc.FileUrl
                   };
        }
    }
}
