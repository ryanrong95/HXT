
using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Underly;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 分拣结果View
    /// </summary>
    public class CenterSortingsView : UniqueView<Models.Sorting, ScCustomsReponsitory>
    {
        public CenterSortingsView()
        {
        }

        internal CenterSortingsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Sorting> GetIQueryable()
        {
            var itemView = new OrderItemsView(this.Reponsitory);          
            var clientsView = new ClientsView(this.Reponsitory);
            return from sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclaresTopView>()
                   //where sorting.Status == (int)Enums.Status.Normal
                   join item in itemView on sorting.ItemID equals item.ID
                   join order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>() on item.OrderID equals order.ID
                   join client in clientsView on order.ClientID equals client.ID                 
                   select new Models.Sorting
                   {
                       ID = sorting.UnqiueID,
                       AdminID = sorting.Packer,
                       OrderID = sorting.TinyOrderID,
                       OrderItem = item,
                       //EntryNoticeItemID = sorting.EntryNoticeItemID,
                       //WarehouseType = (Enums.WarehouseType)sorting.WarehouseType,
                       //WrapType = sorting.WrapType,
                       //Product = item.Product,
                       Quantity = sorting.Quantity,
                       BoxIndex = sorting.BoxCode,
                       NetWeight = sorting.NetWeight==null?0:sorting.NetWeight.Value,
                       GrossWeight = sorting.Weight==null?0: sorting.Weight.Value,
                       //DecStatus = (Enums.SortingDecStatus)sorting.DecStatus,
                       //Status = (Enums.Status)sorting.Status,
                       CreateDate = sorting.BoxingDate,
                       UpdateDate = sorting.BoxingDate,
                      
                       Order = new Order
                       {
                           ID = order.ID,
                           Currency = order.Currency,
                           Client = client,
                       },

                       SZPackingDate = sorting.BoxingDate

                   };
        }
       
    }
}