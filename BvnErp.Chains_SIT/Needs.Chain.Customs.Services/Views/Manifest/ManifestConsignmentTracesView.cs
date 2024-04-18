using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class ManifestConsignmentTracesView : UniqueView<Models.ManifestConsignmentTrace, ScCustomsReponsitory>
    {
        public ManifestConsignmentTracesView()
        {
        }
        internal ManifestConsignmentTracesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ManifestConsignmentTrace> GetIQueryable()
        {
            return from trace in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ManifestConsignmentTraces>()
                   orderby trace.NoticeDate descending
                   select new Models.ManifestConsignmentTrace
                   {
                       ID = trace.ID,
                       ManifestConsignmentID = trace.ManifestConsignmentID,
                       StatementCode = trace.StatementCode,
                       Message = trace.Message,
                       NoticeDate = trace.NoticeDate,
                       FileName = trace.FileName,
                       BackupUrl = trace.BackupUrl
                   };
        }
    }
}
