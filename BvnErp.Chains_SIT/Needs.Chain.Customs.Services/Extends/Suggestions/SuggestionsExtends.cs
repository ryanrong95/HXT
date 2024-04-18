using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 
    /// </summary>
    public static class SuggestionsExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Suggestions ToLinq(this Models.Suggestions entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Suggestions
            {
                ID = entity.ID,
                Name = entity.Name,
                Phone = entity.Phone,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary
            };
        }
    }
}