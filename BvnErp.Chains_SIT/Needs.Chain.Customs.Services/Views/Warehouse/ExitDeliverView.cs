using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 出库交货信息视图
    /// </summary>
    public class ExitDeliverView : UniqueView<Models.ExitDeliver, ScCustomsReponsitory>
    {
        public ExitDeliverView()
        {
        }

        internal ExitDeliverView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.ExitDeliver> GetIQueryable()
        {
            var consigneeView = new ConsigneeView(this.Reponsitory);
            var deliverView = new DeliverView(this.Reponsitory);
            var expressageView = new ExpressageView(this.Reponsitory);

            return from exitDelivers in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitDelivers>()
                   join consignee in consigneeView on exitDelivers.ConsigneeID equals consignee.ID into consignees
                   from consignee in consignees.DefaultIfEmpty()
                   join deliver in deliverView on exitDelivers.DeliverID equals deliver.ID into delivers
                   from deliver in delivers.DefaultIfEmpty()
                   join expressage in expressageView on exitDelivers.ExpressageID equals expressage.ID into expressages
                   from expressage in expressages.DefaultIfEmpty()
                   select new Models.ExitDeliver
                   {
                       ID = exitDelivers.ID,
                       ExitNoticeID = exitDelivers.ExitNoticeID,
                       Code = exitDelivers.Code,
                       Name = exitDelivers.Name,
                       Consignee = consignee,
                       Deliver = deliver,
                       Expressage = expressage,
                       PackNo = exitDelivers.PackNo,
                       DeliverDate = exitDelivers.DeliverDate,
                       Status = (Enums.Status)exitDelivers.Status,
                       CreateDate = exitDelivers.CreateDate,
                       UpdateDate = exitDelivers.UpdateDate,
                       Summary = exitDelivers.Summary
                   };
        }
    }
}
