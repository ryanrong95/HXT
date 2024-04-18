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
    public class MyOrderPackingsView : OrderPackingsView
    {
        IGenericAdmin Admin;

        public MyOrderPackingsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.OrderPacking> GetIQueryable()
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
    /// 装箱信息
    /// </summary>
    public class MyOrderPackingsView1 : OrderPackingsView1
    {
        IGenericAdmin Admin;

        public MyOrderPackingsView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.OrderPacking> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.OrderPacking, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(c => c.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(c=>c.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }
}
