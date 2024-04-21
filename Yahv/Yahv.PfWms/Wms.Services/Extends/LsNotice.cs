using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wms.Services.Extends
{
    public static class LsNotice
    {
        public static Layers.Data.Sqls.PvWms.LsNotice ToLinq(this Models.LsNotice entity)
        {
            return new Layers.Data.Sqls.PvWms.LsNotice
            {
                ID = entity.ID,
                SpecID = entity.SpecID,
                Quantity = entity.Quantity,
                StartDate = entity.StartDate,
                EndDate = entity.EndDate,
                CreateDate = entity.CreateDate,
                Summary = entity.Summary,
                OrderID = entity.OrderID,
                ClientID = entity.ClientID,
                PayeeID = entity.PayeeID,
                Status = (int)entity.Status,
            };
        }
    }
}
