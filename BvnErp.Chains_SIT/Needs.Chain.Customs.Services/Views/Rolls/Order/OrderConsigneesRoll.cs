using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Rolls
{
    internal class OrderConsigneesRoll : UniqueView<Models.OrderConsignee, ScCustomsReponsitory>
    {
        internal OrderConsigneesRoll()
        {
        }

        internal OrderConsigneesRoll(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderConsignee> GetIQueryable()
        {
            var orderConsignees = new Origins.OrderConsigneesOrigin(this.Reponsitory);
            var clientSuppliers = new Origins.ClientSuppliersOrigin(this.Reponsitory);

            return from entity in orderConsignees
                   join clientSupplier in clientSuppliers on entity.ClientSupplierID equals clientSupplier.ID
                   select new Models.OrderConsignee
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       ClientSupplier = clientSupplier,
                       Type = entity.Type,
                       Contact = entity.Contact,
                       Mobile = entity.Mobile,
                       Tel = entity.Tel,
                       Address = entity.Address,
                       PickUpTime = entity.PickUpTime,
                       WayBillNo = entity.WayBillNo,
                       Status = entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
