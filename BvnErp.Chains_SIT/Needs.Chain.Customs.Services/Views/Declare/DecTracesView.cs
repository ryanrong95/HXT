using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class DecTracesView : UniqueView<Models.DecTrace, ScCustomsReponsitory>
    {
        public DecTracesView()
        {
        }
        internal DecTracesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.DecTrace> GetIQueryable()
        {
            return from trace in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecTraces>()
                   orderby trace.NoticeDate descending
                   select new Models.DecTrace
                   {
                       ID = trace.ID,
                       DeclarationID = trace.DeclarationID,
                       Channel = trace.Channel,
                       Message = trace.Message,
                       NoticeDate = trace.NoticeDate,
                       FileName = trace.FileName,
                       BackupUrl = trace.BackupUrl
                   };
        }
    }
}
