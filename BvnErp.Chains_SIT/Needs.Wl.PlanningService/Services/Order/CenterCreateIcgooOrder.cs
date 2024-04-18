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
    public class CenterCreateIcgooOrder
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
        /// <summary>
        /// 一个大订单下的所有小订单，一起推送
        /// </summary>
        public List<PvWsOrderInsApiModel> PushModels { get; set; }
        public event IcgooGenerateOrderBillHanlder IcgooGenerateOrderBill;
        public event IcgooQuoteConfirmHandler IcgooQuoteConfirm;
        public event IcgooGenerateOrderBillHanlder IcgooShouldReceive;
        public event IcgooPostForWayBillHandler IcgooPostForWayBill;
        public event IcgooQuoteConfirmHandler IcgooPreQuoteConfrirm;
        public event IcgooPackingHandler IcgooPacking;
        public event IcgooSealHandler IcgooSeal;


        public CenterCreateIcgooOrder()
        {
            this.ConnectionString = System.Configuration.ConfigurationManager.ConnectionStrings["foricScCustomsConnectionString"].ConnectionString;
            IcgooGenerateOrderBill += CreateOrderBill;
            IcgooQuoteConfirm += QuoteConfirm;
            IcgooShouldReceive += ShouldReceive;
            IcgooPostForWayBill += PostForWayBill;
            IcgooPacking += Packing;
            IcgooSeal += Seal;
            IcgooPreQuoteConfrirm += ToEntryNotice;
            this.PushModels = new List<PvWsOrderInsApiModel>();
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
                PartNoReceive model = JsonConvert.DeserializeObject<PartNoReceive>(message.PostData);               
                this.IcgooPayExchangeSupplier = model.PayExchangeSupplier;


                #region
                List<IcgooPackingModel> wouldBeOrders = new List<IcgooPackingModel>();
                var caseNos = model.info.Select(item => item.orgCarton).Distinct().ToList();
                int allcount = 0;
                foreach (var caseNo in caseNos)
                {
                    bool isfind = false;
                    allcount = 0;
                    allcount = model.info.Where(item => item.orgCarton == caseNo).Count();
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
                        IcgooPackingModel packmodel = new IcgooPackingModel();
                        packmodel.AllCount = allcount;
                        packmodel.boxes.Add(caseNo);
                        wouldBeOrders.Add(packmodel);
                    }
                }

                var OrderIDs = new List<string>();
                foreach (var p in wouldBeOrders)
                {
                    var ProcessModel = model.info.Where(item => p.boxes.Contains(item.orgCarton)).ToList();
                    if (ProcessModel.Count <= 50)
                    {
                        string orderID = "";
                        Create(ProcessModel, model.currency, message.Summary, ref orderID, model.refNo, model.warehouse);
                        Map(model.refNo, orderID);
                        OrderIDs.Add(orderID);
                    }
                    else
                    {
                        var limit = 50;
                        for (int i = 0; i < Math.Ceiling(Convert.ToDecimal(ProcessModel.Count() / 50M)); i++)
                        {
                            var mc = ProcessModel.Skip(i * limit).Take(limit).ToList();
                            string orderID = "";
                            Create(mc, model.currency, message.Summary, ref orderID, model.refNo, model.warehouse);
                            Map(model.refNo, orderID);
                            OrderIDs.Add(orderID);
                        }
                    }
                }


                #region 推送给新的客户端
                Post2NewClientForWayBill();
                #endregion
                
                IcgooDiffTariffSend icgooDiffTariffSend = new IcgooDiffTariffSend(OrderIDs, Icgoo.IcgooTariffDiffMQName);
                icgooDiffTariffSend.Send();

                ////关联PI文件 20191130
                //if (this.IcgooPayExchangeSupplier.Trim().Equals("ICGOO ELECTRONICS LIMITED"))
                //{
                //    var icgoopi = new IcgooPIRequest(model.refNo, OrderIDs);
                //    icgoopi.Process();
                //}
                //else
                //{
                //    var icgooMultipi = new IcgooMultiPIRequest(model.refNo, OrderIDs);
                //    icgooMultipi.Process();
                //}

                //关联PI文件 20191130
                //关联PI文件再次修改 20201015 以后不会有特殊渠道的供应商，所以不会有多个PI文件
                var icgoopi = new IcgooInXDTPIRequest(model.refNo, OrderIDs);
                icgoopi.Process();

                #endregion
            }
        }

        private void Map(string IcgooOrder, string OrderID)
        {
            IcgooMap map = new IcgooMap();
            map.ID = ChainsGuid.NewGuidUp();
            map.IcgooOrder = IcgooOrder;
            map.OrderID = OrderID;
            map.CompanyType = CompanyTypeEnums.Icgoo;
            map.Enter();
        }

        public bool Create(List<PartNoReceiveItem> singlemodel, string currency, string clientID, ref string msg, string icgooOrderID, string warehouse)
        {
            bool isSuccess = true;
            try
            {
                Console.WriteLine("开始创建订单");

                //客户               
                var clientView = new Needs.Ccs.Services.Views.ClientsView();
                var client = clientView[clientID];

                var payExchangeSupplier = client.Suppliers.Where(item => item.Name == this.IcgooPayExchangeSupplier).FirstOrDefault();
                if (payExchangeSupplier == null)
                {
                    payExchangeSupplier = client.Suppliers.Where(item => item.Name == "ICGOO ELECTRONICS LIMITED").FirstOrDefault();
                }
                string supplierID = payExchangeSupplier.ID;
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
                //order.OrderConsignee.Type = HKDeliveryType.SentToHKWarehouse;

                //20200310  庆永经理提出 展示实际提货库房、件数 用于指导司机
                //根据实际库房，安排订单中的提货地址 warehouse :"zhongmei" , warehouse:"yisheng"
                //快包库房默认为中美
                if (client.Company.Name.Equals("深圳市快包电子科技有限责任公司"))
                {
                    warehouse = "zhongmei";
                }

                if (warehouse == "zhongmei")
                {
                    order.OrderConsignee.Type = HKDeliveryType.PickUp;
                    order.OrderConsignee.Contact = "莫焯榮";
                    order.OrderConsignee.Tel = "61537341";
                    order.OrderConsignee.Address = "香港 观塘区 兴业街15-17号中美中心b座801";
                }
                else if (warehouse == "yisheng")
                {
                    order.OrderConsignee.Type = HKDeliveryType.PickUp;
                    order.OrderConsignee.Contact = "陈惠晓";
                    order.OrderConsignee.Tel = "852-23638855";
                    order.OrderConsignee.Address = "香港 观塘区 成业街16号怡生工业中心11楼B2室";
                }
                else
                {
                    order.OrderConsignee.Type = HKDeliveryType.SentToHKWarehouse;
                }

                //客户收货地址
                var clientConsignees = new Needs.Ccs.Services.Views.ClientConsigneesView();
                var defaultConsignee = clientConsignees.Where(item => item.ClientID == clientID && item.IsDefault == true).FirstOrDefault();
                //国内交货信息
                order.OrderConsignor.Type = SZDeliveryType.SentToClient;
                order.OrderConsignor.Name = defaultConsignee.Name;
                order.OrderConsignor.Contact = defaultConsignee.Contact.Name;
                order.OrderConsignor.Mobile = defaultConsignee.Contact.Mobile;
                order.OrderConsignor.Address = defaultConsignee.Address;

                //付汇供应商        
                //付汇供应商                
               
                //var payExchangeSupplier = client.Suppliers.Where(item => item.ID == this.IcgooPayExchangeSupplier).FirstOrDefault();
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
                    var DoneClassifyView = view.Where(item => item.ClassifyStatus == ClassifyStatus.Done && (item.CompanyType == CompanyTypeEnums.Icgoo || item.CompanyType == CompanyTypeEnums.FastBuy));
                    var CountriesView = new Needs.Ccs.Services.Views.BaseCountriesView();

                    foreach (PartNoReceiveItem p in singlemodel)
                    {
                        ClassifyProduct classifyProduct = new ClassifyProduct();
                        classifyProduct.Client = order.Client;
                        classifyProduct.Currency = currency;
                        classifyProduct.OrderID = OrderID;
                        classifyProduct.CaseNo = p.orgCarton.ToString();

                        classifyProduct.Origin = CountriesView.Where(code => code.EditionOneCode == p.origin || code.EnglishName == p.origin).FirstOrDefault() == null ? Icgoo.UnknownCountry : CountriesView.Where(code => code.EditionOneCode == p.origin || code.EnglishName == p.origin).FirstOrDefault().Code;
                        classifyProduct.Quantity = p.qty;
                        classifyProduct.Unit = Icgoo.Gunit;
                        classifyProduct.UnitPrice = Math.Round(p.price, 5, MidpointRounding.AwayFromZero);
                        classifyProduct.TotalPrice = Math.Round(p.qty * p.price, 4, MidpointRounding.AwayFromZero);
                        classifyProduct.GrossWeight = p.gw / Icgoo.UnitConvert;
                        TotalDeclarePrice += classifyProduct.TotalPrice;
                        //归类结果
                        ClassifyResult classifyResult = DoneClassifyView.Where(item => item.PreProductUnicode == p.sale_orderline_id && item.ClassifyStatus == ClassifyStatus.Done).FirstOrDefault();
                        if (classifyResult != null)
                        {
                            

                            var categoryType = classifyResult.Type;
                            if ((classifyResult.Type & ItemCategoryType.Inspection) > 0)
                            {
                                classifyProduct.IsInsp = true;
                                classifyProduct.InspectionFee = classifyResult.InspectionFee.HasValue ? classifyResult.InspectionFee.Value : 0;
                                if(order.ClassifyProducts.Where(t => t.ProductUniqueCode == p.sale_orderline_id).Any(t => t.InspectionFee != 0))
                                {
                                    classifyProduct.InspectionFee = 0;
                                }
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
                                }
                                catch (Exception ex)
                                {
                                    string receivers = ConfigurationManager.AppSettings["Receivers"].ToString();
                                    var message = "调用时间：【" + DateTime.Now + "】, 异常原因：【" + ex.Message + "】";
                                    SmtpContext.Current.Send(receivers, "原产地检疫接口调用异常", message);
                                }
#else
                                string origin = classifyProduct.Origin;
                                var quarantines = new Needs.Ccs.Services.Views.CustomsQuarantinesView();
                                var quarantine = quarantines.Where(cq => cq.Origin == origin && cq.StartDate <= DateTime.Now && cq.EndDate >= DateTime.Now).FirstOrDefault();
                                if (quarantine != null)
                                {
                                    categoryType = categoryType | ItemCategoryType.Quarantine;
                                }
#endif
#pragma warning restore

                            }

                            classifyProduct.Name = classifyResult.ProductName;
                            classifyProduct.Model = p.pn;
                            classifyProduct.Manufacturer = classifyResult.Manufacturer;
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
                                SmtpContext.Current.Send(receivers, "产品报价失败", message);
                            }
#endif
#pragma warning restore

                            OrderItemCategory Category = new OrderItemCategory();
                            Category.ClassifyFirstOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                            Category.ClassifySecondOperator = Needs.Underly.FkoFactory<Admin>.Create(Icgoo.DefaultCreator);
                            //Category.Declarant = Needs.Underly.FkoFactory<Admin>.Create(classifyResult.DeclarantID);
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

#pragma warning disable
#if PvData
                            try
                            {
                                var pvdataApi = new PvDataApiSetting();
                                //获取海关编码对应的原产地加征税率
                                var url = ConfigurationManager.AppSettings[pvdataApi.ApiName] + pvdataApi.GetOriginATRate;
                                var result = Needs.Utils.Http.ApiHelper.Current.Get<Needs.Underly.JSingle<dynamic>>(url, new
                                {
                                    hsCode = classifyProduct.Category.HSCode,
                                    origin = classifyProduct.Origin
                                });
                                if (result.code == 200)
                                {
                                    tariff.Rate = classifyResult.TariffRate.Value + (decimal)result.data.originRate;
                                }
                                else
                                {
                                    tariff.Rate = classifyResult.TariffRate.Value;
                                }
                            }
                            catch (Exception ex)
                            {
                                string receivers = ConfigurationManager.AppSettings["Receivers"].ToString();
                                var message = "调用时间：【" + DateTime.Now + "】, 异常原因：【" + ex.Message + "】";
                                SmtpContext.Current.Send(receivers, "获取产地加征税率接口调用异常", message);
                            }
#else
                            var tariffView = new Needs.Ccs.Services.Views.CustomsOriginTariffsView();

                            //确定他们提供的Origin 是什么，是USA 还是America
                            var reTariff = tariffView.Where(item => item.Type == CustomsRateType.ImportTax &&
                                                            item.Origin == classifyProduct.Origin &&
                                                            item.CustomsTariffID == classifyProduct.Category.HSCode).FirstOrDefault();
                            if (reTariff != null)
                            {
                                tariff.Rate = reTariff.Rate / 100 + classifyResult.TariffRate.Value;
                            }
                            else
                            {
                                tariff.Rate = classifyResult.TariffRate.Value;
                            }
#endif
#pragma warning restore

                            classifyProduct.ImportTax = tariff;

                            OrderItemTax ValueAddedTax = new OrderItemTax();
                            ValueAddedTax.Type = CustomsRateType.AddedValueTax;
                            ValueAddedTax.Rate = classifyResult.AddedValueRate.Value;
                            classifyProduct.AddedValueTax = ValueAddedTax;

                            OrderItemTax exciseTax = new OrderItemTax();
                            exciseTax.Type = CustomsRateType.ConsumeTax;
                            exciseTax.Rate = classifyResult.ExciseTaxRate.GetValueOrDefault();
                            classifyProduct.ExciseTax = exciseTax;
                            classifyProduct.UpdateDate = classifyResult.UpdateDate;
                        }
                        else
                        {

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
                order.UpdateDate = order.CreateDate = DateTime.Now;

                if (isAllClassified)
                {
                    order.OrderStatus = OrderStatus.Classified;
                    order.ConnectionString = this.ConnectionString;
                    List<PvOrderItems> pvOrderItems = order.EnterSpeed();
                    this.OnCreated(new IcgooCreateOrderEventArgs(order, singlemodel, pvOrderItems));
                }
                else
                {
                    order.OrderStatus = OrderStatus.Confirmed;
                    order.ConnectionString = this.ConnectionString;
                    List<PvOrderItems> pvOrderItems = order.EnterSpeed();
                    this.OnPartCreated(new IcgooCreateOrderEventArgs(order, singlemodel, pvOrderItems));
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
                ex.CcsLog("Icgoo内单生成出错");
                return isSuccess;
            }


        }

        public virtual void OnCreated(IcgooCreateOrderEventArgs args)
        {
            Console.WriteLine(args.order.ID + "开始生成对账单");
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

            this.IcgooPostForWayBill?.Invoke(this,args);

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

            this.IcgooPostForWayBill?.Invoke(this, args);

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

        public void ShouldReceive(object sender, IcgooCreateOrderEventArgs e)
        {
            e.order.ToReceivables();
        }

        public void PostForWayBillOrigin(object sender, IcgooCreateOrderEventArgs e)
        {
            PvWsOrderInsApiModel pushModel = new PvWsOrderInsApiModel();
            pushModel.Items = new List<PvOrderItems>();
            pushModel.Items = e.pvOrderItems;

            List<string> payExchangeSuppliers = new List<string>();
            foreach (var t in e.order.PayExchangeSuppliers)
            {
                payExchangeSuppliers.Add(t.ClientSupplier.Name);
            }

            pushModel.PayExchangeSuppliers = payExchangeSuppliers;

            foreach (var item in pushModel.Items)
            {
                var receiveModel = e.partno.Where(t => t.sale_orderline_id == item.ProductUnicode).FirstOrDefault();
                item.PackNo = receiveModel.orgCarton.ToString();
                if (item.GrossWeight != null)
                {
                    item.NetWeight = Math.Round(item.GrossWeight.Value / 1.1M, 4, MidpointRounding.AwayFromZero);
                }
                if (payExchangeSuppliers.Count > 0)
                {
                    item.SupplierName = payExchangeSuppliers[0];
                }
            }

            pushModel.VastOrderID = e.order.MainOrderID;
            pushModel.ClientName = e.order.Client.Company.Name;
            pushModel.HKWareHouseID = System.Configuration.ConfigurationManager.AppSettings["HKWareHouseID"];
            pushModel.DeclarationCompany = System.Configuration.ConfigurationManager.AppSettings["DeclareCompany"];

            var OriginAdmin = new AdminsTopView2().FirstOrDefault(t => t.OriginID == e.order.AdminID);
            if (OriginAdmin != null)
            {
                pushModel.AdminID = OriginAdmin.ID;
            }
        }

        public void Post2NewClientForWayBillOrigin()
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
            string PostData = model.Json();
            try
            {
                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, PostData);
                //e.PvWsOrderInsApiModel.WayBillID = result;
                //string test = e.PvWsOrderInsApiModel.Json();
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

        public void PostToWareHouseOrigin(PvWsOrderInsApiModel model)
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

        public void PostForWayBill(object sender, IcgooCreateOrderEventArgs e)
        {
            PvWsOrderInsApiModel pushModel = new PvWsOrderInsApiModel();
            pushModel.Items = new List<PvOrderItems>();
            pushModel.Items = e.pvOrderItems;

            List<string> payExchangeSuppliers = new List<string>();
            foreach (var t in e.order.PayExchangeSuppliers)
            {
                payExchangeSuppliers.Add(t.ClientSupplier.Name);
            }

            pushModel.PayExchangeSuppliers = payExchangeSuppliers;

            foreach (var item in pushModel.Items)
            {
                //var receiveModel = e.partno.Where(t => t.sale_orderline_id == item.ProductUnicode).FirstOrDefault();
                //item.PackNo = receiveModel.orgCarton.ToString();
                if (item.GrossWeight != null)
                {
                    item.NetWeight = Math.Round(item.GrossWeight.Value / 1.1M, 4, MidpointRounding.AwayFromZero);
                }
                if (item.Origin.Equals("NG"))
                {
                    item.Origin = "Unknown";
                }
                if (payExchangeSuppliers.Count > 0)
                {
                    item.SupplierName = payExchangeSuppliers[0];
                }
            }

            pushModel.VastOrderID = e.order.MainOrderID;
            pushModel.ClientName = e.order.Client.Company.Name;
            pushModel.HKWareHouseID = System.Configuration.ConfigurationManager.AppSettings["HKWareHouseID"];
            pushModel.DeclarationCompany = System.Configuration.ConfigurationManager.AppSettings["DeclareCompany"];
          
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
            //this.InsidePostToWareHouse?.Invoke(this, new CenterInsideCreateOrderEventArgs(pushModel));
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
                    //var orderitem = e.order.Items.Where(item => item.Model == p.pn && item.Quantity == p.qty).FirstOrDefault();
                    #region 防止一个订单中，有型号，和数量一样的情况出现
                    var orderitems = e.order.Items.Where(item => item.Model == p.pn && item.Quantity == p.qty).ToList();
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
                        model.Quantity = p.qty;
                        model.NetWeight = Math.Round(p.nw / Icgoo.UnitConvert, 2, MidpointRounding.AwayFromZero) == 0 ? 0.01M : Math.Round(p.nw / Icgoo.UnitConvert, 2, MidpointRounding.AwayFromZero);
                        model.GrossWeight = Math.Round(model.NetWeight * Icgoo.BaseGWPara, 2, MidpointRounding.AwayFromZero);
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
    }
}
