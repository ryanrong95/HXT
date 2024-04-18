using Needs.Ccs.Services;
using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Utils.Npoi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Wl.PlanningService.Services.Order
{
    public class CreateDyjOrder
    {
        public class InsidePackingModel
        {
            public int AllCount { get; set; }

            public List<string> boxes { get; set; }

            public InsidePackingModel()
            {
                boxes = new List<string>();
            }
        }


        public string ConnectionString { get; set; }
        public string InsidePayExchangeSupplier { get; set; }
        public string DeclareCompanyName { get; set; }
        public string AgentCompanyName { get; set; }
        public event InsideGenerateOrderBillHanlder InsideGenerateOrderBill;
        public event InsideQuoteConfirmHandler InsideQuoteConfirm;
        public event InsideQuoteConfirmHandler InsidePreQuoteConfrirm;
        public event InsidePackingHandler InsidePacking;
        public event InsideSealHandler InsideSeal;
        public event InsideGenerateOrderBillHanlder InsideShouldReceive;
        public event InsideProductSupplierHandler InsideProductSupplierMap;

        public CreateDyjOrder(string PayExchangeSupplier, string declarecompany, string agentcompanyname)
        {
            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["ScCustomsConnectionString"].ConnectionString;
            this.InsidePayExchangeSupplier = PayExchangeSupplier;
            this.DeclareCompanyName = declarecompany;
            this.AgentCompanyName = agentcompanyname;
            InsideGenerateOrderBill += CreateOrderBill;
            InsideQuoteConfirm += QuoteConfirm;
            InsidePreQuoteConfrirm += ToEntryNotice;
            InsidePacking += Packing;
            InsideSeal += Seal;
            InsideShouldReceive += ShouldReceive;
            InsideProductSupplierMap += ProductSupplierMap;
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
                var CustomsTariffsView = new Needs.Ccs.Services.Views.CustomsTariffsView();
                var CountriesView = new Needs.Ccs.Services.Views.BaseCountriesView();

                #region //转换              
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject(message.PostData, typeof(DataTable)) as DataTable;
                List<InsideOrderItem> models = new List<InsideOrderItem>();

                #region 供应商--废弃
                //Dictionary<string, string> SupplierMap = new Dictionary<string, string>();
                //var Suppliers = dt.AsEnumerable().Select(c => c.Field<string>("付款公司")).ToList().Distinct();
                //var client = new Needs.Ccs.Services.Views.ClientsView().Where(item => item.Company.Name == "杭州比一比电子科技有限公司").FirstOrDefault();
                //var SupplierView = new Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client.ID);
                //foreach (var supplierName in Suppliers)
                //{
                //    var supplier = SupplierView.Where(t => t.Name == supplierName || t.ChineseName == supplierName).FirstOrDefault();
                //    if (supplier == null)
                //    {
                //        var saveSupplier = new Needs.Ccs.Services.Models.ClientSupplier();
                //        saveSupplier.Name = supplierName;
                //        saveSupplier.ClientID = client.ID;
                //        saveSupplier.ChineseName = supplierName;
                //        saveSupplier.Enter();

                //        var newsupplier = SupplierView.Where(t => t.Name == supplierName || t.ChineseName == supplierName).FirstOrDefault();
                //        SupplierMap.Add(supplierName, newsupplier.ID);
                //    }
                //    else
                //    {
                //        SupplierMap.Add(supplierName, supplier.ID);
                //    }
                //}
                #endregion

                if (dt != null && dt.Rows.Count > 0)
                {

                    if (dt.Rows[0]["报关公司"].ToString() == DeclareCompanyName)
                    {

                        var client1 = new Needs.Ccs.Services.Views.ClientsView().Where(item => item.Company.Name == this.AgentCompanyName).FirstOrDefault();
                        var SupplierView1 = new Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID).ToList();

                        InsideOrderItem orderItem = new InsideOrderItem();
                        foreach (DataRow row in dt.Rows)
                        {

                            #region  整理供应商
                            //Anda International Trade Group Limited
                            //HK Lianchuang Electronics Co., Limited
                            //IC360 Electronics Limited
                            //IC360 Group Limited
                            //Hongkong Hongtu International Logistics co., ltd.   
                            //HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO., LIMITED
                            //B ONE B ELECTRONIC CO., LIMITED
                            //HK Huanyu Electronics Technology co., ltd. 
                            //ICGOO GROUP LIMITED
                            //HONG KONG WANLUTONG INTERNATIONAL LOGISTICS CO LIMITED
                            //KB ELECTRONICS DEVELOPMENT LIMITED
                            var SupplierName = row["付款公司"].ToString();
                            switch (SupplierName.ToLower())
                            {
                                case "hk lianchuang electronics co.,limited":
                                    SupplierName = "HK Lianchuang Electronics Co.,Limited";
                                    break;
                                case "ic360 electronics limited":
                                    SupplierName = "IC360 Electronics Limited";
                                    break;
                                case "ic360 group limited":
                                    SupplierName = "IC360 Group Limited";
                                    break;
                                case "hongkong hongtu international logistics co., ltd.":
                                case "hong kong changyun international logistics co.,limited":
                                    SupplierName = "HONG KONG CHANGYUN INTERNATIONAL LOGISTICS CO.,LIMITED";
                                    break;
                                case "b one b electronic co.,limited":
                                    SupplierName = "B ONE B ELECTRONIC CO.,LIMITED";
                                    break;
                                case "hk huanyu electronics technology co.,limited":
                                    SupplierName = "HK HUANYU ELECTRONICS TECHNOLOGY CO.,LIMITED";
                                    break;
                                case "icgoo group limited":
                                    SupplierName = "ICGOO GROUP LIMITED";
                                    break;
                                case "hong kong wanlutong international logistics co limited":
                                    SupplierName = "HONG KONG WANLUTONG INTERNATIONAL LOGISTICS CO LIMITED";
                                    break;
                                case "kb electronics development limited":
                                    SupplierName = "KB ELECTRONICS DEVELOPMENT LIMITED";
                                    break;
                                default:
                                    SupplierName = "Anda International Trade Group Limited";
                                    break;
                            }

                            var supplier = SupplierView1.Find(t => t.Name == SupplierName);
                            if (supplier == null)
                            {
                                var SupplierView2 = new Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID).ToList();
                                if (SupplierView2.Find(t => t.Name == SupplierName) == null)
                                {
                                    var saveSupplier = new Needs.Ccs.Services.Models.ClientSupplier();
                                    saveSupplier.Name = SupplierName;
                                    saveSupplier.ClientID = client1.ID;
                                    saveSupplier.ChineseName = SupplierName;
                                    saveSupplier.Enter();
                                    supplier = new Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID && item.Name == SupplierName)?.FirstOrDefault();
                                }
                            }
                            #endregion


                            if (string.IsNullOrEmpty(row["海关编码"].ToString()))
                            {
                                continue;
                            }
                            orderItem = new InsideOrderItem();
                            orderItem.No = row["序号"].ToString();
                            orderItem.PreProductID = row["报关号"].ToString();
                            orderItem.CustomsCode = row["海关编码"].ToString();
                            orderItem.ProductName = row["商品名称"].ToString();
                            orderItem.Elements = row["规格型号"].ToString();
                            orderItem.Quantity = Convert.ToDecimal(row["数量(PCS)"].ToString());
                            orderItem.Unit = "007";
#pragma warning disable
#if PvData
                            try
                            {
                                var apisetting = new PvDataApiSetting();
                                var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.GetTariff;

                                //调用中心数据接口获取海关税则信息
                                var result = Needs.Utils.Http.ApiHelper.Current.Get<Needs.Underly.JSingle<dynamic>>(url, new
                                {
                                    hsCode = row["海关编码"].ToString()
                                });
                                if (result.code == 200)
                                {
                                    orderItem.FirstLegalUnit = result.data.LegalUnit1;
                                    orderItem.SecondLegalUnit = result.data.LegalUnit2;
                                }
                            }
                            catch (Exception ex)
                            {
                                string receivers = ConfigurationManager.AppSettings["Receivers"].ToString();
                                SmtpContext.Current.Send(receivers, "获取海关税则接口调用异常", "调用时间：【" + DateTime.Now + "】, 异常原因：【" + ex.Message + "】");
                            }
#else
                            orderItem.FirstLegalUnit = CustomsTariffsView.Where(m => m.HSCode == row["海关编码"].ToString()).FirstOrDefault()?.Unit1;
                            orderItem.SecondLegalUnit = CustomsTariffsView.Where(m => m.HSCode == row["海关编码"].ToString()).FirstOrDefault()?.Unit2;
#endif
#pragma warning restore
                            string place = row["产地"].ToString().Trim();
                            orderItem.PlaceOfProduction = CountriesView.Where(code => code.EditionOneCode == place || code.Code == place || code.EnglishName == place).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == place || code.Code == place || code.EnglishName == place).FirstOrDefault().Code;
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
                            orderItem.CompanyName = this.AgentCompanyName;
                            //orderItem.SupplierName = row["付款公司"].ToString();
                            //orderItem.SupplierID = SupplierMap[orderItem.SupplierName];
                            orderItem.SupplierName = SupplierName;
                            orderItem.SupplierID = supplier.ID;
                            orderItem.IsInspection = row["是否商检"].ToString() == "True" ? true : false;
                            orderItem.InspFee = row["商检费"].ToString() == "" ? 0 : Convert.ToDecimal(row["商检费"].ToString());
                            orderItem.IsCCC = row["3C"].ToString() == "3C" ? true : false;
                            orderItem.IsSysForbid = false;
                            orderItem.CIQCode = row["检疫码"].ToString();
                            orderItem.DeclareCompany = row["报关公司"].ToString();

                            models.Add(orderItem);

                        }
                    }
                }
                #endregion


                #region 老的装箱方式，按照箱号排序，每50个装一箱
                //var Companys = models.Select(item => item.CompanyName).Distinct().ToList();

                //foreach (string company in Companys)
                //{
                //    var modelByCompany = models.Where(item => item.CompanyName == company).OrderBy(item => item.PackNo);

                //    var limit = 50;
                //    for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(modelByCompany.Count() / 50M)); i++)
                //    {
                //        var mc = modelByCompany.Skip(i * limit).Take(limit).ToList();
                //        string orderID = "";
                //        Create(mc, ref orderID);
                //        Map(message.Summary, orderID);
                //    }
                //}
                #endregion

                #region 新的装箱方式，每个箱号里的，每个供应商，为一单
                //var PackNos = models.Select(item => item.PackNo).Distinct().ToList();
                //foreach (string packno in PackNos)
                //{
                //    var modelByPackNo = models.Where(item => item.PackNo == packno);
                //    var suppliers = modelByPackNo.Select(item => item.SupplierName).Distinct().ToList();
                //    foreach (string supplier in suppliers)
                //    {
                //        var modelBySupplier = modelByPackNo.Where(item => item.SupplierName == supplier);
                //        var limit = 50;
                //        for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(modelBySupplier.Count() / 50M)); i++)
                //        {
                //            var mc = modelBySupplier.Skip(i * limit).Take(limit).ToList();
                //            string orderID = "";
                //            Create(mc, ref orderID);
                //            Map(message.Summary, orderID);
                //        }
                //    }
                //}
                #endregion

                #region 20191121大赢家拆分规则
                //1、筛选一箱中只有一个供应商的这些箱号，根据供应商排序后，按照整箱累加规则生成一单（一单中只会有一个供应商）；大部分会是此种情况；
                //2、剩余的混装箱子，根据箱号单独生成一单（一单中会有多个供应商）；情况较少，商检属于这个情况；
                //3、按照以上，并且目前一个箱子不会超过50，则不会出现一个箱子跨两个报关单；
                //整箱累加：A、B、C箱子属于同一个供应商，三个箱子加起来不超过50个型号，则这三个箱子生成一单

                //所有箱号
                var allPacks = models.Select(item => item.PackNo).Distinct().ToList();
                //混装的箱号
                var mutilpacks = (from m in from model in models
                                            group model by new { model.PackNo, model.SupplierName } into gmodels
                                            select new
                                            {
                                                gmodels.Key.PackNo,
                                                gmodels.Key.SupplierName
                                            }
                                  group m by m.PackNo into h
                                  select new
                                  {
                                      caseNo = h.Key,
                                      count = h.Count()
                                  } into j
                                  where j.count > 1
                                  select j).ToList();

                #region 混装--每个箱子生成一个订单

                foreach (var caseNo in mutilpacks.Select(t => t.caseNo))
                {
                    var ProcessModel = models.Where(item => item.PackNo == caseNo).ToList();
                    if (ProcessModel.Count <= 50)
                    {
                        string orderID = "";
                        Create(ProcessModel, ref orderID,message.Summary);
                        Map(message.Summary, orderID);
                    }
                    else
                    {
                        var limit = 50;
                        for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(ProcessModel.Count() / 50M)); i++)
                        {
                            var mc = ProcessModel.Skip(i * limit).Take(limit).ToList();
                            string orderID = "";
                            Create(mc, ref orderID,message.Summary);
                            Map(message.Summary, orderID);
                        }
                    }
                }

                #endregion

                #region 未混装--同一个供应商按箱子累加到小于等于50
                //var mutilpackStr = string.Join(",", mutilpacks.Select(t => t.caseNo).ToList());
                //未混装箱子的型号
                var cleanModels = models.Where(t => !mutilpacks.Select(c => c.caseNo).Contains(t.PackNo)).OrderBy(t => t.SupplierName).OrderBy(t => t.PackNo).ToList();
                //处理未混装的箱子
                foreach (var supplier in cleanModels.Select(t => t.SupplierName).Distinct())
                {
                    int allcount = 0;
                    List<InsidePackingModel> wouldBeOrders = new List<InsidePackingModel>();
                    foreach (var caseNo in cleanModels.Where(t => t.SupplierName == supplier).Select(t => t.PackNo).Distinct())
                    {
                        bool isfind = false;
                        allcount = 0;
                        allcount = models.Where(item => item.PackNo == caseNo).Count();
                        foreach (var t in wouldBeOrders)
                        {
                            if (t.AllCount + allcount <= 50)
                            {
                                t.AllCount += allcount;
                                t.boxes.Add(caseNo);
                                isfind = true;
                                break;
                            }
                        }

                        if (!isfind)
                        {
                            InsidePackingModel packmodel = new InsidePackingModel();
                            packmodel.AllCount = allcount;
                            packmodel.boxes.Add(caseNo);
                            wouldBeOrders.Add(packmodel);
                        }
                    }

                    foreach (var p in wouldBeOrders)
                    {
                        var ProcessModel = cleanModels.Where(item => p.boxes.Contains(item.PackNo)).ToList();
                        if (ProcessModel.Count <= 50)
                        {
                            string orderID = "";
                            Create(ProcessModel, ref orderID,message.Summary);
                            Map(message.Summary, orderID);
                        }
                        else
                        {
                            var limit = 50;
                            for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(ProcessModel.Count() / 50M)); i++)
                            {
                                var mc = ProcessModel.Skip(i * limit).Take(limit).ToList();
                                string orderID = "";
                                Create(mc, ref orderID,message.Summary);
                                Map(message.Summary, orderID);
                            }
                        }
                    }
                }

                #endregion

                #endregion

                //等待一分钟，获取PI开始
                System.Threading.Thread.Sleep(60000);
                var pi = new DyjPIRequest(message.Summary);
                pi.Process();

                Console.WriteLine("创建结束!");
            }
        }
        public bool Create(List<InsideOrderItem> model, ref string msg,string icgooOrderID)
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
                string MainOrderID = "";

                #region 一个订单
                IcgooOrder order = new IcgooOrder();
                order.Client = client;

                using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                {
                    //把当天，这个单号所生成的订单 全都查询出来
                    var checkOrders = (from c in reponsitory.ReadTable<Layer.Data.Sqls.ScCustoms.IcgooOrderMap>()
                                      where c.IcgooOrder == icgooOrderID
                                      && c.CreateDate>DateTime.Now.Date
                                      orderby c.CreateDate descending
                                      select new
                                      {
                                          OrderID = c.OrderID,
                                          CreateDate = c.CreateDate
                                      }).ToList();


                    if (checkOrders.Count>0)
                    {
                        //循环单号，防止最新的一个单号是“逻辑出错!”
                        foreach (var checkOrder in checkOrders)
                        {
                            string[] splitOrderID = checkOrder.OrderID.Split('-');
                            if (splitOrderID.Length == 2)
                            {
                                MainOrderID = splitOrderID[0];
                                int irow = Convert.ToInt16(splitOrderID[1]) + 1;
                                OrderID = splitOrderID[0] +"-"+irow.ToString().PadLeft(2, '0');
                                order.MainOrderID = MainOrderID;
                                order.ID = OrderID;
                                break;
                            }
                        }

                        if (OrderID == "")
                        {
                            OrderID = order.ID;
                        }
                    }
                    else
                    {
                        OrderID = order.ID;
                    }
                }


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
                //order.OrderConsignee.Type = HKDeliveryType.SentToHKWarehouse;

                order.OrderConsignee.Type = HKDeliveryType.PickUp;
                order.OrderConsignee.Contact = "林团裕";
                order.OrderConsignee.Tel = "852-31019258";
                order.OrderConsignee.Address = "香港 观塘区 鸿图16号志成工业大厦2/F";


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
                var suppliers = model.Select(t => t.SupplierID).Distinct().ToList();
                foreach (var supplierid in suppliers)
                {
                    var payExchangeSupplier = new Needs.Ccs.Services.Models.ClientSupplier { ID = supplierid };
                    OrderPayExchangeSupplier orderSupplier = new OrderPayExchangeSupplier();
                    orderSupplier.ClientSupplier = payExchangeSupplier;
                    orderSupplier.OrderID = OrderID;
                    orderSupplier.Status = Status.Normal;
                    orderSupplier.UpdateDate = orderSupplier.CreateDate = DateTime.Now;
                    order.PayExchangeSuppliers.Add(orderSupplier);
                }


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

