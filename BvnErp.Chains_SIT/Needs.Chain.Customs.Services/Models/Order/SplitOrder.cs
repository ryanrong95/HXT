using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Linq;
using Needs.Utils.Converters;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    /// <summary>
    /// 拆分订单
    /// </summary>
    [Serializable]
    public class SplitOrder : Order
    {
        OrderItems splitItems;
        public OrderItems SplitItems
        {
            get
            {
                if (splitItems == null)
                {
                    using (var view = new Views.OrderItemsView())
                    {
                        var query = view.Where(item => item.OrderID == this.ID && item.Status == Enums.Status.Normal);
                        this.SplitItems = new OrderItems(query);
                    }
                }
                return this.splitItems;
            }
            set
            {
                if (value == null)
                {
                    return;
                }

                this.splitItems = new OrderItems(value, new Action<OrderItem>(delegate (OrderItem item)
                {
                    item.OrderID = this.ID;
                }));
            }
        }
        /// <summary>
        /// 拆分的箱号
        /// </summary>
        public List<string> Packs { get; set; }
        /// <summary>
        /// 原来的OrderID
        /// </summary>
        public string OldOrderID { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public Admin Operator { get; set; }
        /// <summary>
        /// OrderItem 没被拆之前，订单里面的数量
        /// </summary>
        public Dictionary<string, decimal> SplitQuantity { get; set; }
        /// <summary>
        /// 记录Orderitem 和 Input的对应关系，提交给库房接口
        /// </summary>
        public Dictionary<string,string> OrderItemInputMap { get; set; }
        /// <summary>
        /// 提交给库房的数据接口
        /// </summary>
        public List<SubmitForWarehouse> OrderItemInputOrderMap { get; set; }

        public event OrderSplitHanlder OrderSplit;

        public event OrderSplitSubmitHanlder OrderSplitSubmit;

        public SplitOrder()
        {
            this.OrderBillType = OrderBillType.Normal;
            //this.OrderSplit += Order_DeclarationNotice;
            this.OrderSplit += Order_SpecialType;
            this.OrderSplit += Order_ReGenerateBill;
            this.OrderSplitSubmit += SubmitChanges;
            this.OrderItemInputOrderMap = new List<SubmitForWarehouse>();
        }
        public void Split()
        {
            //string entryNoticeID = "";
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory(false))
            {

                //新增一条Order记录               
                reponsitory.Insert(this.ToLinq());

                ////新增一条EntryNotice
                //entryNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                //EntryNotice entryNotice = new EntryNotice();
                //entryNotice.ID = entryNoticeID;
                //entryNotice.Order = this;
                //entryNotice.ClientCode = this.Client.ClientCode;
                //entryNotice.SortingRequire = SortingRequire.Packed;
                //entryNotice.WarehouseType = WarehouseType.HongKong;
                //entryNotice.EntryNoticeStatus = EntryNoticeStatus.Sealed;
                //entryNotice.Status = Status.Normal;
                //entryNotice.UpdateDate = entryNotice.CreateDate = DateTime.Now;
                //reponsitory.Insert(entryNotice.ToLinq());

                foreach (var orderitem in this.SplitItems)
                {
                    string InputID = OrderItemInputMap[orderitem.ID];
                    string tinyOrderID = this.ID;
                    //之前订单里面的数量
                    var NewQty = SplitQuantity[orderitem.ID];

                   //如果拆分的型号的数量，跟订单项的数量相等，只要改OrderItemID的OrderID,如果不等，需要新增新的OrderItem
                    if (orderitem.Quantity == NewQty)
                    {
                        //把拆分后的订单项OrderItem的OrderID，改成新的订单号
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new { OrderID = this.ID }, item => item.ID == orderitem.ID);
                        //OrderPremiums,商检费用是记在OrderItem上的，根据OrderItem 把订单的商检费改掉
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderPremiums>(new { OrderID = this.ID }, item => item.OrderItemID == orderitem.ID);

                        ////把原来EntryNoticeItem的EntryNoticeID 改成新的，根据Orderitemid去改
                        //reponsitory.Update<Layer.Data.Sqls.ScCustoms.EntryNoticeItems>(new { EntryNoticeID = entryNoticeID }, item => item.OrderItemID==orderitem.ID);

                        ////把Sorting里面的OrderID改成新的，根据Orderitemid去改
                        //reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { OrderID = this.ID }, item => item.OrderItemID==orderitem.ID);
                        this.OrderItemInputOrderMap.Add(new SubmitForWarehouse
                        {
                            InputID = InputID,
                            OrderItemID = null,
                            TinyOrderID = tinyOrderID,
                        });
                    }
                    else
                    {
                        decimal TotalPrice = orderitem.TotalPrice;
                        decimal OldQty = orderitem.Quantity;
                        decimal newTotalprice = (orderitem.TotalPrice / NewQty) * (NewQty - OldQty);
                        decimal OldTotalPrice = TotalPrice - newTotalprice;
                        //1.更新原来的 订单项的数量，和报关总价
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.OrderItems>(new
                        {
                            Quantity = NewQty - OldQty,
                            TotalPrice = newTotalprice,

                        }, item => item.ID == orderitem.ID);

                        //2.向orderItem 表中插入一笔新的记录
                        var prefix = System.Configuration.ConfigurationManager.AppSettings["Purchaser"];
                        OrderItem newOrderItem = new OrderItem();
                        string singleOrderItemID = prefix + Needs.Overall.PKeySigner.Pick(PKeyType.OrderItem);
                        newOrderItem.ID = singleOrderItemID;
                        newOrderItem.OrderID = this.ID;
                        newOrderItem.Origin = orderitem.Origin;
                        newOrderItem.Quantity = OldQty;
                        newOrderItem.Unit = orderitem.Unit;
                        newOrderItem.UnitPrice = orderitem.UnitPrice;
                        newOrderItem.TotalPrice = OldTotalPrice;
                        newOrderItem.IsSampllingCheck = orderitem.IsSampllingCheck;
                        newOrderItem.ClassifyStatus = orderitem.ClassifyStatus;
                        newOrderItem.ProductUniqueCode = orderitem.ProductUniqueCode;
                        newOrderItem.Status = Status.Normal;
                        newOrderItem.CreateDate = DateTime.Now;
                        newOrderItem.UpdateDate = DateTime.Now;
                        newOrderItem.Name = orderitem.Name;
                        newOrderItem.Model = orderitem.Model;
                        newOrderItem.Manufacturer = orderitem.Manufacturer;
                        newOrderItem.Batch = orderitem.Batch;
                        reponsitory.Insert(newOrderItem.ToLinq());


                        ////3.向入库通知项中插入一条记录
                        //EntryNoticeItem entrynoticeItem = new EntryNoticeItem();
                        //entrynoticeItem.ID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem);
                        //entrynoticeItem.EntryNoticeID = entryNoticeID;
                        //entrynoticeItem.OrderItem = newOrderItem;
                        //entrynoticeItem.IsSportCheck = orderitem.IsSampllingCheck;
                        //entrynoticeItem.EntryNoticeStatus = EntryNoticeStatus.Boxed;
                        //reponsitory.Insert(entrynoticeItem.ToLinq());


                        //4.归类结果也插入相应记录
                        OrderItemCategory category = new OrderItemCategory();
                        category.ID = string.Concat(newOrderItem.ID).MD5();
                        category.OrderItemID = newOrderItem.ID;
                        category.Type = orderitem.Category.Type;
                        category.TaxCode = orderitem.Category.TaxCode;
                        category.TaxName = orderitem.Category.TaxName;
                        category.HSCode = orderitem.Category.HSCode;
                        category.Name = orderitem.Category.Name;
                        category.Elements = orderitem.Category.Elements;
                        category.Unit1 = orderitem.Category.Unit1;
                        category.Unit2 = orderitem.Category.Unit2;
                        category.Qty1 = orderitem.Category.Qty1;
                        category.Qty2 = orderitem.Category.Qty2;
                        category.CIQCode = orderitem.Category.CIQCode;
                        category.ClassifyFirstOperatorID = orderitem.Category.ClassifyFirstOperator?.ID;
                        category.ClassifySecondOperatorID = orderitem.Category.ClassifySecondOperator?.ID;
                        category.Status = Status.Normal;
                        category.CreateDate = DateTime.Now;
                        category.UpdateDate = DateTime.Now;
                        reponsitory.Insert(category.ToLinq());

                        //5.添加订单项关税、增值税记录
                        OrderItemTax itemTax = new OrderItemTax();
                        itemTax.ID = string.Concat(newOrderItem.ID, CustomsRateType.ImportTax).MD5();
                        itemTax.OrderItemID = newOrderItem.ID;
                        itemTax.Type = CustomsRateType.ImportTax;
                        itemTax.Rate = orderitem.ImportTax.Rate;
                        itemTax.Value = orderitem.ImportTax.Value;
                        itemTax.Status = Status.Normal;
                        itemTax.CreateDate = DateTime.Now;
                        itemTax.UpdateDate = DateTime.Now;
                        reponsitory.Insert(itemTax.ToLinq());

                        OrderItemTax itemAddedValue = new OrderItemTax();
                        itemAddedValue.ID = string.Concat(newOrderItem.ID, CustomsRateType.AddedValueTax).MD5();
                        itemAddedValue.OrderItemID = newOrderItem.ID;
                        itemAddedValue.Type = CustomsRateType.AddedValueTax;
                        itemAddedValue.Rate = orderitem.AddedValueTax.Rate;
                        itemAddedValue.Value = orderitem.AddedValueTax.Value;
                        itemAddedValue.Status = Status.Normal;
                        itemAddedValue.CreateDate = DateTime.Now;
                        itemAddedValue.UpdateDate = DateTime.Now;
                        reponsitory.Insert(itemAddedValue.ToLinq());


                        //6.商检
                        if ((orderitem.Category.Type & ItemCategoryType.Inspection) > 0)
                        {
                            var premium = new OrderPremium
                            {
                                ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderPremium),
                                OrderID = this.ID,
                                OrderItemID = newOrderItem.ID,
                                Type = Enums.OrderPremiumType.InspectionFee,
                                Count = 1,
                                UnitPrice = orderitem.InspectionFee.GetValueOrDefault(),
                                Currency = MultiEnumUtils.ToCode<Enums.Currency>(Enums.Currency.CNY),
                                Rate = 1,
                                Admin = this.Operator ?? orderitem.Category.Declarant
                            };
                            reponsitory.Insert(premium.ToLinq());
                        }

                        this.OrderItemInputOrderMap.Add(new SubmitForWarehouse
                        {
                            InputID = InputID,
                            OrderItemID = singleOrderItemID,
                            TinyOrderID = tinyOrderID,
                        });

                        ////把Sorting里面的OrderID改成新的，OrderItem也改成新的，根据原来的Orderitemid,和箱号去改
                        //reponsitory.Update<Layer.Data.Sqls.ScCustoms.Sortings>(new { OrderID = this.ID, OrderItemID= newOrderItem.ID }, item => item.OrderItemID == orderitem.ID&& this.Packs.Contains(item.BoxIndex));      
                    }
                }

                ////把Packing里面的OrderID改成新的，根据OrderID 和 boxIndex 去改
                //reponsitory.Update<Layer.Data.Sqls.ScCustoms.Packings>(new { OrderID = this.ID }, item => this.Packs.Contains(item.BoxIndex) && item.OrderID == this.OldOrderID);

                //保存香港交货信息
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderConsignees>(item => item.OrderID == this.ID);
                reponsitory.Insert(this.OrderConsignee.ToLinq());

                //保存国内交货信息
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderConsignors>(item => item.OrderID == this.ID);
                reponsitory.Insert(this.OrderConsignor.ToLinq());

                //保存付汇供应商
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderPayExchangeSuppliers>(item => item.OrderID == this.ID);
                foreach (var payExchangeSupplier in this.PayExchangeSuppliers)
                {
                    reponsitory.Insert(payExchangeSupplier.ToLinq());
                }

                //保存文件
                reponsitory.Delete<Layer.Data.Sqls.ScCustoms.OrderFiles>(item => item.OrderID == this.ID && item.FileType != (int)FileType.OrderFeeFile);
                foreach (var file in this.Files)
                {
                    file.ID = Needs.Overall.PKeySigner.Pick(PKeyType.OrderFile);
                    reponsitory.Insert(file.ToLinq());
                }
                reponsitory.Submit();
            }

            OnSplitted();
        }

        public void OnSplitted()
        {
            if (this != null && this.OrderSplit != null)
            {
                this.OrderSplit(this, new OrderSplitEventArgs(this.Operator, this.OldOrderID, this));
            }

            if (this != null && this.OrderSplitSubmit != null)
            {
                this.OrderSplitSubmit(this, new OrderSplitSubmitEventArgs(this.Operator, this));
            }
        }

        /// <summary>
        /// 判断特殊类型
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_SpecialType(object sender, OrderSplitEventArgs e)
        {
            var orderVoyages = new Needs.Ccs.Services.Views.OrderVoyagesOriginView().Where(item => item.Order.ID == e.OldOrderID).ToList();
            if (orderVoyages.Count > 0)
            {
                SetOrderVoyage(e.OldOrderID);
                SetOrderVoyage(e.Order.ID);
            }
        }

        private void SetOrderVoyage(string orderID)
        {
            OrderItem[] orderItems = new Order() { ID = orderID, }.Items.ToArray();
            OrderItem[] orderItemsHasCategory = orderItems.Where(t => t.Category != null).ToArray();
            Dictionary<OrderSpecialType, ItemCategoryType> dicCheckOrderSpecialType = new Dictionary<OrderSpecialType, ItemCategoryType>();
            dicCheckOrderSpecialType.Add(OrderSpecialType.HighValue, ItemCategoryType.HighValue);
            dicCheckOrderSpecialType.Add(OrderSpecialType.Inspection, ItemCategoryType.Inspection);
            dicCheckOrderSpecialType.Add(OrderSpecialType.Quarantine, ItemCategoryType.Quarantine);
            dicCheckOrderSpecialType.Add(OrderSpecialType.CCC, ItemCategoryType.CCC);

            foreach (var dic in dicCheckOrderSpecialType)
            {
                if (orderItemsHasCategory != null && orderItemsHasCategory.Any())
                {
                    bool isTheType = orderItemsHasCategory.Any(t => (t.Category.Type.GetHashCode() & dic.Value.GetHashCode()) > 0);
                    var orderVoyage = new Needs.Ccs.Services.Models.OrderVoyage();
                    orderVoyage.Order = new Order() { ID = orderID, };
                    orderVoyage.Type = dic.Key;
                    if (isTheType)
                    {
                        orderVoyage.Enter();
                    }
                    else
                    {
                        orderVoyage.Abandon();
                    }
                }
            }
        }

        /// <summary>
        /// 重新生成对账单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_ReGenerateBill(object sender, OrderSplitEventArgs e)
        {
            decimal PointedAgencyFee = 0;
            if (this.OrderBillType == OrderBillType.Pointed)
            {
                var agency = new Needs.Ccs.Services.Views.Origins.OrderPremiumsOrigin().Where(
                                                            t => t.OrderID == this.ID
                                                                && t.Type == OrderPremiumType.AgencyFee
                                                                && t.Status == Status.Normal).FirstOrDefault();
                if (agency != null)
                {
                    PointedAgencyFee = agency.UnitPrice;
                }
            }

            this.GenerateBill(this.OrderBillType, PointedAgencyFee);
            var oldOrder = new Needs.Ccs.Services.Views.OrdersView().Where(item => item.ID == e.OldOrderID).FirstOrDefault();
            if (oldOrder != null)
            {
                oldOrder.GenerateBill(this.OrderBillType, PointedAgencyFee);
            }
        }

        ///// <summary>
        ///// 生成报关通知
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void Order_DeclarationNotice(object sender, OrderSplitEventArgs e)
        //{
        //    var entryNotice = new Needs.Ccs.Services.Views.HKEntryNoticeView().Where(item=>item.ID==e.EntryNoticeID).FirstOrDefault();
        //    if (entryNotice != null)
        //    {
        //        entryNotice.SetAdmin(e.Operator);
        //        entryNotice.Seal("splitorder");
        //    }
        //}

        ///// <summary>
        ///// 记录拆分日志
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void OrderSplit_Log(object sender, OrderSplitEventArgs e)
        //{
        //    var MainOrder = new Order();
        //    MainOrder.ID = e.Order.ID;
        //    MainOrder.Trace(e.Operator, OrderTraceStep.Declaring, "您的订单由"+e.OldOrderID+"拆分而来");
        //    MainOrder.Log(e.Operator, "订单由" + e.OldOrderID + "拆分而来");
        //}

        /// <summary>
        /// 提交结果
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SubmitChanges(object sender, OrderSplitSubmitEventArgs e)
        {
            List<Needs.Ccs.Services.Views.OrderItemChanges> OrderItemChanges = new List<Views.OrderItemChanges>();
            var Orders = new Needs.Ccs.Services.Views.Orders2View().OrderBy(item => item.ID).Where(item => item.MainOrderID == e.Order.MainOrderID && item.OrderStatus != OrderStatus.Canceled && item.OrderStatus != OrderStatus.Returned).ToList();

            foreach (var t in Orders)
            {
                foreach (var item in t.Items)
                {
                    OrderItemChanges.Add(new Needs.Ccs.Services.Views.OrderItemChanges()
                    {
                        OrderItemID = item.ID,                       
                        CustomName = item.Category.Name,
                        Product = new Needs.Ccs.Services.Views.CenterProduct()
                        {
                            PartNumber = item.Model,
                            Manufacturer = item.Manufacturer,
                        },
                        Origin = item.Origin,                       
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice,
                        Unit = item.Unit,
                        TinyOrderID = item.OrderID,
                    });
                }
            }

            string batchID = Guid.NewGuid().ToString("N");
            Needs.Ccs.Services.Views.CurrentOrderInfo currentOrderInfo = new Needs.Ccs.Services.Views.CurrentOrderInfo()
            {
                OrderID = this.MainOrderID,
                Currency = this.Currency,
                Confirmed = false,
                items = OrderItemChanges,
            };

            var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SubmitChanged;

            Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
            {
                ID = Guid.NewGuid().ToString("N"),
                BatchID = batchID,
                Url = apiurl,
                RequestContent = currentOrderInfo.Json(),
                Status = Needs.Ccs.Services.Enums.Status.Normal,
                CreateDate = DateTime.Now,
                UpdateDate = DateTime.Now,
            };
            apiLog.Enter();

            try
            {
                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, currentOrderInfo);
                apiLog.ResponseContent = result;
                apiLog.Enter();
            }
            catch (Exception ex)
            {
                ex.CcsLog("到货通知接口中调用代仓储接口传当前到货信息");
            }
        }

        private void Submit2WareHouse(object sender,OrderSplitSubmitEventArgs e)
        {
            //拆分订单后， 小订单号：splitOrder.ID, InputID：datas[i].InputID, 大订单号：datas[0].MainOrderID
            string batchID = Guid.NewGuid().ToString("N");

            foreach (var item in e.Order.OrderItemInputOrderMap)
            {
                var requestModel = new
                {
                    inputID = item.InputID,
                    itemID = item.OrderItemID,
                    tinyOrderID = item.TinyOrderID,
                };

                var apisetting = new Needs.Ccs.Services.ApiSettings.PfWmsApiSetting();
                var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.UpdateItem;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    Url = apiurl,
                    RequestContent = requestModel.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "拆分订单",
                };
                apiLog.Enter();

                try
                {
                    var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, requestModel);
                    apiLog.ResponseContent = result;
                    apiLog.Enter();
                }
                catch (Exception ex)
                {
                    ex.CcsLog("拆分订单中调用库房接口(/PfWmsApi/UpdateItem)");
                }
            }

        }
    }

    public class SubmitForWarehouse
    {
        public string InputID { get; set; }
        public string OrderItemID { get; set; }
        public string TinyOrderID { get; set; }
    }
}
