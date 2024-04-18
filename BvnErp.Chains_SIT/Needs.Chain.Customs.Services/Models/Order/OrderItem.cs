using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 代理订单项
    /// </summary>
    [Serializable]
    public class OrderItem : IUnique, IPersistence, IFulError, IFulSuccess
    {
        #region 属性

        public string ID { get; set; }

        public string OrderID { get; set; }

        public string MainOrderID { get; set; }

        //public string ProductID { get; set; }

        ///// <summary>
        ///// 标准产品
        ///// </summary>
        //public Product Product { get; set; }

        /// <summary>
        /// 原产地
        /// </summary>
        public string Origin { get; set; }

        /// <summary>
        /// 数量
        /// </summary>
        public decimal Quantity { get; set; }

        /// <summary>
        /// 已申报数量（分批申报时填写）
        /// DeclaredQuantity为空，表示该订单项未申报
        /// DeclaredQuantity小于Quantity，表示该订单项已经部分申报
        /// DeclaredQuantity等于Quantity，表示该订单项已申报
        /// </summary>
        public decimal? DeclaredQuantity { get; set; }

        public Enums.ProductDeclareStatus ProductDeclareStatus
        {
            get
            {
                if (!this.DeclaredQuantity.HasValue)
                    return Enums.ProductDeclareStatus.UnDeclare;
                else if (this.Quantity < this.DeclaredQuantity)
                    return Enums.ProductDeclareStatus.PartDeclare;
                else
                    return Enums.ProductDeclareStatus.AllDeclare;
            }
        }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// 毛重
        /// </summary>
        public decimal? GrossWeight { get; set; }

        /// <summary>
        /// 对于四级客户，报价时需要选择是否抽检
        /// </summary>
        public bool IsSampllingCheck { get; set; }

        /// <summary>
        /// 归类状态：未归类、首次归类、归类完成，归类异常
        /// </summary>
        public Enums.ClassifyStatus ClassifyStatus { get; set; }

        /// <summary>
        /// 产品唯一编码
        /// </summary>
        public string ProductUniqueCode { get; set; }

        /// <summary>
        /// 状态：正常、删除
        /// </summary>
        public Enums.Status Status { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime UpdateDate { get; set; }

        public string Summary { get; set; }

        /// <summary>
        /// 产品归类
        /// </summary>
        public OrderItemCategory Category { get; set; }

        /// <summary>
        /// 进口关税
        /// </summary>
        public OrderItemTax ImportTax { get; set; }

        /// <summary>
        /// 增值税
        /// </summary>
        public OrderItemTax AddedValueTax { get; set; }

        /// <summary>
        /// 消费税
        /// </summary>
        public OrderItemTax ExciseTax { get; set; }

        /// <summary>
        /// 商检费
        /// </summary>
        public decimal? InspectionFee { get; set; }

        /// <summary>
        /// 库房分拣员
        /// </summary>
        public Admin SorterAdmin { get; set; }

        public string Name { get; set; }

        public string Model { get; set; }

        public string Manufacturer { get; set; }

        public string Batch { get; set; }

        #endregion

        public OrderItem()
        {
            this.IsSampllingCheck = false;
            this.ClassifyStatus = Enums.ClassifyStatus.Unclassified;
            this.Status = Enums.Status.Normal;
            this.UpdateDate = this.CreateDate = DateTime.Now;

            this.OriginChanged += OrderItem_OriginChanged;
            this.ManufacturerChanged += OrderItem_ManufacturerChanged;
            this.OrderHangUped += Order_HangUped;
            this.BatchChanged += OrderItem_BatchChanged;
            this.SplitModelChanged += OrderItem_SplitModelChanged;
            this.ProductModelChanged += OrderItem_ProductModelChanged;
            this.SplitModelSync += OrderItem_SplitModelSync;
        }

        public event SuccessHanlder AbandonSuccess;
        public event ErrorHanlder EnterError;
        public event SuccessHanlder EnterSuccess;
        public event ErrorHanlder AbandonError;

        public event BatchChangedHanlder BatchChanged;
        /// <summary>
        /// 当订单挂起时发生
        /// </summary>
        public event OrderHangUpedHanlder OrderHangUped;
        /// <summary>
        /// 当产地变更时发生
        /// </summary>
        public event OriginChangedHanlder OriginChanged;
        /// <summary>
        /// 当品牌变更时发生
        /// </summary>
        public event ManufacturerChangedHanlder ManufacturerChanged;
        /// <summary>
        /// 当型号变更时发生
        /// </summary>
        public event ProductModelChangedHanlder ProductModelChanged;
        public event SplitModelChangedHanlder SplitModelChanged;
        public event SplitModelChangedHanlder SplitModelSync;
        /// <summary>
        /// 持久化
        /// </summary>
        public void Enter()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderItems>().Count(item => item.ID == this.ID);

                if (count == 0)
                {
                    //主键ID（OrderItem +8位年月日+10位流水号）
                    var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                    this.ID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                    reponsitory.Insert(this.ToLinq());

                }
                else
                {
                    reponsitory.Update(this.ToLinq(), item => item.ID == this.ID);
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

        /// <summary>
        /// 删除
        /// </summary>
        public void Abandon()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { Status = Enums.Status.Delete }, item => item.ID == this.ID);
            }

            this.OnAbandonSuccess();
        }

        virtual protected void OnAbandonSuccess()
        {
            if (this != null && this.AbandonSuccess != null)
            {
                //成功后触发事件
                this.AbandonSuccess(this, new SuccessEventArgs(this.ID));
            }
        }

       

        virtual protected void OnOriginChanged(string origin)
        {
            if (this != null && this.OriginChanged != null)
            {
                this.OriginChanged(this, new OriginChangedEventArgs(this, origin));
            }
        }

        /// <summary>
        /// 修改产地
        /// </summary>
        /// <param name="origin"></param>
        public void ChangeOrigin(string origin)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderItems
                {
                    Origin = origin
                }, item => item.ID == this.ID);
            }

            this.OnOriginChanged(origin);
        }

        private void OrderItem_OriginChanged(object sender, OriginChangedEventArgs e)
        {
            var orderItem = (OrderItem)e.Object;
            var oldOrigion = orderItem.Origin;
            var newOrigin = e.Origin;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //生成产地变更通知
                var changeNotice = new OrderItemChangeNotice()
                {
                    Type = OrderItemChangeType.OriginChange,
                    OrderItemID = orderItem.ID,
                    OldValue = oldOrigion,
                    NewValue = newOrigin,
                    ProcessState = ProcessState.UnProcess,
                    Sorter = orderItem.SorterAdmin,
                    OrderID = orderItem.OrderID,
                    TriggerSource = Enums.TriggerSource.HKPacking,
                };
                changeNotice.Enter();

                //记录变更日志
                orderItem.Log( OrderItemChangeType.OriginChange, "库房管理员[" + orderItem.SorterAdmin.RealName + "]做了产地变更操作，从[" + orderItem.Origin + "]变为[" + newOrigin + "]");
            }
        }

     

        virtual protected void OnManufacturerChanged(string manufacturer)
        {
            if (this != null && this.ManufacturerChanged != null)
            {
                this.ManufacturerChanged(this, new ManufacturerChangedEventArgs(this, manufacturer));
            }
        }

        /// <summary>
        /// 修改品牌
        /// </summary>
        /// <param name="origin"></param>
        public void ChangeManufacturer(string manufacturer)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderItems
                {
                    Manufacturer = manufacturer
                }, item => item.ID == this.ID);
            }
            this.OnManufacturerChanged(manufacturer);
        }

        private void OrderItem_ManufacturerChanged(object sender, ManufacturerChangedEventArgs e)
        {
            var orderItem = (OrderItem)e.Object;
            var manufacturer = e.Manufacturer;

            //修改申报要素中的品牌
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //var hsCode = orderItem.Category.HSCode;
                //var tariff = new Views.CustomsTariffsView(reponsitory).Where(item => item.HSCode == hsCode.Trim()).FirstOrDefault();
                //if (tariff != null)
                //{
                //    var elements = tariff.Elements;
                //    var elementArr = elements.Split(';');
                //    for (int i = 0; i < elementArr.Length; i++)
                //    {
                //        var arr = elementArr[i].Split(':');
                //        if (arr[1] == "品牌")
                //        {
                //            var categoryElementArr = orderItem.Category.Elements.Split('|');
                //            categoryElementArr[i] = manufacturer + "牌";
                //            reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItemCategories>(new
                //            {
                //                Elements = string.Join("|", categoryElementArr)
                //            }, t => t.ID == orderItem.Category.ID);
                //        }
                //    }
                //}
                
                //加入品牌变更通知
                var changeNotice = new OrderItemChangeNotice()
                {
                    Type = OrderItemChangeType.BrandChange,
                    OrderItemID = orderItem.ID,
                    ProcessState = ProcessState.UnProcess,
                    Sorter = orderItem.SorterAdmin,
                    OldValue = orderItem.Manufacturer,
                    NewValue = manufacturer,
                    OrderID = orderItem.OrderID,
                    TriggerSource = Enums.TriggerSource.HKPacking,
                };
                changeNotice.Enter();

                //记录变更日志
                orderItem.Log( OrderItemChangeType.BrandChange, "库房管理员[" + orderItem.SorterAdmin.RealName + "]做了品牌变更操作，从[" + orderItem.Manufacturer + "]变为[" + manufacturer + "]");
            }
        }


        virtual protected void OnProductModelChanged(string productModel)
        {
            if (this != null && this.ProductModelChanged != null)
            {
                this.ProductModelChanged(this, new ProductModelChangedEventArgs(this, productModel));
            }
        }

        /// <summary>
        /// 修改型号
        /// </summary>
        /// <param name="productModel"></param>
        public void ChangeProductModel(string productModel)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderItems
                {
                    Model = productModel
                }, item => item.ID == this.ID);
            }
            this.OnProductModelChanged(productModel);
        }

        private void OrderItem_ProductModelChanged(object sender, ProductModelChangedEventArgs e)
        {
            var orderItem = (OrderItem)e.Object;
            string orderid = orderItem.OrderID;
            var productmodel = e.ProductModel;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //生成型号变更通知
                var changeNotice = new OrderItemChangeNotice()
                {
                    Type = OrderItemChangeType.ProductModelChange,
                    OrderItemID = orderItem.ID,
                    ProcessState = ProcessState.UnProcess,
                    Sorter = orderItem.SorterAdmin,
                    OldValue = orderItem.Model,
                    NewValue = e.ProductModel,
                    OrderID = orderItem.OrderID,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    TriggerSource = Enums.TriggerSource.HKPacking,
                };
                changeNotice.Enter();

                //记录变更日志
                orderItem.Log(OrderItemChangeType.ProductModelChange, "库房管理员[" + orderItem.SorterAdmin.RealName + "]做了型号变更操作，从["+ orderItem.Model+ "]变为["+productmodel + "]");

                //生成订单变更通知
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderChangeNotices>().Count(item => item.OderID == this.OrderID && item.Type == (int)Enums.OrderChangeType.ProductChange && item.ProcessState == (int)Enums.ProcessState.Processing);
                if (count == 0)
                {
                    reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.OrderChangeNotices
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        OderID = this.OrderID,
                        Type = (int)(int)Enums.OrderChangeType.ProductChange,
                        ProcessState = (int)Enums.ProcessState.Processing,
                        Status = (int)this.Status,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    });
                }
            }
        }


        /// <summary>
        /// 修改批次号
        /// </summary>
        public void ChangeBatch(string batch)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update(new Layer.Data.Sqls.ScCustoms.OrderItems
                {
                    Batch = batch
                }, item => item.ID == this.ID);
            }
            this.OnBatchChanged(batch);
        }

        private void OrderItem_BatchChanged(object sender, BatchChangedEventArgs e)
        {
            var orderItem = (OrderItem)e.Object;
            var oldBatch = orderItem.Batch;
            var newBatch = e.Batch;

            orderItem.Log( OrderItemChangeType.BatchChange, "库房管理员[" + orderItem.SorterAdmin.RealName + "]修改了批次号，从[" + oldBatch + "]变为[" + newBatch + "]");
        }

        virtual protected void OnBatchChanged(string batch)
        {
            if (this != null && this.BatchChanged != null)
            {
                this.BatchChanged(this, new BatchChangedEventArgs(this, batch));
            }
        }


       

        virtual protected void OnSplitModelChanged(string NewOrigin, decimal NewQty, string Manufature, Admin admin,decimal oldTotalPrice, string newOrderItemID)
        {
            if (this != null && this.OriginChanged != null)
            {
                this.SplitModelChanged(this, new SplitModelChangedEventArgs(this, NewOrigin,NewQty, Manufature, admin,oldTotalPrice, newOrderItemID));
            }
        }

        virtual protected void OnSplitModelSync(string NewOrigin, decimal NewQty, string Manufature, Admin admin, decimal oldTotalPrice, string newOrderItemID)
        {
            if (this != null && this.OriginChanged != null)
            {
                this.SplitModelSync(this, new SplitModelChangedEventArgs(this, NewOrigin, NewQty, Manufature, admin, oldTotalPrice, newOrderItemID));
            }
        }

        /// <summary>
        /// 拆分型号2
        /// </summary>
        /// <param name="NewOrigin"></param>
        /// <param name="NewQty"></param>
        /// <param name="Manufature"></param>
        /// <param name="admin"></param>
        /// <returns></returns>
        public string SplitModel2(string NewOrigin, decimal NewQty, string Manufature, Admin admin)
        {
            string newOrderItemID = string.Empty;

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                decimal TotalPrice = this.TotalPrice;
                decimal OldQty = this.Quantity;
                decimal newTotalprice = (this.TotalPrice / OldQty) * (OldQty - NewQty);
                decimal OldTotalPrice = TotalPrice - newTotalprice;
                //1.更新原来的 订单项的数量，和报关总价
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                {
                    Quantity = OldQty - NewQty,
                    TotalPrice = newTotalprice,

                }, item => item.ID == this.ID);

                var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                newOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);

                this.OnSplitModelChanged(NewOrigin, NewQty, Manufature, admin, OldTotalPrice, newOrderItemID);

            }

            return newOrderItemID;
        }

        /// <summary>
        /// 拆分型号
        /// </summary>

        public void SplitModel(string NewOrigin,decimal NewQty,string Manufature,Admin admin)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                decimal TotalPrice = this.TotalPrice;
                decimal OldQty = this.Quantity;
                decimal newTotalprice = (this.TotalPrice / OldQty) * (OldQty - NewQty);
                decimal OldTotalPrice = TotalPrice - newTotalprice;
                //1.更新原来的 订单项的数量，和报关总价
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new 
                {
                    Quantity = OldQty - NewQty,
                    TotalPrice = newTotalprice,

                }, item => item.ID == this.ID);

                var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                string newOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);

                this.OnSplitModelChanged(NewOrigin, NewQty, Manufature, admin,OldTotalPrice, newOrderItemID);
                this.OnSplitModelSync(NewOrigin, NewQty, Manufature, admin, OldTotalPrice, newOrderItemID);
            }
        }

        private void OrderItem_SplitModelChanged(object sender,SplitModelChangedEventArgs e)
        {
            var orderItem = (OrderItem)e.Object;
            var oldID = orderItem.ID;
            var oldOrigion = orderItem.Origin;
            var newOrigin = e.Origin;
            var oldProductUniqueCode = orderItem.ProductUniqueCode;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                //2.向orderItem 表中插入一笔新的记录

                this.ID = e.NewOrderItemID;
                this.Quantity = e.Qty;
                this.Manufacturer =e.Manufature;
                this.SorterAdmin = e.Admin;
                this.Origin = newOrigin;
                this.CreateDate = DateTime.Now;
                this.TotalPrice = e.OldTotalPrice;
                this.ProductUniqueCode = oldProductUniqueCode;
                reponsitory.Insert(this.ToLinq());

                //3.向入库通知项中插入一条记录
                var entryItem = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>().FirstOrDefault(x => x.OrderItemID == oldID);
                entryItem.ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem);
                entryItem.OrderItemID = this.ID;
                entryItem.CreateDate = DateTime.Now;
                reponsitory.Insert(entryItem);

                //4.归类结果也插入相应记录
                this.Category.OrderItemID = entryItem.OrderItemID;
                this.Category.ID = string.Concat(this.ID).MD5();
                this.Category.CreateDate = DateTime.Now;
                this.Category.ClassifyFirstOperatorID = this.Category.ClassifyFirstOperator.ID;
                this.Category.ClassifySecondOperatorID = this.Category.ClassifySecondOperator.ID;
                reponsitory.Insert(this.Category.ToLinq());

                //5.添加订单项关税、增值税记录
                this.ImportTax.ID = string.Concat(this.ID, CustomsRateType.ImportTax).MD5();
                this.ImportTax.OrderItemID = this.ID;
                this.ImportTax.CreateDate = DateTime.Now;
                reponsitory.Insert(this.ImportTax.ToLinq());

                this.AddedValueTax.ID = string.Concat(this.ID, CustomsRateType.AddedValueTax).MD5();
                this.AddedValueTax.OrderItemID = this.ID;
                this.AddedValueTax.CreateDate = DateTime.Now;
                reponsitory.Insert(this.AddedValueTax.ToLinq());

                //6.商检
                if ((this.Category.Type & ItemCategoryType.Inspection) >0)
                {
                    var premium = new OrderPremium
                    {
                        ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium),
                        OrderID = this.OrderID,
                        OrderItemID = this.ID,
                        Type = Enums.OrderPremiumType.InspectionFee,
                        Count = 1,
                        UnitPrice = this.InspectionFee.GetValueOrDefault(),
                        Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY),
                        Rate = 1,
                        Admin = e.Admin ?? this.Category.Declarant
                    };
                    reponsitory.Insert(premium.ToLinq());
                }

                //7.生成订单项通知（产地变更）
                var changeItemNotice = new OrderItemChangeNotice();
                changeItemNotice.Type = OrderItemChangeType.OriginChange;
                changeItemNotice.OrderItemID = ID;
                changeItemNotice.ProcessState = ProcessState.UnProcess;
                changeItemNotice.Sorter = this.SorterAdmin;
                changeItemNotice.IsSplited = true;
                changeItemNotice.OldValue = oldOrigion;
                changeItemNotice.NewValue = newOrigin;
                changeItemNotice.OrderID = this.OrderID;
                changeItemNotice.TriggerSource = Enums.TriggerSource.HKPacking;
                changeItemNotice.Enter();
                //8.记录日志
                this.Log(OrderItemChangeType.OriginChange, "库房管理员[" + changeItemNotice.Sorter.RealName + "]拆分了型号[" + this.Model + "],产地为[" + newOrigin + "]，数量为："+ e.Qty);
            }
        }

        private void OrderItem_SplitModelSync(object sender, SplitModelChangedEventArgs e)
        {
            //变更信息同步客户端
            var OriginOrder = new Needs.Ccs.Services.Views.OrdersView().Where(t => t.ID == this.OrderID).FirstOrDefault();
            MatchPost2AgentWarehouse post = new MatchPost2AgentWarehouse(OriginOrder.MainOrderID, OriginOrder);
            post.ItemChangePost();
            //同步归类结果
            List<OrderItemAssitant> orderItems = new List<OrderItemAssitant>();
            OrderItemAssitant itemAssitant = new OrderItemAssitant();
            itemAssitant.ID = e.NewOrderItemID;
            orderItems.Add(itemAssitant);
            MatchSyncClassify matchSyncClassify = new MatchSyncClassify(orderItems);
            matchSyncClassify.SyncResult();
        }

        /// <summary>
        /// 订单挂起
        /// </summary>
        public void OrderHangUp(Enums.OrderControlType controlType, Enums.OrderControlStep controlStep = Enums.OrderControlStep.Merchandiser, string summary = null)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.Orders>(new { IsHangUp = true }, item => item.ID == this.OrderID);
            }

            this.OnOrderHangUped(new OrderHangUpedEventArgs(this.OrderID, this.ID, controlType, controlStep, summary));
        }

        virtual protected void OnOrderHangUped(OrderHangUpedEventArgs args)
        {
            if (this.OrderHangUped != null)
            {
                this.OrderHangUped(this, args);
            }
        }

        private void Order_HangUped(object sender, Hanlders.OrderHangUpedEventArgs e)
        {
            this.GenerateControl(e.OrderControlType, e.OrderControlStep, e.Summary);
            this.SendControlNotice(e.OrderControlType, e.OrderControlStep, e.Summary);
        }

        /// <summary>
        /// 生成订单管控
        /// </summary>
        /// <param name="type">管控类型</param>
        /// <param name="step">审核步骤/审核层级</param>
        private void GenerateControl(Enums.OrderControlType type, Enums.OrderControlStep step, string summary)
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.OrderControls>().Where(oc => oc.OrderID == this.OrderID &&
                                                                    oc.OrderItemID == this.ID && oc.ControlType == (int)type).Count();
                if (count > 0)
                    return;

                var control = new OrderControlData();
                control.OrderID = this.OrderID;
                control.OrderItemID = this.ID;
                control.ControlType = type;
                control.Summary = summary;
                reponsitory.Insert(control.ToLinq());

                var controlStep = new OrderControlStep();
                controlStep.OrderControlID = control.ID;
                controlStep.Step = step;
                reponsitory.Insert(controlStep.ToLinq());
            }
        }

        /// <summary>
        /// 归类成特殊型号，发送通知
        /// </summary>
        /// <param name="type"></param>
        /// <param name="step"></param>
        /// <param name="summary"></param>
        private void SendControlNotice(Enums.OrderControlType type, Enums.OrderControlStep step, string summary)
        {
            NoticeLog log = new NoticeLog();
            log.MainID = this.OrderID;

            //跟单审批
            if (type == Enums.OrderControlType.CCC && step == Enums.OrderControlStep.Merchandiser)
            {               
                log.NoticeType = Enums.SendNoticeType.CCC;
            }

            //跟单审批
            if (type == Enums.OrderControlType.OriginCertificate)
            {
                log.NoticeType = Enums.SendNoticeType.OriginCertificate;
            }

            //总部审批
            if (type== Enums.OrderControlType.CCC&&step== Enums.OrderControlStep.Headquarters)
            {
                log.NoticeType = Enums.SendNoticeType.HQCCC;
            }          

            //总部审批
            if (type == Enums.OrderControlType.Forbid)
            {
                log.NoticeType = Enums.SendNoticeType.Forbid;
            }

            log.SendNotice();
        }

        /// <summary>
        /// 抽检
        /// </summary>
        public void SampllingCheck()
        {
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                this.IsSampllingCheck = true;
                reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { IsSampllingCheck = true }, item => item.ID == this.ID);
            }
        }
    }
}