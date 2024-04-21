using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Enums;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    public class PersonsOrigin : UniqueView<Person, PvFinanceReponsitory>
    {
        internal PersonsOrigin() { }

        internal PersonsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Person> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Persons>()
                   select new Person()
                   {
                       ID = entity.ID,
                       RealName = entity.RealName,
                       Gender = (GenderType)entity.Gender,
                       IDCard = entity.IDCard,
                       Mobile = entity.Mobile,
                       CreatorID = entity.CreatorID,
                       ModifierID = entity.ModifierID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (Underly.GeneralStatus)entity.Status,
                   };
        }
    }
}
