using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class PaySupplierInfoRecord
    {
        /// <summary>
        /// 应付, 内单, 根据 DecHeadID 记录付汇供应商应付信息
        /// </summary>
        /// <param name="decHeadID"></param>
        public void RecordByDecHeadIDForInside(string decHeadID)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //判断是内单则继续执行，如果是外单则挑出
                    //查出订单号 -> 客户号 -> 客户类型
                    var decHeadModel = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.ID == decHeadID).Select(item => new
                    {
                        DecHeadID = item.ID,
                        OrderID = item.OrderID,
                    }).FirstOrDefault();
                    if (decHeadModel == null)
                    {
                        throw new Exception("不存在ID为 " + decHeadID + " 的DecHead");
                    }

                    var orderModel = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>().Where(t => t.ID == decHeadModel.OrderID).Select(item => new
                    {
                        OrderID = item.ID,
                        ClientID = item.ClientID,
                        Currency = item.Currency,
                    }).FirstOrDefault();
                    if (orderModel == null)
                    {
                        throw new Exception("不存在ID为 " + decHeadModel.OrderID + " 的Order");
                    }

                    var clientModel = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Clients>().Where(t => t.ID == orderModel.ClientID).Select(item => new
                    {
                        ClientID = item.ID,
                        ClientType = item.ClientType,
                    }).FirstOrDefault();
                    if (clientModel == null)
                    {
                        throw new Exception("不存在ID为 " + orderModel.ClientID + " 的Client");
                    }

                    //如果是外单, 则直接跳出
                    if (clientModel.ClientType != null && clientModel.ClientType == (int)Enums.ClientType.External)
                    {
                        return;
                    }

                    //如果是内单, 则开始执行
                    //查出状态为200的, 对应 DecHeadID 的 PaySupplierInfo, 将这些旧的 PaySupplierInfo 置为 400
                    PaySupplierInfo[] oldPaySupplierInfos = new Views.PaySupplierInfoView(reponsitory)
                        .Where(t => t.DecHeadID == decHeadID && t.Status == Enums.Status.Normal).ToArray();
                    if (oldPaySupplierInfos != null && oldPaySupplierInfos.Any())
                    {
                        string[] oldPaySupplierInfoIDs = oldPaySupplierInfos.Select(t => t.ID).ToArray();
                        PaySupplierInfo.AbandonByIDs(reponsitory, oldPaySupplierInfoIDs);
                    }

                    //查出这个 DecHead 对应的订单中, 所有的 OrderItem, 并按照供应商对 Amount 求和。
                    //如果 供应商查不到, null 算一个供应商

                    var orderItems = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>();
                    var productSupplierMap = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ProductSupplierMap>();
                    var clientSuppliers = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ClientSuppliers>();

                    var supplierAmounts = (from orderItem in orderItems
                                           join map in productSupplierMap on orderItem.ProductUniqueCode equals map.ID into productSupplierMap2
                                           from map in productSupplierMap2.DefaultIfEmpty()
                                           join clientSupplier in clientSuppliers on map.SupplierID equals clientSupplier.ID into clientSuppliers2
                                           from clientSupplier in clientSuppliers2.DefaultIfEmpty()
                                           where orderItem.Status == (int)Enums.Status.Normal && orderItem.OrderID == decHeadModel.OrderID
                                           group new { orderItem, clientSupplier } by clientSupplier.Name into g
                                           select new
                                           {
                                               SupplierName = g.Key,
                                               Amount = g.Sum(t => t.orderItem.TotalPrice),
                                           }).ToArray();

                    if (supplierAmounts != null && supplierAmounts.Any())
                    {
                        foreach (var supplierAmount in supplierAmounts)
                        {
                            PaySupplierInfo newPaySupplierInfo = new PaySupplierInfo()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                DecHeadID = decHeadID,
                                OrderID = orderModel.OrderID,
                                ClientID = clientModel.ClientID,
                                Amount = supplierAmount.Amount,
                                Currency = orderModel.Currency,
                                SupplierName = supplierAmount.SupplierName,
                                PayType = Enums.PaySupplierPayType.Payable,
                                Status = Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                Summary = "应付, 内单, 根据 DecHeadID 记录付汇供应商应付信息",
                            };

                            newPaySupplierInfo.Enter();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("根据DecHeadID记录供应商信息");
                throw ex;
            }
        }

        /// <summary>
        /// 应付, 外单, 根据 PayExchangeApplyID 记录付汇供应商应付信息
        /// </summary>
        /// <param name="payExchangeApplyID"></param>
        public void RecordByPayExchangeApplyIDForOutside(string payExchangeApplyID)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var payExchangeApplyModel = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                        .Where(t => t.ID == payExchangeApplyID).Select(item => new
                        {
                            PayExchangeApply = item.ID,
                            ClientID = item.ClientID,
                            SupplierEnglishName = item.SupplierEnglishName,
                            Currency = item.Currency,

                        }).FirstOrDefault();

                    if (payExchangeApplyModel == null)
                    {
                        throw new Exception("不存在ID为 " + payExchangeApplyID + " 的PayExchangeApply");
                    }

                    var payExchangeApplyItemModels = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplyItems>()
                        .Where(t => t.PayExchangeApplyID == payExchangeApplyID && t.Status == (int)Enums.Status.Normal).Select(item => new
                        {
                            PayExchangeApplyItemID = item.ID,
                            OrderID = item.OrderID,
                            Amount = item.Amount,
                        }).ToArray();

                    if (payExchangeApplyItemModels != null && payExchangeApplyItemModels.Any())
                    {
                        string[] orderIDs = payExchangeApplyItemModels.Select(t => t.OrderID).ToArray();
                        var decHeadModels = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                            .Where(t => orderIDs.Contains(t.OrderID)).Select(item => new
                            {
                                DecHeaedID = item.ID,
                                OrderID = item.OrderID,
                            }).ToArray();

                        foreach (var payExchangeApplyItem in payExchangeApplyItemModels)
                        {
                            var theDecHeadModel = decHeadModels.Where(t => t.OrderID == payExchangeApplyItem.OrderID).FirstOrDefault();

                            PaySupplierInfo newPaySupplierInfo = new PaySupplierInfo()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                DecHeadID = theDecHeadModel != null ? theDecHeadModel.DecHeaedID : "",
                                OrderID = payExchangeApplyItem.OrderID,
                                ClientID = payExchangeApplyModel.ClientID,
                                Amount = payExchangeApplyItem.Amount,
                                Currency = payExchangeApplyModel.Currency,
                                SupplierName = payExchangeApplyModel.SupplierEnglishName,
                                PayType = Enums.PaySupplierPayType.Payable,
                                Status = Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                Summary = "应付, 外单, 根据 PayExchangeApplyID 记录付汇供应商应付信息",
                            };

                            newPaySupplierInfo.Enter();
                        }
                    }


                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("根据PayExchangeApplyID记录供应商信息");
                throw ex;
            }
        }

        /// <summary>
        /// 实付, 外单, 根据 PaymentNoticeID 记录供应商实付信息
        /// </summary>
        /// <param name="paymentNoticeID"></param>
        public void RecordByPaymentNoticeIDForForOutside(string paymentNoticeID)
        {
            try
            {
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    var paymentNoticeModel = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNotices>()
                        .Where(t => t.ID == paymentNoticeID).Select(item => new
                        {
                            PaymentNoticeID = item.ID,
                            Currency = item.Currency,
                            PayExchangeApplyID = item.PayExchangeApplyID,
                        }).FirstOrDefault();

                    if (paymentNoticeModel == null)
                    {
                        throw new Exception("不存在ID为 " + paymentNoticeID + " 的PaymentNotice");
                    }

                    var payExchangeApplyModel = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PayExchangeApplies>()
                        .Where(t => t.ID == paymentNoticeModel.PayExchangeApplyID).Select(item => new
                        {
                            PayExchangeApplyID = item.ID,
                            SupplierEnglishName = item.SupplierEnglishName,
                        }).FirstOrDefault();

                    if (payExchangeApplyModel == null)
                    {
                        throw new Exception("不存在ID为 " + paymentNoticeModel.PayExchangeApplyID + " 的PayExchangeApply");
                    }

                    var paymentNoticeItemModels = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.PaymentNoticeItems>()
                        .Where(t => t.PaymentNoticeID == paymentNoticeID).Select(item => new
                        {
                            PaymentNoticeItemID = item.ID,
                            OrderID = item.OrderID,
                            Amount = item.Amount,
                        }).ToArray();

                    if (paymentNoticeItemModels != null && paymentNoticeItemModels.Any())
                    {
                        string[] orderIDs = paymentNoticeItemModels.Select(t => t.OrderID).ToArray();
                        var decHeadModels = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>()
                            .Where(t => orderIDs.Contains(t.OrderID)).Select(item => new
                            {
                                DecHeaedID = item.ID,
                                OrderID = item.OrderID,
                            }).ToArray();

                        var orderModels = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.Orders>()
                            .Where(t => orderIDs.Contains(t.ID)).Select(item => new
                            {
                                OrderID = item.ID,
                                ClientID = item.ClientID,
                            }).ToArray();

                        foreach (var paymentNoticeItem in paymentNoticeItemModels)
                        {
                            var theDecHeadModel = decHeadModels.Where(t => t.OrderID == paymentNoticeItem.OrderID).FirstOrDefault();
                            var theOrderModel = orderModels.Where(t => t.OrderID == paymentNoticeItem.OrderID).FirstOrDefault();

                            PaySupplierInfo newPaySupplierInfo = new PaySupplierInfo()
                            {
                                ID = Guid.NewGuid().ToString("N"),
                                DecHeadID = theDecHeadModel != null ? theDecHeadModel.DecHeaedID : "",
                                OrderID = paymentNoticeItem.OrderID,
                                ClientID = theOrderModel.ClientID,
                                Amount = paymentNoticeItem.Amount,
                                Currency = paymentNoticeModel.Currency,
                                SupplierName = payExchangeApplyModel.SupplierEnglishName,
                                PayType = Enums.PaySupplierPayType.Payable,
                                Status = Enums.Status.Normal,
                                CreateDate = DateTime.Now,
                                UpdateDate = DateTime.Now,
                                Summary = "应付, 外单, 根据 PayExchangeApplyID 记录付汇供应商应付信息",
                            };

                            newPaySupplierInfo.Enter();
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("根据PaymentNoticeID记录供应商信息");
                throw ex;
            }
        }

    }
}
