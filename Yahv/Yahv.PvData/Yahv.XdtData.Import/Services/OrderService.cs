using Layers.Data.Sqls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Yahv.Services.Enums;
using Yahv.Services.Models;
using Yahv.Services.Views;
using Yahv.Underly;
using Yahv.Utils.Serializers;
using Yahv.XdtData.Import.Connections;
using Yahv.XdtData.Import.Enums;
using Yahv.XdtData.Import.Extends;

namespace Yahv.XdtData.Import.Services
{
    /// <summary>
    /// 订单数据导入
    /// </summary>
    public sealed class OrderService : IDataService
    {
        #region 从芯达通查询的老数据

        Layers.Data.Sqls.PvWsOrder.XdtMainOrdersTopView[] xdtMainOrders;
        Layers.Data.Sqls.PvWsOrder.XdtTinyOrdersTopView[] xdtTinyOrders;
        Layers.Data.Sqls.PvWsOrder.XdtOrderPayExchangeSuppliersView[] xdtPayExchangeSuppliers;
        Layers.Data.Sqls.PvWsOrder.XdtOrderItemsTopView[] xdtOrderItems;

        #endregion

        #region 需要保存的数据

        //订单
        List<Layers.Data.Sqls.PvWsOrder.Orders> orders = new List<Layers.Data.Sqls.PvWsOrder.Orders>();
        List<Layers.Data.Sqls.PvWsOrder.MapsSupplier> mapsSuppliers = new List<Layers.Data.Sqls.PvWsOrder.MapsSupplier>();
        List<Layers.Data.Sqls.PvWsOrder.OrderInputs> orderInputs = new List<Layers.Data.Sqls.PvWsOrder.OrderInputs>();
        List<Layers.Data.Sqls.PvWsOrder.OrderOutputs> orderOutputs = new List<Layers.Data.Sqls.PvWsOrder.OrderOutputs>();
        List<Layers.Data.Sqls.PvWsOrder.OrderItems> orderItems = new List<Layers.Data.Sqls.PvWsOrder.OrderItems>();
        List<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd> orderItemsChcd = new List<Layers.Data.Sqls.PvWsOrder.OrderItemsChcd>();
        List<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm> orderItemsTerm = new List<Layers.Data.Sqls.PvWsOrder.OrderItemsTerm>();

        //中心数据
        Dictionary<string, Layers.Data.Sqls.PvData.Products> products = new Dictionary<string, Layers.Data.Sqls.PvData.Products>();
        Dictionary<string, Layers.Data.Sqls.PvData.ClassifiedPartNumbers> cpns = new Dictionary<string, Layers.Data.Sqls.PvData.ClassifiedPartNumbers>();

        //运单
        Dictionary<string, Layers.Data.Sqls.PvCenter.WayParters> wayParters = new Dictionary<string, Layers.Data.Sqls.PvCenter.WayParters>();
        List<Layers.Data.Sqls.PvCenter.Waybills> waybills = new List<Layers.Data.Sqls.PvCenter.Waybills>();
        List<Layers.Data.Sqls.PvCenter.WayLoadings> wayLoadings = new List<Layers.Data.Sqls.PvCenter.WayLoadings>();

        //日志
        List<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder> logsOrder = new List<Layers.Data.Sqls.PvCenter.Logs_PvWsOrder>();
        List<Layers.Data.Sqls.PvCenter.Logs_Waybills> logsWaybill = new List<Layers.Data.Sqls.PvCenter.Logs_Waybills>();

        #endregion

        private string[] xdtMainOrderIDs;

        public OrderService(params string[] mainOrderIDs)
        {
            this.xdtMainOrderIDs = mainOrderIDs;
        }

