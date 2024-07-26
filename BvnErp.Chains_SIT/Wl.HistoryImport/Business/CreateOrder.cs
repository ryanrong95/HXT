using Layer.Data.Sqls.BvnErp;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Converters;
using Needs.Utils.Npoi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Wl.HistoryImport.HistoryCreateOrderHanlder;

namespace Wl.HistoryImport
{
    public class CreateOrder
    {
        public string ConnectionString { get; set; }
        public string InsidePayExchangeSupplier { get; set; }

        public event HistoryGenerateOrderBillHanlder InsideGenerateOrderBill;
        public event HistoryGenerateOrderBillHanlder InsideQuoteConfirm;
        public event HistoryGenerateOrderBillHanlder InsidePreQuoteConfrirm;
        public event HistoryGenerateOrderBillHanlder InsidePacking;
        public event HistoryGenerateOrderBillHanlder InsideSeal;
        public event HistoryGenerateOrderBillHanlder InsideShouldReceive;
        public event HistoryGenerateOrderBillHanlder InsideGenerateOrderOld;

        public CreateOrder()
        {
            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ScCustomsConnectionString"].ConnectionString;
            InsideGenerateOrderBill += CreateOrderBill;
            InsideQuoteConfirm += QuoteConfirm;
            InsidePreQuoteConfrirm += ToEntryNotice;
            InsidePacking += Packing;
            InsideSeal += Seal;
            InsideShouldReceive += ShouldReceive;

            InsideGenerateOrderOld += CreateHead;
            InsideGenerateOrderOld += HKAutoOut;
            InsideGenerateOrderOld += SZOnStock;           
        }

