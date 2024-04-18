using Layer.Data.Sqls;
using Needs.Erp.Generic;
using Needs.Linq;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace NtErp.Crm.Services.Views
{
    public class MyOrdersView :OrderAlls
    {
        IGenericAdmin Admin;
        MyOrdersView()
        {

        }
        public MyOrdersView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        public IQueryable<Layer.Data.Sqls.BvCrm.Orders> GetQueries()
        {
            AdminTop admin = Extends.AdminExtends.GetTop(this.Admin.ID);

            switch (admin.JobType)
            {
                case JobType.Sales:
                    return this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Orders>().Where(item => item.AdminID == admin.ID);
                case JobType.TPM:
                    {
                        return this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Orders>();
                    }
                default:
                    throw new NotImplementedException($"{admin.JobType}'s Implement is not exsit!");
            }

        }

        protected override IQueryable<Order> GetIQueryable()
        {
            return from order in base.GetIQueryable(this.GetQueries())
                   where order.Status != Enums.Status.Delete
                   select order;
        }
    }
}
