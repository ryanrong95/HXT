using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 运输批次明细的视图
    /// </summary>
    public class VoyageDetailsView : UniqueView<Models.VoyageDetail, ScCustomsReponsitory>
    {
        public VoyageDetailsView()
        {
        }

        internal VoyageDetailsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<VoyageDetail> GetIQueryable()
        {
            var packingsView = new Views.PackingsView(this.Reponsitory);
            var clientsView = new Views.ClientsView(this.Reponsitory);
            var decHeadsView = new Views.DecHeadsView(this.Reponsitory).Where(dc => dc.CusDecStatus != "04");

            var orderItemsCount = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                  where c.Status == (int)Enums.Status.Normal
                                  group c by c.OrderID into d
                                  select new
                                  {
                                      OrderID = d.Key,
                                      ItemsCount = d.Count()
                                  };                                  

            return from entity in packingsView
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on entity.OrderID equals order.ID
                   join orderitem in orderItemsCount on entity.OrderID equals orderitem.OrderID
                   join client in clientsView on order.ClientID equals client.ID
                   join decHead in decHeadsView on order.ID equals decHead.OrderID
                   orderby entity.OrderID, entity.BoxIndex
                   select new Models.VoyageDetail
                   {
                       ID = entity.ID,
                       VoyageNo = decHead.VoyNo,
                       OrderID = order.ID,
                       Client = client,
                       PackingDate = entity.PackingDate,
                       BoxIndex = entity.BoxIndex,
                       ItemsCount = orderitem.ItemsCount
                   };
        }
    }
}
