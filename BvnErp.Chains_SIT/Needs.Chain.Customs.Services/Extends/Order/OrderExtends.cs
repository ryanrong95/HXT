using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单扩展方法
    /// </summary>
    public static class OrderExtends
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.Orders ToLinq(this Interfaces.IOrder entity)
        {
            return new Layer.Data.Sqls.ScCustoms.Orders
            {
                ID = entity.ID,
                Type = (int)entity.Type,
                AdminID = entity.AdminID,
                UserID = entity.UserID,
                ClientID = entity.Client.ID,
                ClientAgreementID = entity.ClientAgreement.ID,
                Currency = entity.Currency,
                CustomsExchangeRate = entity.CustomsExchangeRate,
                RealExchangeRate = entity.RealExchangeRate,
                IsFullVehicle = entity.IsFullVehicle,
                IsLoan = entity.IsLoan,
                PackNo = entity.PackNo,
                WarpType = entity.WarpType,
                DeclarePrice = entity.DeclarePrice,
                InvoiceStatus = (int)entity.InvoiceStatus,
                PaidExchangeAmount = entity.PaidExchangeAmount,
                IsHangUp = entity.IsHangUp,
                OrderStatus = (int)entity.OrderStatus,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                MainOrderId = entity.MainOrderID,
                OrderBillType = (int)entity.OrderBillType,
                DeclareFlag = (int)entity.DeclareFlag
            };
        }
    }
}
