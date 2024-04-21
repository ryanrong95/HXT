using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.CrmPlus.Service.Models.Origins;
using Yahv.CrmPlus.Service.Views.Rolls;
using Yahv.Underly;
using YaHv.CrmPlus.Services.Views.Rolls;

namespace Yahv.CrmPlus.Service.Views.Origins
{
    public class AgentQuoteOrigin : Yahv.Linq.UniqueView<AgentQuote, PvdCrmReponsitory>
    {

        internal AgentQuoteOrigin()
        {
        }

        protected override IQueryable<AgentQuote> GetIQueryable()
        {
            var AdminTopView = new AdminsAllRoll(this.Reponsitory);
            return from entity in Reponsitory.ReadTable<Layers.Data.Sqls.PvdCrm.AgentQuotes>()
                 
                   join admin in AdminTopView on entity.CreatorID equals admin.ID into g
                   from admin in g.DefaultIfEmpty()
                   select new AgentQuote
                   {
                       ID = entity.ID,
                       SpnID = entity.SpnID,
                       QuoteType = (QuoteType)entity.QuoteType,
                       ClientID = entity.ClientID,
                       MinQuantity = entity.MinQuantity,
                       MaxQuantity = entity.MaxQuantity,
                       Currency = (Currency)entity.Currency,
                       UnitCostPrice = entity.UnitCostPrice,
                       ResalePrice = entity.ResalePrice,
                       ApprovedPrice = entity.ApprovedPrice,
                       ProfitRate = entity.ProfitRate,
                       StartDate = entity.StartDate,
                       EndDate = entity.EndDate,
                       Status = (DataStatus)entity.Status,
                       CreatorID = entity.CreatorID
                   };
        }

    }
}
