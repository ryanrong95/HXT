using Needs.Ccs.Services.Enums;
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
    public class MyOrderUnSealedsView : OrderUnSealedsView
    {
        IGenericAdmin Admin;

        public MyOrderUnSealedsView(IGenericAdmin admin)
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
    /// 待封箱
    /// </summary>
    public class MyOrderUnSealedsView1 : OrderUnSealedsView1
    {
        IGenericAdmin Admin;

        public MyOrderUnSealedsView1(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.Order> GetIQueryable(Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression, params LambdaExpression[] expressions)
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable(expression, expressions).OrderByDescending(item => item.CreateDate);
            }

            var clientIds = new Views.MyClientsView(this.Admin).Select(c => c.ID).ToArray();
            
            return base.GetIQueryable(expression, expressions).Where(item => clientIds.Contains(item.ClientID)).OrderByDescending(item=>item.CreateDate);
        }

        protected override Needs.Ccs.Services.Models.OrderItem[] GetItems(string orderid)
        {
            return base.GetItems(orderid);
        }
    }
}
