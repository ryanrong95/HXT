using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.XDTModels;

namespace Yahv.PvWsOrder.Services.XDTClientView
{
    /// <summary>
    /// 报关委托书视图
    /// </summary>
    public class OrderAgentProxyView : UniqueView<OrderAgentProxy, PvWsOrderReponsitory>
    {
        private string orderid;

        private OrderAgentProxyView()
        {

        }

        public OrderAgentProxyView(string OrderID)
        {
            this.orderid = OrderID;
        }

        protected override IQueryable<OrderAgentProxy> GetIQueryable()
        {
            var order = new ClientViews.OrderAlls(this.Reponsitory)[orderid];
            var Waybill = new Yahv.Services.Views.WaybillsTopView<PvWsOrderReponsitory>(this.Reponsitory)[order.Input.WayBillID];
            var orderitems = new ClientViews.OrderItemAlls(this.Reponsitory).SearchByOrderID(orderid);

            var linq = from item in orderitems
                       join term in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>() on item.ID equals term.ID
                       join chcd in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>() on item.ID equals chcd.ID
                       join product in new Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>(this.Reponsitory) on item.ProductID equals product.ID
                       join classfied in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ClassifiedPartNumbersTopView>()
                       on chcd.SecondHSCodeID equals classfied.ID
                       select new ClientModels.OrderItem
                       {
                           ID = item.ID,
                           Name = item.Name,
                           OrderID = item.OrderID,
                           InputID = item.InputID,
                           ProductID = item.ProductID,
                           TinyOrderID = item.TinyOrderID,
                           Origin = item.Origin,
                           DateCode = item.DateCode,
                           Quantity = item.Quantity,
                           Currency = item.Currency,
                           UnitPrice = item.UnitPrice,
                           Unit = item.Unit,
                           TotalPrice = item.TotalPrice,
                           CreateDate = item.CreateDate,
                           ModifyDate = item.ModifyDate,
                           GrossWeight = item.GrossWeight,
                           Volume = item.Volume,
                           Conditions = item.Conditions,
                           Status = item.Status,
                           Product = product,
                           TraiffRate = classfied.ImportPreferentialTaxRate + term.OriginRate,
                       };
            return null;
        }
    }
}