        /// <summary>
        /// 数据查询
        /// </summary>
        public IDataService Query()
        {
            using (var reponsitory = new PvWsOrderReponsitory())
            {
                //芯达通主订单
                xdtMainOrders = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.XdtMainOrdersTopView>()
                    .Where(item => this.xdtMainOrderIDs.Contains(item.ID)).ToArray();

                //主订单下的子订单
                xdtTinyOrders = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.XdtTinyOrdersTopView>()
                    .Where(item => this.xdtMainOrderIDs.Contains(item.VastOrderID)).ToArray();
                var tinyOrderIDs = xdtTinyOrders.Select(item => item.TinyOrderID).ToArray();

                //订单的付汇供应商
                xdtPayExchangeSuppliers = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.XdtOrderPayExchangeSuppliersView>()
                    .Where(item => tinyOrderIDs.Contains(item.OrderID)).ToArray();

                //订单项
                xdtOrderItems = reponsitory.ReadTable<Layers.Data.Sqls.PvWsOrder.XdtOrderItemsTopView>()
                    .Where(item => tinyOrderIDs.Contains(item.TinyOrderID)).ToArray();
            }

            return this;
        }

        /// <summary>
        /// 数据封装
        /// </summary>
        public IDataService Encapsule()
        {
            Purchaser purchaser = PurchaserContext.Current;
            Vendor vendor = VendorContext.Current;
            string declareCompany = System.Configuration.ConfigurationManager.AppSettings["DeclareCompany"];
            List<Layers.Data.Sqls.PvbErm.Admins> admins;

            using (var reponsitory = new PvbErmReponsitory())
            {
                admins = reponsitory.ReadTable<Layers.Data.Sqls.PvbErm.Admins>().ToList();
            }

            using (var reponsitory = new PvWsOrderReponsitory())
            {
                
                var company = new CompaniesTopView<PvWsOrderReponsitory>(reponsitory).SingleOrDefault(item => item.Name == declareCompany); //平台公司
                //var companyContact = new ContactsTopView<PvWsOrderReponsitory>(reponsitory).FirstOrDefault(item => item.EnterpriseID == company.ID); //公司联系人
                var suppliersView = new WsSuppliersTopView<PvWsOrderReponsitory>(reponsitory); //客户供应商

                foreach (var currMainOrder in xdtMainOrders)
                {
                    string clientName = System.Configuration.ConfigurationManager.AppSettings[currMainOrder.ClientID];
                    var client = new WsClientsTopView<PvWsOrderReponsitory>(reponsitory).FirstOrDefault(item => item.Name == clientName); //客户
                    var invoice = new InvoicesTopView<PvWsOrderReponsitory>(Business.WarehouseServicing).SingleOrDefault(item => item.EnterpriseID == client.ID && (int)item.Status == 200); //客户发票

                    var currTinyOrders = xdtTinyOrders.Where(item => item.VastOrderID == currMainOrder.ID).ToList();
                    var currTinyOrder = currTinyOrders.First();
                    var currTinyOrderIDs = currTinyOrders.Select(item => item.TinyOrderID).ToArray();
                    var currPayExchangeSuppliers = xdtPayExchangeSuppliers.Where(item => currTinyOrderIDs.Contains(item.OrderID)).ToList();
                    var currPayExchangeSupplierNames = currPayExchangeSuppliers.Select(item => item.Name).ToArray();
                    var currOrderItems = xdtOrderItems.Where(item => currTinyOrderIDs.Contains(item.TinyOrderID)).ToList();
                    var adminID = admins.SingleOrDefault(item => item.OriginID == currMainOrder.AdminID).ID;
                    var currency = (Currency)Enum.Parse(typeof(Currency), currTinyOrder.Currency);

                    #region 订单

                    var order = new Layers.Data.Sqls.PvWsOrder.Orders();
                    order.ID = currMainOrder.ID;
                    order.Type = (int)OrderType.Declare;
                    order.ClientID = client.ID;
                    order.InvoiceID = invoice.ID;
                    order.PayeeID = company.ID;
                    order.CreateDate = currMainOrder.CreateDate;
                    order.ModifyDate = currMainOrder.UpdateDate;
                    order.CreatorID = adminID;
                    order.SupplierID = suppliersView.SingleOrDefault(item => item.ClientID == client.ID && item.Name == currTinyOrder.supplierName)?.ID;
                    order.SettlementCurrency = (int)Currency.CNY;
                    orders.Add(order);

                    #endregion

                    #region 订单日志

                    //主状态日志
                    var mainLog = new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = order.ID,
                        CreatorID = adminID,
                        Type = (int)OrderStatusType.MainStatus,
                        CreateDate = order.CreateDate,
                        Status = (int)CgOrderStatus.客户已收货,
                        IsCurrent = true
                    };
                    logsOrder.Add(mainLog);

                    //支付状态日志
                    var paymentLog = new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = order.ID,
                        CreatorID = adminID,
                        Type = (int)OrderStatusType.PaymentStatus,
                        CreateDate = order.CreateDate,
                        Status = (int)OrderPaymentStatus.Paid,
                        IsCurrent = true
                    };
                    logsOrder.Add(paymentLog);

                    //开票状态日志
                    var invoiceLog = new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = order.ID,
                        CreatorID = adminID,
                        Type = (int)OrderStatusType.InvoiceStatus,
                        CreateDate = order.CreateDate,
                        Status = (int)OrderInvoiceStatus.Invoiced,
                        IsCurrent = true
                    };
                    logsOrder.Add(invoiceLog);

