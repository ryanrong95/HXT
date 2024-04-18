using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public static class PayExchangelogExtends
    {
        public static Layer.Data.Sqls.ScCustoms.PayExchangeApplyLogs ToLinq(this Models.PayExchangeLog entity)
        {
            return new Layer.Data.Sqls.ScCustoms.PayExchangeApplyLogs
            {
                ID = entity.ID,
                PayExchangeApplyID=entity.PayExchangeApplyID,
                AdminID = entity.Admin?.ID,
                UserID = entity.User?.ID,
                PayExchangeApplyStatus=(int)entity.PayExchangeApplyStatus,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary
            };
        }
    }
}
