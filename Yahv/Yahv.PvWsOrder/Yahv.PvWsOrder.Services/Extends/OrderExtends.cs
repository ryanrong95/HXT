using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Underly;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Extends
{
    public static class OrderExtends
    {
        /// <summary>
        /// Order To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.Orders ToLinq(this Order entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.Orders
            {
                ID = entity.ID,
                CreatorID = entity.CreatorID,
                Type = (int)entity.Type,
                ClientID = entity.ClientID,
                InvoiceID = entity.InvoiceID,
                PayeeID = entity.PayeeID,
                BeneficiaryID = entity.BeneficiaryID,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
                Summary = entity.Summary,
                SupplierID = entity.SupplierID,
                SettlementCurrency = (int)entity.SettlementCurrency,
            };
        }

        /// <summary>
        /// OrderInput To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.OrderInputs ToLinq(this OrderInput entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.OrderInputs
            {
                ID = entity.ID,
                BeneficiaryID = entity.BeneficiaryID,
                WayBillID = entity.WayBillID,
                IsPayCharge = entity.IsPayCharge,
                Conditions = entity.Conditions,
                Currency = (int)entity.Currency,
            };
        }

        /// <summary>
        /// OrderOutput To Linq
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public static Layers.Data.Sqls.PvWsOrder.OrderOutputs ToLinq(this OrderOutput entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.OrderOutputs
            {
                ID = entity.ID,
                BeneficiaryID = entity.BeneficiaryID,
                WayBillID = entity.WayBillID,
                IsReciveCharge = entity.IsReciveCharge,
                Conditions = entity.Conditions,
                Currency = (int)entity.Currency,
            };
        }

        /// <summary>
        /// 订单的状态日志更新
        /// </summary>
        public static void StatusLogUpdate(this Order entity)
        {
            using (Layers.Data.Sqls.PvCenterReponsitory reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                //查询订单所有状态的当前状态值
                var current = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.CurrentWsOrderStatusTopView>()
                    .Where(item => item.MainID == entity.ID).FirstOrDefault();
                if (current == null)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = entity.ID,
                        Type = (int)OrderStatusType.MainStatus,
                        Status = (int)entity.MainStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = entity.OperatorID,
                        IsCurrent = true,
                    });
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = entity.ID,
                        Type = (int)OrderStatusType.PaymentStatus,
                        Status = (int)entity.PaymentStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = entity.OperatorID,
                        IsCurrent = true,
                    });
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = entity.ID,
                        Type = (int)OrderStatusType.InvoiceStatus,
                        Status = (int)entity.InvoiceStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = entity.OperatorID,
                        IsCurrent = true,
                    });
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = entity.ID,
                        Type = (int)OrderStatusType.RemittanceStatus,
                        Status = (int)entity.RemittanceStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = entity.OperatorID,
                        IsCurrent = true,
                    });
                }
                else
                {
                    if (current.MainStatus != (int)entity.MainStatus)
                    {
                        Logs_PvWsOrder log = new Logs_PvWsOrder();
                        log.MainID = entity.ID;
                        log.Type = OrderStatusType.MainStatus;
                        log.Status = (int)entity.MainStatus;
                        log.CreatorID = entity.OperatorID;
                        log.Enter();
                    }
                    if (current.PaymentStatus != (int)entity.PaymentStatus)
                    {
                        Logs_PvWsOrder log = new Logs_PvWsOrder();
                        log.MainID = entity.ID;
                        log.Type = OrderStatusType.PaymentStatus;
                        log.Status = (int)entity.PaymentStatus;
                        log.CreatorID = entity.OperatorID;
                        log.Enter();
                    }
                    if (current.InvoiceStatus != (int)entity.InvoiceStatus)
                    {
                        Logs_PvWsOrder log = new Logs_PvWsOrder();
                        log.MainID = entity.ID;
                        log.Type = OrderStatusType.InvoiceStatus;
                        log.Status = (int)entity.InvoiceStatus;
                        log.CreatorID = entity.OperatorID;
                        log.Enter();
                    }
                    if (current.RemittanceStatus != (int)entity.RemittanceStatus)
                    {
                        Logs_PvWsOrder log = new Logs_PvWsOrder();
                        log.MainID = entity.ID;
                        log.Type = OrderStatusType.RemittanceStatus;
                        log.Status = (int)entity.RemittanceStatus;
                        log.CreatorID = entity.OperatorID;
                        log.Enter();
                    }
                }
            }
        }

        /// <summary>
        /// 订单操作日志
        /// </summary>
        /// <param name="entity"></param>
        public static void OperateLog(this Order entity, string operation, string summary = "", LogType type = LogType.WsOrder)
        {
            Yahv.Services.OperatingLogger log = new CenterLog(entity.OperatorID)[type];
            log.Log(new OperatingLog
            {
                MainID = entity.ID,
                Operation = operation,
                Summary = summary
            });
        }

        ///// <summary>
        ///// 人工归类判断
        ///// </summary>
        ///// <param name="entity"></param>
        //public static void CheckClassify(this Order entity)
        //{
        //    using (Layers.Data.Sqls.PvWsOrderReponsitory reponsitory = new Layers.Data.Sqls.PvWsOrderReponsitory())
        //    {
        //        if (entity.Type != OrderType.Declare || entity.Type != OrderType.TransferDeclare)
        //        {
        //            return;
        //        }
        //        //查询报关订单的所有订单项
        //        var items = new Views.OrderItemsAlls(reponsitory).Where(item => item.OrderID == entity.ID);
        //        //判断是否都进行了二次归类
        //        var AllClassified = items.Any(item => item.OrderItemsChcd != null && string.IsNullOrEmpty(item.OrderItemsChcd.SecondHSCodeID));
        //        if (AllClassified)
        //        {
        //            if (entity.Type == OrderType.TransferDeclare)
        //            {
        //                entity.ExecutionStatus = ExcuteStatus.香港待装箱;
        //                entity.StatusLogUpdate();
        //                //TODO:产生香港库房的装箱通知
        //            }
        //            if (entity.Type == OrderType.Declare)
        //            {
        //                entity.ExecutionStatus = ExcuteStatus.香港待入库;
        //                entity.StatusLogUpdate();
        //                //TODO:产生香港库房的入库通知
        //            }
        //        }
        //        else
        //        {

        //        }
        //    }
        //}
    }
}
