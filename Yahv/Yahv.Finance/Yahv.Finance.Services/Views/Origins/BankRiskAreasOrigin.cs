using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;

namespace Yahv.Finance.Services.Views.Origins
{
    public class BankRiskAreasOrigin : UniqueView<BankRiskArea, PvFinanceReponsitory>
    {
        internal BankRiskAreasOrigin() { }

        internal BankRiskAreasOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {

        }

        protected override IQueryable<BankRiskArea> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.BankRiskAreas>()
                   select new BankRiskArea()
                   {
                       ID = entity.ID,
                       BankID = entity.BankID,
                       District = entity.District,
                       CreatorID = entity.CreatorID,
                       ModifierID = entity.ModifierID,
                       CreateDate = entity.CreateDate,
                       ModifyDate = entity.ModifyDate,
                       Status = (Underly.GeneralStatus)entity.Status,
                   };
        }
    }
}