        public bool Create(List<InsideOrderItem> model, HistoryUseOnly only,List<PackHistoryOnly> Packs)
        {
            
            bool isSuccess = true;
            try
            {
                Console.WriteLine("开始创建订单");
                var client = only.Client;


                string OrderID = "";

                #region 一个订单
                IcgooOrder order = new IcgooOrder();
                order.Client = client;
                order.ID = only.OrderNo;
                OrderID = order.ID;

                //香港交货方式：送货上门
                order.OrderConsignee = new OrderConsignee();
                order.OrderConsignee.OrderID = OrderID;
                //国内交货方式：Icgoo 送货上门
                order.OrderConsignor = new OrderConsignor();
                order.OrderConsignor.OrderID = OrderID;

                switch (only.IsLocal)
                {
                    case 0:
                        order.Type = OrderType.Outside;
                        break;

                    case 1:
                        order.Type = OrderType.Inside;
                        break;

                    case 2:
                        order.Type = OrderType.Icgoo;
                        break;
                }

                order.SetAPIAdmin(client.Merchandiser);
                order.AdminID = client.Merchandiser.ID;
                order.ClientAgreement = client.Agreement;

                order.Currency = only.Currency;
                order.IsFullVehicle = false;
                order.IsLoan = false;

                order.PackNo = only.TotalPacks;
                order.WarpType = Icgoo.WrapType;

                //客户供应商
                var supplier = client.Suppliers.FirstOrDefault();

                if (supplier == null)
                {
                    //TODO:记录日志
                    WriteLog(order.ID+"没有供应商");
                    return false;
                }

                //香港交货信息
                order.OrderConsignee.ClientSupplier = supplier;
                order.OrderConsignee.Type = HKDeliveryType.SentToHKWarehouse;


                //客户收货地址
                var clientConsignees = new Needs.Ccs.Services.Views.ClientConsigneesView();
                var defaultConsignee = clientConsignees.Where(item => item.ClientID == client.ID && item.IsDefault == true).FirstOrDefault();
                //国内交货信息
                if (defaultConsignee != null)
                {
                    order.OrderConsignor.Type = SZDeliveryType.Shipping;
                    order.OrderConsignor.Name = defaultConsignee.Name;
                    order.OrderConsignor.Contact = defaultConsignee.Contact.Name;
                    order.OrderConsignor.Mobile = defaultConsignee.Contact.Mobile;
                    order.OrderConsignor.Address = defaultConsignee.Address;
                }

                //付汇供应商              
                //var payExchangeSupplier = client.Suppliers.Where(item => item.ID == this.InsidePayExchangeSupplier).FirstOrDefault();
                //OrderPayExchangeSupplier orderSupplier = new OrderPayExchangeSupplier();
                //orderSupplier.ClientSupplier = payExchangeSupplier;
                //orderSupplier.OrderID = OrderID;
                //orderSupplier.Status = Status.Normal;
                //orderSupplier.UpdateDate = orderSupplier.CreateDate = DateTime.Now;
                //order.PayExchangeSuppliers.Add(orderSupplier);

               
                order.ClassifyProducts = new List<ClassifyProduct>();
                var TotalDeclarePrice = 0M;
                //组装ClassifyProducts     
                var productTaxCategoriesView = new Needs.Ccs.Services.Views.ProductTaxCategoriesAllsView();
                using (Needs.Ccs.Services.Views.TaxCategoriesDefaultsAllsView view = new Needs.Ccs.Services.Views.TaxCategoriesDefaultsAllsView())
                {


                    foreach (InsideOrderItem p in model)
                    {
                        ClassifyProduct classifyProduct = new ClassifyProduct();
                        classifyProduct.Client = order.Client;
                        classifyProduct.Currency = order.Currency;
                        classifyProduct.OrderID = OrderID;


                        classifyProduct.Origin = p.PlaceOfProduction;
                        classifyProduct.Quantity = p.Quantity;
                        classifyProduct.Unit = Icgoo.Gunit;
                        classifyProduct.UnitPrice = p.UnitPrice;
                        classifyProduct.TotalPrice = p.TotalDeclarePrice;
                        classifyProduct.GrossWeight = p.GrossWeight;
                        TotalDeclarePrice += classifyProduct.TotalPrice;

                        classifyProduct.IsInsp = p.IsInspection;
                        classifyProduct.InspectionFee = p.InspFee;

                        classifyProduct.IsCCC = p.IsCCC;


                        classifyProduct.Name = p.ProductName;
                        classifyProduct.Model = p.Model;
                        classifyProduct.Manufacturer = p.Brand;

                        classifyProduct.ClassifyStatus = ClassifyStatus.Done;


                        OrderItemCategory Category = new OrderItemCategory();
                        Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create("XDTAdmin");
                        Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create("XDTAdmin");
                        if (p.IsInspection)
                        {
                            Category.Type = ItemCategoryType.Inspection;
                        }
                        else if (p.IsCCC)
                        {
                            Category.Type = ItemCategoryType.CCC;
                        }
                        else
                        {
                            Category.Type = ItemCategoryType.Normal;
                        }


                        if (p.TaxName != null&&p.TaxCode!=null)
                        {                            
                            Category.TaxCode = p.TaxCode;
                            Category.TaxName = p.TaxName;
                            if (!p.TaxName.Contains("*"))
                            {
                                var productTax = productTaxCategoriesView.Where(item => item.Name == p.ProductName).FirstOrDefault();
                                if (productTax != null)
                                {
                                    Category.TaxName = productTax.TaxName;
                                }
                                else
                                {
                                    var taxRelated = view.Where(item => item.TaxSecondCategory == p.ProductName).FirstOrDefault();
                                    if (taxRelated != null)
                                    {
                                        Category.TaxCode = taxRelated.TaxCode;
                                        Category.TaxName = "*" + taxRelated.TaxFirstCategory + "*" + taxRelated.TaxSecondCategory;        
                                    }
                                    else
                                    {
                                        Category.TaxCode = "UnKown";
                                        Category.TaxName = "UnKown";
                                        WriteLog(order.ID + " 型号:" + p.Model + " 报关品名:" + p.ProductName + " 没有开票品名");
                                          
                                    }
                                }
                            }
                        }
                        else
                        {
                            var productTax = productTaxCategoriesView.Where(item => item.Name == p.ProductName).FirstOrDefault();
                            if (productTax != null)
                            {
                                Category.TaxName = productTax.TaxName;
                                Category.TaxCode = productTax.TaxCode;
                            }
                            else
                            {
                                var taxRelated = view.Where(item => item.TaxSecondCategory == p.ProductName).FirstOrDefault();
                                if (taxRelated != null)
                                {
                                    Category.TaxCode = taxRelated.TaxCode;
                                    Category.TaxName = "*" + taxRelated.TaxFirstCategory + "*" + taxRelated.TaxSecondCategory;
                                }
                                else
                                {
                                    Category.TaxCode = "UnKown";
                                    Category.TaxName = "UnKown";
                                    WriteLog(order.ID + " 型号:" + p.Model + " 报关品名:" + p.ProductName + " 没有开票品名");                                   
                                }
                            }
                        }


                        Category.HSCode = p.CustomsCode;
                        Category.Name = p.ProductName;
                        Category.Elements = p.Elements;
                        Category.Unit1 = p.FirstLegalUnit;
                        Category.Unit2 = p.SecondLegalUnit;
                        Category.CIQCode = p.CIQCode;
                        classifyProduct.Category = Category;


                        //TODO
                        //if (p.IsInspection==)
                        //{


                        //    if (quarantine != null)
                        //    {
                        //        classifyProduct.Category.Type |= ItemCategoryType.Quarantine;
                        //    }                           
                        //}

                        OrderItemTax tariff = new OrderItemTax();
                        tariff.Type = CustomsRateType.ImportTax;
                        tariff.Rate = p.TariffRate;
                        classifyProduct.ImportTax = tariff;

                        OrderItemTax ValueAddedTax = new OrderItemTax();
                        ValueAddedTax.Type = CustomsRateType.AddedValueTax;
                        ValueAddedTax.Rate = p.VauleAddedRate;
                        classifyProduct.AddedValueTax = ValueAddedTax;

                        classifyProduct.OrderType = OrderType.Inside;
                        classifyProduct.ProductUniqueCode = p.PreProductID;
                        order.ClassifyProducts.Add(classifyProduct);



                    }
                }
                order.DeclarePrice = model.Sum(t => t.TotalDeclarePrice);

                order.CustomsExchangeRate = only.CustomsExchangeRate;
                order.RealExchangeRate = only.RealExchangeRate;
                order.OrderStatus = OrderStatus.Classified;
                order.ConnectionString = this.ConnectionString;
                order.CreateDate = order.UpdateDate = only.OrderCreateDate;
                order.InvoiceStatus = InvoiceStatus.Invoiced;
                order.EnterSpeed();
                this.OnCreated(new HistoryCreateOrderEventArgs(order, model, only, Packs));
                this.OnHeadCreated(new HistoryCreateOrderEventArgs(order, model, only, Packs));
                Console.WriteLine("创建订单结束");

                #endregion

                //msg = OrderID;
                return isSuccess;


            }
            catch (Exception ex)
            {
                WriteLog(only.OrderNo + ex.ToString());
                return false;
            }
        }

