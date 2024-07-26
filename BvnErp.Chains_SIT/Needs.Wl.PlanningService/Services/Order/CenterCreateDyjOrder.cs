using Needs.Ccs.Services;
using Needs.Ccs.Services.ApiSettings;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Underly;
using Needs.Utils.Serializers;
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
    public class CenterCreateDyjOrder
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

        public class PendingPIs
        {
            public string CompanyName { get; set; }
            public List<string> OrderIDs { get; set; }
            public PendingPIs()
            {
                OrderIDs = new List<string>();
            }
        }

        public class LegalUnitCache
        {
            public string HSCode { get; set; }
            public string FirstUnit { get; set; }
            public string SecondUnit { get; set; }
        }
        public class CompanyChange
        {
            public string ProductUniqueCode { get; set; }
            public string CompanyName { get; set; }
        }

        public string ConnectionString { get; set; }

        public string DeclareCompanyName { get; set; }
        public string AgentCompanyName { get; set; }
        /// <summary>
        /// 一个大订单下的所有小订单，一起推送
        /// </summary>
        public List<PvWsOrderInsApiModel> PushModels { get; set; }
        public List<LegalUnitCache> LegalUnitCaches { get; set; }
        public List<CompanyChange> ChangeCompanies { get; set; }
        /// <summary>
        /// 已经下过单的OrderItem      
        /// </summary>
        public List<string> OrderedItems { get; set; }

        public event InsideGenerateOrderBillHanlder InsideGenerateOrderBill;
        public event InsideQuoteConfirmHandler InsideQuoteConfirm;
        public event InsideGenerateOrderBillHanlder InsideShouldReceive;
        public event InsideProductSupplierHandler InsideProductSupplierMap;
        public event InsidePostForWayBillHandler InsidePostForWayBill;
        public event InsidePostToWareHouse InsidePostToWareHouse;
        public event InsidePackingHandler InsidePacking;
        public event InsideSealHandler InsideSeal;
        public event InsideQuoteConfirmHandler InsidePreQuoteConfrirm;

        public CenterCreateDyjOrder(string declarecompany, string agentcompanyname)
        {
            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["foricScCustomsConnectionString"].ConnectionString;

            this.DeclareCompanyName = declarecompany;
            this.AgentCompanyName = agentcompanyname;
            InsideGenerateOrderBill += CreateOrderBill;
            InsideQuoteConfirm += QuoteConfirm;
            InsideShouldReceive += ShouldReceive;
            InsidePostForWayBill += PostForWayBill;
            //InsidePostToWareHouse += PostToWareHouse;
            InsideProductSupplierMap += ProductSupplierMap;
            InsidePacking += Packing;
            InsideSeal += Seal;
            InsidePreQuoteConfrirm += ToEntryNotice;
            this.PushModels = new List<PvWsOrderInsApiModel>();
            this.LegalUnitCaches = new List<LegalUnitCache>();
            this.ChangeCompanies = new List<CompanyChange>();
            this.OrderedItems = new List<string>();
        }

        /// <summary>
        /// 先按委托公司拆分，委托公司为北京远大创新科技有限公司，北京北方科讯电子技术有限公司的，这些记录按委托公司拆分
        /// 委托公司为 华芯通的，按下单公司拆分
        /// </summary>
        /// <param name="ID"></param>
        public void CreateNew(string ID)
        {
            Logs logStart = new Logs();
            logStart.Name = "大赢家下单";
            logStart.MainID = ID;
            logStart.AdminID = "";
            logStart.Json = "";
            logStart.Summary = "报文已接收!";
            logStart.Enter();
            
            var message = new Needs.Ccs.Services.Views.MessageView()[ID];
            if (message != null)
            {
                //转换
                DataTable dt = Newtonsoft.Json.JsonConvert.DeserializeObject(message.PostData, typeof(DataTable)) as DataTable;
                //校验报文的合法性
                if (dt != null && dt.Rows.Count > 0)
                {
                    if (dt.Rows[0]["报关公司"].ToString() != DeclareCompanyName)
                    {
                        Logs log = new Logs();
                        log.Name = "大赢家下单";
                        log.MainID = ID;
                        log.AdminID = "";
                        log.Json = "";
                        log.Summary = "报关公司不匹配!";
                        log.Enter();
                        return;
                    }
                }

                #region 委托公司处理
                List<string> agencyCompanies = (from c in dt.AsEnumerable()
                                                select c.Field<string>("委托公司")).Distinct().ToList();
                agencyCompanies.Remove(Needs.Wl.PlanningService.Services.AgentCompanies.xdtSeries);

                foreach (var item in agencyCompanies)
                {
                    //委托公司未维护，不能下单                             
                    var client1 = new Needs.Ccs.Services.Views.ClientCompanyView().Where(t => t.Company.Name == item).FirstOrDefault();
                    if (client1 == null)
                    {
                        Logs log = new Logs();
                        log.Name = "大赢家下单";
                        log.MainID = ID;
                        log.AdminID = "";
                        log.Json = "";
                        log.Summary = item + "该客户不存在!";
                        log.Enter();
                        return;
                    }
                    else
                    {
                        if (client1.Merchandiser == null)
                        {
                            Logs logMer = new Logs();
                            logMer.Name = "大赢家下单";
                            logMer.MainID = ID;
                            logMer.AdminID = "";
                            logMer.Json = "";
                            logMer.Summary = item + "该客户没有维护跟单员!";
                            logMer.Enter();
                            return;
                        }
                    }
                }
                #endregion

                #region 下单公司处理
                List<string> handledCompanies = new List<string>();
                string orderfilter = "委托公司 = '" + Needs.Wl.PlanningService.Services.AgentCompanies.xdtSeries + "'";
                DataRow[] drOrderArr = dt.Select(orderfilter);

                if (drOrderArr.Count() > 0)
                {
                    DataTable dtOrderCompany = drOrderArr.CopyToDataTable();
                    List<string> orderCompanies = (from c in dtOrderCompany.AsEnumerable()
                                                   select c.Field<string>("下单公司")).Distinct().ToList();


                    //下单公司为空，不准下单
                    if (orderCompanies.Any(t => string.IsNullOrEmpty(t)))
                    {
                        Logs log = new Logs();
                        log.Name = "大赢家下单";
                        log.MainID = ID;
                        log.AdminID = "";
                        log.Json = "";
                        log.Summary = "下单公司为空";
                        log.Enter();
                        return;
                    }


                    foreach (var item in orderCompanies)
                    {
                        if (!handledCompanies.Contains(item.Trim().Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", "")))
                        {
                            handledCompanies.Add(item.Trim().Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""));
                        }
                    }

                    //var apiOrderCompanies = ApiService.Current.OrderCompanies;
                    foreach (var item in handledCompanies)
                    {
                        ////下单公司未维护，不能下单
                        //var existCompany = apiOrderCompanies.Where(t => t.Name == item).FirstOrDefault();                  
                        var client1 = new Needs.Ccs.Services.Views.ClientCompanyView().Where(t => t.Company.Name == item).FirstOrDefault();
                        if (client1 == null)
                        {
                            Logs log = new Logs();
                            log.Name = "大赢家下单";
                            log.MainID = ID;
                            log.AdminID = "";
                            log.Json = "";
                            log.Summary = item + "该客户不存在!";
                            log.Enter();
                            return;
                        }
                        else
                        {
                            if (client1.Merchandiser == null)
                            {
                                Logs logMer = new Logs();
                                logMer.Name = "大赢家下单";
                                logMer.MainID = ID;
                                logMer.AdminID = "";
                                logMer.Json = "";
                                logMer.Summary = item + "该客户没有维护跟单员!";
                                logMer.Enter();
                                return;
                            }
                        }
                    }
                }
                
                #endregion

                List<DataTable> dts = new List<DataTable>();
                foreach (var item in agencyCompanies)
                {
                    string filter = "委托公司 = '" + item + "'";
                    DataRow[] drArr = dt.Select(filter);
                    DataTable dtone = drArr.CopyToDataTable();
                    dts.Add(dtone);
                }

                foreach (var item in handledCompanies)
                {
                    string filter = "下单公司 = '" + item + "' and 委托公司 = '" + Needs.Wl.PlanningService.Services.AgentCompanies.xdtSeries + "'";
                    DataRow[] drArr = dt.Select(filter);
                    DataTable dtone = drArr.CopyToDataTable();
                    dts.Add(dtone);
                }

                SplitOrder(dts,message.Summary);
            }
        }

        public void SplitOrder(List<DataTable> dataTables,string icgooOrder)
        {
            try
            {
                List<PendingPIs> pIs = new List<PendingPIs>();
                var CountriesView = new Needs.Ccs.Services.Views.BaseCountriesView();
                foreach (DataTable dt in dataTables)
                {
                    List<InsideOrderItem> models = new List<InsideOrderItem>();
                    string agencyCompany = dt.Rows[0]["委托公司"].ToString();
                    
                    string orderCom = dt.Rows[0]["下单公司"].ToString();
                    if (agencyCompany== Needs.Wl.PlanningService.Services.AgentCompanies.xdtSeries)
                    {
                        orderCom = dt.Rows[0]["下单公司"].ToString();
                    }
                    else
                    {
                        orderCom = dt.Rows[0]["委托公司"].ToString();
                    }

                    var client1 = new Needs.Ccs.Services.Views.ClientCompanyView().Where(item => item.Company.Name == orderCom).FirstOrDefault();
                    var SupplierView1 = new Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID).ToList();

                    InsideOrderItem orderItem = new InsideOrderItem();
                    //整理原始数据
                    foreach (DataRow row in dt.Rows)
                    {
                        ///防止一个产品，委托公司是远大创新，但是下单公司是 华芯通系的产品 会重复
                        if (!OrderedItems.Contains(row["报关号"].ToString()))
                        {
                            OrderedItems.Add(row["报关号"].ToString());
                        }
                        else
                        {
                            continue;
                        }
                       
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
                            case "hongkong hongtu international logistics co., limited":
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
                            case "kb electronics development ltd":
                                SupplierName = "KB ELECTRONICS DEVELOPMENT LTD";
                                break;
                            default:
                                SupplierName = "Anda International Trade Group Limited";
                                break;
                        }

                        var supplier = SupplierView1.Find(t => t.Name == SupplierName);
                        if (supplier == null)
                        {
                            string newSupplierID = ChainsGuid.NewGuidUp();
                            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
                            {
                                reponsitory.Insert(new Layer.Data.Sqls.ScCustoms.ClientSuppliers
                                {
                                    ID = newSupplierID,
                                    ClientID = client1.ID,
                                    Name = SupplierName,
                                    ChineseName = SupplierName,
                                    Status = 200,
                                    CreateDate = DateTime.Now,
                                    UpdateDate = DateTime.Now,
                                    Grade = 1,
                                });
                            }
                            supplier = new ClientSupplier
                            {
                                ID = newSupplierID
                            };
                            SupplierView1 = new Ccs.Services.Views.ClientSuppliersView().Where(item => item.ClientID == client1.ID).ToList();
                        }
                        #endregion

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
                        string dyjHSCode = row["海关编码"].ToString();
                        if (this.LegalUnitCaches.Any(t => t.HSCode == dyjHSCode))
                        {
                            var unit = this.LegalUnitCaches.Where(t => t.HSCode == dyjHSCode).FirstOrDefault();
                            if (unit != null)
                            {
                                orderItem.FirstLegalUnit = unit.FirstUnit;
                                orderItem.SecondLegalUnit = unit.SecondUnit;
                            }
                        }
                        else
                        {
                            try
                            {
                                var apisetting = new PvDataApiSetting();
                                var url = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.GetTariff;

                                //调用中心数据接口获取海关税则信息
                                var result = Needs.Utils.Http.ApiHelper.Current.Get<Needs.Underly.JSingle<dynamic>>(url, new
                                {
                                    hsCode = row["海关编码"].ToString()
                                });
                                if (result.code == 200)
                                {
                                    orderItem.FirstLegalUnit = result.data.LegalUnit1;
                                    orderItem.SecondLegalUnit = result.data.LegalUnit2;
                                    LegalUnitCache cache = new LegalUnitCache();
                                    cache.HSCode = dyjHSCode;
                                    cache.FirstUnit = result.data.LegalUnit1;
                                    cache.SecondUnit = result.data.LegalUnit2;
                                    this.LegalUnitCaches.Add(cache);
                                }
                            }
                            catch (Exception ex)
                            {
                                string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                                SmtpContext.Current.Send(receivers, "获取海关税则接口调用异常", "调用时间：【" + DateTime.Now + "】, 异常原因：【" + ex.Message + "】");
                            }
                        }

#else
                            //orderItem.FirstLegalUnit = CustomsTariffsView.Where(m => m.HSCode == row["海关编码"].ToString()).FirstOrDefault()?.Unit1;
                            //orderItem.SecondLegalUnit = CustomsTariffsView.Where(m => m.HSCode == row["海关编码"].ToString()).FirstOrDefault()?.Unit2;
#endif
#pragma warning restore
                        if (string.IsNullOrEmpty(orderItem.FirstLegalUnit))
                        {
                            orderItem.FirstLegalUnit = "007";
                        }
                        string place = row["产地"].ToString().Trim();
                        orderItem.PlaceOfProduction = CountriesView.Where(code => code.EditionOneCode == place || code.Code == place || code.EnglishName == place).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == place || code.Code == place || code.EnglishName == place).FirstOrDefault().Code;
                        string jine = row["金额"].ToString();
                        orderItem.UnitPrice = ((Convert.ToDecimal(jine) / ConstConfig.TransPremiumInsurance).ToRound(2) / orderItem.Quantity).ToRound(4);
                        orderItem.TotalDeclarePrice = (Convert.ToDecimal(jine) / ConstConfig.TransPremiumInsurance).ToRound(2);
                        orderItem.Currency = "USD";
                        orderItem.PackNo = row["临时箱号"].ToString().Trim();
                        string[] packNos = orderItem.PackNo.Replace("TW,CN","TWCN").Split(',');
                        if (packNos.Length > 1)
                        {
                            string lastPackNo = packNos[packNos.Length - 1];
                            string[] splitInfo = lastPackNo.Split('-');
                            orderItem.PackNo = packNos[0].Replace("TWCN", "TW,CN") + "-" + splitInfo[2];
                        }
                        string netw = row["称重"].ToString();
                        orderItem.NetWeight = Math.Round(Convert.ToDecimal(netw), 2, MidpointRounding.AwayFromZero) < 0.01M ? 0.01M : Math.Round(Convert.ToDecimal(netw), 2, MidpointRounding.AwayFromZero);
                        orderItem.GrossWeight = Math.Round(Convert.ToDecimal(netw) * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero) < 0.02M ? 0.02M : Math.Round(Convert.ToDecimal(netw) * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero);
                        orderItem.CaseWeight = Convert.ToDecimal(row["毛重"].ToString());
                        orderItem.Model = row["型号"].ToString();
                        orderItem.TaxName = row["品名"].ToString();
                        orderItem.TaxCode = row["型号信息分类值"].ToString();
                        orderItem.Brand = row["品牌"].ToString();
                        orderItem.TariffRate = Convert.ToDecimal(row["关税"].ToString());
                        orderItem.CompanyName = orderCom;
                        //orderItem.CompanyName = this.AgentCompanyName;
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
                    //拆分订单,根据下单公司拆分                  
                    
                    StrategyContext strategyContext = new StrategyContext(orderCom, models);
                    List<OrderModel> orderOne = strategyContext.SplitOrder();

                    var OrderIDs = new List<string>();
                    this.PushModels = new List<PvWsOrderInsApiModel>();
                    foreach (var item in orderOne)
                    {
                        var ProcessModel = models.Where(t => item.ProductUniqueCodes.Contains(t.PreProductID)).ToList();
                        string orderID = "";
                        Create(ProcessModel, ref orderID, icgooOrder);
                        OrderIDs.Add(orderID);
                        Map(icgooOrder, orderID);
                    }

                    #region 推送给新的客户端                
                    Post2NewClientForWayBill();
                    #endregion

                    #region PI信息
                    PendingPIs pl = new PendingPIs();
                    pl.CompanyName = orderCom;
                    pl.OrderIDs = OrderIDs;
                    pIs.Add(pl);
                    #endregion
                }

                Logs logPI = new Logs();
                logPI.Name = "大赢家下单";
                logPI.MainID = icgooOrder;
                logPI.AdminID = "";
                logPI.Json = pIs.Json();
                logPI.Summary = "PI信息";
                logPI.Enter();

                ChangeOrderCompany(ChangeCompanies);
                //生成PI
                foreach (var item in pIs)
                {
                    GeneratePIFiles(item.OrderIDs, item.CompanyName);
                }
            }
            catch(Exception ex)
            {
                ex.CcsLog("分批生成订单报错");
            }
            
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
                                case "hongkong hongtu international logistics co., limited":
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
                                case "kb electronics development ltd":
                                    SupplierName = "KB ELECTRONICS DEVELOPMENT LTD";
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


                            //if (string.IsNullOrEmpty(row["海关编码"].ToString()))
                            //{
                            //    continue;
                            //}
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
                                var url = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.GetTariff;

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
                                string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
                                SmtpContext.Current.Send(receivers, "获取海关税则接口调用异常", "调用时间：【" + DateTime.Now + "】, 异常原因：【" + ex.Message + "】");
                            }
