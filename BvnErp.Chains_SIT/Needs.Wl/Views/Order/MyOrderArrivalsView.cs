using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    public class MyOrderArrivalsView : OrderArrivalsView
    {
        IGenericAdmin Admin;

        public MyOrderArrivalsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from order in base.GetIQueryable()
                   where order.Client.Merchandiser.ID == this.Admin.ID
                   select order;
        }
    }

    /// <summary>
    /// 已取消订单的视图（Admin过滤）
    /// </summary>
    public class MyOrderArrivalsView1 : OrderArrivalsView1
    {
        IGenericAdmin Admin;

        public MyOrderArrivalsView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c => c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }
}
