using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class ContactsOrigin : Yahv.Linq.UniqueView<Yahv.CrmPlus.Service.Models.Origins.Contact, Layers.Data.Sqls.PvdCrmReponsitory>
    {
        public ContactsOrigin()
        {


        }
        public ContactsOrigin(PvdCrmReponsitory reponsitory) : base(reponsitory)
        {


        }

        protected override IQueryable<Contact> GetIQueryable()
        {
            var enterprisesView = new EnterprisesOrigin(this.Reponsitory);
            var adminsView = new AdminsAllRoll(this.Reponsitory);
            // var registerView = new EnterpriseRegistersOrigin(this.Reponsitory);

            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.Contacts>()
                   join enterprise in enterprisesView on entity.EnterpriseID equals enterprise.ID
                   join admin in adminsView on entity.OwnerID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new Yahv.CrmPlus.Service.Models.Origins.Contact
                   {

                       ID = entity.ID,
                       EnterpriseID = entity.EnterpriseID,
                       Name = entity.Name,
                       Department = entity.Department,
                       Positon = entity.Positon,
                       Mobile = entity.Mobile,
                       Tel = entity.Tel,
                       Email = entity.Email,
                       Gender = entity.Gender,
                       QQ = entity.qq,
                       Skype = entity.Skype,
                       Character = entity.Character,
                       Taboo = entity.Taboo,
                       RelationType = (RelationType)entity.RelationType,
                       OwnerID = entity.OwnerID,
                       Summary = entity.Summary,
                       Wx = entity.Wx,
                       CreateDate = entity.CreateDate,
                       Status = (DataStatus)entity.Status,
                       Admin = admin,

                   };
        }

    }
}
