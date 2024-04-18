using Layers.Data.Sqls;
using Layers.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Linq;
using Yahv.Payments;
using Yahv.PvWsOrder.Services.Enums;
using Yahv.PvWsOrder.Services.Extends;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Usually;
using Yahv.Utils.Serializers;

namespace Yahv.PvWsOrder.Services.Models
{
    /// <summary>
    /// 客户订单
    /// </summary>
    public class Order : OrderBase
    {
        public Order()
        {
            this.CreateDate = this.ModifyDate = DateTime.Now;
            this.MainStatus = CgOrderStatus.待审核;
            this.PaymentStatus = OrderPaymentStatus.Waiting;
            this.InvoiceStatus = OrderInvoiceStatus.UnInvoiced;
            this.RemittanceStatus = OrderRemittanceStatus.UnRemittance;
            //结算币种默认为人民币
            this.SettlementCurrency = Currency.CNY;

            this.EnterSuccess += Order_EnterSuccess;
        }

        #region 扩展属性

        /// <summary>
        /// 操作人
        /// </summary>
        public string OperatorID { get; set; }

        /// <summary>
        /// 入仓号（用于产生订单ID）
        /// </summary>
        public string EnterCode { get; set; }

        public WsClient OrderClient { get; set; }

        public WsSupplier OrderSupplier { get; set; }

        /// <summary>
        /// 企业客户时候用
        /// </summary>
        public Invoice Invoice
        {
            get
            {
                var invoice = new Yahv.Services.Views.WsInvoicesTopView<PvWsOrderReponsitory>()
                    .Where(item => item.ID == this.InvoiceID).FirstOrDefault();
                return invoice;
            }
        }
        /// <summary>
        /// 个人客户时候用
        /// </summary>
        public vInvoice vInvoice
        {
            get
            {
                var invoice = new Yahv.Services.Views.vInvoicesTopView<PvWsOrderReponsitory>()
                    .Where(item => item.ID == this.InvoiceID).FirstOrDefault();
                return invoice;
            }
        }

        /// <summary>
        /// 通知的信息
        /// </summary>
        public JMessage NoticeMessage { get; private set; }

        /// <summary>
        /// 订单项
        /// </summary>
        IEnumerable<Models.OrderItem> orderItems;
        public IEnumerable<Models.OrderItem> Orderitems
        {
            get
            {
                if (this.orderItems == null)
                {
                    this.orderItems = new Views.OrderItemsRoll(this.ID);
                }
                return this.orderItems;
            }
            set
            {
                this.orderItems = value;
            }
        }

        /// <summary>
        /// 订单附件
        /// </summary>
        IEnumerable<CenterFileDescription> fileitems;
        public IEnumerable<CenterFileDescription> Fileitems
        {
            get
            {
                if (this.fileitems == null)
                {
                    this.fileitems = new Views.OrderFilesRoll(this.ID);
                }
                return this.fileitems;
            }
            set
            {
                this.fileitems = value;
            }
        }

        //付汇供应商
        private string[] paymentSuppliers;
        public string[] PaymentSuppliers
        {
            get
            {
                if (this.paymentSuppliers == null)
                {
                    using (var Reponsitory = new PvWsOrderReponsitory())
                    {
                        this.paymentSuppliers = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.MapsSupplier>()
                            .Where(item => item.OrderID == this.ID).Select(t => t.SupplierID).ToArray();
                    }
                }
                return this.paymentSuppliers;
            }
            set
            {
                this.paymentSuppliers = value;
            }
        }

        /// <summary>
        /// 订单总货值
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 已申请付款金额
        /// </summary>
        public decimal PayAppliedPrice { get; set; }

        /// <summary>
        /// 已申请收款金额
        /// </summary>
        public decimal RecAppliedPrice { get; set; }

        public List<ClientModels.OrderRequirement> OrderRequirements
        {
            get
            {
                var requirements = new ClientViews.OrderRequirementOrigin().Where(item => item.OrderID == this.ID).ToList();
                return requirements;
            }
        }

        /// <summary>
        /// 本位币账单
        /// </summary>
        public IEnumerable<Yahv.Services.Models.VoucherCnyStatistic> CnyStatistics { get; set; }

        /// <summary>
        /// 本位币账单应收总金额
        /// </summary>
        public decimal CnyTotalPrice
        {
            get
            {
                return this.CnyStatistics.Sum(t => t.LeftPrice);
            }
        }

        #endregion

        #region 事件

        public event SuccessHanlder EnterSuccess;

        public void OnEnterSuccess()
        {
            if (this != null && this.EnterSuccess != null)
            {
                //成功后触发事件
                this.EnterSuccess(this, new SuccessEventArgs(this));
            }
        }
        private void Order_EnterSuccess(object sender, SuccessEventArgs e)
        {
            var order = (Order)e.Object;
            //更新订单状态日志
            order.StatusLogUpdate();
        }
        #endregion

        #region 持久化

