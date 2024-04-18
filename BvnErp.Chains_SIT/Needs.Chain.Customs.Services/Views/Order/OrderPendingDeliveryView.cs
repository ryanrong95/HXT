using Layer.Data.Sqls;
using Needs.Ccs.Services.Enums;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理报关委托书
    /// </summary>
    public class OrderPendingDeliveryView : View<Models.OrderPendingDelieveryViewModel, ScCustomsReponsitory>
    {
        public OrderPendingDeliveryView()
        {

        }

        protected override IQueryable<Models.OrderPendingDelieveryViewModel> GetIQueryable()
        {
            var mainOrders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>();
            var orders = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().
                Where(t => t.OrderStatus >=(int)OrderStatus.Declared&&t.OrderStatus!=(int)OrderStatus.Canceled&&t.OrderStatus!=(int)OrderStatus.Returned && t.OrderStatus != (int)OrderStatus.Completed);
            var clients = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companies = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var adminsTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();


            //每个子订单已制作出库通知数量，不是已送货数量
            var exitNoticeItemsView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                                      join sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on item.SortingID equals sorting.ID
                                      where item.Status == (int)Enums.Status.Normal 
                                      //&& item.ExitNoticeStatus >= (int)Enums.ExitNoticeStatus.Exited 跟单制单后，出库通知的状态是未出库                                       
                                      group new {item,sorting } by sorting.OrderID into g                                      
                                      select new
                                      {
                                          orderID = g.Key,
                                          noticeQty = g.Sum(c => c.item.Quantity)
                                      };

            //每个子订单 已送货的数量
            var exitedNoticeView = from item in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ExitNoticeItems>()
                                      join sorting in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Sortings>() on item.SortingID equals sorting.ID
                                      where item.Status == (int)Enums.Status.Normal
                                      && item.ExitNoticeStatus >= (int)Enums.ExitNoticeStatus.Exited                                  
                                   group new { item, sorting } by sorting.OrderID into g
                                      select new
                                      {
                                          orderID = g.Key,
                                          eixtedQty = g.Sum(c => c.item.Quantity)
                                      };

            //每个子订单的已通知送货数量，和已送货数量
            var exitAndNoticeView = from noticeView in exitNoticeItemsView
                                    join exitView in exitedNoticeView on noticeView.orderID equals exitView.orderID
                                    into g
                                    from t in g.DefaultIfEmpty()                                    
                                    select new
                                    {
                                        orderID = noticeView.orderID,
                                        noticeQty = noticeView.noticeQty,
                                        exitQty = t==null?0:t.eixtedQty,                                     
                                    };

            //每个主订单的送货数量,已经送货完成的，就不需要显示在跟单的待发货页面了
            var mainDelQtyView = from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                             join exitQty in exitAndNoticeView on order.ID equals exitQty.orderID                              
                                 group new { order, exitQty } by order.MainOrderId
                             into h
                             select new
                             {
                                 mainOrderID = h.Key,
                                 delQty = h.Sum(c => c.exitQty.exitQty),
                                 noticeQty = h.Sum(c=>c.exitQty.noticeQty),                                
                             };


            //每个主订单的数量
            var orderQtyView = from order in orders
                               join orderitems in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                               on order.ID equals orderitems.OrderID
                               into g                              
                               from t in g.DefaultIfEmpty()
                               group t by order.MainOrderId
                               into h
                               select new
                               {
                                   mainOrderID = h.Key,
                                   orderQty = h.Sum(item => item.Quantity)
                               };

            //订单是否通知送货完全
            var qtyView = from c in orderQtyView
                          join d in mainDelQtyView on c.mainOrderID equals d.mainOrderID
                          into g
                          from t in g.DefaultIfEmpty()
                          select new
                          {
                              mainOrderID = c.mainOrderID,
                              HasNotified = t == null ? false : ((c.orderQty - t.noticeQty) == 0 ? true : false),
                              HasExited = t == null ? false : (c.orderQty - t.delQty) == 0 ? true:false
                          };
                          


            var sumInfo = from order in orders
                          join mainorder in mainOrders on order.MainOrderId equals mainorder.ID
                          into g                        
                          from t in g.DefaultIfEmpty()                       
                          group order by new { order.MainOrderId, order.Currency, order.ClientID, t.CreateDate }
                          into h
                          select new
                          {
                              MainOrderID = h.Key.MainOrderId,                           
                              Currency = h.Key.Currency,
                              ClientID = h.Key.ClientID,
                              DeclarePrice = h.Sum(c => c.DeclarePrice),
                              CreateDate = h.Key.CreateDate == null ? DateTime.Now : h.Key.CreateDate,
                          };

            var result = from c in sumInfo
                         join client in clients on c.ClientID equals client.ID
                         join company in companies on client.CompanyID equals company.ID
                         join delievery in qtyView on c.MainOrderID equals delievery.mainOrderID
                         select new Models.OrderPendingDelieveryViewModel
                         {
                             ID = c.MainOrderID,
                             ClientCode = client.ClientCode,
                             ClientID = client.ID,
                             AdminID = client.AdminID,
                             ClientName = company.Name,
                             ClientType = (ClientType)client.ClientType,
                             Currency = c.Currency,
                             DeclarePrice = c.DeclarePrice,
                             CreateDate = c.CreateDate,
                             HasNotified = delievery.HasNotified, 
                             HasExited = delievery.HasExited
                         };



            return result;
        }
    }
}
