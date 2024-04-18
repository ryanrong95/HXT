using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class OrderConsigneesOrigin : UniqueView<Models.OrderConsignee, ScCustomsReponsitory>
    {
        internal OrderConsigneesOrigin()
        {
        }

        internal OrderConsigneesOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderConsignee> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignees>()
                   select new Models.OrderConsignee
                   {
                       ID = entity.ID,
                       OrderID = entity.OrderID,
                       ClientSupplierID = entity.ClientSupplierID,
                       Type = (Enums.HKDeliveryType)entity.Type,
                       Contact = entity.Contact,
                       Mobile = entity.Mobile,
                       Tel = entity.Tel,
                       Address = entity.Address,
                       PickUpTime = entity.PickUpTime,
                       WayBillNo = entity.WayBillNo,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       UpdateDate = entity.UpdateDate,
                       Summary = entity.Summary
                   };
        }
    }
}
