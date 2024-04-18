using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 库房费用扩展方法
    /// </summary>
    public static class OrderWhesPremiumExtends
    {
        public static Layer.Data.Sqls.ScCustoms.OrderWhesPremiums ToLinq(this Models.OrderWhesPremium entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderWhesPremiums
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                CreaterID = entity.Creater.ID,
                ApproverID = entity.Approver?.ID,
                WarehouseType = (int)entity.WarehouseType,
                WhesFeeType = (int)entity.WarehousePremiumType,
                PaymentType = (int)entity.WhsePaymentType,
                Count = entity.Count,
                UnitPrice = entity.UnitPrice,
                UnitName = entity.UnitName,
                Currency = entity.Currency,
                ExchangeRate = entity.ExchangeRate,
                ApprovalPrice = entity.ApprovalPrice,
                PremiumsStatus = (int)entity.WarehousePremiumsStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = entity.UpdateDate,
                Summary = entity.Summary,
            };
        }
    }

    public static class OrderWhesPremiumLogExtends
    {
        /// <summary>
        /// 写入订单日志
        /// </summary>
        /// <param name="order"></param>
        /// <param name="summary"></param>
        public static void Log(this Models.OrderWhesPremium entity, Admin Operator, string summary)
        {
            OrderWhesPremiumLog log = new OrderWhesPremiumLog();
            log.OrderID = entity.OrderID;
            log.OrderWhesPremiumID = entity.ID;
            log.Type = entity.WarehousePremiumType;
            log.Admin = Operator;
            log.Summary = summary;
            log.Enter();
        }
    }
}