        public void Enter()
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                //保存订单
                int count = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Orders>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    this.ID = Layers.Data.PKeySigner.Pick(PKeyType.Order, this.EnterCode);
                    Reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    this.ModifyDate = DateTime.Now;
                    Reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
                }
                //保存订单发货扩展
                if (this.OrderOutput != null)
                {
                    this.OrderOutput.ID = this.ID;
                    if (this.OrderOutput.Waybill != null)
                    {
                        this.OrderOutput.Waybill.OperatorID = this.OperatorID;
                        this.OrderOutput.Waybill.OrderID = this.ID;
                    }
                    this.OrderOutput.Enter();
                }
                //保存订单收货扩展
                if (this.OrderInput != null)
                {
                    this.OrderInput.ID = this.ID;
                    if (this.OrderInput.Waybill != null)
                    {
                        this.OrderInput.Waybill.OrderID = this.ID;
                        this.OrderInput.Waybill.OperatorID = this.OperatorID;
                        if (this.Type == OrderType.Transport)
                        {
                            //转运的时候(入库运单关联出库)
                            this.OrderInput.Waybill.TransferID = this.OrderOutput?.Waybill?.ID;
                        }
                    }
                    this.OrderInput.Enter();
                }

                //保存订单项
                var oldItems = new Views.OrderItemsRoll(this.ID).Where(item => item.Type == OrderItemType.Normal).ToList();
                SaveOrderItems(this.orderItems, oldItems);

                //保存订单文件
                var oldFiles = new Views.OrderFilesRoll(this.ID).ToList();
                SaveOrderFiles(this.fileitems, oldFiles);