                    //付汇状态日志
                    var remittanceLog = new Layers.Data.Sqls.PvCenter.Logs_PvWsOrder()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = order.ID,
                        CreatorID = adminID,
                        Type = (int)OrderStatusType.RemittanceStatus,
                        CreateDate = order.CreateDate,
                        Status = (int)OrderRemittanceStatus.Remittanced,
                        IsCurrent = true
                    };
                    logsOrder.Add(remittanceLog);

                    #endregion

                    #region 付汇供应商

                    var supplierList = (suppliersView.Where(item => currPayExchangeSupplierNames.Contains(item.Name))).ToArray();
                    mapsSuppliers.AddRange((supplierList.Select(item => new Layers.Data.Sqls.PvWsOrder.MapsSupplier
                    {
                        OrderID = order.ID,
                        SupplierID = item.ID
                    })).ToList());

                    #endregion

                    #region 香港入库运单

                    //收货人
                    var hkConsignee = new Layers.Data.Sqls.PvCenter.WayParters()
                    {
                        Company = vendor.OverseasConsignorCname,
                        Place = Origin.HKG.ToString(),
                        Address = vendor.Address,
                        Contact = vendor.Contact,
                        Phone = vendor.Tel,
                        Email = string.Empty,
                        CreateDate = currTinyOrder.ConsigneeCreateDate,
                    };
                    hkConsignee.ID = hkConsignee.GenID();
                    if (!wayParters.ContainsKey(hkConsignee.ID))
                    {
                        wayParters.Add(hkConsignee.ID, hkConsignee);
                    }

                    //交货人
                    var hkConsignor = new Layers.Data.Sqls.PvCenter.WayParters()
                    {
                        Company = currTinyOrder.supplierName,
                        Place = string.Empty,
                        Address = string.Empty,
                        Contact = string.Empty,
                        Phone = string.Empty,
                        Email = string.Empty,
                        CreateDate = currTinyOrder.ConsigneeCreateDate
                    };
                    hkConsignor.ID = hkConsignor.GenID();
                    if (!wayParters.ContainsKey(hkConsignor.ID))
                    {
                        wayParters.Add(hkConsignor.ID, hkConsignor);
                    }

