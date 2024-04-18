using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YaHv.PvData.Services.Extends;

namespace YaHv.PvData.Services.Models
{
    /// <summary>
    /// 税费变更数据同步
    /// </summary>
    public sealed class TaxChangeSyncHandler : SyncHandlerBase
    {
        public TaxChangeSyncHandler() : base()
        {

        }

        public override void DoSync()
        {
            if (this.ops == null || this.ops.Count() == 0)
                return;

            base.OnSyncing();

            using (var reponsitory = new PvWsOrderReponsitory())
            {
                foreach (var op in this.ops)
                {
                    var item = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().SingleOrDefault(oi => oi.ID == op.ID);
                    if (item == null)
                        continue;

                    OrderItemChcdEnter(op, reponsitory);
                    OrderItemTermEnter(op, reponsitory);
                }
            }
        }

        private void OrderItemChcdEnter(OrderedProduct op, PvWsOrderReponsitory reponsitory)
        {
            string cpnID = op.GenID();
            var itemChcd = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>().SingleOrDefault(oic => oic.ID == op.ID);

            if (itemChcd != null)
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>(new
                {
                    SecondHSCodeID = cpnID,
                    ModifyDate = DateTime.Now,
                }, oic => oic.ID == op.ID);

                //原来的数据插入历史日志表
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsChcd
                {
                    ID = Guid.NewGuid().ToString(),
                    OrderItemID = itemChcd.ID,
                    AutoHSCodeID = itemChcd.AutoHSCodeID,
                    AutoDate = itemChcd.AutoDate,
                    FirstAdminID = itemChcd.FirstAdminID,
                    FirstHSCodeID = itemChcd.FirstHSCodeID,
                    FirstDate = itemChcd.FirstDate,
                    SecondAdminID = itemChcd.SecondAdminID,
                    SecondHSCodeID = itemChcd.SecondHSCodeID,
                    SecondDate = itemChcd.SecondDate,
                    CustomHSCodeID = itemChcd.CustomHSCodeID,
                    CustomTaxCode = itemChcd.CustomTaxCode,
                    SysPriceID = itemChcd.SysPriceID,
                    CustomsPriceID = itemChcd.CustomsPriceID,
                    VATaxedPriceID = itemChcd.VATaxedPriceID,
                    CreateDate = itemChcd.CreateDate,
                    ModifyDate = itemChcd.ModifyDate,
                });
            }
        }

        private void OrderItemTermEnter(OrderedProduct op, PvWsOrderReponsitory reponsitory)
        {
            var itemTerm = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>().SingleOrDefault(oit => oit.ID == op.ID);
            if (itemTerm != null & itemTerm.OriginRate != op.OriginATRate)
            {
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>(new
                {
                    OriginRate = op.OriginATRate,
                }, oit => oit.ID == op.ID);

                //原来的数据插入历史日志表
                reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.Logs_OrderItemsTerm
                {
                    ID = Guid.NewGuid().ToString(),
                    OrderItemID = itemTerm.ID,
                    OriginRate = itemTerm.OriginRate,
                    FVARate = itemTerm.FVARate,
                    Ccc = itemTerm.Ccc,
                    Embargo = itemTerm.Embargo,
                    HkControl = itemTerm.HkControl,
                    Coo = itemTerm.Coo,
                    CIQ = itemTerm.CIQ,
                    CIQprice = itemTerm.CIQprice,
                    IsHighPrice = itemTerm.IsHighPrice,
                    IsDisinfected = itemTerm.IsDisinfected,
                });
            }
        }
    }
}