#else
                            orderItem.FirstLegalUnit = CustomsTariffsView.Where(m => m.HSCode == row["海关编码"].ToString()).FirstOrDefault()?.Unit1;
                            orderItem.SecondLegalUnit = CustomsTariffsView.Where(m => m.HSCode == row["海关编码"].ToString()).FirstOrDefault()?.Unit2;
#endif
#pragma warning restore
                            if (string.IsNullOrEmpty(orderItem.FirstLegalUnit))
                            {
                                orderItem.FirstLegalUnit = "007";
                            }
                            string place = row["产地"].ToString().Trim();
                            orderItem.PlaceOfProduction = CountriesView.Where(code => code.EditionOneCode == place || code.Code == place || code.EnglishName == place).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == place || code.Code == place || code.EnglishName == place).FirstOrDefault().Code;
                            string jine = row["金额"].ToString();
                            orderItem.UnitPrice = ((Convert.ToDecimal(jine) / ConstConfig.TransPremiumInsurance).ToRound(2) / orderItem.Quantity).ToRound(4);
                            orderItem.TotalDeclarePrice = (Convert.ToDecimal(jine) / ConstConfig.TransPremiumInsurance).ToRound(2);
                            orderItem.Currency = "USD";
                            orderItem.PackNo = row["临时箱号"].ToString().Trim();
                            string netw = row["称重"].ToString();
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

                #region 20191121大赢家拆分规则
                //1、筛选一箱中只有一个供应商的这些箱号，根据供应商排序后，按照整箱累加规则生成一单（一单中只会有一个供应商）；大部分会是此种情况；
                //2、剩余的混装箱子，根据箱号单独生成一单（一单中会有多个供应商）；情况较少，商检属于这个情况；
                //3、按照以上，并且目前一个箱子不会超过50，则不会出现一个箱子跨两个报关单；
                //整箱累加：A、B、C箱子属于同一个供应商，三个箱子加起来不超过50个型号，则这三个箱子生成一单

                var OrderIDs = new List<string>();
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
                        CreateOriginal(ProcessModel, ref orderID, message.Summary);
                        OrderIDs.Add(orderID);
                        Map(message.Summary, orderID);
                    }
                    else
                    {
                        var limit = 50;
                        for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(ProcessModel.Count() / 50M)); i++)
                        {
                            var mc = ProcessModel.Skip(i * limit).Take(limit).ToList();
                            string orderID = "";
                            CreateOriginal(mc, ref orderID, message.Summary);
                            OrderIDs.Add(orderID);
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
                            CreateOriginal(ProcessModel, ref orderID, message.Summary);
                            OrderIDs.Add(orderID);
                            Map(message.Summary, orderID);
                        }
                        else
                        {
                            var limit = 50;
                            for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(ProcessModel.Count() / 50M)); i++)
                            {
                                var mc = ProcessModel.Skip(i * limit).Take(limit).ToList();
                                string orderID = "";
                                CreateOriginal(mc, ref orderID, message.Summary);
                                OrderIDs.Add(orderID);
                                Map(message.Summary, orderID);
                            }
                        }
                    }
                }

                #endregion

                #endregion

                #region 推送给新的客户端
                Post2NewClientForWayBill();
                #endregion

                //等待一分钟，获取PI开始
                System.Threading.Thread.Sleep(60000);
                //var pi = new DyjPIRequest(message.Summary);
                //pi.Process();
                //生成大赢家PI文件
                GeneratePIFiles(OrderIDs,this.AgentCompanyName);

                Console.WriteLine("创建结束!");
            }
        }
        public bool Create(List<InsideOrderItem> model, ref string msg, string icgooOrderID)
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
                                       && c.CreateDate > DateTime.Now.Date && c.OrderID.StartsWith(client.ClientCode)
                                       orderby c.CreateDate descending
                                       select new
                                       {
                                           OrderID = c.OrderID,
                                           CreateDate = c.CreateDate
                                       }).ToList();


                    if (checkOrders.Count > 0)
                    {
                        //循环单号，防止最新的一个单号是“逻辑出错!”
                        foreach (var checkOrder in checkOrders)
                        {
                            string[] splitOrderID = checkOrder.OrderID.Split('-');
                            if (splitOrderID.Length == 2)
                            {
                                MainOrderID = splitOrderID[0];
                                int irow = Convert.ToInt16(splitOrderID[1]) + 1;
                                OrderID = splitOrderID[0] + "-" + irow.ToString().PadLeft(2, '0');
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
                        classifyProduct.TotalPrice = p.TotalDeclarePrice;
                        classifyProduct.GrossWeight = p.GrossWeight;
                        TotalDeclarePrice += classifyProduct.TotalPrice;
                        //归类结果
                        ClassifyResult classifyResult = view.Where(item => item.ClassifyStatus == ClassifyStatus.Done &&
                                                                   item.CompanyType == CompanyTypeEnums.Inside &&
                                                                   item.PreProductUnicode == p.PreProductID).FirstOrDefault();
                        var advanceProduct = new Needs.Ccs.Services.Views.PreProductOriginalView().Where(item => item.sale_orderline_id == p.PreProductID).FirstOrDefault();
                        if (classifyResult != null)
                        {

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

                            if (classifyProduct.IsInsp||classifyProduct.IsCCC||classifyProduct.IsSysForbid)
                            {
                                CompanyChange companyChange = new CompanyChange();
                                companyChange.ProductUniqueCode = p.PreProductID;
                                companyChange.CompanyName = p.CompanyName;
                                ChangeCompanies.Add(companyChange);
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
                                    var url = System.Configuration.ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.ProductQuote;
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
                                    string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
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
                            Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                            Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
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
                                    var url = System.Configuration.ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.GetOriginDisinfection;
                                    var result = Needs.Utils.Http.ApiHelper.Current.Get<Needs.Underly.JSingle<dynamic>>(url, new
                                    {
                                        origin = classifyProduct.Origin
                                    });
                                    if (result.code == 200 && (result.data.IsDisinfected) == true)
                                    {
                                        classifyProduct.Category.Type |= ItemCategoryType.Quarantine;
                                    }

                                    //获取海关编码对应的原产地加征税率
                                    url = System.Configuration.ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.GetOriginATRate;
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
                                    string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
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

                            classifyProduct.UpdateDate = classifyResult.UpdateDate;
                            order.ClassifyProducts.Add(classifyProduct);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(p.CustomsCode))
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

                                if (classifyProduct.IsInsp || classifyProduct.IsCCC || classifyProduct.IsSysForbid)
                                {
                                    CompanyChange companyChange = new CompanyChange();
                                    companyChange.ProductUniqueCode = p.PreProductID;
                                    companyChange.CompanyName = p.CompanyName;
                                    ChangeCompanies.Add(companyChange);
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

                                OrderItemTax ExciseTax = new OrderItemTax();
                                ExciseTax.Type = CustomsRateType.ConsumeTax;
                                ExciseTax.Rate = InsideOrder.ExciseTax;
                                classifyProduct.ExciseTax = ExciseTax;


                            }
                            else
                            {
                                classifyProduct.Name = p.ProductName;
                                classifyProduct.Model = p.Model;
                                classifyProduct.Manufacturer = p.Brand;
                                classifyProduct.ClassifyStatus = ClassifyStatus.Unclassified;
                                isAllClassified = false;
                            }

                            classifyProduct.OrderType = OrderType.Inside;
                            classifyProduct.ProductUniqueCode = p.PreProductID;
                            order.ClassifyProducts.Add(classifyProduct);
                        }


                    }
                }
                order.DeclarePrice = model.Sum(t => t.TotalDeclarePrice).ToRound(2);
                order.UpdateDate = order.CreateDate = DateTime.Now;

                if (isAllClassified)
                {
                    order.OrderStatus = OrderStatus.Classified;
                    order.ConnectionString = this.ConnectionString;
                    List<PvOrderItems> pvOrderItems = order.EnterSpeed();
                    this.OnCreated(new InsideCreateOrderEventArgs(order, model, pvOrderItems));
                }
                else
                {
                    order.OrderStatus = OrderStatus.Confirmed;
                    order.ConnectionString = this.ConnectionString;
                    List<PvOrderItems> pvOrderItems = order.EnterSpeed();
                    this.OnPartCreated(new InsideCreateOrderEventArgs(order, model, pvOrderItems));
                }
                Console.WriteLine("创建订单结束");
              
                #endregion

                msg = OrderID;
                return isSuccess;


            }
            catch (Exception ex)
            {
                msg = ex.ToString();
                throw ex;
                //return false;
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

            this.InsidePostForWayBill?.Invoke(this, args);
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

            this.InsidePostForWayBill?.Invoke(this, args);
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

        public void ShouldReceive(object sender, InsideCreateOrderEventArgs e)
        {
            e.order.ToReceivables();
        }

        public void ProductSupplierMap(object sender, InsideCreateOrderEventArgs e)
        {
            string deleteIDS = "";
            DataTable dtProductSupplierMap = new DataTable();
            dtProductSupplierMap.Columns.Add("ID");
            dtProductSupplierMap.Columns.Add("SupplierID");

            foreach (var item in e.partno)
            {
                deleteIDS += item.PreProductID + "D";
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
                    SqlCommand sqlDeleteProductSupplierMap = new SqlCommand("DeleteProductSupplierMap", conn);
                    sqlDeleteProductSupplierMap.CommandType = CommandType.StoredProcedure;
                    sqlDeleteProductSupplierMap.Parameters.Add(new SqlParameter("@IDS", SqlDbType.VarChar, 2000));
                    sqlDeleteProductSupplierMap.Parameters["@IDS"].Value = deleteIDS;
                    sqlDeleteProductSupplierMap.ExecuteNonQuery();

                    SqlBulkCopy bulkPremiums = new SqlBulkCopy(conn);
                    bulkPremiums.DestinationTableName = "ProductSupplierMap";
                    bulkPremiums.BatchSize = dtProductSupplierMap.Rows.Count;
                    bulkPremiums.WriteToServer(dtProductSupplierMap);
                }
                conn.Close();
            }
        }

        public void PostForWayBill(object sender, InsideCreateOrderEventArgs e)
        {
            PvWsOrderInsApiModel pushModel = new PvWsOrderInsApiModel();
            pushModel.Items = new List<PvOrderItems>();
            pushModel.Items = e.pvOrderItems;

            foreach (var item in pushModel.Items)
            {
                var receiveModel = e.partno.Where(t => t.PreProductID == item.ProductUnicode).FirstOrDefault();
                item.PackNo = receiveModel.PackNo.ToString();
                if (item.GrossWeight != null)
                {
                    item.NetWeight = Math.Round(item.GrossWeight.Value / 1.1M, 4, MidpointRounding.AwayFromZero);
                }
                item.SupplierName = receiveModel.SupplierName;
                if (item.Origin.Equals("NG"))
                {
                    item.Origin = "Unknown";
                }
            }

            pushModel.VastOrderID = e.order.MainOrderID;
            pushModel.ClientName = e.order.Client.Company.Name;
            pushModel.HKWareHouseID = System.Configuration.ConfigurationManager.AppSettings["HKWareHouseID"];
            pushModel.DeclarationCompany = System.Configuration.ConfigurationManager.AppSettings["DeclareCompany"];

            List<string> payExchangeSuppliers = new List<string>();
            foreach (var t in e.order.PayExchangeSuppliers)
            {
                payExchangeSuppliers.Add(t.ClientSupplier.Name);
            }
            pushModel.PayExchangeSuppliers = payExchangeSuppliers;

            var OriginAdmin = new AdminsTopView2().FirstOrDefault(t => t.OriginID == e.order.AdminID);
            if (OriginAdmin != null)
            {
                pushModel.AdminID = OriginAdmin.ID;
            }

            pushModel.OrderConsignee = new PvConsignee();
            pushModel.OrderConsignee.ClientSupplierName = e.order.OrderConsignee.ClientSupplier.Name;
            pushModel.OrderConsignee.Type = HKDeliveryType.SentToHKWarehouse;

            pushModel.OrderConsignor = new PvConsignor();
            pushModel.OrderConsignor.Type = SZDeliveryType.SentToClient;
            pushModel.OrderConsignor.Name = e.order.OrderConsignor.Name;
            pushModel.OrderConsignor.Contact = e.order.OrderConsignor.Contact;
            pushModel.OrderConsignor.Mobile = e.order.OrderConsignor.Mobile;
            pushModel.OrderConsignor.Address = e.order.OrderConsignor.Address;

            this.PushModels.Add(pushModel);
            this.InsidePostToWareHouse?.Invoke(this, new CenterInsideCreateOrderEventArgs(pushModel));
        }

        public void Post2NewClientForWayBill()
        {
            PvWsOrderInsApiModel model = new PvWsOrderInsApiModel();
            model.PayExchangeSuppliers = new List<string>();
            model.Items = new List<PvOrderItems>();
            int i = 0;
            foreach (var item in this.PushModels)
            {
                if (i == 0)
                {
                    model.VastOrderID = item.VastOrderID;
                    model.ClientName = item.ClientName;
                    model.HKWareHouseID = item.HKWareHouseID;
                    model.DeclarationCompany = item.DeclarationCompany;

                    if (item.AdminID != null)
                    {
                        model.AdminID = item.AdminID;
                    }

                    model.OrderConsignee = item.OrderConsignee;
                    model.OrderConsignor = item.OrderConsignor;
                }

                foreach (var t in item.PayExchangeSuppliers)
                {
                    if (!model.PayExchangeSuppliers.Contains(t))
                    {
                        model.PayExchangeSuppliers.Add(t);
                    }
                }

                model.Items.AddRange(item.Items);

                i++;
            }


            var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.OrderSubmit;
            //string PostData = model.Json();
            try
            {
                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, model);

                var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);

                if (jResult.success)
                {
                    Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        OrderID = model.VastOrderID,
                        TinyOrderID = model.VastOrderID,
                        Url = apiurl,
                        RequestContent = model.Json(),
                        ResponseContent = jResult.data,
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };
                    apiLog.Enter();

                    model.WayBillID = jResult.data;
                    //PostToWareHouse(model);
                }
                else
                {
                    Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        OrderID = model.VastOrderID,
                        TinyOrderID = model.VastOrderID,
                        Url = apiurl,
                        RequestContent = model.Json(),
                        ResponseContent = jResult.data,
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };
                    apiLog.Enter();
                }
                //string test = e.PvWsOrderInsApiModel.Json();
            }
            catch (Exception ex)
            {
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = model.VastOrderID,
                    TinyOrderID = model.VastOrderID,
                    Url = apiurl,
                    RequestContent = model.Json(),
                    ResponseContent = ex.ToString(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "调用接口报错",
                };
                apiLog.Enter();
                ex.CcsLog("内单信息调用客户端接口");
            }
        }

        public void PostToWareHouse(PvWsOrderInsApiModel model)
        {
            var apisetting = new Needs.Ccs.Services.ApiSettings.PfWmsApiSetting();
            var apiurl = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.cgInternalOrders;
            try
            {


                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, model);
                var jResult = JsonConvert.DeserializeObject<JSingle<dynamic>>(result);

                if (jResult.success)
                {
                    Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        OrderID = model.VastOrderID,
                        TinyOrderID = model.VastOrderID,
                        Url = apiurl,
                        RequestContent = model.Json(),
                        ResponseContent = jResult.data,
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };
                    apiLog.Enter();
                }
                else
                {
                    Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                    {
                        ID = Guid.NewGuid().ToString("N"),
                        OrderID = model.VastOrderID,
                        TinyOrderID = model.VastOrderID,
                        Url = apiurl,
                        RequestContent = model.Json(),
                        ResponseContent = jResult.data,
                        Status = Needs.Ccs.Services.Enums.Status.Normal,
                        CreateDate = DateTime.Now,
                        UpdateDate = DateTime.Now,
                    };
                    apiLog.Enter();
                }
            }
            catch (Exception ex)
            {
                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = model.VastOrderID,
                    TinyOrderID = model.VastOrderID,
                    Url = apiurl,
                    RequestContent = model.Json(),
                    ResponseContent = ex.ToString(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "调用接口报错",
                };
                apiLog.Enter();
                ex.CcsLog("内单信息调用客户端接口");
            }
        }

        public void GeneratePIFiles(List<string> OrderIDs,string orderCompany)
        {
            try
            {
                foreach (var t in OrderIDs)
                {
                    PIGener pIGener = new PIGener(t, orderCompany);
                    pIGener.Execute();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void ChangeOrderCompany(List<CompanyChange> companyChanges)
        {
            List<ApiOrderCompany> apiOrderCompanies = ApiService.Current.OrderCompanies;
            using (Layer.Data.Sqls.ScCustomsReponsitory reponsitory = new Layer.Data.Sqls.ScCustomsReponsitory())
            {
                foreach(var item in companyChanges)
                {
                    string clientID = "";
                    var com = apiOrderCompanies.Where(t => t.Name == item.CompanyName).FirstOrDefault();
                    if (com != null)
                    {
                        clientID = com.ID;
                    }
                    if (!string.IsNullOrEmpty(clientID))
                    {
                        reponsitory.Update<Layer.Data.Sqls.ScCustoms.PreProducts>(new { ClientID = clientID }, t => t.ProductUnionCode == item.ProductUniqueCode);
                    }
                }
            }
        }

        /// <summary>
        /// 华芯通内销之前的版本
        /// </summary>
        /// <param name="model"></param>
        /// <param name="msg"></param>
        /// <param name="icgooOrderID"></param>
        /// <returns></returns>
        public bool CreateOriginal(List<InsideOrderItem> model, ref string msg, string icgooOrderID)
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
                                       && c.CreateDate > DateTime.Now.Date
                                       orderby c.CreateDate descending
                                       select new
                                       {
                                           OrderID = c.OrderID,
                                           CreateDate = c.CreateDate
                                       }).ToList();


                    if (checkOrders.Count > 0)
                    {
                        //循环单号，防止最新的一个单号是“逻辑出错!”
                        foreach (var checkOrder in checkOrders)
                        {
                            string[] splitOrderID = checkOrder.OrderID.Split('-');
                            if (splitOrderID.Length == 2)
                            {
                                MainOrderID = splitOrderID[0];
                                int irow = Convert.ToInt16(splitOrderID[1]) + 1;
                                OrderID = splitOrderID[0] + "-" + irow.ToString().PadLeft(2, '0');
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
                        classifyProduct.TotalPrice = p.TotalDeclarePrice;
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
                                    var url = System.Configuration.ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.ProductQuote;
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
                                    string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
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
                                    var url = System.Configuration.ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.GetOriginDisinfection;
                                    var result = Needs.Utils.Http.ApiHelper.Current.Get<Needs.Underly.JSingle<dynamic>>(url, new
                                    {
                                        origin = classifyProduct.Origin
                                    });
                                    if (result.code == 200 && (result.data.IsDisinfected) == true)
                                    {
                                        classifyProduct.Category.Type |= ItemCategoryType.Quarantine;
                                    }

                                    //获取海关编码对应的原产地加征税率
                                    url = System.Configuration.ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.GetOriginATRate;
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
                                    string receivers = System.Configuration.ConfigurationManager.AppSettings["Receivers"].ToString();
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

                            classifyProduct.UpdateDate = classifyResult.UpdateDate;
                            order.ClassifyProducts.Add(classifyProduct);
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(p.CustomsCode))
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

                                OrderItemTax ExciseTax = new OrderItemTax();
                                ExciseTax.Type = CustomsRateType.ConsumeTax;
                                ExciseTax.Rate = InsideOrder.ExciseTax;
                                classifyProduct.ExciseTax = ExciseTax;


                            }
                            else
                            {
                                classifyProduct.Name = p.ProductName;
                                classifyProduct.Model = p.Model;
                                classifyProduct.Manufacturer = p.Brand;
                                classifyProduct.ClassifyStatus = ClassifyStatus.Unclassified;
                                isAllClassified = false;
                            }

                            classifyProduct.OrderType = OrderType.Inside;
                            classifyProduct.ProductUniqueCode = p.PreProductID;
                            order.ClassifyProducts.Add(classifyProduct);
                        }


                    }
                }
                order.DeclarePrice = model.Sum(t => t.TotalDeclarePrice).ToRound(2);
                order.UpdateDate = order.CreateDate = DateTime.Now;

                if (isAllClassified)
                {
                    order.OrderStatus = OrderStatus.Classified;
                    order.ConnectionString = this.ConnectionString;
                    List<PvOrderItems> pvOrderItems = order.EnterSpeed();
                    this.OnCreated(new InsideCreateOrderEventArgs(order, model, pvOrderItems));
                }
                else
                {
                    order.OrderStatus = OrderStatus.Confirmed;
                    order.ConnectionString = this.ConnectionString;
                    List<PvOrderItems> pvOrderItems = order.EnterSpeed();
                    this.OnPartCreated(new InsideCreateOrderEventArgs(order, model, pvOrderItems));
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
    }
}
