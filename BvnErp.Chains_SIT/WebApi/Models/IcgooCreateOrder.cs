using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http.Controllers;

namespace WebApi.Models
{
    [Obsolete("废弃,用消息队列的方式生成订单")]
    public class IcgooCreateOrder
    {

        public IcgooCreateOrder()
        {
            IcgooGenerateOrderBill += CreateOrderBill;
            IcgooQuote += Quote;
            IcgooQuoteConfirm += QuoteConfirm;
            IcgooPacking += Packing;
            IcgooSeal += Seal;
            IcgooPreQuoteConfrirm += ToEntryNotice;
        }

        public event IcgooGenerateOrderBillHanlder IcgooGenerateOrderBill;
        public event IcgooQuoteHandler IcgooQuote;
        public event IcgooQuoteConfirmHandler IcgooQuoteConfirm;
        public event IcgooQuoteConfirmHandler IcgooPreQuoteConfrirm;
        public event IcgooPackingHandler IcgooPacking;
        public event IcgooSealHandler IcgooSeal;

        public bool Create(List<PartNoReceiveItem> model, string currency, ref string msg)
        {
            bool isSuccess = true;
            try
            {
                //客户
                string clientID = "";
                var clientView = new Needs.Ccs.Services.Views.ClientsView();
                var client = clientView[clientID];

                IcgooOrder order = new IcgooOrder();
                order.Client = client;
                //香港交货方式：Icgoo 上门自提
                order.OrderConsignee = new OrderConsignee();
                order.OrderConsignee.OrderID = order.ID;
                order.OrderConsignee.PickUpTime = DateTime.Now.AddDays(1);
                //国内交货方式：Icgoo 送货上门
                order.OrderConsignor = new OrderConsignor();
                order.OrderConsignor.OrderID = order.ID;

                order.Type = OrderType.Icgoo;
                order.SetAPIAdmin(Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator));
                order.AdminID = client.Merchandiser.ID;
                order.ClientAgreement = client.Agreement;


                order.Currency = currency;
                order.IsFullVehicle = false;
                order.IsLoan = false;
                order.PackNo = model.Select(item => item.orgCarton).Distinct().Count();
                order.WarpType = "";

                string supplierID = "";
                //客户供应商
                var supplier = client.Suppliers[supplierID];
                var supplierAddress = supplier.Addresses.Where(item => item.IsDefault == true).FirstOrDefault();
                //香港交货信息
                order.OrderConsignee.ClientSupplier = supplier;
                order.OrderConsignee.Type = HKDeliveryType.PickUp;
                order.OrderConsignee.Contact = supplierAddress.Contact.Name;
                order.OrderConsignee.Mobile = supplierAddress.Contact.Mobile;
                order.OrderConsignee.Address = supplierAddress.Address;

                //客户收货地址
                var clientConsignees = new Needs.Ccs.Services.Views.ClientConsigneesView();
                var defaultConsignee = clientConsignees.Where(item => item.IsDefault == true).FirstOrDefault();
                //国内交货信息
                order.OrderConsignor.Type = SZDeliveryType.SentToClient;
                order.OrderConsignor.Name = defaultConsignee.Name;
                order.OrderConsignor.Contact = defaultConsignee.Contact.Name;
                order.OrderConsignor.Mobile = defaultConsignee.Contact.Mobile;
                order.OrderConsignor.Address = defaultConsignee.Address;


                bool isAllClassified = true;
                order.ClassifyProducts = new List<ClassifyProduct>();
                //组装ClassifyProducts
                using (Needs.Ccs.Services.Views.ClassifyResultView view = new Needs.Ccs.Services.Views.ClassifyResultView())
                {
                    var DoneClassifyView = view.Where(item => item.ClassifyStatus == ClassifyStatus.Done);

                    foreach (PartNoReceiveItem p in model)
                    {
                        ClassifyProduct classifyProduct = new ClassifyProduct();
                        classifyProduct.Client = order.Client;
                        classifyProduct.Currency = currency;
                        classifyProduct.OrderID = order.ID;


                        classifyProduct.Origin = p.origin;
                        classifyProduct.Quantity = p.qty;
                        classifyProduct.Unit = p.unit;
                        classifyProduct.UnitPrice = p.price;
                        classifyProduct.TotalPrice = p.qty * p.price;
                        classifyProduct.GrossWeight = p.gw / 1000;

                        //归类结果
                        ClassifyResult classifyResult = DoneClassifyView.Where(item => item.PreProduct.sale_orderline_id == p.sale_orderline_id).FirstOrDefault();
                        if (classifyResult != null)
                        {
                            var categoryType = ItemCategoryType.Normal;
                            switch (classifyResult.ClassifyType)
                            {
                                case IcgooClassifyTypeEnums.Normal:
                                    break;

                                case IcgooClassifyTypeEnums.Inspection:
                                    classifyProduct.IsInsp = true;
                                    classifyProduct.InspectionFee = classifyResult.InspectionFee.Value;
                                    categoryType = ItemCategoryType.Inspection;
                                    break;

                                case IcgooClassifyTypeEnums.CCC:
                                    classifyProduct.IsCCC = true;
                                    categoryType = ItemCategoryType.CCC;
                                    break;

                                case IcgooClassifyTypeEnums.Embargo:
                                    classifyProduct.IsSysForbid = true;
                                    categoryType = ItemCategoryType.Forbid;
                                    break;

                                default:
                                    break;
                            }

                            if (classifyProduct.IsCCC || classifyProduct.IsSysForbid)
                            {
                                isSuccess = false;
                                msg = "不能提交3C或者禁运产品";
                                return isSuccess;
                            }

                            //var product = new Product
                            //{
                            //    Name = classifyResult.ProductName,
                            //    Model = classifyResult.Model,
                            //    Manufacturer = classifyResult.Manufacturer,
                            //};
                            //classifyProduct.Product = product;
                            classifyProduct.Name = classifyResult.ProductName;
                            classifyProduct.Model = classifyResult.Model;
                            classifyProduct.Manufacturer = classifyResult.Manufacturer;
                            classifyProduct.ClassifyStatus = ClassifyStatus.Done;

                            OrderItemCategory Category = new OrderItemCategory();
                            Category.Declarant = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.ClassifySecondOperator.ID);
                            Category.Type = categoryType;
                            Category.TaxCode = classifyResult.TaxCode;
                            Category.TaxName = classifyResult.TaxName;
                            Category.HSCode = classifyResult.HSCode;
                            Category.Name = classifyResult.ProductName;
                            Category.Elements = classifyResult.Elements;
                            Category.Unit1 = classifyResult.Unit1;
                            Category.Unit2 = classifyResult.Unit2;
                            Category.CIQCode = classifyResult.CIQCode;

                            classifyProduct.Category = Category;

                            OrderItemTax tariff = new OrderItemTax();
                            tariff.Type = CustomsRateType.ImportTax;
                            var tariffView = new Needs.Ccs.Services.Views.CustomsOriginTariffsView();
                            //确定他们提供的Origin 是什么，是USA 还是America
                            var reTariff = tariffView.Where(item => item.Type == CustomsRateType.ImportTax && item.Origin == p.origin).FirstOrDefault();
                            tariff.Rate = reTariff?.Rate / 100 ?? classifyResult.TariffRate.Value;
                            classifyProduct.ImportTax = tariff;

                            OrderItemTax ValueAddedTax = new OrderItemTax();
                            ValueAddedTax.Type = CustomsRateType.AddedValueTax;
                            ValueAddedTax.Rate = classifyResult.AddedValueRate.Value;
                            classifyProduct.AddedValueTax = ValueAddedTax;


                        }
                        else
                        {
                            //var product = new Product
                            //{
                            //    Name = p.pn,
                            //    Model = p.pn,
                            //    Manufacturer = p.mfr,
                            //};
                            //classifyProduct.Product = product;
                            classifyProduct.Name = p.pn;
                            classifyProduct.Model = p.pn;
                            classifyProduct.Manufacturer = p.mfr;
                            classifyProduct.ClassifyStatus = ClassifyStatus.Unclassified;
                            isAllClassified = false;
                        }

                        classifyProduct.OrderType = OrderType.Icgoo;
                        order.ClassifyProducts.Add(classifyProduct);
                    }
                }

