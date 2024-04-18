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
    /// 运输批次的报关单的视图
    /// </summary>
    public class VoyageDecHeadsView : UniqueView<Models.VoyageDecHead, ScCustomsReponsitory>
    {
        protected override IQueryable<VoyageDecHead> GetIQueryable()
        {
            var clientsView = new Views.ClientsView(this.Reponsitory);
            var decHeads = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(dc => dc.CusDecStatus != "04");
            var orderItemsCount = from c in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                                  where c.Status == (int)Enums.Status.Normal
                                  group c by c.OrderID into d
                                  select new
                                  {
                                      OrderID = d.Key,
                                      ItemsCount = d.Count()
                                  };

            return from head in decHeads
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on head.OrderID equals order.ID
                   join orderitem in orderItemsCount on head.OrderID equals orderitem.OrderID
                   join client in clientsView on order.ClientID equals client.ID
                   join decList in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecLists>() on head.ID equals decList.DeclarationID into decLists
                   select new Models.VoyageDecHead
                   {
                       ID = head.ID,
                       VoyageNo = head.VoyNo,
                       OrderID = head.OrderID,
                       Client = client,
                       PackNo = head.PackNo,
                       GQty = decLists.Sum(dl => dl.GQty),
                       GrossWt = head.GrossWt,
                       DeclTotal = decLists.Sum(dl => dl.DeclTotal),
                       ItemsCount = orderitem.ItemsCount,
                   };
        }
    }
}
