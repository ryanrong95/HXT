using Needs.Ccs.Services.Views;
using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Admin.Plat.Views
{
    public class MyMainOrderBillsView : MainOrderBillsView
    {
        IGenericAdmin Admin;

        public MyMainOrderBillsView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        protected override IQueryable<Needs.Ccs.Services.Models.OrderBill> GetIQueryable()
        {
            if (this.Admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from bill in base.GetIQueryable()
                   where bill.Client.Merchandiser.ID == this.Admin.ID
                   select bill;
        }
    }
}
