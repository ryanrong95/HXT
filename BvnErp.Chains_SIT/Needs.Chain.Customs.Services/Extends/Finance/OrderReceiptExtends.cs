using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 订单收款扩展方法
    /// </summary>
    public static class OrderReceiptExtends
    {
        /// <summary>
        /// 订单应收
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.OrderReceipts ToLinq(this Models.OrderReceivable entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderReceipts
            {
                ID = entity.ID,
                ClientID = entity.ClientID,
                OrderID = entity.OrderID,
                FeeSourceID = entity.FeeSourceID,
                FeeType = (int)entity.FeeType,
                Type = (int)entity.Type,
                Currency = entity.Currency,
                Rate = entity.Rate,
                Amount = entity.Amount,
                AdminID = entity.Admin.ID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                ReImport = entity.ReImport
            };
        }

        /// <summary>
        /// 订单实收
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.OrderReceipts ToLinq(this Models.OrderReceived entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderReceipts
            {
                ID = entity.ID,
                FinanceReceiptID = entity.ReceiptNoticeID,
                ClientID = entity.ClientID,
                OrderID = entity.OrderID,
                FeeSourceID = entity.FeeSourceID,
                FeeType = (int)entity.FeeType,
                Type = (int)entity.Type,
                Currency = entity.Currency,
                Rate = entity.Rate,
                //实收金额在订单收款表中存负值
                Amount = -entity.Amount,
                AdminID = entity.Admin.OriginID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                ReImport = entity.ReImport
            };
        }

        /// <summary>
        /// 退款实收
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layer.Data.Sqls.ScCustoms.OrderReceipts ToLinq(this Models.UnmarkOrderReceipt entity)
        {
            return new Layer.Data.Sqls.ScCustoms.OrderReceipts
            {
                ID = entity.ID,
                FinanceReceiptID = entity.ReceiptNoticeID,
                ClientID = entity.ClientID,
                OrderID = entity.OrderID,
                FeeSourceID = entity.FeeSourceID,
                FeeType = (int)entity.FeeType,
                Type = (int)entity.Type,
                Currency = entity.Currency,
                Rate = entity.Rate,
                //实收金额在订单收款表中存负值
                Amount = -entity.Amount,
                AdminID = entity.Admin.ID,
                Status = (int)entity.Status,
                CreateDate = entity.CreateDate,
                UpdateDate = DateTime.Now,
                Summary = entity.Summary,
                ReImport = entity.ReImport
            };
        }
    }
}
