using Needs.Erp.Generic;
using NtErp.Wss.Oss.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 我的订单视图
    /// </summary>
    public class MyOrdersView : OrderAlls
    {
        IGenericAdmin admin;
        public MyOrdersView(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<Order> GetIQueryable()
        {
            if (this.admin.IsSa)
            {
                return base.GetIQueryable();
            }

            var linq = from entity in base.GetIQueryable()
                       join map in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.MapsOrderAdmin>() on entity.ID equals map.OrderID
                       where map.AdminID == this.admin.ID
                       select entity;

            return linq;
        }

    }
}
