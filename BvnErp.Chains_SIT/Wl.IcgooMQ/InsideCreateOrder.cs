using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Converters;
using Needs.Utils.Npoi;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;


namespace Wl.IcgooMQ
{
    public class InsideCreateOrder
    {
        public string ConnectionString { get; set; }
        public string InsidePayExchangeSupplier { get; set; }
        public event InsideGenerateOrderBillHanlder InsideGenerateOrderBill;
        public event InsideQuoteConfirmHandler InsideQuoteConfirm;
        public event InsideQuoteConfirmHandler InsidePreQuoteConfrirm;
        public event InsidePackingHandler InsidePacking;
        public event InsideSealHandler InsideSeal;
        public event InsideGenerateOrderBillHanlder InsideShouldReceive;

        public event InsideGenerateOrderBillHanlder InsideGenerateOrderOld;

        public InsideCreateOrder()
        {
            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ScCustomsConnectionString"].ConnectionString;
            this.InsidePayExchangeSupplier = System.Configuration.ConfigurationManager.AppSettings["InsidePayExchangeSupplier"];
            InsideGenerateOrderBill += CreateOrderBill;
            InsideQuoteConfirm += QuoteConfirm;
            InsidePreQuoteConfrirm += ToEntryNotice;
            InsidePacking += Packing;
            InsideSeal += Seal;
            InsideShouldReceive += ShouldReceive;
            //IcgooPreQuoteConfrirm += ToEntryNotice;
            InsideGenerateOrderOld += CreateHead;
            InsideGenerateOrderOld += HKAutoOut;
            InsideGenerateOrderOld += SZOnStock;
            InsideGenerateOrderOld += ShouldReceive;
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
                var CustomsTariffsView = new CustomsTariffsView();
                var CountriesView = new BaseCountriesView();

                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject(message.PostData, typeof(DataTable)) as DataTable;
                List<InsideOrderItem> models = new List<InsideOrderItem>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    InsideOrderItem orderItem = new InsideOrderItem();
                    foreach (DataRow row in dt.Rows)
                    {
                        orderItem = new InsideOrderItem();
                        orderItem.No = row["序号"].ToString();
                        orderItem.PreProductID = row["报关号"].ToString();
                        orderItem.CustomsCode = row["海关编码"].ToString();
                        orderItem.ProductName = row["商品名称"].ToString();
                        orderItem.Elements = row["规格型号"].ToString();
                        orderItem.Quantity = Convert.ToDecimal(row["数量(PCS)"].ToString());
                        orderItem.Unit = "007";
                        orderItem.FirstLegalUnit = CustomsTariffsView.Where(m => m.HSCode == row["海关编码"].ToString()).FirstOrDefault()?.Unit1;
                        orderItem.SecondLegalUnit = CustomsTariffsView.Where(m => m.HSCode == row["海关编码"].ToString()).FirstOrDefault()?.Unit2;
                        string place = row["产地"].ToString();
                        orderItem.PlaceOfProduction = CountriesView.Where(code => code.EditionOneCode == place || code.Code == place).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == place || code.Code == place).FirstOrDefault().Code;
                        string jine = row["金额"].ToString();
                        orderItem.UnitPrice = ((Convert.ToDecimal(jine) / ConstConfig.TransPremiumInsurance).ToRound(2) / orderItem.Quantity).ToRound(4);
                        orderItem.TotalDeclarePrice = (Convert.ToDecimal(jine) / ConstConfig.TransPremiumInsurance).ToRound(2);
                        orderItem.Currency = "USD";
                        orderItem.PackNo = row["临时箱号"].ToString();
                        string netw = row["净重"].ToString();
                        orderItem.NetWeight = Math.Round(Convert.ToDecimal(netw), 2, MidpointRounding.AwayFromZero) < 0.01M ? 0.01M : Math.Round(Convert.ToDecimal(netw), 2, MidpointRounding.AwayFromZero);
                        orderItem.GrossWeight = Math.Round(Convert.ToDecimal(netw) * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero) < 0.02M ? 0.02M : Math.Round(Convert.ToDecimal(netw) * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero);
                        orderItem.CaseWeight = Convert.ToDecimal(row["毛重"].ToString());
                        orderItem.Model = row["型号"].ToString();
                        orderItem.TaxName = row["品名"].ToString();
                        orderItem.TaxCode = row["型号信息分类值"].ToString();
                        orderItem.Brand = row["品牌"].ToString();
                        orderItem.TariffRate = Convert.ToDecimal(row["关税"].ToString());
                        orderItem.CompanyName = "杭州比一比电子科技有限公司";
                        orderItem.SupplierName = row["付款公司"].ToString();
                        orderItem.IsInspection = row["是否商检"].ToString() == "True" ? true : false;
                        orderItem.InspFee = row["商检费"].ToString() == "" ? 0 : Convert.ToDecimal(row["商检费"].ToString());
                        orderItem.IsCCC = row["3C"].ToString() == "3C" ? true : false;
                        orderItem.IsSysForbid = false;
                        orderItem.CIQCode = row["检疫码"].ToString();
                        orderItem.DeclareCompany = row["报关公司"].ToString();

                        models.Add(orderItem);
                    }
                }
             
  
                #region // 老的转换方式
                //List<InsideOrderJsonItem> model = JsonConvert.DeserializeObject<List<InsideOrderJsonItem>>(message.PostData.Replace("(PCS)", "").Replace("\"3C\":", "\"CCC\":"));
                //Console.WriteLine("转换对象开始");
                //var models = model.Select(item => new InsideOrderItem
                //{
                //    No = item.序号,
                //    PreProductID = item.报关号,
                //    CustomsCode = item.海关编码,
                //    ProductName = item.商品名称,
                //    Elements = item.规格型号,
                //    Quantity = Convert.ToDecimal(item.数量),
                //    Unit = "007",
                //    FirstLegalUnit = CustomsTariffsView.Where(m => m.HSCode == item.海关编码).FirstOrDefault()?.Unit1,
                //    SecondLegalUnit = CustomsTariffsView.Where(m => m.HSCode == item.海关编码).FirstOrDefault()?.Unit2,
                //    PlaceOfProduction = CountriesView.Where(code => code.EditionOneCode == item.产地 || code.Code == item.产地).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == item.产地 || code.Code == item.产地).FirstOrDefault().Code,
                //    UnitPrice = ((Convert.ToDecimal(item.金额) / ConstConfig.TransPremiumInsurance).ToRound(2) / Convert.ToDecimal(item.数量)).ToRound(4),
                //    TotalDeclarePrice = (Convert.ToDecimal(item.金额) / ConstConfig.TransPremiumInsurance).ToRound(2),
                //    Currency = "USD",
                //    PackNo = item.临时箱号,
                //    NetWeight = Math.Round(Convert.ToDecimal(item.净重), 2, MidpointRounding.AwayFromZero) < 0.01M ? 0.01M : Math.Round(Convert.ToDecimal(item.净重), 2, MidpointRounding.AwayFromZero),
                //    GrossWeight = Math.Round(Convert.ToDecimal(item.净重) * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero) < 0.02M ? 0.02M : Math.Round(Convert.ToDecimal(item.净重) * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero),
                //    CaseWeight = Convert.ToDecimal(item.毛重),
                //    Model = item.型号,
                //    TaxName = item.品名,
                //    TaxCode = item.型号信息分类值,
                //    Brand = item.品牌,
                //    TariffRate = Convert.ToDecimal(item.关税),
                //    CompanyName = "杭州比一比电子科技有限公司",
                //    SupplierName = item.付款公司,
                //    IsInspection = item.是否商检 == "True" ? true : false,
                //    InspFee = item.商检费 == "" ? 0 : Convert.ToDecimal(item.商检费),
                //    IsCCC = item.CCC == "3C" ? true : false,
                //    IsSysForbid = false,
                //    CIQCode = item.检疫码,
                //    DeclareCompany = item.报关公司,
                //}).ToList();