                //保存付汇供应商
                SaveMapsSupplier(Reponsitory);
            }
            this.OnEnterSuccess();
        }

        public void Abandon()
        {
            this.MainStatus = CgOrderStatus.取消;
            this.StatusLogUpdate();
            //删除订单的运单
            if (this.OrderInput?.Waybill != null)
            {
                this.OrderInput.Waybill.ExcuteStatus = (int)CgSortingExcuteStatus.Anomalous;
                this.OrderInput.Waybill.OperatorID = this.OperatorID;
                this.OrderInput.Waybill.Abandon();
            }
            if (this.OrderOutput?.Waybill != null)
            {
                this.OrderOutput.Waybill.ExcuteStatus = (int)CgPickingExcuteStatus.Anomalous;
                this.OrderOutput.Waybill.OperatorID = this.OperatorID;
                this.OrderOutput.Waybill.Abandon();
            }
            //废弃账单
            PaymentManager.Erp(this.OperatorID).Received.Abolish(ID);
        }

        /// <summary>
        /// 小订单退回后重新提交
        /// </summary>
        public void BackToSubmit()
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                //更新订单总金额
                Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                {
                    TotalPrice = this.TotalPrice,
                }, item => item.ID == this.ID);
                //更新到货订单项
                var oldItems = new Views.OrderItemsRoll(this.ID).Where(item => item.Type == OrderItemType.Modified).ToList();
                BackToSaveOrderItems(this.orderItems, oldItems);
            }
        }

        /// <summary>
        /// 订单审批
        /// </summary>
        public void Approve(CgOrderStatus status)
        {
            this.MainStatus = status;
            this.StatusLogUpdate();
        }

        /// <summary>
        /// 保存订单的到货信息
        /// </summary>
        /// <param name="orderid"></param>
        public void SaveOrderDeliveries(string orderid)
        {
            using (PvWsOrderReponsitory reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
            {
                //该订单的到货信息
                var orderDeliveries = new Yahv.Services.Views.CgDeliveriesTopView<PvWsOrderReponsitory>(reponsitory).Where(d => d.Input.OrderID == orderid);
                var deliveriesInputid = orderDeliveries.Select(t => t.InputID).ToArray();
                //根据相同inputid数量合并（分批到货），到货异常新的inputid和数量。
                var Deliveries = (from delivery in orderDeliveries
                                  group delivery by new { delivery.Input.ID } into _delivery
                                  select new
                                  {
                                      Quantity = _delivery.Sum(item => item.Quantity),
                                      Storage = _delivery.FirstOrDefault(),
                                  }).ToArray();

                var orderitemAlls = new Views.OrderItemsRoll(orderid, reponsitory);

                #region 到货信息已经存入订单项的,更新到货数量
                //查询出已经更新的订单项
                var modifiedItemsInputid = orderitemAlls.Where(item => item.Type == OrderItemType.Modified).Select(item => item.InputID).ToArray();
                //inputid未发生变化的更新数量
                var UpdateDeliveries = Deliveries.Where(item => modifiedItemsInputid.Contains(item.Storage.Input.ID));
                //更新数量
                foreach (var Delivery in UpdateDeliveries)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        Delivery.Quantity,
                        Currency = Delivery.Storage.Input.Currency,
                        UnitPrice = Delivery.Storage.Input.UnitPrice,
                        TotalPrice = Delivery.Storage.Input.UnitPrice * Delivery.Quantity,
                    }, orderitem => orderitem.InputID == Delivery.Storage.Input.ID && orderitem.Type == OrderItemType.Modified.GetHashCode());
                }
                //删除以前多余的到货
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                {
                    Status = (int)GeneralStatus.Closed,
                    ModifyDate = DateTime.Now,
                }, item => item.OrderID == orderid && item.Type == (int)OrderItemType.Modified && !deliveriesInputid.Contains(item.InputID));
                #endregion

                #region 到货信息没有存入订单项的,新增,并导入归类信息
                var InsertDeliveries = Deliveries.Except(UpdateDeliveries);
                //将到货数据转成订单数据
                var orderitems = InsertDeliveries.Select(item => new Layers.Data.Sqls.PvWsOrder.OrderItems
                {
                    ID = Layers.Data.PKeySigner.Pick(PKeyType.OrderItem),
                    Type = (int)OrderItemType.Modified,
                    InputID = item.Storage.Input.ID,
                    TinyOrderID = item.Storage.Input.TinyOrderID,
                    OrderID = item.Storage.Input.OrderID,
                    ProductID = item.Storage.Product.ID,
                    Origin = item.Storage.Origin,
                    DateCode = item.Storage.DateCode,
                    Quantity = item.Quantity,
                    Currency = item.Storage.Input.Currency == null ? (int)Currency.Unknown : (int)item.Storage.Input.Currency,
                    UnitPrice = item.Storage.Input.UnitPrice == null ? 0.00M : (decimal)item.Storage.Input.UnitPrice,
                    Unit = (int)LegalUnit.个,
                    TotalPrice = item.Storage.Input.UnitPrice == null ? 0.00M : item.Quantity * (decimal)item.Storage.Input.UnitPrice,
                    GrossWeight = 0.02m,
                    Volume = 0.00m,
                    Conditions = new OrderItemCondition().Json(),
                    Status = (int)OrderItemStatus.Normal,
                    CreateDate = DateTime.Now,
                    ModifyDate = DateTime.Now,
                    IsAuto = false,
                });
                reponsitory.Insert(orderitems.ToArray());
                //自动更新分拣产品数据到订单
                foreach (var orderitem in InsertDeliveries)
                {
                    reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        DateCode = orderitem.Storage.DateCode,
                        ProductID = orderitem.Storage.ProductID,
                    }, item => item.ID == orderitem.Storage.Input.ItemID);
                }
                //更新订单总数量
                var totalprice = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>().Where(item => item.OrderID == orderid && item.Type == 2).Sum(item => item.TotalPrice);
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.Orders>(new
                {
                    TotalPrice = totalprice,
                }, item => item.ID == orderid);


                #region 更新归类信息
                //更新现有的归类信息
                var inputids = InsertDeliveries.Select(item => item.Storage.Input.ID).ToArray();
                if (inputids.Count() == 0)
                {
                    return;
                }

                var Items = orderitemAlls.Where(item => item.Type == OrderItemType.Normal).
                    Where(item => item.OrderItemsTerm != null).ToArray();
                var modifyItems = orderitemAlls.Where(item => item.Type == OrderItemType.Modified).
                    Where(item => item.OrderItemsTerm == null).ToArray();
                var terms = (from _item in modifyItems
                             join item in Items on _item.ProductID equals item.ProductID
                             where item.Origin == _item.Origin
                             select new Layers.Data.Sqls.PvWsOrder.OrderItemsTerm
                             {
                                 ID = _item.ID,
                                 OriginRate = item.OrderItemsTerm.OriginRate,
                                 FVARate = item.OrderItemsTerm.FVARate,
                                 Ccc = item.OrderItemsTerm.Ccc,
                                 Embargo = item.OrderItemsTerm.Embargo,
                                 HkControl = item.OrderItemsTerm.HkControl,
                                 Coo = item.OrderItemsTerm.Coo,
                                 CIQ = item.OrderItemsTerm.CIQ,
                                 CIQprice = item.OrderItemsTerm.CIQprice,
                                 IsHighPrice = item.OrderItemsTerm.IsHighPrice,
                                 IsDisinfected = item.OrderItemsTerm.IsDisinfected,
                             }).Distinct();

                var chcds = (from _item in modifyItems
                             join item in Items on _item.ProductID equals item.ProductID
                             where item.Origin == _item.Origin
                             select new Layers.Data.Sqls.PvWsOrder.OrderItemsChcd
                             {
                                 ID = _item.ID,
                                 AutoHSCodeID = item.OrderItemsChcd.AutoHSCodeID,
                                 AutoDate = item.OrderItemsChcd.AutoDate,
                                 FirstAdminID = item.OrderItemsChcd.FirstAdminID,
                                 FirstHSCodeID = item.OrderItemsChcd.FirstHSCodeID,
                                 FirstDate = item.OrderItemsChcd.FirstDate,
                                 SecondAdminID = item.OrderItemsChcd.SecondAdminID,
                                 SecondHSCodeID = item.OrderItemsChcd.SecondHSCodeID,
                                 SecondDate = item.OrderItemsChcd.SecondDate,
                                 CustomHSCodeID = item.OrderItemsChcd.CustomHSCodeID,
                                 CustomTaxCode = item.OrderItemsChcd.CustomTaxCode,
                                 SysPriceID = item.OrderItemsChcd.SysPriceID,
                                 CustomsPriceID = item.OrderItemsChcd.CustomsPriceID,
                                 VATaxedPriceID = item.OrderItemsChcd.VATaxedPriceID,
                                 CreateDate = item.OrderItemsChcd.CreateDate,
                                 ModifyDate = item.OrderItemsChcd.ModifyDate,
                             }).Distinct();
                reponsitory.Insert(terms.ToArray());
                reponsitory.Insert(chcds.ToArray());

                var updateids = terms.Select(a => a.ID).ToArray();
                //更新归类标志
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                {
                    IsAuto = true,
                }, a => updateids.Contains(a.ID));
                #endregion

                #endregion

                //#region 重发入库通知
                //Task task = Task.Run(() =>
                //{
                //    System.Threading.Thread.Sleep(5000);
                //    var order = new Views.OrderAlls()[orderid];
                //    if (order.MainStatus == CgOrderStatus.已交货)
                //    {
                //        this.CgEntryNotice(new string[] { orderid }, order.Type);
                //    }
                //});
                //#endregion
            }
        }

        /// <summary>
        /// 分拣异常后，修改订单项
        /// </summary>
        /// <param name="newItems">订单项</param>
        public void ModifyOrderItems(IEnumerable<OrderItem> newItems)
        {
            using (PvWsOrderReponsitory reponsitory = new PvWsOrderReponsitory())
            {
                //新增的下单项
                var addItems = newItems.Where(t => string.IsNullOrEmpty(t.ID));
                foreach (var item in addItems)
                {
                    var arrivals = this.Orderitems.Where(t => t.Type == OrderItemType.Modified);
                    foreach (var arrival in arrivals)
                    {
                        if (arrival.Product.Manufacturer == item.Product.Manufacturer && arrival.Product.PartNumber == item.Product.PartNumber &&
                            arrival.Origin == item.Origin && arrival.Quantity == item.Quantity)
                        {
                            item.InputID = arrival.InputID;
                        }
                    }
                    if (string.IsNullOrEmpty(item.InputID))
                    {
                        item.InputID = Layers.Data.PKeySigner.Pick(PKeyType.Input);
                    }
                    item.OrderID = this.ID;
                    item.TinyOrderID = this.ID + "-01";
                    item.Enter();
                }

                //订单项（正常下单）
                var normals = this.Orderitems.Where(item => item.Type == OrderItemType.Normal);
                string[] newids = newItems.Select(item => item.ID).ToArray();
                string[] ids = normals.Select(item => item.ID).ToArray();
                //相同的ids,
                string[] updateIds = ids.Intersect(newids).ToArray();
                //不同的ids
                string[] deleteIds = ids.Where(item => !updateIds.Contains(item)).ToArray();

                //更新下单项
                foreach (var id in updateIds)
                {
                    var newobj = newItems.Where(t => t.ID == id).FirstOrDefault();
                    var normal = normals.Where(t => t.ID == id).FirstOrDefault();
                    if (newobj.Product.Manufacturer != normal.Product.Manufacturer || newobj.Product.PartNumber != normal.Product.PartNumber ||
                        newobj.Origin != normal.Origin || newobj.DateCode != normal.DateCode || newobj.Quantity != normal.Quantity || newobj.TotalPrice != normal.TotalPrice)
                    {
                        normal.Product = newobj.Product;
                        normal.Origin = newobj.Origin;
                        normal.DateCode = newobj.DateCode;
                        normal.Quantity = newobj.Quantity;
                        normal.TotalPrice = newobj.TotalPrice;
                        normal.UnitPrice = normal.TotalPrice / normal.Quantity;
                        normal.Enter();
                    }
                }
                //删除下单项
                reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                {
                    ModifyDate = DateTime.Now,
                    Status = (int)OrderItemStatus.Deleted,
                }, item => deleteIds.Contains(item.ID));
                reponsitory.Submit();

                //修改订单状态，重新发入库通知
                this.MainStatus = CgOrderStatus.已提交;
                this.StatusLogUpdate();
                //修改运单的状态
                this.OrderInput.Waybill.ExcuteStatus = (int)CgSortingExcuteStatus.Sorting;
                this.OrderInput.Waybill.OperatorID = this.OperatorID;
                this.OrderInput.Waybill.UpdateStatus();
            }
        }

        /// <summary>
        /// 入库通知重构后
        /// </summary>
        /// <param name="orderids"></param>
        /// <param name="type"></param>
        public void CgEntryNotice(string[] orderids, OrderType type)
        {
            using (PvWsOrderReponsitory Reponsitory = LinqFactory<PvWsOrderReponsitory>.Create())
            {
                var orders = new WsOrdersTopView<PvWsOrderReponsitory>(Reponsitory).Where(item => orderids.Contains(item.ID)).ToArray();
                //根据订单生成入库通知
                foreach (var order in orders)
                {
                    try
                    {
                        #region 查询出需要的订单数据
                        var waybill = new ClientViews.WayBillAlls(Reponsitory)[order.Input.WayBillID];

                        var items = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderItems>()
                            .Where(item => item.OrderID == order.ID && item.Type == (int)OrderItemType.Normal && item.Status != (int)OrderItemStatus.Deleted);
                        //查询出订单项数据
                        var orderitems = from item in items
                                         join product in new ProductsTopView<PvWsOrderReponsitory>(Reponsitory) on item.ProductID equals product.ID
                                         where item.OrderID == order.ID
                                         select new OrderItem
                                         {
                                             ID = item.ID,
                                             OrderID = item.OrderID,
                                             InputID = item.InputID,
                                             ProductID = item.ProductID,
                                             TinyOrderID = item.TinyOrderID,
                                             Origin = (Origin)Enum.Parse(typeof(Origin), item.Origin),
                                             DateCode = item.DateCode,
                                             Quantity = item.Quantity,
                                             Currency = (Currency)item.Currency,
                                             UnitPrice = item.UnitPrice,
                                             Unit = (LegalUnit)item.Unit,
                                             TotalPrice = item.TotalPrice,
                                             CreateDate = item.CreateDate,
                                             ModifyDate = item.ModifyDate,
                                             GrossWeight = item.GrossWeight,
                                             Volume = item.Volume,
                                             Conditions = item.Conditions,
                                             Status = (OrderItemStatus)item.Status,
                                             IsAuto = item.IsAuto,
                                             WayBillID = item.WayBillID,
                                             Product = product,
                                         };

                        #endregion

                        #region 拼凑入库通知对象
                        var noticeSource = type == OrderType.Recieved ? CgNoticeSource.AgentEnter : CgNoticeSource.Transfer;
                        var Notices = orderitems.Select(item => new
                        {
                            Type = CgNoticeType.Enter,
                            WareHouseID = Yahv.Services.WhSettings.HK["HK01"].ID,//代仓储默认香港万路通库房
                            WaybillID = waybill.ID,
                            item.InputID,
                            item.OutputID,
                            item.CustomName,
                            item.DateCode,
                            item.Origin,
                            item.Quantity,
                            Source = noticeSource,
                            Target = NoticesTarget.Default,
                            Weight = item.GrossWeight,
                            item.Volume,
                            Conditions = new
                            {
                                DevanningCheck = waybill.WayCondition.UnBoxed,//是否拆箱验货 ????
                                Weigh = item.OrderItemCondition.IsWeighing,//是否称重
                                CheckNumber = item.OrderItemCondition.CheckNumber,//是否点数
                                OnlineDetection = item.OrderItemCondition.IsDetection,//是否上机检测
                                AttachLabel = item.OrderItemCondition.IsCustomLabel,//是否贴标签
                                PaintLabel = item.OrderItemCondition.IsDaubLotCode,//是否涂抹标签
                                Repacking = item.OrderItemCondition.IsRepackaging,//是否重新标签
                            }.Json(),
                            Product = new
                            {
                                item.Product.PartNumber,
                                item.Product.Manufacturer,
                                item.Product.PackageCase,
                                item.Product.Packaging,
                            },
                            Input = new
                            {
                                ID = item.InputID,
                                Code = item.InputID,
                                item.OrderID,
                                item.TinyOrderID,
                                ItemID = item.ID,
                                ClientID = order.ClientID,
                                PayeeID = order.PayeeID,
                                Currency = item.Currency,
                                UnitPrice = item.UnitPrice,
                            },
                        }).ToArray();

                        if (type == OrderType.Transport)
                        {
                            //获取订单特殊要求
                            var Requirements = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderRequirements>().Where(item => item.OrderID == order.ID);
                            //获取收款申请
                            var receive = (from apply in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.Applications>()
                                           join item in Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.ApplicationItems>()
                                           on apply.ID equals item.ApplicationID
                                           where item.OrderID == order.ID && apply.Type == (int)ApplicationType.Receival
                                           select apply).FirstOrDefault();
                            var Waybill = new
                            {
                                WaybillID = waybill.ID,
                                Supplier = waybill.Supplier,
                                Type = waybill.Type,
                                Requirement = Requirements.Select(item => new
                                {
                                    item.ID,
                                    item.OrderID,
                                    item.Type,
                                    item.Name,
                                    item.Quantity,
                                    item.UnitPrice,
                                    item.TotalPrice,
                                    item.Requirement,
                                }).ToArray(),
                                CheckRequirement = new
                                {
                                    order.Input.IsPayCharge,
                                    ApplicationID = receive?.ID,
                                    DelivaryOpportunity = receive?.DelivaryOpportunity,
                                }
                            };
                            var data = new { Waybill, Enter = new { Notices } };
                            //调用库房接口
                            var apisetting = new PvWmsApiSetting();
                            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgNoticeEnter;
                            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);
                        }
                        else
                        {
                            var Waybill = new
                            {
                                WaybillID = waybill.ID,
                                Supplier = waybill.Supplier,
                                Type = waybill.Type,
                                CheckRequirement = new
                                {
                                    order.Input.IsPayCharge,
                                }
                            };
                            var data = new { Waybill, Enter = new { Notices } };
                            //调用库房接口
                            var apisetting = new PvWmsApiSetting();
                            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgNoticeEnter;
                            var result = Yahv.Utils.Http.ApiHelper.Current.JPost(apiurl, data);
                        }
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        continue;
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// 保存订单项
        /// </summary>
        private void SaveOrderItems(IEnumerable<OrderItem> newItems, IEnumerable<OrderItem> oldItems)
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                string[] newids = newItems.Select(item => item.ID).ToArray();
                string[] oldids = oldItems.Select(item => item.ID).ToArray();
                //相同的ids,不同的ids
                string[] sameids = oldids.Intersect(newids).ToArray();
                string[] diffids = oldids.Where(item => !sameids.Contains(item)).ToArray();
                #region 订单项新增处理
                var InsertItems = newItems.Where(item => item.ID == null);
                Task t1 = new Task(() =>
                {
                    foreach (var item in InsertItems)
                    {
                        item.ID = Layers.Data.PKeySigner.Pick(PKeyType.OrderItem);
                        if (this.Type != OrderType.Delivery)
                        {
                            item.InputID = Layers.Data.PKeySigner.Pick(PKeyType.Input);
                        }
                        else
                        {
                            item.OutputID = Layers.Data.PKeySigner.Pick(PKeyType.Output);
                        }
                        item.OrderID = this.ID;
                        item.TinyOrderID = this.ID + "-01";
                        Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(item.Product);
                        //用线程池执行无参数方法
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(item.SaveOrderItem));
                    }
                });
                t1.Start();
                #endregion
                #region 订单项删除处理
                foreach (var id in diffids)
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        ModifyDate = DateTime.Now,
                        Status = (int)OrderItemStatus.Deleted,
                    }, item => item.ID == id);
                }
                #endregion
                #region 订单项更新处理
                foreach (var id in sameids)
                {
                    var newItem = newItems.Where(t => t.ID == id).FirstOrDefault();
                    var oldItem = oldItems.Where(t => t.ID == id).FirstOrDefault();

                    //判断是否需要更新
                    if (!IsUpdateItem(newItem, oldItem))
                    {
                        //保存日志
                        Reponsitory.Insert(oldItem.ToLinqLog());
                        //更新数据
                        Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(newItem.Product);
                        Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            ProductID = newItem.Product.ID,
                            CustomName = newItem.CustomName,
                            Origin = newItem.OriginGetCode,
                            DateCode = newItem.DateCode,
                            Quantity = newItem.Quantity,
                            Currency = (int)newItem.Currency,
                            UnitPrice = newItem.UnitPrice,
                            Unit = (int)newItem.Unit,
                            TotalPrice = newItem.TotalPrice,
                            ModifyDate = DateTime.Now,
                            GrossWeight = newItem.GrossWeight,
                            Volume = newItem.Volume,
                            Conditions = newItem.Conditions,
                            Status = (int)newItem.Status,
                            IsAuto = newItem.IsAuto,
                            WayBillID = newItem.WayBillID,
                            StorageID = newItem.StorageID,
                        }, item => item.ID == newItem.ID);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 保存订单项（小订单退回重新提交专用）
        /// </summary>
        private void BackToSaveOrderItems(IEnumerable<OrderItem> newItems, IEnumerable<OrderItem> oldItems)
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                string[] newids = newItems.Select(item => item.ID).ToArray();
                string[] oldids = oldItems.Select(item => item.ID).ToArray();
                //相同的ids,不同的ids
                string[] sameids = oldids.Intersect(newids).ToArray();
                string[] diffids = oldids.Where(item => !sameids.Contains(item)).ToArray();
                #region 订单项新增处理
                var InsertItems = newItems.Where(item => item.ID == null);
                Task t1 = new Task(() =>
                {
                    foreach (var item in InsertItems)
                    {
                        item.ID = Layers.Data.PKeySigner.Pick(PKeyType.OrderItem);
                        if (this.Type != OrderType.Delivery)
                        {
                            item.InputID = Layers.Data.PKeySigner.Pick(PKeyType.Input);
                        }
                        else
                        {
                            item.OutputID = Layers.Data.PKeySigner.Pick(PKeyType.Output);
                        }
                        item.OrderID = this.ID;
                        Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(item.Product);
                        //用线程池执行无参数方法
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(item.SaveOrderItem));
                    }
                });
                t1.Start();
                #endregion
                #region 订单项删除处理
                foreach (var id in diffids)
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        ModifyDate = DateTime.Now,
                        Status = (int)OrderItemStatus.Deleted,
                    }, item => item.ID == id);
                }
                #endregion
                #region 订单项更新处理
                foreach (var id in sameids)
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                    {
                        Status = 200,
                    }, item => item.ID == id);

                    var newItem = newItems.Where(t => t.ID == id).FirstOrDefault();
                    var oldItem = oldItems.Where(t => t.ID == id).FirstOrDefault();
                    //判断是否需要更新
                    if (!IsUpdateItem(newItem, oldItem))
                    {
                        //保存日志
                        Reponsitory.Insert(oldItem.ToLinqLog());
                        //更新数据
                        Yahv.Services.Views.ProductsTopView<PvWsOrderReponsitory>.Enter(newItem.Product);
                        Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderItems>(new
                        {
                            ProductID = newItem.Product.ID,
                            CustomName = newItem.CustomName,
                            Origin = newItem.OriginGetCode,
                            DateCode = newItem.DateCode,
                            Quantity = newItem.Quantity,
                            Currency = (int)newItem.Currency,
                            UnitPrice = newItem.UnitPrice,
                            Unit = (int)newItem.Unit,
                            TotalPrice = newItem.TotalPrice,
                            ModifyDate = DateTime.Now,
                            GrossWeight = newItem.GrossWeight,
                            Volume = newItem.Volume,
                            Conditions = newItem.Conditions,
                            Status = (int)newItem.Status,
                            IsAuto = newItem.IsAuto,
                            WayBillID = newItem.WayBillID,
                            StorageID = newItem.StorageID,
                        }, item => item.ID == newItem.ID);
                    }
                }
                #endregion
            }
        }

        /// <summary>
        /// 是否需要更新Item
        /// </summary>
        /// <param name="newItem"></param>
        /// <param name="oldItem"></param>
        /// <returns></returns>
        private bool IsUpdateItem(OrderItem newItem, OrderItem oldItem)
        {
            var newEntity = new
            {
                ProductID = newItem.Product.ID,
                CustomName = newItem.CustomName,
                Origin = newItem.Origin,
                DateCode = newItem.DateCode,
                Quantity = newItem.Quantity.ToString("0.0000000"),
                Currency = newItem.Currency,
                UnitPrice = newItem.UnitPrice.ToString("0.0000000"),
                Unit = newItem.Unit,
                TotalPrice = newItem.TotalPrice.ToString("0.0000000"),
            };
            var oldEntity = new
            {
                ProductID = oldItem.Product.ID,
                CustomName = oldItem.CustomName,
                Origin = oldItem.Origin,
                DateCode = oldItem.DateCode,
                Quantity = oldItem.Quantity.ToString("0.0000000"),
                Currency = oldItem.Currency,
                UnitPrice = oldItem.UnitPrice.ToString("0.0000000"),
                Unit = oldItem.Unit,
                TotalPrice = oldItem.TotalPrice.ToString("0.0000000"),
            };
            if (newEntity.Json() == oldEntity.Json())
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 保存订单文件
        /// </summary>
        /// <param name="newFiles"></param>
        /// <param name="oldFiles"></param>
        private void SaveOrderFiles(IEnumerable<CenterFileDescription> newFiles, IEnumerable<CenterFileDescription> oldFiles)
        {
            string[] newids = newFiles.Select(item => item.ID).ToArray();
            string[] oldids = oldFiles.Select(item => item.ID).ToArray();
            using (PvCenterReponsitory reponsitory = LinqFactory<PvCenterReponsitory>.Create())
            {
                //删除原文件
                foreach (var id in oldids)
                {
                    if (!newids.Contains(id))
                    {
                        //删除原来的项
                        reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                        {
                            WsOrderID = "",
                        }, item => item.ID == id && item.WsOrderID == this.ID);
                    }
                }
                //订单绑定新文件
                reponsitory.Update<Layers.Data.Sqls.PvCenter.FilesDescription>(new
                {
                    WsOrderID = this.ID,
                    WaybillID = this.OrderInput != null ? this.OrderInput?.Waybill?.ID : this.OrderOutput?.Waybill?.ID
                }, item => newids.Contains(item.ID));
            }
        }

        /// <summary>
        /// 保存付汇供应商
        /// </summary>
        /// <param name="reponsitory"></param>
        private void SaveMapsSupplier(PvWsOrderReponsitory reponsitory)
        {
            if (this.PaymentSuppliers.Count() > 0)
            {
                //先删
                reponsitory.Delete<Layers.Data.Sqls.PvWsOrder.MapsSupplier>(item => item.OrderID == this.ID);
                //后插
                foreach (var PaymentSupplier in this.PaymentSuppliers)
                {
                    reponsitory.Insert(new Layers.Data.Sqls.PvWsOrder.MapsSupplier()
                    {
                        OrderID = this.ID,
                        SupplierID = PaymentSupplier,
                    });
                }
            }
        }
    }

    public class OrderBase : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 客户ID
        /// </summary>
        public string ClientID { get; set; }

        /// <summary>
        /// 管理员（会员）ID
        /// </summary>
        public string CreatorID { get; set; }

        /// <summary>
        /// 订单服务类型
        /// </summary>
        public OrderType Type { get; set; }

        /// <summary>
        /// 发票ID
        /// </summary>
        public string InvoiceID { get; set; }

        /// <summary>
        /// 平台公司ID
        /// </summary>
        public string PayeeID { get; set; }

        /// <summary>
        /// 受益人ID
        /// </summary>
        public string BeneficiaryID { get; set; }

        public CgOrderStatus MainStatus { get; set; }

        public OrderPaymentStatus PaymentStatus { get; set; }

        public OrderInvoiceStatus InvoiceStatus { get; set; }

        public OrderRemittanceStatus RemittanceStatus { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime ModifyDate { get; set; }

        /// <summary>
        /// 供应商ID
        /// </summary>
        public string SupplierID { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 结算币种
        /// </summary>
        public Currency SettlementCurrency { get; set; }

        public OrderInput OrderInput { get; set; }

        public OrderOutput OrderOutput { get; set; }

    }

    /// <summary>
    /// 订单收货拓展
    /// </summary>
    public class OrderInput : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 受益人(代付货款)
        /// </summary>
        public string BeneficiaryID { get; set; }

        /// <summary>
        /// 是否代付货款
        /// </summary>
        public bool? IsPayCharge { get; set; }

        public string WayBillID { get; set; }

        public string Conditions { get; set; }

        public Currency Currency { get; set; }

        #region 扩展属性

        public Waybill Waybill { get; set; }

        /// <summary>
        /// 订单条件
        /// </summary>
        public OrderCondition OrderCondition
        {
            get
            {
                if (this.Conditions == null)
                {
                    this.Conditions = new OrderCondition().Json();
                }
                return this.Conditions.JsonTo<OrderCondition>();
            }
        }

        public string CurrencyDec
        {
            get
            {
                return this.Currency.GetDescription();
            }
        }

        #endregion

        public OrderInput()
        {
        }

        #region 持久化
        public void Enter()
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                //保存订单的运单信息
                if (this.Waybill != null)
                {
                    this.Waybill.Enter();
                    this.WayBillID = this.Waybill.ID;
                }
                //保存订单收货扩展
                int count = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderInputs>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    Reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderInputs>(new
                    {
                        BeneficiaryID = this.BeneficiaryID,
                        WayBillID = this.WayBillID,
                        IsPayCharge = this.IsPayCharge,
                        Conditions = this.Conditions,
                        Currency = (int)this.Currency,

                    }, item => item.ID == this.ID);
                }
            }
        }

        #endregion
    }

    /// <summary>
    /// 订单发货拓展
    /// </summary>
    public class OrderOutput : IUnique
    {
        public string ID { get; set; }

        /// <summary>
        /// 受益人(代收货款)
        /// </summary>
        public string BeneficiaryID { get; set; }

        /// <summary>
        /// 是否代收货款
        /// </summary>
        public bool? IsReciveCharge { get; set; }

        /// <summary>
        /// 代发货运单
        /// </summary>
        public string WayBillID { get; set; }

        /// <summary>
        /// 出库条件
        /// </summary>
        public string Conditions { get; set; }

        public Waybill Waybill { get; set; }

        public Currency Currency { get; set; }

        /// <summary>
        /// 订单条件
        /// </summary>
        public OrderCondition OrderCondition
        {
            get
            {
                if (this.Conditions == null)
                {
                    this.Conditions = new OrderCondition().Json();
                }
                return this.Conditions.JsonTo<OrderCondition>();
            }
        }

        public OrderOutput()
        {
        }

        public string CurrencyDec
        {
            get
            {
                return this.Currency.GetDescription();
            }
        }


        #region 持久化

        public void Enter()
        {
            using (PvWsOrderReponsitory Reponsitory = new PvWsOrderReponsitory())
            {
                //保存订单的运单信息
                if (this.Waybill != null)
                {
                    this.Waybill.Enter();
                    this.WayBillID = this.Waybill.ID;
                }
                //保存订单收货扩展
                int count = Reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.OrderOutputs>().Count(item => item.ID == this.ID);
                if (count == 0)
                {
                    Reponsitory.Insert(this.ToLinq());
                }
                else
                {
                    Reponsitory.Update<Layers.Data.Sqls.PvWsOrder.OrderOutputs>(new
                    {
                        BeneficiaryID = this.BeneficiaryID,
                        WayBillID = this.WayBillID,
                        IsReciveCharge = this.IsReciveCharge,
                        Conditions = this.Conditions,
                        Currency = (int)this.Currency
                    }, item => item.ID == this.ID);
                }
            }
        }

        #endregion
    }
}
