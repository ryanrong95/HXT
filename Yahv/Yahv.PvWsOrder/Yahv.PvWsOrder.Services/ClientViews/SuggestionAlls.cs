using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Layers.Data.Sqls;
using Yahv.Linq;
using Yahv.PvWsOrder.Services.ClientModels;

namespace Yahv.PvWsOrder.Services.ClientViews
{
    public class SuggestionAlls : UniqueView<Suggestion, PvWsOrderReponsitory>
    {
        protected SuggestionAlls()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override IQueryable<Suggestion> GetIQueryable()
        {
            return from suggestion in this.Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Suggestions>()
                   select new Suggestion
                   {
                       ID = suggestion.ID,
                       Name = suggestion.Name,
                       Phone = suggestion.Phone,
                       Status = suggestion.Status,
                       CreateDate = suggestion.CreateDate,
                       Summary = suggestion.Summary,
                   };
        }
    }
}