                //Console.WriteLine("转换对象结束");
                #endregion

                //报关公司是 深圳市芯达通供应链管理有限公司 才生成订单
                string declareCompany = System.Configuration.ConfigurationManager.AppSettings["DeclareCompany"];
                if (models[0].DeclareCompany != declareCompany)
                {

                }
                else
                {
                    var Companys = models.Select(item => item.CompanyName).Distinct().ToList();

                    foreach (string company in Companys)
                    {
                        var modelByCompany = models.Where(item => item.CompanyName == company).OrderBy(item => item.PackNo);

                        var limit = 50;
                        for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(modelByCompany.Count() / 50M)); i++)
                        {
                            var mc = modelByCompany.Skip(i * limit).Take(limit).ToList();
                            string orderID = "";
                            Create(mc, ref orderID);
                            Map(message.Summary, orderID);
                        }
                    }
                }

                Console.WriteLine("创建结束!");
            }
        }
        public bool Create(List<InsideOrderItem> model, ref string msg)
        {
            bool isSuccess = true;
            try
            {
                Console.WriteLine("开始创建订单");
                var client = new Needs.Ccs.Services.Views.ClientsView().Where(item => item.Company.Name == model[0].CompanyName).FirstOrDefault();
                if (client == null)
                {
                    msg = "没有该公司";
                    return false;
                }

                string OrderID = "";

                #region 一个订单
                IcgooOrder order = new IcgooOrder();               
                order.Client = client;
                OrderID = order.ID;

                //香港交货方式：送货上门
                order.OrderConsignee = new OrderConsignee();
                order.OrderConsignee.OrderID = OrderID;
                //国内交货方式：Icgoo 送货上门
                order.OrderConsignor = new OrderConsignor();
                order.OrderConsignor.OrderID = OrderID;

                order.Type = OrderType.Inside;
                order.SetAPIAdmin(Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator));
                order.AdminID = client.Merchandiser.ID;
                order.ClientAgreement = client.Agreement;

                order.Currency = model[0].Currency;
                order.IsFullVehicle = false;
                order.IsLoan = false;
                order.PackNo = model.Select(item => item.PackNo).Distinct().Count();
                order.WarpType = Icgoo.WrapType;

                //客户供应商
                var supplier = client.Suppliers.FirstOrDefault();

                if (supplier == null)
                {
                    msg = "请先维护供应商";
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
                var payExchangeSupplier = client.Suppliers.Where(item => item.ID == this.InsidePayExchangeSupplier).FirstOrDefault();
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
                    //var DoneClassifyView = view.Where(item => item.ClassifyStatus == ClassifyStatus.Done && item.CompanyType == CompanyTypeEnums.Inside);                  

                    foreach (InsideOrderItem p in model)
                    {
                        ClassifyProduct classifyProduct = new ClassifyProduct();
                        classifyProduct.Client = order.Client;
                        classifyProduct.Currency = order.Currency;
                        classifyProduct.OrderID = OrderID;


                        classifyProduct.Origin = p.PlaceOfProduction;
                        classifyProduct.Quantity = p.Quantity;
                        classifyProduct.Unit = Icgoo.Gunit;
                        classifyProduct.UnitPrice = Math.Round(p.UnitPrice, 4, MidpointRounding.AwayFromZero);
                        classifyProduct.TotalPrice = p.Quantity * classifyProduct.UnitPrice;
                        classifyProduct.GrossWeight = p.GrossWeight;
                        TotalDeclarePrice += classifyProduct.TotalPrice;
                        //归类结果
                        ClassifyResult classifyResult = view.Where(item => item.ClassifyStatus == ClassifyStatus.Done &&
                                                                   item.CompanyType == CompanyTypeEnums.Inside &&
                                                                   item.PreProductUnicode == p.PreProductID).FirstOrDefault();
                        var advanceProduct = new Needs.Ccs.Services.Views.IcgooPreProductView().Where(item => item.sale_orderline_id == p.PreProductID).FirstOrDefault();
                        if (classifyResult != null)
                        {
                            //if (classifyResult.Type == ItemCategoryType.HKForbid || classifyResult.Type == ItemCategoryType.Forbid)
                            //{
                            //    isSuccess = false;
                            //    msg = "不能提交禁运产品";
                            //    return isSuccess;
                            //}

                            switch (classifyResult.Type)
                            {
                                case ItemCategoryType.Inspection:
                                    classifyProduct.IsInsp = true;
                                    classifyProduct.InspectionFee = classifyResult.InspectionFee.HasValue ? classifyResult.InspectionFee.Value : 0;

                                    break;

                                case ItemCategoryType.CCC:
                                    classifyProduct.IsCCC = true;

                                    break;

                                case ItemCategoryType.Forbid:
                                    classifyProduct.IsSysForbid = true;
                                    break;

                                default:
                                    break;
                            }
                           
                         
                            if (classifyResult.Manufacturer == p.Brand && classifyResult.Model == p.Model)
                            {
                                classifyProduct.Name = classifyResult.ProductName;
                                classifyProduct.Model = p.Model;
                                classifyProduct.Manufacturer = classifyResult.Manufacturer;
                                classifyProduct.Batch = advanceProduct == null ? "" : advanceProduct.Pack;
                                classifyProduct.ClassifyStatus = ClassifyStatus.Done;
                            }
                            else
                            {
                                classifyProduct.Name = classifyResult.ProductName;
                                classifyProduct.Model = p.Model;
                                classifyProduct.Manufacturer = p.Brand;
                                classifyProduct.Batch = advanceProduct == null ? "" : advanceProduct.Pack;
                                classifyProduct.ClassifyStatus = ClassifyStatus.First;
                                isAllClassified = false;
                            }
                                                      
                            OrderItemCategory Category = new OrderItemCategory();
                            //Category.Declarant = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.DeclarantID);
                            Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.ClassifyFirstOperatorID);
                            Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.ClassifySecondOperatorID);
                            Category.Type = classifyResult.Type == null ? ItemCategoryType.Normal : classifyResult.Type.Value;
                            Category.TaxCode = classifyResult.TaxCode;
                            Category.TaxName = classifyResult.TaxName;
                            Category.HSCode = classifyResult.HSCode;
                            Category.Name = classifyResult.ProductName;
                            Category.Elements = classifyResult.Elements;
                            Category.Unit1 = classifyResult.Unit1;
                            Category.Unit2 = classifyResult.Unit2;
                            Category.CIQCode = classifyResult.CIQCode;

                            classifyProduct.Category = Category;
                            var reTariffRate = 0M;
                            if (classifyProduct.Origin != null)
                            {
                                var quarantines = new Needs.Ccs.Services.Views.CustomsQuarantinesView();
                                var quarantine = quarantines.Where(cq => cq.Origin == classifyProduct.Origin && cq.StartDate <= DateTime.Now && cq.EndDate >= DateTime.Now).FirstOrDefault();
                                if (quarantine != null)
                                {
                                    classifyProduct.Category.Type |= ItemCategoryType.Quarantine;
                                }

                                var tariffView = new Needs.Ccs.Services.Views.CustomsOriginTariffsView();
                               
                                var reTariff = tariffView.Where(item => item.Type == CustomsRateType.ImportTax &&
                                                                item.Origin == classifyProduct.Origin &&
                                                                item.CustomsTariffID == classifyProduct.Category.HSCode).FirstOrDefault();
                                if (reTariff != null)
                                {
                                    reTariffRate = reTariff.Rate / 100 ;
                                }                               
                            }

                            OrderItemTax tariff = new OrderItemTax();
                            tariff.Type = CustomsRateType.ImportTax;
                            tariff.Rate = classifyResult.TariffRate.Value + reTariffRate;

                            classifyProduct.ImportTax = tariff;

                            OrderItemTax ValueAddedTax = new OrderItemTax();
                            ValueAddedTax.Type = CustomsRateType.AddedValueTax;
                            ValueAddedTax.Rate = classifyResult.AddedValueRate.Value;
                            classifyProduct.AddedValueTax = ValueAddedTax;

                            classifyProduct.OrderType = OrderType.Inside;
                            classifyProduct.ProductUniqueCode = p.PreProductID;
                            order.ClassifyProducts.Add(classifyProduct);
                        }
                        else
                        {
                            classifyProduct.IsInsp = p.IsInspection;
                            classifyProduct.InspectionFee = p.InspFee;
                            classifyProduct.IsCCC = p.IsCCC;
                            classifyProduct.IsSysForbid = p.IsSysForbid;

                            classifyProduct.Category = new OrderItemCategory();
                            classifyProduct.Category.Type = ItemCategoryType.Normal;
                            if (classifyProduct.IsInsp)
                            {
                                classifyProduct.Category.Type |= ItemCategoryType.Inspection;
                            }
                            if (classifyProduct.IsCCC)
                            {
                                classifyProduct.Category.Type |= ItemCategoryType.CCC;
                            }
                            if (classifyProduct.IsSysForbid)
                            {
                                classifyProduct.Category.Type |= ItemCategoryType.Forbid;
                            }

                            if (classifyProduct.Origin != null)
                            {
                                var quarantines = new Needs.Ccs.Services.Views.CustomsQuarantinesView();
                                var quarantine = quarantines.Where(cq => cq.Origin == classifyProduct.Origin && cq.StartDate <= DateTime.Now && cq.EndDate >= DateTime.Now).FirstOrDefault();
                                if (quarantine != null)
                                {
                                    classifyProduct.Category.Type |= ItemCategoryType.Quarantine;
                                }
                            }


                            classifyProduct.Name = p.ProductName;
                            classifyProduct.Model = p.Model;
                            classifyProduct.Manufacturer = p.Brand;
                            classifyProduct.Batch = advanceProduct == null ? "" : advanceProduct.Pack;                                                    
                            classifyProduct.ClassifyStatus = ClassifyStatus.Done;

                            //classifyProduct.Category.Declarant = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                            classifyProduct.Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                            classifyProduct.Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                            classifyProduct.Category.Type = classifyProduct.Category.Type;
                            classifyProduct.Category.TaxCode = p.TaxCode;
                            classifyProduct.Category.TaxName = p.TaxName;
                            classifyProduct.Category.HSCode = p.CustomsCode;
                            classifyProduct.Category.Name = p.ProductName;
                            classifyProduct.Category.Elements = p.Elements;
                            //一期,法一单位是空的
                            classifyProduct.Category.Unit1 = p.FirstLegalUnit;
                            classifyProduct.Category.Unit2 = p.SecondLegalUnit;
                            classifyProduct.Category.CIQCode = p.CIQCode;



                            OrderItemTax tariff = new OrderItemTax();
                            tariff.Type = CustomsRateType.ImportTax;

                            var tariffView = new Needs.Ccs.Services.Views.CustomsOriginTariffsView();
                            var reTariffRate = 0M;
                            if (classifyProduct.Origin != null)
                            {                              
                                var reTariff = tariffView.Where(item => item.Type == CustomsRateType.ImportTax &&
                                                            item.Origin == classifyProduct.Origin &&
                                                            item.CustomsTariffID == classifyProduct.Category.HSCode).FirstOrDefault();
                                if (reTariff != null)
                                {
                                    reTariffRate = reTariff.Rate / 100;
                                }                              
                            }
                               
                            tariff.Rate = Convert.ToDecimal(p.TariffRate)+ reTariffRate;
                            classifyProduct.ImportTax = tariff;

                            OrderItemTax ValueAddedTax = new OrderItemTax();
                            ValueAddedTax.Type = CustomsRateType.AddedValueTax;
                            ValueAddedTax.Rate = InsideOrder.ValueAddedTaxRate;
                            classifyProduct.AddedValueTax = ValueAddedTax;

                            classifyProduct.OrderType = OrderType.Inside;
                            classifyProduct.ProductUniqueCode = p.PreProductID;
                            order.ClassifyProducts.Add(classifyProduct);
                        }


                    }
                }
                order.DeclarePrice = model.Sum(t => t.TotalDeclarePrice);

                if (isAllClassified)
                {
                    order.OrderStatus = OrderStatus.Classified;
                    order.ConnectionString = this.ConnectionString;
                    order.EnterSpeed();
                    //this.OnCreated(new InsideCreateOrderEventArgs(order, model));
                }
                else
                {
                    order.OrderStatus = OrderStatus.Confirmed;
                    order.ConnectionString = this.ConnectionString;
                    order.EnterSpeed();
                   // this.OnPartCreated(new InsideCreateOrderEventArgs(order, model));
                }
                Console.WriteLine("创建订单结束");

                #endregion

                msg = OrderID;
                return isSuccess;


            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        private void Map(string IcgooOrder, string OrderID)
        {
            IcgooMap map = new IcgooMap();
            map.ID = ChainsGuid.NewGuidUp();
            map.IcgooOrder = IcgooOrder;
            map.OrderID = OrderID;
            map.CompanyType = CompanyTypeEnums.Inside;
            map.Enter();
        }

        public virtual void OnCreated(InsideCreateOrderEventArgs args)
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

        public virtual void OnPartCreated(InsideCreateOrderEventArgs args)
        {
            Console.WriteLine(args.order.ID + "开始确认报价");
            this.InsidePreQuoteConfrirm?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "报价确认结束");
            Console.WriteLine(args.order.ID + "开始装箱");
            this.InsidePacking?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "装箱结束");
            Console.WriteLine(args.order.ID + "开始封箱");
            this.InsideSeal?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "封箱结束");
        }

        public virtual void OnOldCreated(InsideCreateOrderEventArgs args)
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
            Console.WriteLine(args.order.ID + "开始生成报关单");
            this.InsideGenerateOrderOld?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "报关单生成结束");
        }

        /// <summary>
        /// 生成对账单
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CreateOrderBill(object sender, InsideCreateOrderEventArgs e)
        {
            e.order.GenerateBillSpeed();
        }



        /// <summary>
        /// 客户确认
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void QuoteConfirm(object sender, InsideCreateOrderEventArgs e)
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
        public void ToEntryNotice(object sender, InsideCreateOrderEventArgs e)
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
        public void Packing(object sender, InsideCreateOrderEventArgs e)
        {
            var EnterNoticeId = new Needs.Ccs.Services.Views.EntryNoticeView().Where(item => item.Order.ID == e.order.ID).Select(item => item.ID).FirstOrDefault();
            string OrderId = e.order.ID;
            var Boxes = e.partno.Select(item => item.PackNo).Distinct().ToList();
            IcgooHKSortingContext hkSorting;

            List<string> OrderItemIDs = new List<string>();

            Boxes.ForEach(b =>
            {
                var Products = e.partno.Where(item => item.PackNo == b).ToList();
                hkSorting = new IcgooHKSortingContext();
                hkSorting.ToShelve(Icgoo.InsideShelveNumber, b.ToString());

                //创建packing对象
                PackingModel packing = new PackingModel();
                packing.AdminID = Icgoo.DefaultCreator;
                packing.OrderID = OrderId;
                packing.BoxIndex = b.ToString();
                packing.Weight = Products.Select(item => Convert.ToDecimal(item.GrossWeight)).Sum();
                packing.WrapType = Icgoo.WrapType;
                packing.PackingDate = DateTime.Now;
                packing.Quantity = Products.Select(item => Convert.ToDecimal(item.Quantity)).Sum();
                hkSorting.SetPacking(packing);

                List<InsideSortingModel> list = new List<InsideSortingModel>();

                Products.ForEach(p =>
                {
                    #region 防止一个订单中，有型号，和数量一样的情况出现
                    //var orderitem = e.order.Items.Where(item => item.Product.Model == p.Model && item.Quantity == Convert.ToDecimal(p.Quantity)).FirstOrDefault();
                    var orderitems = e.order.Items.Where(item => item.Model == p.Model && item.Quantity == Convert.ToDecimal(p.Quantity)).ToList();
                    var orderitem = new Needs.Ccs.Services.Models.OrderItem();
                    if (orderitems.Count == 1)
                    {
                        orderitem = orderitems[0];
                    }
                    else
                    {
                        foreach (var t in orderitems)
                        {
                            if (OrderItemIDs.Where(m => m == t.ID).Count() == 0)
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
                        model.OrderItemID = orderitem.ID;
                        model.Quantity = Convert.ToDecimal(p.Quantity);
                        model.NetWeight = p.NetWeight;
                        model.GrossWeight = p.GrossWeight;
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
        public void Seal(object sender, InsideCreateOrderEventArgs e)
        {
            var EnterNoticeId = new Needs.Ccs.Services.Views.EntryNoticeView().Where(item => item.Order.ID == e.order.ID).Select(item => item.ID).FirstOrDefault();
            var entryNotice = new Needs.Ccs.Services.Views.IcgooHKEntryNoticeView()[EnterNoticeId];

            entryNotice.SetAdmin(Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator));
            entryNotice.ConnectionString = this.ConnectionString;
            entryNotice.SealSpeed();

        }


        public DataTable dtOld()
        {
            var workbook = ExcelFactory.ReadFile(@"C:\Users\user1\Desktop\OLD\SJ201906-24-013.xlsx");
            var npoi = new NPOIHelper(workbook);
            DataTable dt2 = npoi.ExcelToDataTable(false);

            return dt2;
        }

        public void CreateOld()
        {
            DataTable dt = dtOld();

            #region //转换
            Console.WriteLine("转换对象开始");

            var CustomsTariffsView = new CustomsTariffsView();
            var CountriesView = new BaseCountriesView();

            List<InsideOrderItem> models = new List<InsideOrderItem>();
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                InsideOrderItem p = new InsideOrderItem();
                p.No = dt.Rows[i][0].ToString();
                p.PreProductID = dt.Rows[i][48].ToString();//实际放的是合同号
                p.CustomsCode = dt.Rows[i][1].ToString();
                p.ProductName = dt.Rows[i][2].ToString();
                p.Elements = dt.Rows[i][3].ToString();
                p.Quantity = Convert.ToDecimal(dt.Rows[i][4].ToString());
                p.Unit = "007";
                p.FirstLegalUnit = CustomsTariffsView.Where(m => m.HSCode == p.CustomsCode).FirstOrDefault()?.Unit1;
                p.SecondLegalUnit = CustomsTariffsView.Where(m => m.HSCode == p.CustomsCode).FirstOrDefault()?.Unit2;
                p.PlaceOfProduction = CountriesView.Where(code => code.EditionOneCode == dt.Rows[i][28].ToString() || code.Code == dt.Rows[i][28].ToString()).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == dt.Rows[i][28].ToString() || code.Code == dt.Rows[i][28].ToString()).FirstOrDefault().Code;
                p.UnitPrice = ((Convert.ToDecimal(dt.Rows[i][12].ToString()) / ConstConfig.TransPremiumInsurance).ToRound(2) / Convert.ToDecimal(p.Quantity)).ToRound(4);
                p.TotalDeclarePrice = (Convert.ToDecimal(dt.Rows[i][12].ToString()) / ConstConfig.TransPremiumInsurance).ToRound(2);
                p.Currency = "USD";
                p.PackNo = dt.Rows[i][22].ToString();
                p.NetWeight = Math.Round(Convert.ToDecimal(dt.Rows[i][19].ToString()), 2, MidpointRounding.AwayFromZero) < 0.01M ? 0.01M : Math.Round(Convert.ToDecimal(dt.Rows[i][19].ToString()), 2, MidpointRounding.AwayFromZero);
                p.GrossWeight = Math.Round(Convert.ToDecimal(dt.Rows[i][19].ToString()) * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero) < 0.02M ? 0.02M : Math.Round(Convert.ToDecimal(dt.Rows[i][19].ToString()) * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero);
                p.CaseWeight = Convert.ToDecimal(dt.Rows[i][20].ToString());
                p.Model = dt.Rows[i][27].ToString();
                p.TaxName = dt.Rows[i][24].ToString();
                p.TaxCode = dt.Rows[i][44].ToString();
                p.Brand = dt.Rows[i][26].ToString();
                p.TariffRate = Convert.ToDecimal(dt.Rows[i][37].ToString());
                p.CompanyName = "杭州比一比电子科技有限公司";
                p.SupplierName = dt.Rows[i][40].ToString();
                p.IsInspection = dt.Rows[i][41].ToString().ToUpper() == "TRUE" ? true : false;
                p.InspFee = 0;
                p.IsCCC = false;
                p.IsSysForbid = false;
                p.CIQCode = dt.Rows[i][47].ToString();
                p.DeclareCompany = dt.Rows[i][35].ToString(); //实际放的是 大赢家单号

                models.Add(p);
            }

            Console.WriteLine("转换对象结束");
            #endregion


            var Contracts = models.Select(item => item.PreProductID).Distinct().ToList();

            foreach (string contract in Contracts)
            {
                var modelByContract = models.Where(item => item.PreProductID == contract).OrderBy(item => item.PackNo).ToList();
                string orderID = "";
                CreateOld(modelByContract, ref orderID);
                Map(modelByContract[0].DeclareCompany, orderID);
            }
        }

        public bool CreateOld(List<InsideOrderItem> model, ref string msg)
        {
            bool isSuccess = true;
            try
            {
                Console.WriteLine("开始创建订单");
                var client = new Needs.Ccs.Services.Views.ClientsView().Where(item => item.Company.Name == model[0].CompanyName).FirstOrDefault();
                if (client == null)
                {
                    msg = "没有该公司";
                    return false;
                }

                string ContractNo = model[0].PreProductID;
                string[] contracts = ContractNo.Split('-');

                string OrderID = "NL020" + contracts[0].Replace("SJ", "") + contracts[1] + Convert.ToInt16(contracts[2]).ToString().PadLeft(3, '0');
                //string OrderID = "NL02020190620011";

                #region 一个订单
                IcgooOrder order = new IcgooOrder();
                order.Client = client;
                order.ID = OrderID;

                //香港交货方式：送货上门
                order.OrderConsignee = new OrderConsignee();
                order.OrderConsignee.OrderID = OrderID;
                //国内交货方式：Icgoo 送货上门
                order.OrderConsignor = new OrderConsignor();
                order.OrderConsignor.OrderID = OrderID;

                order.Type = OrderType.Inside;
                order.SetAPIAdmin(Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator));
                order.AdminID = client.Merchandiser.ID;
                order.ClientAgreement = client.Agreement;

                order.Currency = model[0].Currency;
                order.IsFullVehicle = false;
                order.IsLoan = false;
                order.PackNo = model.Select(item => item.PackNo).Distinct().Count();
                order.WarpType = Icgoo.WrapType;

                //客户供应商
                var supplier = client.Suppliers.FirstOrDefault();

                if (supplier == null)
                {
                    msg = "请先维护供应商";
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
                var payExchangeSupplier = client.Suppliers.Where(item => item.ID == this.InsidePayExchangeSupplier).FirstOrDefault();
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
                    //var DoneClassifyView = view.Where(item => item.ClassifyStatus == ClassifyStatus.Done && item.CompanyType == CompanyTypeEnums.Inside);                  

                    foreach (InsideOrderItem p in model)
                    {
                        ClassifyProduct classifyProduct = new ClassifyProduct();
                        classifyProduct.Client = order.Client;
                        classifyProduct.Currency = order.Currency;
                        classifyProduct.OrderID = OrderID;


                        classifyProduct.Origin = p.PlaceOfProduction;
                        classifyProduct.Quantity = p.Quantity;
                        classifyProduct.Unit = Icgoo.Gunit;
                        classifyProduct.UnitPrice = Math.Round(p.UnitPrice, 4, MidpointRounding.AwayFromZero);
                        classifyProduct.TotalPrice = p.TotalDeclarePrice;
                        classifyProduct.GrossWeight = p.GrossWeight;
                        TotalDeclarePrice += classifyProduct.TotalPrice;



                        classifyProduct.IsInsp = p.IsInspection;
                        classifyProduct.InspectionFee = p.InspFee;
                        classifyProduct.IsCCC = p.IsCCC;
                        classifyProduct.IsSysForbid = p.IsSysForbid;

                        classifyProduct.Category = new OrderItemCategory();
                        classifyProduct.Category.Type = ItemCategoryType.Normal;
                        if (classifyProduct.IsInsp)
                        {
                            classifyProduct.Category.Type |= ItemCategoryType.Inspection;
                        }
                        if (classifyProduct.IsCCC)
                        {
                            classifyProduct.Category.Type |= ItemCategoryType.CCC;
                        }
                        if (classifyProduct.IsSysForbid)
                        {
                            classifyProduct.Category.Type |= ItemCategoryType.Forbid;
                        }

                        if (classifyProduct.Origin != null)
                        {
                            var quarantines = new Needs.Ccs.Services.Views.CustomsQuarantinesView();
                            var quarantine = quarantines.Where(cq => cq.Origin == classifyProduct.Origin && cq.StartDate <= DateTime.Now && cq.EndDate >= DateTime.Now).FirstOrDefault();
                            if (quarantine != null)
                            {
                                classifyProduct.Category.Type |= ItemCategoryType.Quarantine;
                            }
                        }


                        classifyProduct.Name = p.ProductName;
                        classifyProduct.Model = p.Model;
                        classifyProduct.Manufacturer = p.Brand;                       
                        classifyProduct.ClassifyStatus = ClassifyStatus.Done;


                        classifyProduct.Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                        classifyProduct.Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                        classifyProduct.Category.Type = classifyProduct.Category.Type;
                        classifyProduct.Category.TaxCode = p.TaxCode;
                        classifyProduct.Category.TaxName = p.TaxName;
                        classifyProduct.Category.HSCode = p.CustomsCode;
                        classifyProduct.Category.Name = p.ProductName;
                        classifyProduct.Category.Elements = p.Elements;
                        //一期,法一单位是空的
                        classifyProduct.Category.Unit1 = p.FirstLegalUnit;
                        classifyProduct.Category.Unit2 = p.SecondLegalUnit;
                        classifyProduct.Category.CIQCode = p.CIQCode;



                        OrderItemTax tariff = new OrderItemTax();
                        tariff.Type = CustomsRateType.ImportTax;
                        tariff.Rate = Convert.ToDecimal(p.TariffRate);
                        classifyProduct.ImportTax = tariff;

                        OrderItemTax ValueAddedTax = new OrderItemTax();
                        ValueAddedTax.Type = CustomsRateType.AddedValueTax;
                        ValueAddedTax.Rate = InsideOrder.ValueAddedTaxRate;
                        classifyProduct.AddedValueTax = ValueAddedTax;

                        classifyProduct.OrderType = OrderType.Inside;
                        classifyProduct.ProductUniqueCode = p.PreProductID;
                        order.ClassifyProducts.Add(classifyProduct);



                    }
                }
                order.DeclarePrice = TotalDeclarePrice;


                order.OrderStatus = OrderStatus.Confirmed;
                order.ConnectionString = this.ConnectionString;

                order.CustomsExchangeRate = 6.8649M;
                order.RealExchangeRate = 6.8915M;

                order.EnterSpeed();
                //this.OnOldCreated(new InsideCreateOrderEventArgs(order, model));

                Console.WriteLine("创建订单结束");

                #endregion

                msg = OrderID;
                return isSuccess;


            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

        public void CreateHead(object sender, InsideCreateOrderEventArgs e)
        {
            Purchaser purchaser = PurchaserContext.Current;
            Vendor vendor = new VendorContext(VendorContextInitParam.OrderID, e.order.ID).Current1;

            #region 组装实体
            Needs.Ccs.Services.Models.DecHead head = new Needs.Ccs.Services.Models.DecHead();
            ///  head.ID = Needs.Overall.PKeySigner.Pick(PKeyType.DecHead);
            head.ID = string.Concat(PurchaserContext.Current.DecHeadIDPrefix, Needs.Overall.PKeySigner.Pick(PKeyType.DecHead));

            var declarationNotice = new Needs.Ccs.Services.Views.DeclarationNoticesView().Where(item => item.OrderID == e.order.ID).FirstOrDefault();

            head.DeclarationNoticeID = declarationNotice.ID;
            head.OrderID = e.order.ID;
            head.CustomMaster = "5303";
            head.SeqNo = "";
            head.PreEntryId = "";
            head.EntryId = "";
            head.IEPort = "5303";


            string date = e.order.ID.Substring(5, 8).Insert(4, "-").Insert(7, "-");
            head.ContrNo = e.partno[0].PreProductID;

            head.IEDate = e.order.ID.Substring(5, 8);
            head.DDate = Convert.ToDateTime(date);
            head.ConsigneeName = "深圳市创新恒远供应链管理有限公司";
            head.ConsigneeScc = "91440300687582405X";
            head.ConsigneeCusCode = "4453066631";
            head.ConsigneeCiqCode = "4700629085";

            head.ConsignorName = "HONGKONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED";
            head.ConsignorCode = "香港畅运国际物流有限公司";
            head.OwnerName = "杭州比一比电子科技有限公司";
            head.OwnerScc = "91330110MA2B2B1675";
            head.OwnerCusCode = "MA2B2B167";
            head.OwnerCiqCode = "";
            head.AgentName = "深圳市创新恒远供应链管理有限公司";
            head.AgentScc = "91440300687582405X";
            head.AgentCusCode = "4453066631";
            head.AgentCiqCode = "4700629085";
            head.TrafMode = "4";

            head.VoyNo = "1100319013932";
            head.BillNo = "CX0531001";

            head.TradeMode = "0110";
            head.CutMode = "101";

            head.TradeCountry = "HKG";
            head.DistinatePort = "HKG000";
            head.TransMode = 1;

            var sorting = new Needs.Ccs.Services.Views.HKSortingsView().Where(item => item.OrderID == e.order.ID);

            head.PackNo = sorting.Select(p => p.BoxIndex).Distinct().Count();
            head.WrapType = "22";
            head.GrossWt = Math.Ceiling(sorting.Select(p => p.GrossWeight).Sum()) < 2 ? 2 : Math.Ceiling(sorting.Select(p => p.GrossWeight).Sum());
            head.NetWt = sorting.Select(p => p.NetWeight).Sum().ToRound(2) < 1 ? 1 : sorting.Select(p => p.NetWeight).Sum().ToRound(2);
            head.TradeAreaCode = "HKG";
            head.EntyPortCode = "470501";
            head.GoodsPlace = "深圳市龙华区龙华街道富康社区东环二路110号中执时代广场B栋16H";
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
            head.CreateTime = Convert.ToDateTime(date); ;

            //订单
            var order = new Needs.Ccs.Services.Views.OrdersView().Where(p => p.ID == e.order.ID).FirstOrDefault();

            #region 表体信息

            var DeclarationNoticeItems = declarationNotice.Items.AsQueryable();

            var i = 1;

            List<string> orderItemIDS = new List<string>();

            var sortingorder = new Needs.Ccs.Services.Views.HKSortingsView().Where(item => item.OrderID == e.order.ID).
                               OrderBy(item => item.BoxIndex).Select(item => item.ID);

            var CountryNameView = new Needs.Ccs.Services.Views.BaseCountriesView();

            //按装箱顺序排列
            foreach (var itemorder in sortingorder)
            {
                var item = DeclarationNoticeItems.Where(t => t.Sorting.ID == itemorder).FirstOrDefault();
                orderItemIDS.Add(item.Sorting.OrderItem.ID);
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
                list.OriginCountry = item.Sorting.OrderItem.Origin;
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

            #endregion

            #region 商检/检疫                   
            //判断是否商检/检疫,不是判断这个订单中的所有项是否要商检，而是判断该报关单的所有型号是否要商检，一个订单可以拆分成好几个报关单
            var isQuarantines = e.order.Items.Where(item => orderItemIDS.Contains(item.ID)).Any(t => (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Quarantine.GetHashCode()) > 0);
            var isInspections = e.order.Items.Where(item => orderItemIDS.Contains(item.ID)).Any(
                t => (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.Inspection.GetHashCode()) > 0
                || (t.Category.Type.GetHashCode() & Needs.Ccs.Services.Enums.ItemCategoryType.CCC.GetHashCode()) > 0);

            //一车只有一单检疫的
            isQuarantines = isQuarantines && !new Needs.Ccs.Services.Views.DecHeadsListView().Any(t => t.VoyageID == head.VoyNo && t.IsQuarantine && t.Status != "04");

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
                head.CmplDschrgDt = Convert.ToDateTime(date).AddDays(3).ToString("yyyyMMdd");

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

            //foreach (var item in head.Lists)
            //{
            //    if (head.IsInspection || head.IsQuarantine.Value)
            //    {
            //        item.GoodsSpec = ";;;;;" + item.GoodsModel + ";" + item.GoodsBrand + ";"+ order.Items.Where(t => t.ID == item.OrderItemID).Select(t=>t.Product.Batch) + ";";
            //        item.Purpose = "99";//用途 默认：99 其它
            //        item.GoodsAttr = "12,19";//默认：19 正常
            //    }
            //}


            #endregion
            head.CreateDeclare();
        }

        public void HKAutoOut(object sender, InsideCreateOrderEventArgs e)
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
                head.SZWarehouseAutoOut();
            }
        }

        public void SZOnStock(object sender, InsideCreateOrderEventArgs e)
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

            string voyageID = "1100319013932";
            string stockCode = "InsideStock";

            new Needs.Ccs.Services.Models.SZEntryNotice().OnStock(voyageID, stockCode, listBox);


            new Needs.Ccs.Services.Models.SZEntryNotice().Complete(voyageID);
        }

        public void ShouldReceive(object sender, InsideCreateOrderEventArgs e)
        {
            e.order.ToReceivables();
        }

        public DataTable dtOldCustoms()
        {
            var workbook = ExcelFactory.ReadFile(@"C:\Users\user1\Desktop\OLD\6-18update.xls");
            var npoi = new NPOIHelper(workbook);
            DataTable dt2 = npoi.ExcelToDataTable(false);

            return dt2;
        }

        public void UpdateOld()
        {
            DataTable dt = dtOldCustoms();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(
                        new
                        {
                            BillNo = dt.Rows[i][3].ToString(),
                            VoyNo = dt.Rows[i][9].ToString(),
                            EntryId = dt.Rows[i][4].ToString(),
                            SeqNo = dt.Rows[i][5].ToString(),
                        },
                        item => item.ContrNo == dt.Rows[i][6].ToString());

                    string orderid = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(item => item.ContrNo == dt.Rows[i][6].ToString()).FirstOrDefault().OrderID;
                    string DeclarationNoticesID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>().Where(item => item.OrderID == orderid).FirstOrDefault().ID;

                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>().Where(item => item.DecNoticeID == DeclarationNoticesID).Count();
                    if (count == 0)
                    {
                        var decvoyages = new Layer.Data.Sqls.ScCustoms.DecNoticeVoyages
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            DecNoticeID = DeclarationNoticesID,
                            VoyageID = dt.Rows[i][9].ToString(),
                            AdminID = "Admin0000000309",
                            Status = (int)Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = "",
                        };

                        reponsitory.Insert(decvoyages);
                    }
                }
            }
        }


        public DataTable dtEditionOne()
        {
            var workbook = ExcelFactory.ReadFile(@"C:\Users\user1\Desktop\OLD\EditionOne\5.29-5.31.xlsx");
            var npoi = new NPOIHelper(workbook);
            DataTable dt2 = npoi.ExcelToDataTable(false);

            return dt2;
        }

        public void UpdateEditionOne()
        {
            DataTable dt = dtEditionOne();

            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                for (int i = 1; i < dt.Rows.Count; i++)
                {
                    reponsitory.Update<Layer.Data.Sqls.ScCustoms.DecHeads>(
                        new
                        {                            
                            EntryId = dt.Rows[i][1].ToString(),                          
                        },
                        item => item.BillNo == dt.Rows[i][4].ToString());

                    string orderid = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecHeads>().Where(item => item.BillNo == dt.Rows[i][4].ToString()).FirstOrDefault().OrderID;
                    string DeclarationNoticesID = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DeclarationNotices>().Where(item => item.OrderID == orderid).FirstOrDefault().ID;

                    int count = reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.DecNoticeVoyages>().Where(item => item.DecNoticeID == DeclarationNoticesID).Count();
                    if (count == 0)
                    {
                        var decvoyages = new Layer.Data.Sqls.ScCustoms.DecNoticeVoyages
                        {
                            ID = ChainsGuid.NewGuidUp(),
                            DecNoticeID = DeclarationNoticesID,
                            VoyageID = dt.Rows[i][9].ToString(),
                            AdminID = "XDTAdmin",
                            Status = (int)Status.Normal,
                            CreateDate = DateTime.Now,
                            UpdateDate = DateTime.Now,
                            Summary = "",
                        };

                        reponsitory.Insert(decvoyages);
                    }
                }
            }
        }


    }
}
