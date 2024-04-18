using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services
{
    public static class LogsExtends
    {
        public static Layer.Data.Sqls.ScCustoms.Logs ToLinq(this Models.Logs entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Logs
            {
                ID = entity.ID,
                Name = entity.Name,
                MainID = entity.MainID,
                AdminID = entity.AdminID,
                Summary = entity.Summary,
                Json = entity.Json,
                CreateDate = entity.CreateDate
            };
        }
    }
}
