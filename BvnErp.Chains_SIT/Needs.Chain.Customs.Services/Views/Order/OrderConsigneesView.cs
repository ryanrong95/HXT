using Layer.Data.Sqls;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 代理订单收货人的视图
    /// </summary>
    public class OrderConsigneesView : UniqueView<Models.OrderConsignee, ScCustomsReponsitory>
    {

        public OrderConsigneesView()
        {
        }

        public OrderConsigneesView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderConsignee> GetIQueryable()
        {
            var clientSuppliersView = new ClientSuppliersView(this.Reponsitory);

            return from orderConsignee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>()
                   join clientSupplier in clientSuppliersView on orderConsignee.ClientSupplierID equals clientSupplier.ID
                   select new Models.OrderConsignee
                   {
                       ID = orderConsignee.ID,
                       OrderID = orderConsignee.OrderID,
                       ClientSupplier = clientSupplier,
                       Type = (Enums.HKDeliveryType)orderConsignee.Type,
                       Contact = orderConsignee.Contact,
                       Mobile = orderConsignee.Mobile,
                       Tel = orderConsignee.Tel,
                       Address = orderConsignee.Address,
                       PickUpTime = orderConsignee.PickUpTime,
                       WayBillNo = orderConsignee.WayBillNo,
                       Status = (Enums.Status)orderConsignee.Status,
                       CreateDate = orderConsignee.CreateDate,
                       UpdateDate = orderConsignee.UpdateDate,
                       Summary = orderConsignee.Summary,
                       DriverID = orderConsignee.DriverID,
                       CarrierID = orderConsignee.CarrierID,
                       CarNumber = orderConsignee.CarNumber
                   };
        }
    }
}
