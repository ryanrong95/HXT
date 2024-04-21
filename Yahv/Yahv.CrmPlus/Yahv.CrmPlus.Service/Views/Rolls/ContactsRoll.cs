using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;

namespace Yahv.CrmPlus.Service.Views.Rolls
{
    public class ContactsRoll : Origins.ContactsOrigin
    {
        string enterpriseid;
        RelationType relationType;
        public ContactsRoll()
        {


        }

        public ContactsRoll(string enterpriseid, RelationType type)
        {
            this.enterpriseid = enterpriseid;
            this.relationType = type;

        }
        protected override IQueryable<Models.Origins.Contact> GetIQueryable()
        {
            return from item in base.GetIQueryable()
                   where item.EnterpriseID == this.enterpriseid && item.RelationType == this.relationType
                   select item;
        }
    }
}