                if (isAllClassified)
                {
                    order.OrderStatus = OrderStatus.Classified;
                    order.Enter();
                    //this.OnCreated(new IcgooCreateOrderEventArgs(order, model));
                }
                else
                {
                    order.OrderStatus = OrderStatus.Confirmed;
                    order.Enter();
                    //this.OnPartCreated(new IcgooCreateOrderEventArgs(order, model));
                }

                msg = "订单处理成功";
                return isSuccess;

            }
            catch (Exception ex)
            {
                isSuccess = false;
                msg = "逻辑出错!";
                return isSuccess;
            }

        }

        public virtual void OnCreated(IcgooCreateOrderEventArgs args)
        {
            this.IcgooGenerateOrderBill?.Invoke(this, args);
            this.IcgooQuote?.Invoke(this, args);
            this.IcgooQuoteConfirm?.Invoke(this, args);
            this.IcgooPacking?.Invoke(this, args);
            this.IcgooSeal?.Invoke(this, args);
        }

        public virtual void OnPartCreated(IcgooCreateOrderEventArgs args)
        {
            this.IcgooPreQuoteConfrirm?.Invoke(this, args);
            this.IcgooPacking?.Invoke(this, args);
            this.IcgooSeal?.Invoke(this, args);
        }

        /// <summary>
        /// 生成对账单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CreateOrderBill(object sender, IcgooCreateOrderEventArgs e)
        {
            e.order.GenerateBill();
        }

        /// <summary>
        /// 报价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Quote(object sender, IcgooCreateOrderEventArgs e)
        {
            ClassifiedOrder order = new ClassifiedOrder();
            order.ID = e.order.ID;
            order.Quote();
        }

        /// <summary>
        /// 客户确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void QuoteConfirm(object sender, IcgooCreateOrderEventArgs e)
        {
            var quotedOrder = new Needs.Ccs.Services.Views.QuotedOrdersView()[e.order.ID];
            if (quotedOrder != null)
            {
                quotedOrder.QuoteConfirm();
            }
        }

        /// <summary>
        /// 有没有归类的型号，先生成香港仓库的入库通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ToEntryNotice(object sender, IcgooCreateOrderEventArgs e)
        {
            var order = new Needs.Ccs.Services.Views.OrdersView()[e.order.ID];
            if (order != null)
            {
                order.ToEntryNotice();
            }
        }

        /// <summary>
        /// 装箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Packing(object sender, IcgooCreateOrderEventArgs e)
        {
            var EnterNoticeId = new Needs.Ccs.Services.Views.EntryNoticeView().Where(item => item.Order.ID == e.order.ID).Select(item => item.ID).FirstOrDefault();
            string OrderId = e.order.ID;
            var Boxes = e.partno.Select(item => item.orgCarton).Distinct().ToList();
            HKSortingContext hkSorting;
            List<PartNoReceiveItem> ModelItems = e.partno;
            Boxes.ForEach(b =>
            {
                var Products = ModelItems.Where(item => item.orgCarton == b).ToList();
                hkSorting = new HKSortingContext();
                hkSorting.ToShelve(Icgoo.ShelveNumber, b.ToString());

                //创建packing对象
                PackingModel packing = new PackingModel();
                packing.AdminID = Icgoo.DefaultCreator;
                packing.OrderID = OrderId;
                packing.BoxIndex = b.ToString();
                packing.Weight = Products.Select(item => item.gw).Sum() / 1000;
                packing.WrapType = Icgoo.WrapType;
                packing.PackingDate = DateTime.Now;
                packing.Quantity = Products.Select(item => item.qty).Sum();
                hkSorting.SetPacking(packing);

                List<InsideSortingModel> list = new List<InsideSortingModel>();

                Products.ForEach(p =>
                {
                    var orderitem = e.order.Items.Where(item => item.Model == p.pn && item.Quantity == p.qty).FirstOrDefault();
                    if (orderitem != null)
                    {
                        InsideSortingModel model = new InsideSortingModel();
                        model.EntryNoticeItemID = EnterNoticeId;
                        //model.ProductID = orderitem.Product.ID;
                        model.OrderItemID = orderitem.ID;
                        model.Quantity = p.qty;
                        model.NetWeight = p.nw;
                        model.GrossWeight = p.gw;
                        list.Add(model);
                    }
                });

                hkSorting.Items = list;
                hkSorting.Pack();
            });
        }

        /// <summary>
        /// 封箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Seal(object sender, IcgooCreateOrderEventArgs e)
        {
            var EnterNoticeId = new Needs.Ccs.Services.Views.EntryNoticeView().Where(item => item.Order.ID == e.order.ID).Select(item => item.ID).FirstOrDefault();
            var entryNotice = new Needs.Ccs.Services.Views.HKEntryNoticeView()[EnterNoticeId];
            entryNotice.Seal();
        }
    }
}