                    //运单
                    var hkWaybill = new Layers.Data.Sqls.PvCenter.Waybills()
                    {
                        ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill),
                        Code = currTinyOrder.ConsigneeWayBillNo,
                        Type = currTinyOrder.ConsigneeType == 1 ? (int)WaybillType.DeliveryToWarehouse : (int)WaybillType.PickUp,
                        ConsignorID = hkConsignor.ID,
                        ConsigneeID = hkConsignee.ID,
                        FreightPayer = currTinyOrder.ConsigneeType == 1 ? (int)WaybillPayer.Consignor : (int)WaybillPayer.Consignee,
                        Condition = new WayCondition().Json(),
                        CreateDate = currTinyOrder.ConsigneeCreateDate,
                        ModifyDate = currTinyOrder.ConsigneeUpdateDate,
                        EnterCode = client.EnterCode,
                        Status = (int)GeneralStatus.Normal,
                        CreatorID = adminID,
                        IsClearance = false,
                        Supplier = currTinyOrder.supplierName,
                        ExcuteStatus = (int)SortingExcuteStatus.Stocked,
                        OrderID = order.ID,
                        Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                        NoticeType = (int)CgNoticeType.Enter,
                    };
                    waybills.Add(hkWaybill);

                    //提货单
                    if (hkWaybill.Type == (int)WaybillType.PickUp)
                    {
                        var hkWayLoading = new Layers.Data.Sqls.PvCenter.WayLoadings()
                        {
                            ID = hkWaybill.ID,
                            TakingDate = currTinyOrder.ConsigneePickUpTime,
                            TakingAddress = currTinyOrder.ConsigneeAddress,
                            TakingContact = currTinyOrder.ConsigneeContact,
                            TakingPhone = currTinyOrder.ConsigneeMobile ?? currTinyOrder.ConsigneeTel,
                            CreateDate = currTinyOrder.ConsigneeCreateDate,
                            ModifyDate = currTinyOrder.ConsigneeUpdateDate,
                            CreatorID = adminID,
                            ModifierID = adminID,
                        };
                        wayLoadings.Add(hkWayLoading);
                    }

                    //日志
                    var hkLogWaybill = new Layers.Data.Sqls.PvCenter.Logs_Waybills()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = hkWaybill.ID,
                        Type = (int)CgSortingExcuteStatus.Completed,
                        Status = hkWaybill.ExcuteStatus.Value,
                        CreateDate = hkWaybill.CreateDate,
                        CreatorID = hkWaybill.CreatorID,
                        IsCurrent = true,
                    };
                    logsWaybill.Add(hkLogWaybill);

                    #endregion

                    #region 深圳出库运单

                    //收货人
                    var szConsignee = new Layers.Data.Sqls.PvCenter.WayParters()
                    {
                        Company = currTinyOrder.ConsignorName,
                        Place = Origin.CHN.ToString(),
                        Address = currTinyOrder.ConsignorAddress,
                        Contact = currTinyOrder.ConsignorContact,
                        Phone = currTinyOrder.ConsignorMobile ?? currTinyOrder.ConsignorTel,
                        CreateDate = currTinyOrder.ConsignorCreateDate
                    };
                    szConsignee.ID = szConsignee.GenID();
                    if (!wayParters.ContainsKey(szConsignee.ID))
                    {
                        wayParters.Add(szConsignee.ID, szConsignee);
                    }

                    //交货人
                    var szConsignor = new Layers.Data.Sqls.PvCenter.WayParters()
                    {
                        Company = company.Name,
                        Place = Origin.CHN.ToString(),
                        Address = purchaser.Address,
                        Contact = purchaser.Contact,
                        Phone = purchaser.Tel,
                        Email = string.Empty,
                        CreateDate = currTinyOrder.ConsignorCreateDate
                    };
                    szConsignor.ID = szConsignor.GenID();
                    if (!wayParters.ContainsKey(szConsignor.ID))
                    {
                        wayParters.Add(szConsignor.ID, szConsignor);
                    }

                    //运单
                    var szWaybill = new Layers.Data.Sqls.PvCenter.Waybills()
                    {
                        ID = Layers.Data.PKeySigner.Pick(PKeyType.Waybill),
                        Type = currTinyOrder.ConsignorType,
                        ConsignorID = szConsignor.ID,
                        ConsigneeID = szConsignee.ID,
                        FreightPayer = currTinyOrder.ConsignorType == 1 ? (int)WaybillPayer.Consignee : (int)WaybillPayer.Consignor,
                        Condition = new WayCondition().Json(),
                        CreateDate = currTinyOrder.ConsignorCreateDate,
                        ModifyDate = currTinyOrder.ConsignorUpdateDate,
                        EnterCode = client.EnterCode,
                        Status = (int)GeneralStatus.Normal,
                        CreatorID = adminID,
                        IsClearance = false,
                        ExcuteStatus = (int)PickingExcuteStatus.OutStock,
                        OrderID = order.ID,
                        Source = (int)CgNoticeSource.AgentBreakCustomsForIns,
                        NoticeType = (int)CgNoticeType.Out,
                    };
                    waybills.Add(szWaybill);

                    //日志
                    var szLogWaybill = new Layers.Data.Sqls.PvCenter.Logs_Waybills()
                    {
                        ID = Guid.NewGuid().ToString(),
                        MainID = szWaybill.ID,
                        Type = (int)CgPickingExcuteStatus.Completed,
                        Status = szWaybill.ExcuteStatus.Value,
                        CreateDate = szWaybill.CreateDate,
                        CreatorID = szWaybill.CreatorID,
                        IsCurrent = true,
                    };
                    logsWaybill.Add(szLogWaybill);

                    #endregion

                    #region 进项

                    var orderInput = new Layers.Data.Sqls.PvWsOrder.OrderInputs()
                    {
                        ID = order.ID,
                        WayBillID = hkWaybill.ID,
                        Currency = (int)currency,
                    };
                    orderInputs.Add(orderInput);

                    #endregion

                    #region 销项

                    var orderOutput = new Layers.Data.Sqls.PvWsOrder.OrderOutputs()
                    {
                        ID = order.ID,
                        WayBillID = szWaybill.ID,
                        Currency = (int)currency,
                    };
                    orderOutputs.Add(orderOutput);

                    #endregion

                    #region 订单项

                    foreach (var currOrderItem in currOrderItems)
                    {
                        //标准产品
                        var product = new Layers.Data.Sqls.PvData.Products()
                        {
                            PartNumber = currOrderItem.Model,
                            Manufacturer = currOrderItem.Manufacturer,
                            CreateDate = currOrderItem.CreateDate,
                        };
                        product.ID = product.GenID();
                        if (!products.ContainsKey(product.ID))
                        {
                            products.Add(product.ID, product);
                        }

                        //订单项
                        var orderItem = new Layers.Data.Sqls.PvWsOrder.OrderItems()
                        {
                            ID = currOrderItem.ID,
                            OrderID = order.ID,
                            TinyOrderID = currOrderItem.TinyOrderID,
                            InputID = currOrderItem.ID,
                            ProductID = product.ID,
                            CustomName = currOrderItem.ProductName,
                            Origin = currOrderItem.Origin,
                            DateCode = currOrderItem.Batch,
                            Quantity = currOrderItem.Quantity,
                            Currency = (int)currency,
                            UnitPrice = currOrderItem.UnitPrice,
                            Unit = int.Parse(currOrderItem.Unit),
                            TotalPrice = currOrderItem.TotalPrice,
                            CreateDate = currOrderItem.CreateDate,
                            ModifyDate = currOrderItem.UpdateDate,
                            GrossWeight = currOrderItem.GrossWeight,
                            Status = (int)GeneralStatus.Normal,
                            IsAuto = true,
                            Type = 1, //正常下单
                        };
                        orderItems.Add(orderItem);

                        //归类信息
                        var cpn = new Layers.Data.Sqls.PvData.ClassifiedPartNumbers()
                        {
                            PartNumber = currOrderItem.Model,
                            Manufacturer = currOrderItem.Manufacturer,
                            HSCode = currOrderItem.HSCode,
                            Name = currOrderItem.ProductName,
                            LegalUnit1 = currOrderItem.FirstLegalUnit,
                            LegalUnit2 = currOrderItem.SecondLegalUnit,

                            VATRate = currOrderItem.ValueAddedRate,
                            ImportPreferentialTaxRate = currOrderItem.TariffRate,
                            ExciseTaxRate = 0,
                            Elements = currOrderItem.Elements,

                            SupervisionRequirements = null,
                            CIQC = null,

                            CIQCode = currOrderItem.CIQCode,
                            TaxCode = currOrderItem.TaxCode,
                            TaxName = currOrderItem.TaxName,
                            CreateDate = currOrderItem.ClassifyCreateDate,
                            OrderDate = currOrderItem.ClassifyUpdateDate
                        };
                        cpn.ID = cpn.GenID();
                        if (!cpns.ContainsKey(cpn.ID))
                        {
                            cpns.Add(cpn.ID, cpn);
                        }

                        //Chcd
                        var chcd = new Layers.Data.Sqls.PvWsOrder.OrderItemsChcd()
                        {
                            ID = currOrderItem.ID,
                            SecondAdminID = admins.SingleOrDefault(a => a.OriginID == currOrderItem.ClassifySecondOperator).ID,
                            SecondHSCodeID = cpn.ID,
                            SecondDate = currOrderItem.ClassifyUpdateDate,
                            CreateDate = currOrderItem.ClassifyCreateDate,
                            ModifyDate = currOrderItem.ClassifyUpdateDate
                        };
                        orderItemsChcd.Add(chcd);

                        //Term
                        var term = new Layers.Data.Sqls.PvWsOrder.OrderItemsTerm()
                        {
                            ID = currOrderItem.ID,
                            OriginRate = 0M,
                            FVARate = 0.002M,
                            Ccc = (currOrderItem.Type & (int)ItemCategoryType.CCC) > 0,
                            Embargo = (currOrderItem.Type & (int)ItemCategoryType.Forbid) > 0,
                            HkControl = (currOrderItem.Type & (int)ItemCategoryType.HKForbid) > 0,
                            Coo = (currOrderItem.Type & (int)ItemCategoryType.OriginProof) > 0,
                            CIQ = (currOrderItem.Type & (int)ItemCategoryType.Inspection) > 0,
                            CIQprice = currOrderItem.InspFee.GetValueOrDefault(),
                            IsHighPrice = (currOrderItem.Type & (int)ItemCategoryType.HighValue) > 0,
                            IsDisinfected = (currOrderItem.Type & (int)ItemCategoryType.Quarantine) > 0,
                        };
                        orderItemsTerm.Add(term);
                    }

                    #endregion
                }
            }

            return this;
        }

        /// <summary>
        /// 数据持久化
        /// </summary>
        public void Enter()
        {
            #region 中心数据

            List<Layers.Data.Sqls.PvData.Products> productList;
            List<Layers.Data.Sqls.PvData.ClassifiedPartNumbers> cpnList;
            using (var reponsitory = new PvDataReponsitory())
            {
                var existedIDs = reponsitory.ReadTable<Layers.Data.Sqls.PvData.Products>().Where(item => this.products.Keys.Contains(item.ID))
                                            .Select(item => item.ID).ToArray();
                productList = this.products.Where(item => !existedIDs.Contains(item.Key)).Select(item => item.Value).ToList();

                existedIDs = reponsitory.ReadTable<Layers.Data.Sqls.PvData.ClassifiedPartNumbers>().Where(item => this.cpns.Keys.Contains(item.ID))
                                        .Select(item => item.ID).ToArray();
                cpnList = this.cpns.Where(item => !existedIDs.Contains(item.Key)).Select(item => item.Value).ToList();
            }

            using (var conn = ConnManager.Current.PvData)
            {
                if (productList.Count > 0)
                    conn.BulkInsert(productList);
                if (cpnList.Count > 0)
                    conn.BulkInsert(cpnList);
            }

            #endregion

            #region 运单收发货人

            using (var reponsitory = new PvCenterReponsitory())
            {
                foreach (var parter in this.wayParters.Values)
                {
                    if (reponsitory.ReadTable<Layers.Data.Sqls.PvCenter.WayParters>().Any(item => item.ID == parter.ID))
                        continue;
                    reponsitory.Insert(parter);
                }
            }

            #endregion

            #region 运单

            using (var conn = ConnManager.Current.PvCenter)
            {
                conn.BulkInsert(this.waybills);
                conn.BulkInsert(this.wayLoadings);
                conn.BulkInsert(this.logsWaybill);
                conn.BulkInsert(this.logsOrder);
            }

            #endregion

            #region 订单

            using (var conn = ConnManager.Current.PvWsOrder)
            {
                conn.BulkInsert(this.orders);
                conn.BulkInsert(this.mapsSuppliers);
                conn.BulkInsert(this.orderInputs);
                conn.BulkInsert(this.orderOutputs);
                conn.BulkInsert(this.orderItems);
                conn.BulkInsert(this.orderItemsChcd);
                conn.BulkInsert(this.orderItemsTerm);
            }

            #endregion

            //task.Wait();
        }
    }
}
