using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Views
{
    /// <summary>
    /// 内部公司视图
    /// </summary>
    public class CompanysAll : Yahv.Services.Views.CompaniesTopView<PvWsOrderReponsitory>
    {
        public CompanysAll()
        {

        }

        public CompanysAll(PvWsOrderReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Yahv.Services.Models.Company> GetIQueryable()
        {
            var contactTopView = new Yahv.Services.Views.ContactsTopView<PvWsOrderReponsitory>(this.Reponsitory);

            var linq = from entity in base.GetIQueryable().Where(item => item.Status == ApprovalStatus.Normal)
                       join contact in contactTopView on entity.ID equals contact.EnterpriseID into contacts
                       from contact in contacts.DefaultIfEmpty()
                       select new Yahv.Services.Models.Company
                       {
                           ID = entity.ID,
                           Name = entity.Name,
                           Type = entity.Type,
                           Status = entity.Status,
                           Range = entity.Range,
                           Place = entity.Place,
                           RegAddress = entity.RegAddress,
                           Uscc = entity.Uscc,
                           Corporation = entity.Corporation,

                           Contacts = contacts,
                       };
            return linq;
        }
    }
}
