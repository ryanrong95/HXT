using Needs.Erp.Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Views
{
  public   class MyContactView :ContactsView
    {
        IGenericAdmin admin;
        public MyContactView(IGenericAdmin admin)
        {
            this.admin = admin;
        }

        protected override IQueryable<Models.Contact> GetIQueryable()
        {
            if (this.admin.IsSa)
            {
                return base.GetIQueryable();
            }

            var linq = from entity in base.GetIQueryable()
                       join map in Reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.MapsAdminCompany>() on entity.ID equals map.CompanyID
                       where map.AdminID == this.admin.ID
                       select entity;

            return linq;
        }
       
        public void Bind(string id)
        {
            var contact =   base.GetIQueryable().Single(t => t.ID == id);
            if (contact  == null)
            {
                throw new Exception("contact does not exist!");
            }
            Bind(contact);
        }
          
        public void Bind(Models.Contact entity)
        {
            using (var repository = new Layer.Data.Sqls.BvnErpReponsitory())
            {
                if (!repository.ReadTable<Layer.Data.Sqls.BvnVrs.MapsAdminCompany>().Any(item => item.CompanyID == entity.ID && item.AdminID == this.admin.ID))
                {
                    repository.Insert(new Layer.Data.Sqls.BvnVrs.MapsAdminCompany
                    {
                        AdminID = this.admin.ID,
                        CompanyID = entity.ID
                    });
                }
            }
        }
     
        public void UnBind(string id)
        {
            var contact = base.GetIQueryable().Single(t => t.ID == id);
            if (contact == null)
            {
                throw new Exception("contact does not exist!");
            }
            UnBind(contact);
        }
     
        public void UnBind(Models.Contact entity)
        {
            using (var repository = new Layer.Data.Sqls.BvnVrsReponsitory())
            {
                repository.Delete<Layer.Data.Sqls.BvnVrs.MapsAdminCompany>(item => item.AdminID == this.admin.ID && item.CompanyID == entity.CompanyID);
            }
        }

    }
}
