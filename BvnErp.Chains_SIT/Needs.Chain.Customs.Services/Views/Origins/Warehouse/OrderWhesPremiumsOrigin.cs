using Layer.Data.Sqls;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services.Models;

namespace Needs.Ccs.Services.Views.Origins
{
    internal class OrderWhesPremiumsOrigin : UniqueView<Models.OrderWhesPremium, ScCustomsReponsitory>
    {
        internal OrderWhesPremiumsOrigin()
        {
        }

        internal OrderWhesPremiumsOrigin(ScCustomsReponsitory reponsitory) : base(reponsitory)
        {
        }

        protected override IQueryable<OrderWhesPremium> GetIQueryable()
        {
            return from fee in this.Reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderWhesPremiums>()
                   select new Models.OrderWhesPremium
                   {
                       ID = fee.ID,
                       OrderID = fee.OrderID,
                       CreaterID = fee.CreaterID,
                       ApproverID = fee.ApproverID,
                       WarehouseType = (Enums.WarehouseType)fee.WarehouseType,
                       WarehousePremiumType = (Enums.WarehousePremiumType)fee.WhesFeeType,
                       Count = fee.Count,
                       UnitPrice = fee.UnitPrice,
                       UnitName = fee.UnitName,
                       Currency = fee.Currency,
                       ExchangeRate = fee.ExchangeRate,
                       ApprovalPrice = fee.ApprovalPrice,
                       WhsePaymentType = (Enums.WhsePaymentType)fee.PaymentType,
                       WarehousePremiumsStatus = (Enums.WarehousePremiumsStatus)fee.PremiumsStatus,
                       Status = (Enums.Status)fee.Status,
                       CreateDate = fee.CreateDate,
                       UpdateDate = fee.UpdateDate,
                       Summary = fee.Summary,
                   };
        }
    }
}
