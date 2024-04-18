using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PruductFeeHandle
    {
        /// <summary>
        /// 小订单号
        /// </summary>
        private string TinyOrderID { get; set; }

        public PruductFeeHandle(string tinyOrderID)
        {
            this.TinyOrderID = tinyOrderID;
        }

        public void Execute()
        {
            if (!this.TinyOrderID.StartsWith("XL002"))
            {
                return;
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //查询订单的外币金额
                var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == this.TinyOrderID)
                    .Select(item => new
                    {
                        OrderID = item.ID,
                        DeclarePrice = item.DeclarePrice,
                        Currency = item.Currency,
                        ClientID = item.ClientID,
                        IsDollared = item.IsDollared,
                    }).FirstOrDefault();


                if (order.IsDollared != null && (bool)order.IsDollared)
                {
                    return;
                }


                //查询申请付汇审核通过后的外币金额和人民币金额
                var payExchangeApplies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
                var payExchangeApplyItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();

                int[] rightApplyStatuses = new int[]
                {
                    (int)Enums.PayExchangeApplyStatus.Audited,
                    (int)Enums.PayExchangeApplyStatus.Approvaled,
                    (int)Enums.PayExchangeApplyStatus.Completed,
                };

                var rightApplyItems = (from payExchangeApply in payExchangeApplies
                                       join payExchangeApplyItem in payExchangeApplyItems on payExchangeApply.ID equals payExchangeApplyItem.PayExchangeApplyID
                                       where payExchangeApply.Status == (int)Enums.Status.Normal
                                          && payExchangeApplyItem.Status == (int)Enums.Status.Normal
                                          && rightApplyStatuses.Contains(payExchangeApply.PayExchangeApplyStatus)
                                          && payExchangeApplyItem.OrderID == this.TinyOrderID
                                       select new
                                       {
                                           PayExchangeApplyItemID = payExchangeApplyItem.ID,
                                           Amount = payExchangeApplyItem.Amount,
                                           ExchangeRate = payExchangeApply.ExchangeRate,
                                       }).ToArray();

                decimal allPayExchangeApplyAmount = 0;
                decimal allPayExchangeApplyAmountRMB = 0;
                if (rightApplyItems != null && rightApplyItems.Length > 0)
                {
                    allPayExchangeApplyAmount = rightApplyItems.Sum(t => t.Amount);
                    allPayExchangeApplyAmountRMB = rightApplyItems.Sum(t => t.Amount * t.ExchangeRate);
                }

                //查询该订单实收的人民币金额
                var productOrderReceipts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                                                .Where(t => t.OrderID == this.TinyOrderID
                                                         && t.Type == (int)Enums.OrderReceiptType.Received
                                                         && t.Status == (int)Enums.Status.Normal
                                                         && t.FeeType == (int)Enums.OrderFeeType.Product)
                                               .Select(item => new
                                               {
                                                   FinanceReceiptID = item.FinanceReceiptID,
                                                   OrderReceiptID = item.ID,
                                                   Amount = 0 - item.Amount,
                                               }).ToList();

                decimal productReceived = 0; //正的
                string allFinanceReceiptIDs = "";
                if (productOrderReceipts != null && productOrderReceipts.Count > 0)
                {
                    productReceived = productOrderReceipts.Sum(t => t.Amount);
                    string[] allFinanceReceiptID_Array = productOrderReceipts.Select(t => t.FinanceReceiptID).ToArray();
                    allFinanceReceiptIDs = string.Join(",", allFinanceReceiptID_Array);
                }

                //比较是否执行
                if (allPayExchangeApplyAmount.ToRound(2) >= order.DeclarePrice.ToRound(2) && productReceived.ToRound(2) >= allPayExchangeApplyAmountRMB.ToRound(2))
                {
                    //插入 DollarEquity 表
                    DollarEquity dollarEquity = new DollarEquity()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        ClientID = order.ClientID,
                        OrderID = this.TinyOrderID,
                        FinanceReceiptID = allFinanceReceiptIDs,
                        Amount = order.DeclarePrice,
                        AvailableAmount = order.DeclarePrice,
                        Currency = order.Currency,
                        Status = Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };
                    dollarEquity.Enter();

                    IcgooBalance icgooBalanceWaiBi = new IcgooBalance();
                    icgooBalanceWaiBi.ClientID = order.ClientID;
                    icgooBalanceWaiBi.Currency = order.Currency;
                    icgooBalanceWaiBi.Balance = order.DeclarePrice;
                    icgooBalanceWaiBi.TriggerSource = "货款更新余额外币";
                    icgooBalanceWaiBi.UpdateBalance();

                    IcgooBalance icgooBalanceRMB = new IcgooBalance();
                    icgooBalanceRMB.ClientID = order.ClientID;
                    icgooBalanceRMB.Currency = "RMB";
                    icgooBalanceRMB.Balance = 0 - productReceived;
                    icgooBalanceRMB.TriggerSource = "货款更新余额人民币";
                    icgooBalanceRMB.UpdateBalance();

                    //置 IsDollared 标志
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new
                    {
                        IsDollared = true,
                    }, item => item.ID == this.TinyOrderID);
                }


            }
        }






    }

    /// <summary>
    /// 设置 IcgooOrderMap 字段
    /// </summary>
    public class IcgooOrderMapSet
    {
        private string[] OrderIDArray { get; set; }

        public IcgooOrderMapSet(string[] orderIDArray)
        {
            this.OrderIDArray = orderIDArray;
        }

        public void Execute()
        {
            if (this.OrderIDArray == null || !this.OrderIDArray.Any())
            {
                return;
            }

            this.OrderIDArray = this.OrderIDArray.Distinct().ToArray();
            this.OrderIDArray = this.OrderIDArray.Where(t => t.StartsWith("XL002")).ToArray();

            if (this.OrderIDArray == null || !this.OrderIDArray.Any())
            {
                return;
            }

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //把与该订单相关的数据查出来
                var orderReceipts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                        .Where(t => this.OrderIDArray.Contains(t.OrderID) && t.Status == (int)Enums.Status.Normal)
                        .Select(t => new
                        {
                            OrderID = t.OrderID,
                            Type = (Enums.OrderReceiptType)t.Type,
                            FeeType = (Enums.OrderFeeType)t.FeeType,
                            Amount = t.Amount,
                        }).ToList();

                foreach (var orderID in this.OrderIDArray)
                {
                    bool receiveExceptProductOK = true;

                    //几个类型，先找有效的应收，再找有效的实收
                    int[] orderFeeTypes = new int[] 
                    {
                        (int)Enums.OrderFeeType.Tariff,
                        (int)Enums.OrderFeeType.AddedValueTax,
                        (int)Enums.OrderFeeType.AgencyFee,
                        (int)Enums.OrderFeeType.Incidental,
                    };

                    //foreach (var item in Enum.GetValues(typeof(Enums.OrderFeeType)))
                    foreach (var item in orderFeeTypes)
                    {
                        //应收
                        var receivableOrderReceiptsSum = orderReceipts
                            .Where(t => t.OrderID == orderID 
                                     && t.Type == Enums.OrderReceiptType.Receivable
                                     && (int)t.FeeType == item)
                            .Sum(t => t.Amount);

                        //实收
                        var receivedOrderReceiptsSum = orderReceipts
                            .Where(t => t.OrderID == orderID 
                                     && t.Type == Enums.OrderReceiptType.Received
                                     && (int)t.FeeType == item)
                            .Sum(t => 0 - t.Amount);

                        if (receivableOrderReceiptsSum <= 50)
                        {
                            continue;
                        }

                        //应收大于实收，就是没有收完
                        if (receivableOrderReceiptsSum > receivedOrderReceiptsSum)
                        {
                            receiveExceptProductOK = false;
                            break;
                        }
                    }






                    if (receiveExceptProductOK)
                    {
                        //查询订单的外币金额
                        var order = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == orderID)
                            .Select(item => new
                            {
                                OrderID = item.ID,
                                DeclarePrice = item.DeclarePrice,
                                Currency = item.Currency,
                                ClientID = item.ClientID,
                                IsDollared = item.IsDollared,
                            }).FirstOrDefault();


                        //if (order.IsDollared != null && (bool)order.IsDollared)
                        //{
                        //    continue;
                        //}


                        //查询申请付汇审核通过后的外币金额和人民币金额
                        var payExchangeApplies = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>();
                        var payExchangeApplyItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>();

                        int[] rightApplyStatuses = new int[]
                        {
                            (int)Enums.PayExchangeApplyStatus.Audited,
                            (int)Enums.PayExchangeApplyStatus.Approvaled,
                            (int)Enums.PayExchangeApplyStatus.Completed,
                        };

                        var rightApplyItems = (from payExchangeApply in payExchangeApplies
                                               join payExchangeApplyItem in payExchangeApplyItems on payExchangeApply.ID equals payExchangeApplyItem.PayExchangeApplyID
                                               where payExchangeApply.Status == (int)Enums.Status.Normal
                                                  && payExchangeApplyItem.Status == (int)Enums.Status.Normal
                                                  && rightApplyStatuses.Contains(payExchangeApply.PayExchangeApplyStatus)
                                                  && payExchangeApplyItem.OrderID == orderID
                                               select new
                                               {
                                                   PayExchangeApplyItemID = payExchangeApplyItem.ID,
                                                   Amount = payExchangeApplyItem.Amount,
                                                   ExchangeRate = payExchangeApply.ExchangeRate,
                                               }).ToArray();

                        decimal allPayExchangeApplyAmount = 0;
                        decimal allPayExchangeApplyAmountRMB = 0;
                        if (rightApplyItems != null && rightApplyItems.Length > 0)
                        {
                            allPayExchangeApplyAmount = rightApplyItems.Sum(t => t.Amount);
                            allPayExchangeApplyAmountRMB = rightApplyItems.Sum(t => t.Amount * t.ExchangeRate);
                        }

                        //查询该订单实收的人民币金额
                        var productOrderReceipts = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderReceipts>()
                                                        .Where(t => t.OrderID == orderID
                                                                 && t.Type == (int)Enums.OrderReceiptType.Received
                                                                 && t.Status == (int)Enums.Status.Normal
                                                                 && t.FeeType == (int)Enums.OrderFeeType.Product)
                                                       .Select(item => new
                                                       {
                                                           FinanceReceiptID = item.FinanceReceiptID,
                                                           OrderReceiptID = item.ID,
                                                           Amount = 0 - item.Amount,
                                                       }).ToList();

                        decimal productReceived = 0; //正的
                        string allFinanceReceiptIDs = "";
                        if (productOrderReceipts != null && productOrderReceipts.Count > 0)
                        {
                            productReceived = productOrderReceipts.Sum(t => t.Amount);
                            string[] allFinanceReceiptID_Array = productOrderReceipts.Select(t => t.FinanceReceiptID).ToArray();
                            allFinanceReceiptIDs = string.Join(",", allFinanceReceiptID_Array);
                        }

                        if (allPayExchangeApplyAmount.ToRound(2) >= order.DeclarePrice.ToRound(2) && productReceived.ToRound(2) >= allPayExchangeApplyAmountRMB.ToRound(2))
                        {
                            //这个订单确认收完款
                            reponsitory.Update<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>(new
                            {
                                IsVerified = true,
                                VerifyDate = DateTime.Now,
                            }, item => item.OrderID == orderID);

                        }
                    }

                }
            }

        }
    }

}
