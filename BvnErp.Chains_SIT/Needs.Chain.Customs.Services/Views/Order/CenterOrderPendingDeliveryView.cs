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
    public class CenterOrderPendingDeliveryView : View<Models.OrderPendingDelieveryViewModel, ScCustomsReponsitory>
    {
        public CenterOrderPendingDeliveryView()
        {

        }

        protected override IQueryable<Models.OrderPendingDelieveryViewModel> GetIQueryable()
        {
            //var szWarehouseName = System.Configuration.ConfigurationManager.AppSettings["SZWareHouseID"];

            var vastOrdersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>();
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companiesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var adminsTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            //



            //写个专有的已发送出库通知
            //需要在华芯通数据中增加库房的视图后， 重新拖动Layer.Data.Sqls.ScCustoms 下 Comm.dbml
            //var qtyView = from c in storagesTopView
            //              where c.WareHouseID == szWarehouseName && (c.Total - c.Quantity) > 0
            //              group c by c.OrderID into g
            //              select new
            //              {
            //                  mainOrderID = g.Key,
            //                  HasNotified = false,
            //                  HasExited = false,
            //              };

            #region notice的方式

            //var cgNoticeTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgNoticedTopView>();

            //var qtyView = from c in cgNoticeTopView
            //              group c by c.OrderID into g
            //              select new
            //              {
            //                  mainOrderID = g.Key,
            //                  Total = g.Sum(t => t.Total),
            //                  Quantity = g.Sum(t => t.Quantity),
            //              };

            //var sumInfo = from order in ordersView
            //              join vastorder in vastOrdersView on order.MainOrderId equals vastorder.ID
            //              where order.OrderStatus == (int)OrderStatus.Declared
            //                && order.OrderStatus != (int)OrderStatus.Canceled
            //                && order.OrderStatus != (int)OrderStatus.Returned
            //                && order.OrderStatus != (int)OrderStatus.Completed
            //              group order by new
            //              {
            //                  order.MainOrderId,
            //                  order.Currency,
            //                  order.ClientID,
            //                  vastorder.CreateDate
            //              }
            //             into h
            //              select new
            //              {
            //                  MainOrderID = h.Key.MainOrderId,
            //                  Currency = h.Key.Currency,
            //                  ClientID = h.Key.ClientID,
            //                  DeclarePrice = h.Sum(c => c.DeclarePrice),
            //                  CreateDate = h.Key.CreateDate == null ? DateTime.Now : h.Key.CreateDate,
            //              };


            //var result = from c in sumInfo
            //             join client in clientsView on c.ClientID equals client.ID
            //             join company in companiesView on client.CompanyID equals company.ID
            //             join delievery in qtyView on c.MainOrderID equals delievery.mainOrderID into de_qty
            //             from delievery in de_qty.DefaultIfEmpty()
            //             orderby c.CreateDate descending
            //             select new Models.OrderPendingDelieveryViewModel
            //             {
            //                 ID = c.MainOrderID,
            //                 ClientCode = client.ClientCode,
            //                 ClientID = client.ID,
            //                 AdminID = client.AdminID,
            //                 ClientName = company.Name,
            //                 ClientType = (ClientType)client.ClientType,
            //                 Currency = c.Currency,
            //                 DeclarePrice = c.DeclarePrice,
            //                 CreateDate = c.CreateDate,
            //                 HasNotified = delievery == null ? false : (delievery.Total == delievery.Quantity ? true : false),
            //                 //HasExited = delievery == null ? false : (delievery.Total == delievery.Quantity ? true : false)
            //             };
            #endregion

            #region ryan自己的方式

            var storagesTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgSzStoragesTopView>();

            var had = (from sto in storagesTopView
                      group sto by sto.TinyOrderID into s
                      select new
                      {
                          OrderID = s.Key,
                          Quantity = s.Sum(c => c.Quantity)
                      }).ToList();

            var itemsInfo = (from order in ordersView
                            join vastorder in vastOrdersView on order.MainOrderId equals vastorder.ID
                            join orderitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>() on order.ID equals orderitem.OrderID
                            where order.OrderStatus == (int)OrderStatus.Declared
                              && order.OrderStatus != (int)OrderStatus.Canceled
                              && order.OrderStatus != (int)OrderStatus.Returned
                              && order.OrderStatus != (int)OrderStatus.Completed
                            select new
                            {
                                order.ID,
                                order.MainOrderId,
                                order.Currency,
                                order.ClientID,
                                vastorder.CreateDate,
                                order.DeclarePrice,
                                orderitem.Quantity
                            } into info
                            group info by new
                            {
                                info.ID,
                                info.MainOrderId,
                                info.Currency,
                                info.ClientID,
                                info.CreateDate,
                                info.DeclarePrice,
                            }
                        into h
                            select new
                            {
                                OrderID = h.Key.ID,
                                MainOrderId = h.Key.MainOrderId,
                                Currency = h.Key.Currency,
                                ClientID = h.Key.ClientID,
                                DeclarePrice = h.Key.DeclarePrice,  //h.Sum(c => c.DeclarePrice),
                                Quantity = h.Sum(c => c.Quantity),
                                CreateDate = h.Key.CreateDate == null ? DateTime.Now : h.Key.CreateDate,
                            }).ToList();


            var result1 = from c in itemsInfo
                          join client in clientsView on c.ClientID equals client.ID
                          join company in companiesView on client.CompanyID equals company.ID
                          join delievery in had on c.OrderID equals delievery.OrderID into de_qty
                          from delievery in de_qty.DefaultIfEmpty()
                          orderby c.CreateDate descending
                          select new Models.OrderPendingDelieveryViewModel
                          {
                              ID = c.OrderID,
                              MainOrderID = c.MainOrderId,
                              ClientCode = client.ClientCode,
                              ClientID = client.ID,
                              AdminID = client.AdminID,
                              ClientName = company.Name,
                              ClientType = (ClientType)client.ClientType,
                              Currency = c.Currency,
                              DeclarePrice = c.DeclarePrice,
                              CreateDate = c.CreateDate,
                              HasNotified = delievery == null ? false : (c.Quantity == delievery.Quantity ? true : false),
                              //HasExited = delievery == null ? false : (delievery.Total == delievery.Quantity ? true : false)
                          };

            #endregion



            return result1.AsQueryable();
        }
    }

    /// <summary>
    /// 深圳分拣结果View,跟单待出库订单，display页面用
    /// </summary>
    public class SZCenterSortingsViewForDisplay : UniqueView<Models.SZSorting, ScCustomsReponsitory>
    {
        public SZCenterSortingsViewForDisplay()
        {
        }

        internal SZCenterSortingsViewForDisplay(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.SZSorting> GetIQueryable()
        {
            var szWarehouseName = System.Configuration.ConfigurationManager.AppSettings["SZWareHouseID"];
            var storagesTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.CgSzStoragesTopView>();

            var result = from sorting in storagesTopView
                         join orderitem in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>()
                         on sorting.ItemID equals orderitem.ID
                         join cate in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemCategories>()
                         on orderitem.ID equals cate.OrderItemID
                         where sorting.WareHouseID == szWarehouseName && sorting.Type == 200
                         select new Models.SZSorting
                         {
                             ID = sorting.ID,
                             OrderID = sorting.OrderID,
                             BoxIndex = sorting.BoxCode,
                             DateBoxIndex = sorting.DateBoxCode,
                             OrderItem = new Models.OrderItem
                             {
                                 Name = cate.Name,
                                 Manufacturer = orderitem.Manufacturer,
                                 Model = orderitem.Model,
                                 Origin = orderitem.Origin,
                             },

                             Quantity = sorting.Total.Value,
                             DeliveriedQuantity = sorting.Quantity,
                         };
            return result;
        }

    }
}
