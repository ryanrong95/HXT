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
    /// 代理订单交货人的视图
    /// </summary>
    public class OrderConsignorsView : UniqueView<Models.OrderConsignor, ScCustomsReponsitory>
    {
        public OrderConsignorsView()
        {
        }

        internal OrderConsignorsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<OrderConsignor> GetIQueryable()
        {
            return from orderConsignor in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderConsignors>()
                   select new Models.OrderConsignor
                   {
                       ID = orderConsignor.ID,
                       OrderID = orderConsignor.OrderID,
                       Type = (Enums.SZDeliveryType)orderConsignor.Type,
                       Name = orderConsignor.Name,
                       Contact = orderConsignor.Contact,
                       Mobile = orderConsignor.Mobile,
                       Tel = orderConsignor.Tel,
                       Address = orderConsignor.Address,
                       IDType = orderConsignor.IDType,
                       IDNumber = orderConsignor.IDNumber,
                       Status = (Enums.Status)orderConsignor.Status,
                       CreateDate = orderConsignor.CreateDate,
                       UpdateDate = orderConsignor.UpdateDate,
                       Summary = orderConsignor.Summary
                   };
        }
    }
}