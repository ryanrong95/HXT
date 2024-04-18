using Needs.Ccs.Services.Views;
using Needs.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    [Serializable]
    public class OrderChangeNotice : IUnique, IFulError, IFulSuccess
    {
        #region 属性
        public string ID { get; set; }
        public string OrderID { get; set; }
        public Enums.OrderStatus OrderStatus { get; set; }

        //blic Order Order { get; set; }

        public Enums.OrderChangeType Type { get; set; }

        public Enums.ProcessState processState { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime UpdateDate { get; set; }

        public Enums.Status Status { get; set; }

        public DecHead DecHead { get; set; }
        public string ClientID { get; set; }
        public string ClientCode { get; set; }
        public string ClientName { get; set; }
        public Admin Admin { get; set; }
        #endregion

        public OrderChangeNotice()
        {
            this.UpdateDate = DateTime.Now;
            this.Status = Enums.Status.Normal;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;


        public void UpdateOrderTax(IEnumerable<ParamsOrderChange> ImportData, IEnumerable<ParamsOrderChange> ExciseTaxData, IEnumerable<ParamsOrderChange> AddedValueData, string CusTariffValue)
        {
            List<PreOrderChangeNoticeLogInfo> listPreOrderChangeNoticeLogInfo = new List<PreOrderChangeNoticeLogInfo>(); //暂存 OrderChangeNoticeLog 信息

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>(
                     new
                     {
                         ProcessState = Enums.ProcessState.Processing,
                     }, t => t.OderID == OrderID);

                //更新关税
                //start-----因拆分装箱，需分组合并累加后，再更新orderitem关税  ryan 20201014
                var ImportDataDo = from im in ImportData
                                   group im by new { im.OrderItemID, im.ProductModel, im.OldImportRate, im.ImportRate, im.OldExciseTaxRate, im.ExciseTaxRate, im.OldAddedValueRate, im.AddedValueRate } into g
                                   select new
                                   {
                                       g.Key.OrderItemID,
                                       g.Key.ProductModel,
                                       g.Key.OldImportRate,
                                       g.Key.ImportRate,
                                       g.Key.OldExciseTaxRate,
                                       g.Key.ExciseTaxRate,
                                       g.Key.OldAddedValueRate,
                                       g.Key.AddedValueRate,
                                       ImportTax = g.Sum(t => t.ImportTax)
                                   };
                //end-----因拆分装箱，需分组合并累加后，再更新orderitem关税
                foreach (var item in ImportDataDo)
                {
                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>().Count(x => x.OrderItemID == item.OrderItemID);
                    if (count > 0)
                    {
                        //更新关税
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(
                       new
                       {
                           Rate = item.ImportRate,
                           ReceiptRate = item.ImportRate,
                           Value = item.ImportTax
                       }, t => t.OrderItemID == item.OrderItemID && t.Type == (int)Enums.CustomsRateType.ImportTax);
                    };

                    var logInfo = listPreOrderChangeNoticeLogInfo.Where(t => t.OrderID == this.OrderID && t.OrderItemID == item.OrderItemID).FirstOrDefault();
                    if (logInfo == null)
                    {
                        listPreOrderChangeNoticeLogInfo.Add(new PreOrderChangeNoticeLogInfo()
                        {
                            OrderID = this.OrderID,
                            OrderItemID = item.OrderItemID,
                            ProductModel = item.ProductModel,
                            OldImportRate = item.OldImportRate,
                            NewImportRate = item.ImportRate,
                            OldExciseTaxRate = item.OldExciseTaxRate,
                            NewExciseTaxRate = item.ExciseTaxRate,
                            OldAddedValueRate = item.OldAddedValueRate,
                            NewAddedValueRate = item.AddedValueRate,
                        });
                    }

                }

                //关税不够50，实收为0，则实缴全部置0
                //更新关税
                if (Decimal.Parse(CusTariffValue) == 0M)
                {
                    var orderItemIDs = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Where(t => t.OrderID == OrderID).Select(t => t.ID).ToArray();
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(
                       new
                       {
                           ReceiptRate = 0M
                       }, t => orderItemIDs.Contains(t.OrderItemID) && t.Type == (int)Enums.CustomsRateType.ImportTax);
                }

                //更新消费税
                //start-----因拆分装箱，需分组合并累加后，再更新orderitem消费税   ryan 20201014
                var ExciseTaxDataDo = from im in ExciseTaxData
                                      group im by new { im.OrderItemID, im.ProductModel, im.OldImportRate, im.ImportRate, im.OldExciseTaxRate, im.ExciseTaxRate, im.OldAddedValueRate, im.AddedValueRate } into g
                                      select new
                                      {
                                          g.Key.OrderItemID,
                                          g.Key.ProductModel,
                                          g.Key.OldImportRate,
                                          g.Key.ImportRate,
                                          g.Key.OldExciseTaxRate,
                                          g.Key.ExciseTaxRate,
                                          g.Key.OldAddedValueRate,
                                          g.Key.AddedValueRate,
                                          ExciseTax = g.Sum(t => t.ExciseTax)
                                      };
                //end-----因拆分装箱，需分组合并累加后，再更新orderitem消费税
                foreach (var item in ExciseTaxDataDo)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(
                        new
                        {
                            Rate = item.ExciseTaxRate,
                            ReceiptRate = item.ExciseTaxRate,
                            Value = item.ExciseTax,
                        }, t => t.OrderItemID == item.OrderItemID && t.Type == (int)Enums.CustomsRateType.ConsumeTax);

                    var logInfo = listPreOrderChangeNoticeLogInfo.Where(t => t.OrderID == this.OrderID && t.OrderItemID == item.OrderItemID).FirstOrDefault();
                    if (logInfo == null)
                    {
                        listPreOrderChangeNoticeLogInfo.Add(new PreOrderChangeNoticeLogInfo()
                        {
                            OrderID = this.OrderID,
                            OrderItemID = item.OrderItemID,
                            ProductModel = item.ProductModel,
                            OldImportRate = item.OldImportRate,
                            NewImportRate = item.ImportRate,
                            OldExciseTaxRate = item.OldExciseTaxRate,
                            NewExciseTaxRate = item.ExciseTaxRate,
                            OldAddedValueRate = item.OldAddedValueRate,
                            NewAddedValueRate = item.AddedValueRate,
                        });
                    }
                }

                //更新增值税
                //start-----因拆分装箱，需分组合并累加后，再更新orderitem增值税   ryan 20201014
                var AddedValueDataDo = from im in AddedValueData
                                       group im by new { im.OrderItemID, im.ProductModel, im.OldImportRate, im.ImportRate, im.OldExciseTaxRate, im.ExciseTaxRate, im.OldAddedValueRate, im.AddedValueRate } into g
                                       select new
                                       {
                                           g.Key.OrderItemID,
                                           g.Key.ProductModel,
                                           g.Key.OldImportRate,
                                           g.Key.ImportRate,
                                           g.Key.OldExciseTaxRate,
                                           g.Key.ExciseTaxRate,
                                           g.Key.OldAddedValueRate,
                                           g.Key.AddedValueRate,
                                           AddedValueTax = g.Sum(t => t.AddedValueTax)
                                       };
                //end-----因拆分装箱，需分组合并累加后，再更新orderitem增值税
                foreach (var item in AddedValueDataDo)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemTaxes>(
                        new
                        {
                            Rate = item.AddedValueRate,
                            ReceiptRate = item.AddedValueRate,
                            Value = item.AddedValueTax,
                        }, t => t.OrderItemID == item.OrderItemID && t.Type == (int)Enums.CustomsRateType.AddedValueTax);

                    var logInfo = listPreOrderChangeNoticeLogInfo.Where(t => t.OrderID == this.OrderID && t.OrderItemID == item.OrderItemID).FirstOrDefault();
                    if (logInfo == null)
                    {
                        listPreOrderChangeNoticeLogInfo.Add(new PreOrderChangeNoticeLogInfo()
                        {
                            OrderID = this.OrderID,
                            OrderItemID = item.OrderItemID,
                            ProductModel = item.ProductModel,
                            OldImportRate = item.OldImportRate,
                            NewImportRate = item.ImportRate,
                            OldExciseTaxRate = item.OldExciseTaxRate,
                            NewExciseTaxRate = item.ExciseTaxRate,
                            OldAddedValueRate = item.OldAddedValueRate,
                            NewAddedValueRate = item.AddedValueRate,
                        });
                    }
                }
                //将完税价格记录在ApiNotices表中；

                // 2021-02-02 LK 外单客户处理的订单异常不需要写入DB中
                var order = new Needs.Ccs.Services.Views.Origins.OrdersOrigin()[this.OrderID];
                if (order.Type != Enums.OrderType.Outside)
                {
                    int apiNoticeCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>().Count(x => x.ItemID == DecHead.ID);
                    if (apiNoticeCount == 0)
                    {
                        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            PushType = (int)Enums.PushType.DutiablePrice,
                            PushStatus = (int)Enums.PushStatus.Unpush,
                            ItemID = DecHead.ID,
                            ClientID = this.ClientID,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now
                        });
                    }
                }

                //外部公司插入apinotice,推送进价
                //if (order.Type != Enums.OrderType.Inside)
                //{
                //    int apiNoticeCount = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.ApiNotices>().Count(x => x.ItemID == DecHead.ID && x.PushType == (int)Enums.PushType.PurchasePrice);
                //    if (apiNoticeCount == 0)
                //    {
                //        reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ApiNotices
                //        {
                //            ID = ChainsGuid.NewGuidUp(),
                //            PushType = (int)Enums.PushType.PurchasePrice,
                //            PushStatus = (int)Enums.PushStatus.Unpush,
                //            ItemID = DecHead.ID,
                //            ClientID = "",
                //            CreateDate = DateTime.Now,
                //            UpdateDate = DateTime.Now
                //        });
                //    }
                //}



                //将修改关税率、增值税率的日志信息插入 OrderChangeNoticeLogs 表 Begin

                if (listPreOrderChangeNoticeLogInfo != null && listPreOrderChangeNoticeLogInfo.Any())
                {
                    foreach (var preOrderChangeNoticeLogInfo in listPreOrderChangeNoticeLogInfo)
                    {
                        int count1 = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNoticeLogs>()
                            .Count(x => x.OrderID == preOrderChangeNoticeLogInfo.OrderID && x.OrderItemID == preOrderChangeNoticeLogInfo.OrderItemID);
                        if (count1 <= 0)
                        {
                            StringBuilder sbSummary = new StringBuilder();
                            sbSummary.Append("报关员【" + this.Admin.ByName + "】，修改了型号【" + preOrderChangeNoticeLogInfo.ProductModel + "】，");
                            if (preOrderChangeNoticeLogInfo.OldImportRate != preOrderChangeNoticeLogInfo.NewImportRate)
                            {
                                sbSummary.Append("关税率【" + preOrderChangeNoticeLogInfo.OldImportRate + "】改为【" + preOrderChangeNoticeLogInfo.NewImportRate + "】，");
                            }
                            if (preOrderChangeNoticeLogInfo.OldExciseTaxRate != preOrderChangeNoticeLogInfo.NewExciseTaxRate)
                            {
                                sbSummary.Append("消费税率【" + preOrderChangeNoticeLogInfo.OldExciseTaxRate + "】改为【" + preOrderChangeNoticeLogInfo.NewExciseTaxRate + "】，");
                            }
                            if (preOrderChangeNoticeLogInfo.OldAddedValueRate != preOrderChangeNoticeLogInfo.NewAddedValueRate)
                            {
                                sbSummary.Append("增值税率【" + preOrderChangeNoticeLogInfo.OldAddedValueRate + "】改为【" + preOrderChangeNoticeLogInfo.NewAddedValueRate + "】，");
                            }

                            reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderChangeNoticeLogs
                            {
                                ID = ChainsGuid.NewGuidUp(),
                                OrderID = this.OrderID,
                                OrderItemID = preOrderChangeNoticeLogInfo.OrderItemID,
                                AdminID = this.Admin.ID,
                                CreateDate = DateTime.Now,
                                Summary = sbSummary.ToString().Trim('，'),
                            });
                        }
                    }
                }

                //将修改关税率、增值税率的日志信息插入 OrderChangeNoticeLogs 表 End

                //处理税费异常后，更新declist的完税价格
                Task.Run(() =>
                {
                    var DecHead = new Ccs.Services.Views.DecHeadsView().Where(t => t.OrderID == this.OrderID).FirstOrDefault();
                    var DecLists = new Ccs.Services.Views.DecOriginListsView().Where(t => t.DeclarationID == DecHead.ID).ToList();
                    var orderCus = new Ccs.Services.Views.Orders2View().Where(t => t.ID == DecHead.OrderID).FirstOrDefault();
                    var orderitems = new Needs.Ccs.Services.Views.OrdersView()[DecHead.OrderID].Items.ToList();
                    var remainDeci = DecHead.isTwoStep ? 2 : 0;
                    foreach (var item in DecLists)
                    {
                        decimal taxedPrice = Math.Round(item.DeclTotal * orderCus.CustomsExchangeRate.Value, remainDeci, MidpointRounding.AwayFromZero);
                        decimal tariff = orderitems.Where(t => t.ID == item.OrderItemID).FirstOrDefault().ImportTax.Value.Value;
                        taxedPrice += tariff;
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecLists>(new { TaxedPrice = taxedPrice }, t => t.ID == item.ID);
                    }
                });
            }

        }


        public void UpdateProcessState()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>(
                   new
                   {
                       ProcessState = Enums.ProcessState.Processed,
                   }, t => t.OderID == OrderID);
            }
        }

        /// <summary>
        /// 所有订单变更处理完之后，将ApiNotice的状态改为未推送
        /// </summary>
        public void UpdateApiNoticeStatus()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                var changeNotices = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Where(t => t.OderID == OrderID).ToList();
                if (!changeNotices.Any(t => t.ProcessState != (int)Enums.ProcessState.Processed))
                {
                    var decHead = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(t => t.OrderID == OrderID).FirstOrDefault();
                    if (decHead != null)
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.ApiNotices>(
                         new
                         {
                             PushStatus = (int)Enums.PushStatus.Unpush
                         }, t => t.ItemID == decHead.ID);
                    }
                }
            }
        }

        /// <summary>
        /// 跟单处理订单变更后，更新订单的是否可以报关标志为可报关
        /// </summary>
        public void UpdateDeclareFlag()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(
                   new
                   {
                       DeclareFlag = (int)Enums.DeclareFlagEnums.Able,
                   }, t => t.ID == OrderID);
            }
        }
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Count(item => item.OderID == this.OrderID && item.Type == (int)this.Type && item.ProcessState == (int)Enums.ProcessState.Processing);

                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderChangeNotices
                    {
                        ID = this.ID,
                        OderID = this.OrderID,
                        Type = (int)this.Type,
                        ProcessState = (int)this.processState,
                        Status = (int)this.Status,
                        CreateDate = DateTime.Now,
                        UpdateDate = this.UpdateDate,
                    });
                }
                else
                {
                    reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderChangeNotices
                    {
                        ID = this.ID,
                        OderID = this.OrderID,
                        Type = (int)this.Type,
                        ProcessState = (int)this.processState,
                        Status = (int)this.Status,
                        CreateDate = this.CreateDate,
                        UpdateDate = this.UpdateDate,
                    }, item => item.ID == this.ID);
                }
            }

            this.OnEnterSuccess();
        }

        virtual protected void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this.ID));
            }
        }
    }

    public class ParamsOrderChange
    {
        public string ID { get; set; }
        public string OrderItemID { get; set; }
        public decimal CustomsExchangeRate { get; set; }
        //public decimal? OrderImportTax { get; set; }
        //public decimal? OrderAddedValueTax { get; set; }
        public string HSCode { get; set; }
        public string ProductName { get; set; }
        public string ProductModel { get; set; }
        public string Origin { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal ImportTax { get; set; }
        public decimal ImportRate { get; set; }
        public decimal ExciseTax { get; set; }
        public decimal ExciseTaxRate { get; set; }
        public decimal AddedValueRate { get; set; }
        public decimal AddedValueTax { get; set; }

        /// <summary>
        /// 旧的 关税率
        /// </summary>
        public decimal OldImportRate { get; set; }

        /// <summary>
        /// 旧的 消费税率
        /// </summary>
        public decimal OldExciseTaxRate { get; set; }

        /// <summary>
        /// 旧的 增值税率
        /// </summary>
        public decimal OldAddedValueRate { get; set; }
    }

    public class OrderChangeDetail : IUnique
    {
        public string OrderID { get; set; }
        public DateTime CreateDate { get; set; }
        public Enums.OrderStatus OrderStatus { get; set; }
        public string ID { get; set; }
        public string ContrNo { get; set; }
        public string EntryId { get; set; }
        public DateTime? DDate { get; set; }
        public decimal? CustomsExchangeRate { get; set; }
        public string Currency { get; set; }
        public decimal DecAmount { get; set; }
        public Enums.DecTaxStatus DecTaxStatus { get; set; }
        public decimal? AddedValue { get; set; }
        public decimal? TariffValue { get; set; }
        public decimal? ExciseTaxValue { get; set; }
    }


    public class OrderChangeProduct : IUnique
    {
        public string ID { get; set; }

        public string OrderItemID { get; set; }
        public string OrderID { get; set; }
        public string HSCode { get; set; }
        public string ProductName { get; set; }
        public string ProductModel { get; set; }
        public string Origin { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal? ImportTax { get; set; }
        public decimal ImportRate { get; set; }
        public decimal? ExciseTax { get; set; }
        public decimal ExciseTaxRate { get; set; }
        public decimal AddedValueRate { get; set; }
        public decimal? AddedValueTax { get; set; }

        public decimal? OrderImportTax { get; set; }
        public decimal? OrderExciseTax { get; set; }
        public decimal? OrderAddedValueTax { get; set; }
        public int GNo { get; set; }
        public decimal? CustomsExchangeRate { get; set; }


    }

    /// <summary>
    /// 用于暂存 OrderChangeNoticeLog 信息
    /// </summary>
    public class PreOrderChangeNoticeLogInfo
    {
        /// <summary>
        /// OrderID
        /// </summary>
        public string OrderID { get; set; } = string.Empty;

        /// <summary>
        /// OrderItemID
        /// </summary>
        public string OrderItemID { get; set; } = string.Empty;

        /// <summary>
        /// 型号
        /// </summary>
        public string ProductModel { get; set; } = string.Empty;

        /// <summary>
        /// 旧的 关税率
        /// </summary>
        public decimal OldImportRate { get; set; }

        /// <summary>
        /// 新的 关税率
        /// </summary>
        public decimal NewImportRate { get; set; }

        /// <summary>
        /// 旧的 消费税率
        /// </summary>
        public decimal OldExciseTaxRate { get; set; }

        /// <summary>
        /// 新的 消费税率
        /// </summary>
        public decimal NewExciseTaxRate { get; set; }

        /// <summary>
        /// 旧的 增值税率
        /// </summary>
        public decimal OldAddedValueRate { get; set; }

        /// <summary>
        /// 新的 增值税率
        /// </summary>
        public decimal NewAddedValueRate { get; set; }
    }

}
