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
    public class DeliveryOrderAdvavceMoneyView : View<Models.OrderPendingDelieveryViewModel, ScCustomsReponsitory>
    {
        public DeliveryOrderAdvavceMoneyView()
        {

        }

        protected override IQueryable<Models.OrderPendingDelieveryViewModel> GetIQueryable()
        {

            var vastOrdersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.MainOrders>();
            var ordersView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>();
            var clientsView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>();
            var companiesView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Companies>();
            var adminsTopView = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdminsTopView>();
            var advanceRecords = this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.AdvanceRecords>();

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
                          join advanceRecord in advanceRecords on c.OrderID equals advanceRecord.OrderID
                          where advanceRecord.PaidAmount != advanceRecord.Amount && (advanceRecord.AdvanceTime.AddDays(advanceRecord.LimitDays) < DateTime.Now) && string.IsNullOrEmpty(advanceRecord.UntieAdvance.ToString())
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
                              Amount = advanceRecord.Amount,
                              AdvanceRecordID = advanceRecord.ID,
                              HasNotified = delievery == null ? false : (c.Quantity == delievery.Quantity ? true : false),
                          };
            return result1.AsQueryable();
        }
    }
}
