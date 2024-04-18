using Layer.Data.Sqls;
using NtErp.Services.Models;
using Needs.Linq;
using System.Linq;
using Needs.Erp.Generic;
using System;

namespace NtErp.Services.Views
{
    public class MyStaffsView : AdminsAlls
    {
        MyStaffsView()
        {

        }
        IGenericAdmin admin;
        public MyStaffsView(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<Admin> GetIQueryable()
        {
            if (this.admin.IsSa)
            {
                return base.GetIQueryable();
            }

            return from admin in base.GetIQueryable()
                   join map in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnErp.MapsAdmin>() on admin.ID equals map.SubID
                   where map.MainID == this.admin.ID
                   select admin;
        }

        public void Bind(string id)
        {
  
            var admin = this[id];
            if (admin == null) 
            {
                admin = base.GetIQueryable().Where(item => item.ID == id).FirstOrDefault();
                Bind(admin);
            }

           
        }
        public void Bind(Admin admin)
        {

          
                this.Reponsitory.Insert(new Layer.Data.Sqls.BvnErp.MapsAdmin
                {
                    MainID =this.admin.ID,
                    SubID = admin.ID,
                    Type= (int)MapsType.Subordinate
                });
           
        }
        public void UnBind(string id)
        {
            var admin = this[id];
            if (admin != null)
            {
                UnBind(admin);
            }

           
        }
        public void UnBind(Admin admin)
        {
           
                this.Reponsitory.Delete<Layer.Data.Sqls.BvnErp.MapsAdmin>(item => item.MainID == this.admin.ID
                    && item.SubID == admin.ID
                    && item.Type == (int)MapsType.Subordinate);
           
        }
    }
}
