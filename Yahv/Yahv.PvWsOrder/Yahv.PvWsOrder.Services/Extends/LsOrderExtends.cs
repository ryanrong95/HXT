using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.PvWsOrder.Services.Models;
using Yahv.Services.Models;
using Yahv.Underly;

namespace Yahv.PvWsOrder.Services.Extends
{
    public static class LsOrderExtends
    {
        public static Layers.Data.Sqls.PvWsOrder.LsOrderTopView ToLinq(this LsOrder entity)
        {
            return new Layers.Data.Sqls.PvWsOrder.LsOrderTopView()
            {
                ID = entity.ID,
                Type = (int)entity.Type,
                Source = (int)entity.Source,
                ClientID = entity.ClientID,
                PayeeID = entity.PayeeID,
                BeneficiaryID = entity.BeneficiaryID,
                Currency = (int)entity.Currency,
                InvoiceID = entity.InvoiceID,
                Status = (int)entity.Status,
                Creator = entity.Creator,
                CreateDate = entity.CreateDate,
                ModifyDate = entity.ModifyDate,
                Summary = entity.Summary,
            };
        }

        /// <summary>
        /// 租赁订单的状态日志更新
        /// </summary>
        public static void StatusLogUpdate(this LsOrder entity)
        {
            using (Layers.Data.Sqls.PvCenterReponsitory reponsitory = new Layers.Data.Sqls.PvCenterReponsitory())
            {
                //查询订单所有状态的当前状态值
                var current = reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.Logs_PvLsOrderCurrentTopView>()
                    .Where(item => item.MainID == entity.ID).FirstOrDefault();
                if (current == null)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = entity.ID,
                        Type = (int)LsOrderStatusType.MainStatus,
                        Status = (int)entity.Status,
                        CreateDate = DateTime.Now,
                        CreatorID = entity.OperatorID,
                        IsCurrent = true,
                    });
                    reponsitory.Insert(new Layers.Data.Sqls.PvCenter.Logs_PvLsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = entity.ID,
                        Type = (int)LsOrderStatusType.InvoiceStatus,
                        Status = (int)entity.InvoiceStatus,
                        CreateDate = DateTime.Now,
                        CreatorID = entity.OperatorID,
                        IsCurrent = true,
                    });
                }
                else
                {
                    if (current.MainStatus != (int)entity.Status)
                    {
                        Logs_PvLsOrder log = new Logs_PvLsOrder();
                        log.MainID = entity.ID;
                        log.Type = LsOrderStatusType.MainStatus;
                        log.Status = (int)entity.Status;
                        log.CreatorID = entity.OperatorID;
                        log.Enter();
                    }
                    if (current.InvoiceStatus != (int)entity.InvoiceStatus)
                    {
                        Logs_PvLsOrder log = new Logs_PvLsOrder();
                        log.MainID = entity.ID;
                        log.Type = LsOrderStatusType.InvoiceStatus;
                        log.Status = (int)entity.InvoiceStatus;
                        log.CreatorID = entity.OperatorID;
                        log.Enter();
                    }
                }
            }
        }
    }
}
