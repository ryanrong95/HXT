using Needs.Erp.Generic;
using NtErp.Crm.Services.Enums;
using NtErp.Crm.Services.Models;
using System;
using System.Linq;

namespace NtErp.Crm.Services.Views
{
    public class MyPlansView : PlanAlls
    {
        IGenericAdmin Admin;

        MyPlansView()
        {

        }

        public MyPlansView(IGenericAdmin admin)
        {
            this.Admin = admin;
        }

        public IQueryable<Layer.Data.Sqls.BvCrm.Actions> GetQueries()
        {
            var alls = new Views.AdminTopView();
            AdminTop admin;
            admin = alls[this.Admin.ID];
            alls.Dispose();

            switch (admin.JobType)
            {
                case JobType.Sales:
                    return this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Actions>().Where(item => item.AdminID == admin.ID);
                case JobType.FAE:
                case JobType.PME:
                    return this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Actions>().Where(item => item.AdminID == admin.ID);
                case JobType.TPM:
                    {
                        return this.Reponsitory.ReadTable<Layer.Data.Sqls.BvCrm.Actions>();
                    }
                default:
                    throw new NotImplementedException($"{admin.JobType}'s Implement is not exsit!");
            }

        }


        protected override IQueryable<Plan> GetIQueryable()
        {
            return from action in base.GetIQueryable(this.GetQueries())
                   where action.Status != ActionStatus.Delete
                   select action;
        }
    }
}