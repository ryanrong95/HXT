using Layer.Data.Sqls.BvnErp;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace Wl.IcgooMQ
{
    public class IcgooCreateOrderSpeed
    {
        public class IcgooPackingModel
        {
            public int AllCount { get; set; }

            public List<string> boxes { get; set; }

            public IcgooPackingModel()
            {
                boxes = new List<string>();
            }
        }
        public string ConnectionString { get; set; }
        public string IcgooPayExchangeSupplier { get; set; }
        public event IcgooGenerateOrderBillHanlder IcgooGenerateOrderBill;
        public event IcgooQuoteHandler IcgooQuote;
        public event IcgooQuoteConfirmHandler IcgooQuoteConfirm;
        public event IcgooQuoteConfirmHandler IcgooPreQuoteConfrirm;
        public event IcgooPackingHandler IcgooPacking;
        public event IcgooSealHandler IcgooSeal;
        public event IcgooGenerateOrderBillHanlder IcgooShouldReceive;

        public IcgooCreateOrderSpeed()
        {
            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ScCustomsConnectionString"].ConnectionString;      
            IcgooGenerateOrderBill += CreateOrderBill;
            IcgooQuote += Quote;
            IcgooQuoteConfirm += QuoteConfirm;
            IcgooPacking += Packing;
            IcgooSeal += Seal;
            IcgooPreQuoteConfrirm += ToEntryNotice;
            IcgooShouldReceive += ShouldReceive;
        }

        /// <summary>
        /// 消息队列，根据LogID取到报文，进行下单
        /// </summary>
        /// <param name="ID"></param>
        public void Create(string ID)
        {
            var message = new Needs.Ccs.Services.Views.MessageView()[ID];           
            if (message != null)
            {
                this.IcgooPayExchangeSupplier = System.Configuration.ConfigurationManager.AppSettings[message.Summary];
                PartNoReceive model = JsonConvert.DeserializeObject<PartNoReceive>(message.PostData);

                #region 50个一装箱,不考虑箱号
                //var modelByBox = model.info.OrderBy(item => item.orgCarton).ToList();
                //var limit = 50;
                //for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(modelByBox.Count / 50M)); i++)
                //{
                //    var mc = modelByBox.Skip(i * limit).Take(limit).ToList();
                //    string orderID = "";
                //    Create(mc, model.currency, message.Summary, ref orderID);
                //    Map(model.refNo, orderID);
                //}
                #endregion

                #region 循环箱号排序
                //if (model.info.Count < 50)
                //{
                //    string orderID = "";
                //    //Create(modelByBox, model.currency,message.Summary,ref orderID);
                //    //Map(model.refNo, orderID);
                //}
                //else
                //{
                //    List<int> boxs = new List<int>();
                //    int allcount = 0;
                //    var caseNos = model.info.Select(item => item.orgCarton).Distinct().ToList();
                //    foreach (var caseNo in caseNos)
                //    {
                //        allcount += model.info.Where(item => item.orgCarton == caseNo).Count();
                //        boxs.Add(caseNo);
                //        if (allcount > 50)
                //        {
                //            boxs.Remove(caseNo);

                //            var ProcessModel = model.info.Where(item => boxs.Contains(item.orgCarton)).ToList();
                //            string orderID = "";
                //            //Create(ProcessModel, model.currency, message.Summary, ref orderID);
                //            //Map(model.refNo, orderID);
                //            test(ProcessModel);
                //            //重新给boxs 和 allcount 赋值
                //            boxs.Clear();
                //            boxs.Add(caseNo);
                //            allcount = model.info.Where(item => item.orgCarton == caseNo).Count();

                //        }
                //    }

                //    if (allcount < 50)
                //    {
                //        var ProcessModel = model.info.Where(item => boxs.Contains(item.orgCarton)).ToList();
                //        string orderID = "";
                //        test(ProcessModel);
                //        //Create(ProcessModel, model.currency, message.Summary, ref orderID);
                //        //Map(model.refNo, orderID);
                //    }
                //}
                #endregion

                #region 查看原始数据
                DataTable dt = new DataTable();
                dt.Columns.Add("origin");
                dt.Columns.Add("gw");
                dt.Columns.Add("mfr");
                dt.Columns.Add("unit");
                dt.Columns.Add("description");
                dt.Columns.Add("price");
                dt.Columns.Add("nw");
                dt.Columns.Add("pn");
                dt.Columns.Add("orgCarton");
                dt.Columns.Add("qty");
                dt.Columns.Add("sale_orderline_id");


                foreach (var item in model.info)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = item.origin;
                    dr[1] = item.gw;
                    dr[2] = item.mfr;
                    dr[3] = item.unit;
                    dr[4] = item.description;
                    dr[5] = item.price;
                    dr[6] = item.nw;
                    dr[7] = item.pn;
                    dr[8] = item.orgCarton;
                    dr[9] = item.qty;
                    dr[10] = item.sale_orderline_id;

                    dt.Rows.Add(dr);
                }
                #endregion

                #region
                List<IcgooPackingModel> wouldBeOrders = new List<IcgooPackingModel>();
                var caseNos = model.info.Select(item => item.orgCarton).Distinct().ToList();
                int allcount = 0;
                foreach (var caseNo in caseNos)
                {
                    bool isfind = false;
                    allcount = 0;
                    allcount = model.info.Where(item => item.orgCarton == caseNo).Count();
                    foreach(var t in wouldBeOrders)
                    {
                        if (t.AllCount+allcount < 50)
                        {
                            t.AllCount += allcount;
                            t.boxes.Add(caseNo);
                            isfind = true;
                            break;
                        }
                    }

                    if (!isfind)
                    {
                        IcgooPackingModel packmodel = new IcgooPackingModel();
                        packmodel.AllCount = allcount;
                        packmodel.boxes.Add(caseNo);
                        wouldBeOrders.Add(packmodel);
                    }                  
                }

                foreach(var p in wouldBeOrders)
                {
                    var ProcessModel = model.info.Where(item => p.boxes.Contains(item.orgCarton)).ToList();
                    string orderID = "";                 
                    Create(ProcessModel, model.currency, message.Summary, ref orderID);
                    Map(model.refNo, orderID);
                }

                #endregion
            }
        }

        private void Map(string IcgooOrder,string OrderID)
        {
            IcgooMap map = new IcgooMap();
            map.ID = ChainsGuid.NewGuidUp();
            map.IcgooOrder = IcgooOrder;
            map.OrderID = OrderID;
            map.CompanyType = CompanyTypeEnums.Icgoo;
            map.Enter();
        }

        public bool Create(List<PartNoReceiveItem> singlemodel,string currency,string clientID,ref string msg)
        {
            bool isSuccess = true;
            try
            {
                Console.WriteLine("开始创建订单");
                var user = new Needs.Ccs.Services.Views.ApiClientsView().Where(item => item.ClientId == clientID).FirstOrDefault();
                string supplierID = user.ClientSupplierID;

                //客户               
                var clientView = new Needs.Ccs.Services.Views.ClientsView();
                var client = clientView[clientID];
                string OrderID = "";

                #region 一个订单
                IcgooOrder order = new IcgooOrder();
                order.Client = client;
                OrderID = order.ID;
              

                //香港交货方式：Icgoo 上门自提
                order.OrderConsignee = new OrderConsignee();
                order.OrderConsignee.OrderID = OrderID;
                order.OrderConsignee.PickUpTime = DateTime.Now.AddDays(1);
                //国内交货方式：Icgoo 送货上门
                order.OrderConsignor = new OrderConsignor();
                order.OrderConsignor.OrderID = OrderID;

                order.Type = OrderType.Icgoo;
                order.SetAPIAdmin(Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator));
                order.AdminID = client.Merchandiser.ID;
                order.ClientAgreement = client.Agreement;


                order.Currency = currency;
                order.IsFullVehicle = false;
                order.IsLoan = false;
                order.PackNo = singlemodel.Select(item => item.orgCarton).Distinct().Count();
                order.WarpType = Icgoo.WrapType;

                //客户供应商
                var supplier = client.Suppliers[supplierID];                
                //香港交货信息
                order.OrderConsignee.ClientSupplier = supplier;
                order.OrderConsignee.Type = HKDeliveryType.SentToHKWarehouse;
               

                //客户收货地址
                var clientConsignees = new Needs.Ccs.Services.Views.ClientConsigneesView();
                var defaultConsignee = clientConsignees.Where(item => item.ClientID== clientID && item.IsDefault == true).FirstOrDefault();
                //国内交货信息
                order.OrderConsignor.Type = SZDeliveryType.SentToClient;
                order.OrderConsignor.Name = defaultConsignee.Name;
                order.OrderConsignor.Contact = defaultConsignee.Contact.Name;
                order.OrderConsignor.Mobile = defaultConsignee.Contact.Mobile;
                order.OrderConsignor.Address = defaultConsignee.Address;

                //付汇供应商              
                var payExchangeSupplier = client.Suppliers.Where(item => item.ID == this.IcgooPayExchangeSupplier).FirstOrDefault();
                OrderPayExchangeSupplier orderSupplier = new OrderPayExchangeSupplier();
                orderSupplier.ClientSupplier = payExchangeSupplier;
                orderSupplier.OrderID = OrderID;
                orderSupplier.Status = Status.Normal;
                orderSupplier.UpdateDate = orderSupplier.CreateDate = DateTime.Now;
                order.PayExchangeSuppliers.Add(orderSupplier);
              
                bool isAllClassified = true;
                order.ClassifyProducts = new List<ClassifyProduct>();
                var TotalDeclarePrice = 0M;
                //组装ClassifyProducts                

                using (Needs.Ccs.Services.Views.IFClassifyResultView view = new Needs.Ccs.Services.Views.IFClassifyResultView())
                {
                    var DoneClassifyView = view.Where(item => item.ClassifyStatus == ClassifyStatus.Done&&(item.CompanyType==CompanyTypeEnums.Icgoo||item.CompanyType==CompanyTypeEnums.FastBuy));
                    var CountriesView = new BaseCountriesView();

                    foreach (PartNoReceiveItem p in singlemodel)
                    {
                        ClassifyProduct classifyProduct = new ClassifyProduct();
                        classifyProduct.Client = order.Client;
                        classifyProduct.Currency = currency;
                        classifyProduct.OrderID = OrderID;


                        classifyProduct.Origin = CountriesView.Where(code => code.EditionOneCode == p.origin||code.EnglishName==p.origin).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == p.origin || code.EnglishName == p.origin).FirstOrDefault().Code;
                        classifyProduct.Quantity = p.qty;
                        classifyProduct.Unit = Icgoo.Gunit;
                        classifyProduct.UnitPrice = Math.Round(p.price,4,MidpointRounding.AwayFromZero);
                        classifyProduct.TotalPrice = p.qty * classifyProduct.UnitPrice;
                        classifyProduct.GrossWeight = p.gw / Icgoo.UnitConvert;
                        TotalDeclarePrice += classifyProduct.TotalPrice;
                        //归类结果
                        ClassifyResult classifyResult = DoneClassifyView.Where(item => item.PreProductUnicode == p.sale_orderline_id&&item.ClassifyStatus== ClassifyStatus.Done).FirstOrDefault();
                        if (classifyResult != null)
                        {
                            //if ((classifyResult.Type & ItemCategoryType.Forbid) > 0)
                            //{
                            //    isSuccess = false;
                            //    msg = "不能提交禁运产品";
                            //    return isSuccess;
                            //}

                            var categoryType = classifyResult.Type;
                            if((classifyResult.Type & ItemCategoryType.Inspection) > 0)
                            {
                                classifyProduct.IsInsp = true;
                                classifyProduct.InspectionFee = classifyResult.InspectionFee.HasValue ? classifyResult.InspectionFee.Value : 0;
                            }
                            if ((classifyResult.Type & ItemCategoryType.CCC) > 0)
                            {
                                classifyProduct.IsCCC = true;
                            }
                            if ((classifyResult.Type & ItemCategoryType.Forbid) > 0)
                            {
                                classifyProduct.IsSysForbid = true;
                            }
                            
                            //如果来自疫区，则为归类类型添加“检疫”
                            if (classifyProduct.Origin != null)
                            {
                                string origin = classifyProduct.Origin;
                                var quarantines = new Needs.Ccs.Services.Views.CustomsQuarantinesView();
                                var quarantine = quarantines.Where(cq => cq.Origin == origin && cq.StartDate <= DateTime.Now && cq.EndDate >= DateTime.Now).FirstOrDefault();
                                if (quarantine != null)
                                {
                                    categoryType = categoryType | ItemCategoryType.Quarantine;
                                }
                            }

                            //var product = new Product
                            //{
                            //    Name = classifyResult.ProductName,
                            //    Model = p.pn,
                            //    Manufacturer = classifyResult.Manufacturer,
                            //};
                            //classifyProduct.Product = product;
                            classifyProduct.Name = classifyResult.ProductName;
                            classifyProduct.Model = p.pn;
                            classifyProduct.Manufacturer = classifyResult.Manufacturer;
                            classifyProduct.ClassifyStatus = ClassifyStatus.Done;

                            OrderItemCategory Category = new OrderItemCategory();
                            //Category.Declarant = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.DeclarantID);
                            Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.ClassifyFirstOperatorID);
                            Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.ClassifySecondOperatorID);
                            Category.Type = categoryType.Value;
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
                            var reTariff = tariffView.Where(item => item.Type == CustomsRateType.ImportTax && 
                                                            item.Origin == classifyProduct.Origin&&
                                                            item.CustomsTariffID==classifyProduct.Category.HSCode).FirstOrDefault();
                            if (reTariff != null)
                            {
                                tariff.Rate = reTariff.Rate / 100 + classifyResult.TariffRate.Value;
                            }
                            else
                            {
                                tariff.Rate =  classifyResult.TariffRate.Value;
                            }
                            
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
                        classifyProduct.ProductUniqueCode = p.sale_orderline_id;
                        order.ClassifyProducts.Add(classifyProduct);
                    }
                }
                order.DeclarePrice = TotalDeclarePrice;

                if (isAllClassified)
                {
                    order.OrderStatus = OrderStatus.Classified;
                    order.ConnectionString = this.ConnectionString;
                    order.EnterSpeed();
                    //this.OnCreated(new IcgooCreateOrderEventArgs(order, singlemodel));
                }
                else
                {
                    order.OrderStatus = OrderStatus.Confirmed;
                    order.ConnectionString = this.ConnectionString;
                    order.EnterSpeed();
                    //this.OnPartCreated(new IcgooCreateOrderEventArgs(order, singlemodel));
                }
                Console.WriteLine("创建订单结束");
                #endregion


                msg = OrderID;
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
            Console.WriteLine(args.order.ID+"开始生成对账单");
            this.IcgooGenerateOrderBill?.Invoke(this, args);
            //不需要报价了，报价内容在客户确认中完成
            //this.IcgooQuote?.Invoke(this, args);  
            Console.WriteLine(args.order.ID + "对账单生成结束");
            Console.WriteLine(args.order.ID + "开始确认报价");
            this.IcgooQuoteConfirm?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "报价确认结束");
            Console.WriteLine(args.order.ID + "开始装箱");
            this.IcgooPacking?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "装箱结束");
            Console.WriteLine(args.order.ID + "开始封箱");
            this.IcgooSeal?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "封箱结束");
            Console.WriteLine(args.order.ID + "收款");
            this.IcgooShouldReceive?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "收款结束");

        }

        public virtual void OnPartCreated(IcgooCreateOrderEventArgs args)
        {
            Console.WriteLine(args.order.ID + "开始确认报价");
            this.IcgooPreQuoteConfrirm?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "报价确认结束");
            Console.WriteLine(args.order.ID + "开始装箱");
            this.IcgooPacking?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "装箱结束");
            Console.WriteLine(args.order.ID + "开始封箱");
            this.IcgooSeal?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "封箱结束");
        }

        /// <summary>
        /// 生成对账单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CreateOrderBill(object sender, IcgooCreateOrderEventArgs e)
        {            
            e.order.GenerateBillSpeed();
        }

        /// <summary>
        /// 报价
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Quote(object sender, IcgooCreateOrderEventArgs e)
        {
            //报价不需要了，因为只是记log，改订单状态，log在客户确认那部中记录，状态直接改为已确认
            //ClassifiedOrder order = new ClassifiedOrder();
            //order.ID = e.order.ID;
            //order.Quote();
        }

        /// <summary>
        /// 客户确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void QuoteConfirm(object sender, IcgooCreateOrderEventArgs e)
        {
            var APIAdmin = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {               
                conn.Open();
                //改状态
                SqlCommand sqlStatus = new SqlCommand("OrderStatusUpdatePro", conn);
                sqlStatus.CommandType = CommandType.StoredProcedure;

                sqlStatus.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                sqlStatus.Parameters.Add(new SqlParameter("@OrderStatus", SqlDbType.Int));

                sqlStatus.Parameters["@ID"].Value = e.order.ID;
                sqlStatus.Parameters["@OrderStatus"].Value = (int)OrderStatus.QuoteConfirmed;

                sqlStatus.ExecuteNonQuery();
            
                #region 新增EntryNotice
                string entryNoticeID = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNotice);
                SqlCommand sqlCmd = new SqlCommand("EntryNotice", conn);
                sqlCmd.CommandType = CommandType.StoredProcedure;

                sqlCmd.Parameters.Add(new SqlParameter("@ID", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@OrderID", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@WarehouseType", SqlDbType.Int));                
                sqlCmd.Parameters.Add(new SqlParameter("@ClientCode", SqlDbType.VarChar, 50));
                sqlCmd.Parameters.Add(new SqlParameter("@SortingRequire", SqlDbType.Int));
                sqlCmd.Parameters.Add(new SqlParameter("@EntryNoticeStatus", SqlDbType.Int));
                sqlCmd.Parameters.Add(new SqlParameter("@Status", SqlDbType.Int));
                sqlCmd.Parameters.Add(new SqlParameter("@CreateDate", SqlDbType.DateTime));
                sqlCmd.Parameters.Add(new SqlParameter("@UpdateDate", SqlDbType.DateTime));
                sqlCmd.Parameters.Add(new SqlParameter("@Summary", SqlDbType.NChar, 300));

                sqlCmd.Parameters["@ID"].Value = entryNoticeID;
                sqlCmd.Parameters["@OrderID"].Value = e.order.ID;
                sqlCmd.Parameters["@WarehouseType"].Value = (int)WarehouseType.HongKong;                
                sqlCmd.Parameters["@ClientCode"].Value = e.order.Client.ClientCode;
                sqlCmd.Parameters["@SortingRequire"].Value = e.order.Client.ClientRank == ClientRank.ClassFive ? SortingRequire.UnPacking : SortingRequire.Packed;
                sqlCmd.Parameters["@EntryNoticeStatus"].Value = (int)EntryNoticeStatus.UnBoxed;
                sqlCmd.Parameters["@Status"].Value = (int)Status.Normal;
                sqlCmd.Parameters["@CreateDate"].Value = DateTime.Now;
                sqlCmd.Parameters["@UpdateDate"].Value = DateTime.Now;
                sqlCmd.Parameters["@Summary"].Value = "";

                sqlCmd.ExecuteNonQuery();
                #endregion

                #region 新增EntryNoticeItem
                DataTable dt = new DataTable();
                dt.Columns.Add("ID");
                dt.Columns.Add("EntryNoticeID");
                dt.Columns.Add("OrderItemID");
                dt.Columns.Add("DecListID");
                dt.Columns.Add("IsSpotCheck");
                dt.Columns.Add("EntryNoticeStatus");
                dt.Columns.Add("Status");
                dt.Columns.Add("CreateDate");
                dt.Columns.Add("UpdateDate");

                foreach (var item in e.order.Items)
                {
                    DataRow dr = dt.NewRow();
                    dr[0] = Needs.Overall.PKeySigner.Pick(PKeyType.EntryNoticeItem);
                    dr[1] = entryNoticeID;
                    dr[2] = item.ID;
                    dr[3] = null;
                    dr[4] = item.IsSampllingCheck;
                    dr[5] = (int)EntryNoticeStatus.Boxed;
                    dr[6] = (int)Status.Normal;
                    dr[7] = DateTime.Now;
                    dr[8] = DateTime.Now;

                    dt.Rows.Add(dr);
                }

                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn);
                bulkCopy.DestinationTableName = "EntryNoticeItems";
                bulkCopy.BatchSize = dt.Rows.Count;
                bulkCopy.WriteToServer(dt);
                #endregion
              
            }
        }

        /// <summary>
        /// 有没有归类的型号，先生成香港仓库的入库通知
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void ToEntryNotice(object sender, IcgooCreateOrderEventArgs e)
        {
            var order = new Needs.Ccs.Services.Views.IcgooOrdersView()[e.order.ID];
            if (order != null)
            {
                order.IcgooToEntryNoticeSpeed();
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
            IcgooHKSortingContext hkSorting;
            List<PartNoReceiveItem> ModelItems = e.partno;

            List<string> OrderItemIDs = new List<string>();

            Boxes.ForEach(b =>
            {
                var Products = ModelItems.Where(item => item.orgCarton == b).ToList();
                hkSorting = new IcgooHKSortingContext();
                hkSorting.ToShelve(Icgoo.ShelveNumber, b.ToString());

                //创建packing对象
                PackingModel packing = new PackingModel();
                packing.AdminID = Icgoo.DefaultCreator;
                packing.OrderID = OrderId;
                packing.BoxIndex = b.ToString();
                packing.Weight = Products.Select(item => item.gw).Sum() / Icgoo.UnitConvert;
                packing.WrapType = Icgoo.WrapType;
                packing.PackingDate = DateTime.Now;
                packing.Quantity = Products.Select(item => item.qty).Sum();
                hkSorting.SetPacking(packing);

                List<InsideSortingModel> list = new List<InsideSortingModel>();

                Products.ForEach(p =>
                {
                    //var orderitem = e.order.Items.Where(item => item.Product.Model == p.pn && item.Quantity == p.qty).FirstOrDefault();
                    #region 防止一个订单中，有型号，和数量一样的情况出现
                    var orderitems = e.order.Items.Where(item => item.Model == p.pn && item.Quantity == p.qty).ToList();
                    var orderitem = new Needs.Ccs.Services.Models.OrderItem();
                    if (orderitems.Count == 1)
                    {
                        orderitem = orderitems[0];
                    }
                    else
                    {
                        foreach(var t in orderitems)
                        {
                            if (OrderItemIDs.Where(m=>m==t.ID).Count()==0)
                            {
                                orderitem = t;
                                break;
                            }
                        }
                    }
                    #endregion


                    if (orderitem != null)
                    {
                        InsideSortingModel model = new InsideSortingModel();
                        model.EntryNoticeItemID = EnterNoticeId;
                        //model.ProductID = orderitem.Product.ID;
                        model.OrderItemID = orderitem.ID;
                        model.Quantity = p.qty;
                        model.NetWeight = Math.Round(p.nw/Icgoo.UnitConvert,2,MidpointRounding.AwayFromZero)==0?0.01M: Math.Round(p.nw / Icgoo.UnitConvert, 2, MidpointRounding.AwayFromZero);
                        model.GrossWeight = Math.Round(model.NetWeight*Icgoo.BaseGWPara,2,MidpointRounding.AwayFromZero);
                        list.Add(model);
                        OrderItemIDs.Add(orderitem.ID);
                    }
                });

                hkSorting.InsideItems = list;
                hkSorting.ConnectionString = this.ConnectionString;
                hkSorting.PackSpeed();

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
            var entryNotice = new Needs.Ccs.Services.Views.IcgooHKEntryNoticeView()[EnterNoticeId];

            entryNotice.SetAdmin(Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator));
            entryNotice.ConnectionString = this.ConnectionString;
            entryNotice.SealSpeed();

        }

        public void ShouldReceive(object sender, IcgooCreateOrderEventArgs e)
        {
            e.order.ToReceivables();
        }
    }
}
