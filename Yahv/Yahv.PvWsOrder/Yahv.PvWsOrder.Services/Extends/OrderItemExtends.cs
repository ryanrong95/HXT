using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Extends
{
    public static class OrderItemExtends
    {
        /// <summary>
        /// OrderItem To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.OrderItems ToLinq(this OrderItem entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.OrderItems
            {
                ID = entity.ID,
                OrderID = entity.OrderID,
                TinyOrderID = entity.TinyOrderID,
                InputID = entity.InputID,
                OutputID = entity.OutputID,
                ProductID = entity.Product?.ID,
                CustomName = entity.CustomName,
                Origin = entity.Origin.GetOrigin().Code,
                DateCode = entity.DateCode,
                Quantity = entity.Quantity,
                Currency = (int)entity.Currency,
                UnitPrice = entity.UnitPrice,
                Unit = (int)entity.Unit,
                TotalPrice = entity.TotalPrice,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
                GrossWeight = entity.GrossWeight,
                Volume = entity.Volume,
                Conditions = entity.Conditions,
                Status = (int)entity.Status,
                IsAuto = entity.IsAuto,
                WayBillID = entity.WayBillID,
                Type = (int)entity.Type,
                StorageID = entity.StorageID,
            };
        }

        /// <summary>
        /// Logs_OrderItems To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.Logs_OrderItems ToLinqLog(this OrderItem entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.Logs_OrderItems
            {
                ID = Guid.NewGuid().ToString(),
                OrderItemID = entity.ID,
                OrderID = entity.OrderID,
                InputID = entity.InputID,
                OutputID = entity.OutputID,
                ProductID = entity.Product?.ID,
                CustomName = entity.CustomName,
                Origin = entity.Origin.GetOrigin().Code,
                DateCode = entity.DateCode,
                Quantity = entity.Quantity,
                Currency = (int)entity.Currency,
                UnitPrice = entity.UnitPrice,
                Unit = (int)entity.Unit,
                TotalPrice = entity.TotalPrice,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
                GrossWeight = entity.GrossWeight,
                Volume = entity.Volume,
                Conditions = entity.Conditions,
                Status = (int)entity.Status,
                IsAuto = entity.IsAuto,
                WayBillID = entity.WayBillID,
            };
        }

        /// <summary>
        /// OrderItemsChcd To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.OrderItemsChcd ToLinq(this OrderItemsChcd entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.OrderItemsChcd
            {
                ID = entity.ID,
                AutoHSCodeID = entity.AutoHSCodeID,
                AutoDate = entity.AutoDate,
                FirstAdminID = entity.FirstAdminID,
                FirstHSCodeID = entity.FirstHSCodeID,
                FirstDate = entity.FirstDate,
                SecondAdminID = entity.SecondAdminID,
                SecondHSCodeID = entity.SecondHSCodeID,
                SecondDate = entity.SecondDate,
                CustomHSCodeID = entity.CustomHSCodeID,
                CustomTaxCode = entity.CustomTaxCode,
                SysPriceID = entity.SysPriceID,
                CustomsPriceID = entity.CustomsPriceID,
                VATaxedPriceID = entity.VATaxedPriceID,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
            };
        }

        /// <summary>
        /// Logs_OrderItemsChcd To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsChcd ToLinqLog(this OrderItemsChcd entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsChcd
            {
                ID = Guid.NewGuid().ToString(),
                OrderItemID = entity.ID,
                AutoHSCodeID = entity.AutoHSCodeID,
                AutoDate = entity.AutoDate,
                FirstAdminID = entity.FirstAdminID,
                FirstHSCodeID = entity.FirstHSCodeID,
                FirstDate = entity.FirstDate,
                SecondAdminID = entity.SecondAdminID,
                SecondHSCodeID = entity.SecondHSCodeID,
                SecondDate = entity.SecondDate,
                CustomHSCodeID = entity.CustomHSCodeID,
                CustomTaxCode = entity.CustomTaxCode,
                SysPriceID = entity.SysPriceID,
                CustomsPriceID = entity.CustomsPriceID,
                VATaxedPriceID = entity.VATaxedPriceID,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
            };
        }

        /// <summary>
        /// OrderItemsTerm To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.OrderItemsTerm ToLinq(this OrderItemsTerm entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.OrderItemsTerm
            {
                ID = entity.ID,
                OriginRate = entity.OriginRate,
                FVARate = entity.FVARate,
                Ccc = entity.Ccc,
                Embargo = entity.Embargo,
                HkControl = entity.HkControl,
                Coo = entity.Coo,
                CIQ = entity.CIQ,
                CIQprice = entity.CIQprice,
                IsHighPrice = entity.IsHighPrice,
                IsDisinfected = entity.IsDisinfected,
            };
        }

        /// <summary>
        /// Logs_OrderItemsTerm To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsTerm ToLinqLog(this OrderItemsTerm entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsTerm
            {
                ID = Guid.NewGuid().ToString(),
                OrderItemID = entity.ID,
                OriginRate = entity.OriginRate,
                FVARate = entity.FVARate,
                Ccc = entity.Ccc,
                Embargo = entity.Embargo,
                HkControl = entity.HkControl,
                Coo = entity.Coo,
                CIQ = entity.CIQ,
                CIQprice = entity.CIQprice,
                IsHighPrice = entity.IsHighPrice,
                IsDisinfected = entity.IsDisinfected,
            };
        }

        /// <summary>
        /// 一键归类
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static void QuickClassify(this IEnumerable<OrderItem> entity, string Creator)
        {
            string[] ids = entity.Select(item => item.ID).ToArray();
            using (Layers.Data.Sqls.PvWsOrderReponsitory Reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
            {
                var chcdView = new Views.Origins.OrderItemsChcdOrigin().Where(en => ids.Contains(en.ID));
                foreach (var chcd in chcdView)
                {
                    //添加日志Log_OrderItemsChcd
                    Reponsitory.Insert(chcd.ToLinqLog());
                    //一键归类，将自动或一次归类的结果给二次归类
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(new
                    {
                        SecondAdminID = Creator,
                        SecondHSCodeID = chcd.FirstHSCodeID == null ? chcd.AutoHSCodeID : chcd.FirstHSCodeID,
                        SecondDate = DateTime.Now,
                        ModifyDate = DateTime.Now,
                    }, en => en.ID == chcd.ID);
                }
            }
        }
    }
}
