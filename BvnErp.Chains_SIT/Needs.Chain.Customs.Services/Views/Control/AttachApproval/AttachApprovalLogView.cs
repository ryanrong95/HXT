using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    public class AttachApprovalLogView
    {
        private ScCustomsReponsitory Reponsitory { get; set; }

        public AttachApprovalLogView()
        {
            this.Reponsitory = new ScCustomsReponsitory();
        }

        public AttachApprovalLogView(ScCustomsReponsitory reponsitory)
        {
            this.Reponsitory = reponsitory;
        }

        public List<AttachApprovalLogViewModel> GetResults(string orderControlID)
        {
            var attachApprovalLogs = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AttachApprovalLogs>();

            var results = from attachApprovalLog in attachApprovalLogs
                          where attachApprovalLog.Status == (int)Enums.Status.Normal
                             && attachApprovalLog.OrderControlID == orderControlID
                          orderby attachApprovalLog.CreateDate descending
                          select new AttachApprovalLogViewModel
                          {
                              AttachApprovalLogID = attachApprovalLog.ID,
                              OrderControlID = attachApprovalLog.OrderControlID,
                              CreateDate = attachApprovalLog.CreateDate,
                              Summary = attachApprovalLog.Summary,
                          };

            return results.ToList();
        }
    }

    public class AttachApprovalLogViewModel
    {
        public string AttachApprovalLogID { get; set; } = string.Empty;

        public string OrderControlID { get; set; } = string.Empty;

        public DateTime CreateDate { get; set; }

        public string Summary { get; set; } = string.Empty;
    }
}