        private Needs.Ccs.Services.Models.TaxCategory getTaxCategory(string productName,string hsCode)
        {
            var productTaxCategoriesView = new Needs.Ccs.Services.Views.ProductTaxCategoriesAllsView();
            var dyjTaxCategoriesView = new Needs.Ccs.Services.Views.TaxCategoriesDefaultsAllsView();
            var productTax = productTaxCategoriesView.Where(item => item.Name == productName).FirstOrDefault();
            if (productTax != null)
            {
                return new Needs.Ccs.Services.Models.TaxCategory {
                    ID = productTax.ID,
                    TaxCode = productTax.TaxCode,
                    TaxName = productTax.TaxName,
                    Name = productTax.Name                                   
                };
            }

            var taxRelated = dyjTaxCategoriesView.Where(item => item.TaxSecondCategory == productName).FirstOrDefault();
            if (taxRelated != null)
            {
                return new Needs.Ccs.Services.Models.TaxCategory
                {
                    ID = taxRelated.ID,
                    TaxCode = taxRelated.TaxCode,
                    TaxName = "*"+ taxRelated.TaxFirstCategory+"*"+taxRelated.TaxSecondCategory,                   
                };
            }
            return new Needs.Ccs.Services.Models.TaxCategory { };
        }

      
        public virtual void OnCreated(HistoryCreateOrderEventArgs args)
        {
            Console.WriteLine(args.order.ID + "开始生成对账单");
            this.InsideGenerateOrderBill?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "生成对账单结束");
            Console.WriteLine(args.order.ID + "开始确认报价");
            this.InsideQuoteConfirm?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "确认报价结束");
            Console.WriteLine(args.order.ID + "开始装箱");
            this.InsidePacking?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "装箱结束");
            Console.WriteLine(args.order.ID + "开始封箱");
            this.InsideSeal?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "封箱结束");
            Console.WriteLine(args.order.ID + "收款");
            this.InsideShouldReceive?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "收款结束");
            
        }

        public virtual void OnHeadCreated(HistoryCreateOrderEventArgs args)
        {
            Console.WriteLine("开始生成报关单");
            this.InsideGenerateOrderOld?.Invoke(this,args);
            Console.WriteLine("报关单生成结束");
        }

 
        /// <summary>
        /// 生成对账单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CreateOrderBill(object sender, HistoryCreateOrderEventArgs e)
        {
            e.order.GenerateBillSpeed();
        }



        /// <summary>
        /// 客户确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void QuoteConfirm(object sender, HistoryCreateOrderEventArgs e)
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
        public void ToEntryNotice(object sender, HistoryCreateOrderEventArgs e)
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
        public void Packing(object sender, HistoryCreateOrderEventArgs e)
        {
            var EnterNoticeId = new Needs.Ccs.Services.Views.EntryNoticeView().Where(item => item.Order.ID == e.order.ID).Select(item => item.ID).FirstOrDefault();
            string OrderId = e.order.ID;
            IcgooHKSortingContext hkSorting;

            foreach(var orderpack in e.Packs)
            {
                hkSorting = new IcgooHKSortingContext();
                hkSorting.ToShelve(Icgoo.InsideShelveNumber, orderpack.BoxIndex);
                PackingModel packing = new PackingModel();
                packing.AdminID = Icgoo.DefaultCreator;
                packing.OrderID = OrderId;
                packing.BoxIndex = orderpack.BoxIndex;
                packing.Weight = orderpack.GrossWeight;
                packing.WrapType = Icgoo.WrapType;
                packing.PackingDate = e.order.CreateDate;
                packing.Quantity = orderpack.Quantity;
                hkSorting.SetPacking(packing);

                List<InsideSortingModel> list = new List<InsideSortingModel>();

                foreach(var packdetail in orderpack.PackItems)
                {
                    var orderitem = e.order.Items.Where(item => item.Model == packdetail.Model&&item.Quantity==packdetail.Quantity&&item.UnitPrice==packdetail.UnitPrice).FirstOrDefault();
                    //有订单只有一个型号，但是这个型号分几个箱子装的情况，所以数量就不等
                    if (orderitem == null)
                    {
                        orderitem = e.order.Items.Where(item => item.Model == packdetail.Model).FirstOrDefault();
                    }
                    InsideSortingModel model = new InsideSortingModel();
                    model.EntryNoticeItemID = EnterNoticeId;
                    model.OrderItemID = orderitem.ID;
                    model.Quantity = packdetail.Quantity;
                    model.NetWeight = packdetail.NetWeight;
                    var grossweight = (packdetail.NetWeight / 0.7m) < 0.02m ? 0.02m : (packdetail.NetWeight / 0.7m);
                    model.GrossWeight = Math.Round(grossweight, 2);
                    list.Add(model);
                }

                hkSorting.InsideItems = list;
                hkSorting.ConnectionString = this.ConnectionString;
                hkSorting.PackSpeed();
            }
        }

        /// <summary>
        /// 封箱
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Seal(object sender, HistoryCreateOrderEventArgs e)
        {
            var EnterNoticeId = new Needs.Ccs.Services.Views.EntryNoticeView().Where(item => item.Order.ID == e.order.ID).Select(item => item.ID).FirstOrDefault();
            var entryNotice = new Needs.Ccs.Services.Views.IcgooHKEntryNoticeView()[EnterNoticeId];

            entryNotice.SetAdmin(Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator));
            entryNotice.ConnectionString = this.ConnectionString;
            entryNotice.SealSpeed();

        }

        public void ShouldReceive(object sender, HistoryCreateOrderEventArgs e)
        {
            e.order.ToReceivables();
        }

        public void CreateHead(object sender, HistoryCreateOrderEventArgs e)
        {
            Purchaser purchaser = PurchaserContext.Current;
            Vendor vendor = new VendorContext(VendorContextInitParam.OrderID, e.order.ID).Current1;


            #region 创建VoyNo
            Needs.Ccs.Services.Models.Voyage voyno = new Needs.Ccs.Services.Models.Voyage();
            voyno.ID = e.historyUseOnly.VoyNo;
            voyno.Carrier = new Needs.Ccs.Services.Models.Carrier();
            voyno.Type = Needs.Ccs.Services.Enums.VoyageType.Normal;
            voyno.UpdateDate = voyno.CreateTime = Convert.ToDateTime(e.historyUseOnly.DeclarationDate);
            voyno.Status = Needs.Ccs.Services.Enums.Status.Normal;
            voyno.CutStatus = Needs.Ccs.Services.Enums.CutStatus.Completed;
            voyno.HKDeclareStatus = true;
            voyno.Enter();
            #endregion


            #region 组装实体
            Needs.Ccs.Services.Models.DecHead head = new Needs.Ccs.Services.Models.DecHead();
            ///  head.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DecHead);
            head.ID = string.Concat(PurchaserContext.Current.DecHeadIDPrefix, Needs.Overall.PKeySigner.Pick(PKeyType.DecHead));

            var declarationNotice = new Needs.Ccs.Services.Views.DeclarationNoticesView().Where(item => item.OrderID == e.order.ID).FirstOrDefault();

            head.DeclarationNoticeID = declarationNotice.ID;
            head.OrderID = e.order.ID;
            head.CustomMaster = e.historyUseOnly.Port;
            head.SeqNo = "";
            head.PreEntryId = "";
            head.EntryId = "";
            head.IEPort = e.historyUseOnly.Port;
            //导入的历史数据，付汇状态直接变为已付汇
            head.SwapStatus = SwapStatus.Audited;


            head.ContrNo = e.historyUseOnly.ContrNO;
            head.IEDate = e.historyUseOnly.DeclarationDate.Replace("-","");
            head.DDate = Convert.ToDateTime(e.historyUseOnly.DeclarationDate);

            head.ConsigneeName = "深圳市创新恒远供应链管理有限公司";
            head.ConsigneeScc = "91440300687582405X";
            head.ConsigneeCusCode = "4453066631";
            head.ConsigneeCiqCode = "4700629085";

            head.ConsignorName = "NEWMAY INTERNATIONAL TRADE LIMITED";
            head.ConsignorCode = "香港纽曼国际物流有限公司";
            vendor.SealUrl = "Content\\images\\HK.png";

            switch (e.historyUseOnly.days)
            {
                case 0:
                    head.ConsignorName = "HONGKONG HONGTU INTERNATIONAL LOGISTICS CO., LIMITED";
                    head.ConsignorCode = "香港鸿图国际物流有限公司";
                    vendor.SealUrl = "Content\\images\\HKHT.png";
                    break;

                case 1:
                    head.ConsignorName = "HONGKONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED";
                    head.ConsignorCode = "香港畅运国际物流有限公司";
                    vendor.SealUrl = "Content\\images\\HKCY.png";
                    break;
            }

          

            head.OwnerName = e.historyUseOnly.Client.Company.Name;
            head.OwnerScc = e.historyUseOnly.Client.Company.Code;
            //为空是内单，用杭州比一比
            head.OwnerCusCode = e.historyUseOnly.OwnerCusCode == null ? "MA2B2B167" : e.historyUseOnly.OwnerCusCode;
            head.OwnerCiqCode = "";

            head.AgentName = "深圳市创新恒远供应链管理有限公司";
            head.AgentScc = "91440300687582405X";
            head.AgentCusCode = "4453066631";
            head.AgentCiqCode = "4700629085";
            head.TrafMode = "4";

            head.VoyNo = e.historyUseOnly.VoyNo;
            head.BillNo = e.historyUseOnly.BillNo;

            head.TradeMode = "0110";
            head.CutMode = "101";

            head.TradeCountry = "HKG";
            head.DistinatePort = "HKG000";
            head.TransMode = 1;

            var sorting = new Needs.Ccs.Services.Views.HKSortingsView().Where(item => item.OrderID == e.order.ID);

            head.PackNo = e.historyUseOnly.TotalPacks;
            head.WrapType = "22";
            head.GrossWt = Math.Ceiling(sorting.Select(p => p.GrossWeight).Sum()) < 2 ? 2 : Math.Ceiling(sorting.Select(p => p.GrossWeight).Sum());
            head.NetWt = sorting.Select(p => p.NetWeight).Sum().ToRound(2) < 1 ? 1 : sorting.Select(p => p.NetWeight).Sum().ToRound(2);
            head.TradeAreaCode = "HKG";
            head.EntyPortCode = "470501";
            head.GoodsPlace = "深圳市龙华区龙华街道富康社区天汇大厦C栋212";
            head.EntryType = "M";

            head.DespPortCode = "HKG000";
            head.MarkNo = "N/M";

            head.NoteS = "";
            head.ApprNo = "";
            head.DeclTrnRel = 0;
            head.BillType = 1;
            head.Inputer = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create("Admin0000000307");
            head.DeclareName = "陈付明";
            head.TypistNo = "8930000060771";
            head.PromiseItmes = "000";
            head.ChkSurety = 0;
            head.Type = "Z";

            #endregion
            head.CreateTime = Convert.ToDateTime(e.historyUseOnly.DeclarationDate); ;

            //订单
            var order = new Needs.Ccs.Services.Views.OrdersView().Where(p => p.ID == e.order.ID).FirstOrDefault();

            #region 表体信息

            var DeclarationNoticeItems = declarationNotice.Items.AsQueryable();
            var sortingorder = new Needs.Ccs.Services.Views.HKSortingsView().Where(item => item.OrderID == e.order.ID);

            var i = 1;

            List<string> orderItemIDS = new List<string>();

            

            var CountryNameView = new Needs.Ccs.Services.Views.BaseCountriesView();

            foreach (var orderpack in e.Packs)
            {
                foreach (var packdetail in orderpack.PackItems)
                {
                    var orderitemid = order.Items.Where(t => t.Model == packdetail.Model&&t.Quantity==packdetail.Quantity&&t.UnitPrice==packdetail.UnitPrice).FirstOrDefault();
                    
                    //有可能一个订单，一个型号，装箱的时候，不装在一起的情况
                    if (orderitemid == null)
                    {
                        orderitemid = order.Items.Where(t => t.Model == packdetail.Model).FirstOrDefault();
                    }
                    var sortingid = sortingorder.Where(t => t.OrderItem.ID == orderitemid.ID && t.Quantity == packdetail.Quantity && t.NetWeight == packdetail.NetWeight).FirstOrDefault();

                    var item = DeclarationNoticeItems.Where(t => t.Sorting.ID == sortingid.ID).FirstOrDefault();
                    orderItemIDS.Add(orderitemid.ID);
                    var list = new Needs.Ccs.Services.Models.DecList();
                    list.ID = string.Concat(head.ID, i).MD5();
                    list.DeclarationID = head.ID;
                    list.DeclarationNoticeItemID = DeclarationNoticeItems.Where(a => a.Sorting.ID == item.Sorting.ID).Select(b => b.ID).FirstOrDefault();
                    list.OrderID = item.Sorting.OrderID;
                    list.OrderItemID = item.Sorting.OrderItem.ID;
                    list.CusDecStatus = CusItemDecStatus.Normal;
                    list.GNo = i;
                    list.CodeTS = item.Sorting.OrderItem.Category.HSCode;
                    list.CiqCode = item.Sorting.OrderItem.Category.CIQCode;
                    list.GName = item.Sorting.OrderItem.Category.Name;
                    list.GModel = item.Sorting.OrderItem.Category.Elements;
                    list.GQty = item.Sorting.Quantity;
                    list.GUnit = item.Sorting.OrderItem.Unit;
                    list.FirstUnit = item.Sorting.OrderItem.Category.Unit1;
                    list.SecondUnit = item.Sorting.OrderItem.Category.Unit2;
                    list.TradeCurr = e.order.Currency;
                    string place = packdetail.PlaceOfProduction.Trim();
                    list.OriginCountry = CountryNameView.Where(code => code.EditionOneCode == place || code.Code == place || code.Name == place).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountryNameView.Where(code => code.EditionOneCode == place || code.Code == place || code.Name == place).FirstOrDefault().Code; 
                    list.OriginCountryName = CountryNameView.Where(t => t.Code == list.OriginCountry).Select(t => t.Name).FirstOrDefault() == null ? "" : CountryNameView.Where(t => t.Code == list.OriginCountry).Select(t => t.Name).FirstOrDefault();
                    //计算价格
                    var totalPrice = item.Sorting.OrderItem.TotalPrice * Needs.Ccs.Services.ConstConfig.TransPremiumInsurance * (item.Sorting.Quantity / item.Sorting.OrderItem.Quantity);
                    list.DeclPrice = (totalPrice / item.Sorting.Quantity).ToRound(4);
                    list.DeclTotal = totalPrice.ToRound(2);

                    //冗余项
                    list.CaseNo = item.Sorting.BoxIndex;
                    list.NetWt = item.Sorting.NetWeight;
                    list.GrossWt = item.Sorting.GrossWeight;
                    list.GoodsModel = item.Sorting.OrderItem.Model;
                    list.GoodsBrand = item.Sorting.OrderItem.Manufacturer;


                    head.Lists.Add(list);

                    i++;
                }
            }
    
            #endregion

            #region 商检/检疫                   
            //TODO:检疫无法确认
            var isQuarantines = false;
            var isInspections = e.order.Items.Where(item => orderItemIDS.Contains(item.ID)).Any(
                t => (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Inspection.GetHashCode()) > 0
                || (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.CCC.GetHashCode()) > 0);
          
            if (isQuarantines || isInspections)
            {
                //检验检疫信息
                //根据申报关别，带出检疫机关
                var masterDefault = new Needs.Ccs.Services.Views.BaseCustomMasterDefaultView().Where(t => t.Code == head.CustomMaster)?.FirstOrDefault();
                if (masterDefault != null)
                {
                    head.OrgCode = masterDefault.OrgCode;
                    head.VsaOrgCode = masterDefault.VsaOrgCode;
                    head.InspOrgCode = masterDefault.InspOrgCode;
                    head.PurpOrgCode = masterDefault.PurpOrgCode;
                }
                head.DespDate = head.IEDate;
                head.UseOrgPersonCode = purchaser.UseOrgPersonCode;
                head.UseOrgPersonTel = purchaser.UseOrgPersonTel;
                head.DomesticConsigneeEname = purchaser.DomesticConsigneeEname;
                head.OverseasConsignorCname = vendor.OverseasConsignorCname;
                head.OverseasConsignorAddr = vendor.OverseasConsignorAddr;
                //TODO:数据库可以直接使用datetime类型，报关是进行格式转换
                //head.CmplDschrgDt = Convert.ToDateTime(head.DespDate.Insert(4, "-").Insert(7, "-")).AddDays(3).ToString("yyyyMMdd");
                //卸毕日期改为当前日期向后三天
                head.CmplDschrgDt = Convert.ToDateTime(e.historyUseOnly.DeclarationDate).AddDays(3).ToString("yyyyMMdd");

                //head.ContrNo = head.ContrNo.Replace(purchaser.ContractNoPrefix, purchaser.SJContractNoPrefix);
                //head.BillNo = head.BillNo.Replace(purchaser.BillNoPrefix, purchaser.SJBillNoPrefix);

                //处理商检/检疫的表体
                foreach (var item in head.Lists)
                {
                    var orderitem = order.Items.FirstOrDefault(t => t.ID == item.OrderItemID);
                    var isItemInspection = (orderitem.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Inspection.GetHashCode()) > 0;
                    var isItemCCC = (orderitem.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.CCC.GetHashCode()) > 0;
                    //var isItemQuarantine = (orderitem.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Quarantine.GetHashCode()) > 0;

                    item.GoodsSpec = "***";
                    item.Purpose = "99";//用途 默认：99 其它
                    item.GoodsAttr = isItemCCC ? "11,19" : (isItemInspection ? "12,19" : "19");//默认：19 正常
                    item.CiqName = new Needs.Ccs.Services.Views.CustomsCiqCodeView().FirstOrDefault(t => t.ID == (orderitem.Category.HSCode + orderitem.Category.CIQCode))?.Name;
                    item.GoodsBatch = string.IsNullOrEmpty(orderitem.Batch) ? "***" : orderitem.Batch;
                }
            }


            head.IsInspection = isInspections;
            head.IsQuarantine = isQuarantines;

            #endregion
            head.CreateDeclare();
        }

        public void HKAutoOut(object sender, HistoryCreateOrderEventArgs e)
        {
            var head = new Needs.Ccs.Services.Views.DecHeadsView().Where(item => item.OrderID == e.order.ID).FirstOrDefault();
            if (head != null)
            {
                //更新报关单状态
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CusDecStatus = "P" }, item => item.ID == head.ID);
                }

                head.DeclareSucceess();
                head.SZWarehouseAutoOut("histroy");

                //更改报关单里的海关汇率 
                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(new { CustomsExchangeRate = e.order.CustomsExchangeRate }, item => item.ID == head.ID);
                }
            }
        }

        public void SZOnStock(object sender, HistoryCreateOrderEventArgs e)
        {
            List<Needs.Ccs.Services.Views.SZOnStockView.TargetSortingModel> listBox = new List<Needs.Ccs.Services.Views.SZOnStockView.TargetSortingModel>();

            var sorting = new Needs.Ccs.Services.Views.HKSortingsView().Where(item => item.OrderID == e.order.ID);
            var boxInfos = sorting.Select(p => p.BoxIndex).Distinct();
            foreach (var boxInfo in boxInfos)
            {
                listBox.Add(new Needs.Ccs.Services.Views.SZOnStockView.TargetSortingModel()
                {
                    OrderID = e.order.ID,
                    BoxIndex = boxInfo,
                });
            }

            string voyageID = e.historyUseOnly.VoyNo;
            string stockCode = "HistoryStock";

            new Needs.Ccs.Services.Models.SZEntryNotice().OnStock(voyageID, stockCode, listBox);


            new Needs.Ccs.Services.Models.SZEntryNotice().Complete(voyageID);
        }

        public  void WriteLog(string strs)
        {
            FileStream fs = new FileStream("D:\\HistoryImport\\log.txt", FileMode.Append, FileAccess.Write);
            //获得字节数组
            byte[] data = System.Text.Encoding.Default.GetBytes(strs);
            //开始写入
            fs.Write(data, 0, data.Length);
            //清空缓冲区、关闭流
            fs.Flush();
            fs.Close();
        }

    }
}
