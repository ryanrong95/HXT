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
    /// 当事人视图
    /// </summary>
    abstract public class PartyBaseView : UniqueView<Models.Party, CvOssReponsitory>
    {
        protected PartyBaseView()
        {

        }
        protected PartyBaseView(CvOssReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Party> GetIQueryable()
        {
            var companiesView = new CompaniesView(this.Reponsitory);
            var contactsView = new ContactsView(this.Reponsitory);

            var linq = from entity in Reponsitory.ReadTable<Layer.Data.Sqls.CvOss.Parties>()
                       join contact in contactsView on entity.ContactID equals contact.ID
                       join company in companiesView on entity.CompanyID equals company.ID
                       select new Models.Party
                       {
                           ID = entity.ID,
                           CompanyID = entity.CompanyID,
                           ContactID = entity.ContactID,
                           Address = entity.Address,
                           Postzip = entity.Postzip,
                           District = (Needs.Underly.District)entity.District,

                           Contact = contact,
                           Company = company
                       };

            return linq;
        }

    }
}
