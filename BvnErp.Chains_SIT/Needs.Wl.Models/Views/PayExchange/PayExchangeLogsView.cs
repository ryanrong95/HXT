using Layer.Data.Sqls;
using Needs.Linq;
using System.Linq;

namespace Needs.Wl.Models.Views
{
    /// <summary>
    /// 付汇申请日志记录
    /// </summary>
    public class PayExchangeLogsView : View<Models.PayExchangeLog, ScCustomsReponsitory>
    {
        private string PayExchangeID;

        public PayExchangeLogsView(string payExchangeID)
        {
            this.PayExchangeID = payExchangeID;
        }

        internal PayExchangeLogsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.PayExchangeLog> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyLogs>()
                   where entity.PayExchangeApplyID == this.PayExchangeID
                   orderby entity.CreateDate descending
                   select new Models.PayExchangeLog
                   {
                       ID = entity.ID,
                       PayExchangeApplyID = entity.PayExchangeApplyID,
                       PayExchangeApplyStatus = (Enums.PayExchangeApplyStatus)entity.PayExchangeApplyStatus,
                       AdminID = entity.AdminID,
                       UserID = entity.UserID,
                       Summary = entity.Summary,
                       CreateDate = entity.CreateDate
                   };
        }

        public Models.PayExchangeLog GetCompleted()
        {
            return this.GetIQueryable().Where(t => t.PayExchangeApplyStatus == Needs.Wl.Models.Enums.PayExchangeApplyStatus.Completed).FirstOrDefault();
        }
    }
}