using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System.Linq;


namespace Needs.Ccs.Services.Views
{
    public class HandOverView : UniqueView<Models.HandOver, ScCustomsReponsitory>
    {
        public HandOverView()
        {
        }

        internal HandOverView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<HandOver> GetIQueryable()
        {
            return from handOver in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.HandOverRecord>()

                   select new Models.HandOver
                   {
                       ID = handOver.ID,
                       ClientID = handOver.ClientID,
                       AdminLeave = handOver.AdminLeave,
                       AdminWork = handOver.AdminWork,
                       ApplyID = handOver.ApplyID,
                       Status = (Enums.Status)handOver.Status,
                       CreateDate = handOver.CreateDate,
                       UpdateDate = handOver.UpdateDate.Value,
                       Summary = handOver.Summary
                   };
        }
    }
}
