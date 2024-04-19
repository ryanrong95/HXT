using NtErp.Wss.Oss.Services;
using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Oss.Services.Views
{
    /// <summary>
    /// 联系人视图
    /// </summary>
    public class ContactsView : UniqueView<Models.Contact, CvOssReponsitory>
    {
        internal ContactsView()
        {

        }
        internal ContactsView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Contact> GetIQueryable()
        {
            var companiesView = new CompaniesView(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Contacts>()
                       join _company in companiesView on entity.CompanyID equals _company.ID into companies
                       from company in companies.DefaultIfEmpty()
                       select new Models.Contact
                       {
                           ID = entity.ID,
                           Name = entity.Name,
                           Email = entity.Email,
                           Mobile = entity.Mobile,
                           Tel = entity.Tel,
                           Company = company
                       };

            return linq;
        }

    }
}