#pragma warning disable
#if PvData
                                try
                                {
                                    //如果归类完成，调用中心数据的接口，提交报价信息
                                    var pvdataApi = new PvDataApiSetting();
                                    var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.ProductQuote;
                                    var result = Needs.Utils.Http.ApiHelper.Current.JPost<Needs.Underly.JMessage>(url, new
                                    {
                                        PartNumber = classifyProduct.Model,
                                        Manufacturer = classifyProduct.Manufacturer,
                                        Origin = classifyProduct.Origin,
                                        Currency = classifyProduct.Currency,
                                        UnitPrice = classifyProduct.UnitPrice,
                                        Quantity = classifyProduct.Quantity,
                                        CIQprice = classifyProduct.InspectionFee.GetValueOrDefault()
                                    });
                                }
                                catch (Exception ex)
                                {
                                    string receivers = ConfigurationManager.AppSettings["Receivers"].ToString();
                                    var message = "报价时间：【" + DateTime.Now + "】, 异常原因：【" + ex.Message + "】";
                                    SmtpContext.Current.Send(receivers, "产品报价接口调用异常", message);
                                }
#endif
#pragma warning restore
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
                            Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.ClassifyFirstOperatorID);
                            Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.ClassifySecondOperatorID);
                            //Category.Declarant = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.DeclarantID);
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
#pragma warning disable
#if PvData
                                try
                                {
                                    var pvdataApi = new PvDataApiSetting();
                                    //判断产地是否需要消毒/检疫
                                    var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.GetOriginDisinfection;
                                    var result = Needs.Utils.Http.ApiHelper.Current.Get<Needs.Underly.JSingle<dynamic>>(url, new
                                    {
                                        origin = classifyProduct.Origin
                                    });
                                    if (result.code == 200 && (result.data.IsDisinfected) == true)
                                    {
                                        classifyProduct.Category.Type |= ItemCategoryType.Quarantine;
                                    }

                                    //获取海关编码对应的原产地加征税率
                                    url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.GetOriginATRate;
                                    result = Needs.Utils.Http.ApiHelper.Current.Get<Needs.Underly.JSingle<dynamic>>(url, new
                                    {
                                        hsCode = classifyProduct.Category.HSCode,
                                        origin = classifyProduct.Origin
                                    });
                                    if (result.code == 200)
                                    {
                                        reTariffRate = result.data.originRate;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    string receivers = ConfigurationManager.AppSettings["Receivers"].ToString();
                                    var message = "调用时间：【" + DateTime.Now + "】, 异常原因：【" + ex.Message + "】";
                                    SmtpContext.Current.Send(receivers, "中心数据接口调用异常", message);
                                }
#else
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
                                    reTariffRate = reTariff.Rate / 100;
                                }
