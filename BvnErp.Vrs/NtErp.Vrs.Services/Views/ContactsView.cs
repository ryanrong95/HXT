using Layer.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Vrs.Services.Views
{
    public class ContactsView : Needs.Linq.UniqueView<Models.Contact, BvnVrsReponsitory>
    {
        public ContactsView()
        {

        }

        internal ContactsView(BvnVrsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Contact> GetIQueryable()
        {
            CompaniesView companiesView = new CompaniesView(this.Reponsitory);

            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.BvnVrs.Contacts>()
                   join company in companiesView on entity.CompanyID equals company.ID
                   select new Models.Contact
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Sex = entity.Sex,
                       Birthday = entity.Birthday,
                       Tel = entity.Tel,
                       Email = entity.Email,
                       Mobile = entity.Mobile,
                       CompanyID = entity.CompanyID,
                       Status = (Enums.Status)entity.Status,
                       Job =(Enums.JobType) entity.Job,
                       Company = company

                   };
        }
    }
}
