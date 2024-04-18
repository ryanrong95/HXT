using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class SCPayExchangeHandler
    {
        public Order CurrentOrder { get; set; }
        public Order OriginOrder { get; set; }

        public SCPayExchangeHandler(Order currentOrder, Order originOrder)
        {
            this.CurrentOrder = currentOrder;
            this.OriginOrder = originOrder;
        }

        public void AdjustPayExchange(string PayExchangeSplit)
        {
            if (string.IsNullOrEmpty(PayExchangeSplit))
            {
                return;
            }

            var pays = PayExchangeSplit.Trim(',').Split(',');

            //拆分金额02:
            var currentTotal02 = CurrentOrder.DeclarePrice;
            //订单金额01(原订单总金额):
            var originTotal01 = OriginOrder.DeclarePrice;
            //已付汇总金额：
            var originPaidPEAmount = OriginOrder.PaidExchangeAmount;
            //需要给02的总金额
            var needSplitAmount = originPaidPEAmount - (originTotal01 - currentTotal02);

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                if (pays.Length == 1)
                {
                    #region 1、只有一个付汇申请
                    var PEItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().FirstOrDefault(t => t.ID == pays[0]);

                    //修改原item付汇金额 01订单
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { Amount = (originPaidPEAmount - needSplitAmount) }, t => t.ID == PEItem.ID);

                    //插入payexchangeitem  02订单
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem),
                        PayExchangeApplyID = PEItem.PayExchangeApplyID,
                        OrderID = CurrentOrder.ID,
                        Amount = needSplitAmount,
                        Status = (int)Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                        ApplyStatus = PEItem.ApplyStatus
                    });
                    #endregion
                }
                else
                {
                    #region 2、有多个。循环给02，最后一个使用减法给02。

                    var PEItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>().Where(t => pays.Contains(t.ID));

                    var last = PEItems.Select(t => t.ID).ToArray().Last();
                    var total = 0M;
                    foreach (var item in PEItems)
                    {
                        //非最后一个，直接分给02
                        if (item.ID != last)
                        {
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { OrderID = CurrentOrder.ID }, t => t.ID == item.ID);
                        }
                        else
                        {
                            //最后一个使用剑法，给02，剩余继续给01
                            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems
                            {
                                ID = Needs.Overall.PKeySigner.Pick(PKeyType.PayExchangeApplyItem),
                                PayExchangeApplyID = item.PayExchangeApplyID,
                                OrderID = CurrentOrder.ID,
                                Amount = needSplitAmount - total,
                                Status = (int)Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                ApplyStatus = item.ApplyStatus
                            });

                            //修改原item付汇金额 01订单
                            var remain = item.Amount - (needSplitAmount - total);
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>(new { Amount = remain }, t => t.ID == item.ID);
                        }

                        total += item.Amount;
                    }

                    #endregion
                }

                //修改01 02订单的已付汇金额
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { PaidExchangeAmount = (OriginOrder.PaidExchangeAmount - needSplitAmount) }, t => t.ID == OriginOrder.ID);
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { PaidExchangeAmount = needSplitAmount }, t => t.ID == CurrentOrder.ID);
            }
        }
    }
}
