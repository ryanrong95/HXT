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
    public class EnterprisesOrigin : UniqueView<Enterprise, PvFinanceReponsitory>
    {
        internal EnterprisesOrigin() { }

        internal EnterprisesOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<Enterprise> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.Enterprises>()
                   select new Enterprise()
                   {
                       ID = entity.ID,
                       Name = entity.Name,
                       Type = (EnterpriseAccountType)entity.Type,
                       District = entity.District,
                       CreatorID = entity.CreatorID,
                       ModifierID = entity.ModifierID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (Underly.GeneralStatus)entity.Status,
                       Summary = entity.Summary,
                   };
        }
    }
}
