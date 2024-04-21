using System.Linq;
using System.Security.Cryptography;
using Layers.Data.Sqls;
using Yahv.Finance.Services.Models.Origins;
using Yahv.Linq;
using Yahv.Underly;

namespace Yahv.Finance.Services.Views.Origins
{
    /// <summary>
    /// 承兑汇票右表
    /// </summary>
    public class AcceptanceRightsOrigin : UniqueView<AcceptanceRight, PvFinanceReponsitory>
    {
        public AcceptanceRightsOrigin()
        {
        }

        public AcceptanceRightsOrigin(PvFinanceReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<AcceptanceRight> GetIQueryable()
        {
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvFinance.AcceptanceRights>()
                   select new AcceptanceRight()
                   {
                       ID = entity.ID,
                       AcceptanceLeftID = entity.AcceptanceLeftID,
                       Price = entity.Price,
                       FlowID = entity.FlowID,
                       CreatorID = entity.CreatorID,
                       CreateDate = entity.CreateDate,
                   };
        }
    }
}