#endif
#pragma warning restore
                            }

                            OrderItemTax tariff = new OrderItemTax();
                            tariff.Type = CustomsRateType.ImportTax;
                            tariff.Rate = classifyResult.TariffRate.Value + reTariffRate;

                            classifyProduct.ImportTax = tariff;

                            OrderItemTax ValueAddedTax = new OrderItemTax();
                            ValueAddedTax.Type = CustomsRateType.AddedValueTax;
                            ValueAddedTax.Rate = classifyResult.AddedValueRate.Value;
                            classifyProduct.AddedValueTax = ValueAddedTax;

                            OrderItemTax exciseTax = new OrderItemTax();
                            exciseTax.Type = CustomsRateType.ConsumeTax;
                            exciseTax.Rate = classifyResult.ExciseTaxRate.GetValueOrDefault();
                            classifyProduct.ExciseTax = exciseTax;

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
                            classifyProduct.ClassifyStatus = ClassifyStatus.First;
                            isAllClassified = false;

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

                            tariff.Rate = Convert.ToDecimal(p.TariffRate) + reTariffRate;
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
                order.DeclarePrice = order.ClassifyProducts.Sum(t => t.TotalPrice).ToRound(2);
                order.UpdateDate = order.CreateDate = DateTime.Now;

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
                    //this.OnPartCreated(new InsideCreateOrderEventArgs(order, model));
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
            Console.WriteLine(args.order.ID + "开始绑定");
            this.InsideProductSupplierMap?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "绑定结束");
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
            Console.WriteLine(args.order.ID + "开始绑定");
            this.InsideProductSupplierMap?.Invoke(this, args);
            Console.WriteLine(args.order.ID + "绑定结束");
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

        public void ShouldReceive(object sender, InsideCreateOrderEventArgs e)
        {
            e.order.ToReceivables();
        }

        public void ProductSupplierMap(object sender, InsideCreateOrderEventArgs e)
        {
            DataTable dtProductSupplierMap = new DataTable();
            dtProductSupplierMap.Columns.Add("ID");
            dtProductSupplierMap.Columns.Add("SupplierID");

            foreach (var item in e.partno)
            {
                DataRow dr = dtProductSupplierMap.NewRow();
                dr[0] = item.PreProductID;
                dr[1] = item.SupplierID;
                dtProductSupplierMap.Rows.Add(dr);
            }

            using (SqlConnection conn = new SqlConnection(this.ConnectionString))
            {
                conn.Open();
                if (dtProductSupplierMap.Rows.Count > 0)
                {
                    SqlBulkCopy bulkPremiums = new SqlBulkCopy(conn);
                    bulkPremiums.DestinationTableName = "ProductSupplierMap";
                    bulkPremiums.BatchSize = dtProductSupplierMap.Rows.Count;
                    bulkPremiums.WriteToServer(dtProductSupplierMap);
                }
                conn.Close();
            }
        }
    }
}
