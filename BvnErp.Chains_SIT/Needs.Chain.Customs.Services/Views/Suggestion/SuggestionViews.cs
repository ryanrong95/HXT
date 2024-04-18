using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views
{
    /// <summary>
    /// 角色的视图
    /// </summary>
    public class SuggestionViews : UniqueView<Models.Suggestions, ScCustomsReponsitory>
    {
        public SuggestionViews()
        {

        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="reponsitory">数据库实体</param>
        public SuggestionViews(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }
        protected override IQueryable<Models.Suggestions> GetIQueryable()
        {
            var result = from suggestion in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Suggestions>()
                         where suggestion.Status == (int)Enums.Status.Normal
                         select new Models.Suggestions
                         {
                             ID = suggestion.ID,
                             Name = suggestion.Name,
                             Phone = suggestion.Phone,
                             Status = (Enums.Status)suggestion.Status,
                             Summary = suggestion.Summary,
                             UpdateDate = suggestion.UpdateDate,
                             CreateDate = suggestion.CreateDate,
                         };
            return result;
        }
    }
}
