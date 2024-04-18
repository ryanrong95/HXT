using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.Models.Views
{
    public class ContactsView : UniqueView<Models.Contact, ScCustomsReponsitory>
    {
        public ContactsView()
        {

        }

        public ContactsView(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Models.Contact> GetIQueryable()
        {
            return from entity in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>()
                   select new Models.Contact
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Tel = entity.Tel,
                       Mobile = entity.Mobile,
                       Email = entity.Email,
                       Fax = entity.Fax,
                       QQ = entity.QQ,
                       Status = (Enums.Status)entity.Status,
                       CreateDate = entity.CreateDate,
                       Summary = entity.Summary,
                   };
        }
    }
}
