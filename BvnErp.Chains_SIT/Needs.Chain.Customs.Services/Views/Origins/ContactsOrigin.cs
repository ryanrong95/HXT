using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class ContactsOrigin : UniqueView<Models.Contact, ScCustomsReponsitory>
    {
        internal ContactsOrigin()
        {
        }

        internal ContactsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<Models.Contact> GetIQueryable()
        {
            return from contact in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Contacts>()
                   select new Models.Contact
                   {
                       ID = contact.ID,
                       Name = contact.Name,
                       Email = contact.Email,
                       Mobile = contact.Mobile,
                       Tel = contact.Tel,
                       Fax = contact.Fax,
                       QQ = contact.QQ,
                       Status = (Enums.Status)contact.Status,
                       Summary = contact.Summary,
                       CreateDate = contact.CreateDate,
                   };
        }
    }
}
