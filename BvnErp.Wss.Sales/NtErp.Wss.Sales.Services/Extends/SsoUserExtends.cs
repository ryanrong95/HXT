using NtErp.Wss.Sales.Services.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NtErp.Wss.Sales.Services.Extends
{
    static public class SsoUserExtends
    {
        static public Layer.Data.Sqls.BvOrders.UserOutputs ToLinq(this UserOutput entity)
        {
            return new Layer.Data.Sqls.BvOrders.UserOutputs
            {
                ID = entity.ID,
                Type = (int)entity.Type,
                OrderID = entity.OrderID,
                UserID = entity.UserID,
                Amount = entity.Amount,
                Currency = (int)entity.Currency,
                CreateDate = entity.CreateDate,
                UserInputID = entity.UserInputID
            };
        }

    }
}
