using Layer.Data.Sqls;
using Needs.Linq;
using Needs.Wl.User.Plat.Models;
using System.Linq;

namespace Needs.Wl.User.Plat.Views
{
    /// <summary>
    /// 未收货的订单
    /// </summary>
    public class MyUnReceivedOrdersView : View<Needs.Wl.Client.Services.PageModels.UnReceivedOrder, ScCustomsReponsitory>
    {
        IPlatUser User;

        public MyUnReceivedOrdersView(IPlatUser user)
        {
            this.User = user;
        }

        protected override IQueryable<Needs.Wl.Client.Services.PageModels.UnReceivedOrder> GetIQueryable()
        {
            if (this.User.IsMain)
            {
                return this.ClientOrders();
            }
            else
            {
                return this.UserOrders();
            }
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.UnReceivedOrder> UserOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join consignors in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignors>() on order.ID equals consignors.OrderID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                    && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.WarehouseExited
                    && order.UserID == this.User.ID
                   orderby order.CreateDate ascending
                   select new Needs.Wl.Client.Services.PageModels.UnReceivedOrder
                   {
                       ID = order.ID,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       Currency = order.Currency,
                       Consignor = new Wl.Models.OrderConsignor()
                       {
                           ID = consignors.ID,
                           Type = (Wl.Models.Enums.SZDeliveryType)consignors.Type,
                           Name = consignors.Name,
                           Contact = consignors.Contact,
                           Address = consignors.Address,                   
                           IDType = consignors.IDType,
                           IDNumber = consignors.IDNumber,
                           Mobile = consignors.Mobile,
                           Tel = consignors.Tel
                       },
                       DeclarePrice = order.DeclarePrice,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       CreateDate = order.CreateDate,
                       Summary = order.Summary
                   };
        }

        private IQueryable<Needs.Wl.Client.Services.PageModels.UnReceivedOrder> ClientOrders()
        {
            return from order in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                   join consignors in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignors>() on order.ID equals consignors.OrderID
                   where order.Status == (int)Needs.Wl.Models.Enums.Status.Normal
                    && order.OrderStatus == (int)Needs.Wl.Models.Enums.OrderStatus.WarehouseExited
                    && order.ClientID == this.User.ClientID
                   orderby order.CreateDate ascending
                   select new Needs.Wl.Client.Services.PageModels.UnReceivedOrder
                   {
                       ID = order.ID,
                       ClientID = order.ClientID,
                       AgreementID = order.ClientAgreementID,
                       Currency = order.Currency,
                       Consignor = new Wl.Models.OrderConsignor()
                       {
                           ID = consignors.ID,
                           Type = (Wl.Models.Enums.SZDeliveryType)consignors.Type,
                           Name = consignors.Name,
                           Contact = consignors.Contact,
                           Address = consignors.Address,                        
                           IDType = consignors.IDType,
                           IDNumber = consignors.IDNumber,
                           Mobile = consignors.Mobile,
                           Tel = consignors.Tel
                       },
                       DeclarePrice = order.DeclarePrice,
                       OrderStatus = (Needs.Wl.Models.Enums.OrderStatus)order.OrderStatus,
                       CreateDate = order.CreateDate,
                       Summary = order.Summary
                   };
        }
    }
}