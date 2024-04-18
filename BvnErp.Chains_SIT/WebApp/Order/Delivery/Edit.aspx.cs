using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.Delivery
{
    /// <summary>
    /// 新增送货信息
    /// </summary>
    public partial class Edit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
            LoadData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.Drivers = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers.Where(item => item.Carrier.CarrierType == CarrierType.DomesticLogistics && item.Status == Status.Normal)
                                                                        .Select(item => new { item.ID, item.Name, item.Mobile, CarrierName = item.Carrier.Name }).Json();
            this.Model.Vehicles = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles.Where(item => item.Carrier.CarrierType == CarrierType.DomesticLogistics && item.Status == Status.Normal)
                .Select(item => new
                {
                    item.ID,
                    item.License,
                    Type = ((VehicleType)Enum.Parse(typeof(VehicleType), item.VehicleType.ToString())).GetDescription(),
                    CarrierName = item.Carrier.Name
                }).Json();
            this.Model.ExpressCompanies = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies.Where(item => item.Status == Status.Normal)
                                                                                            .Select(item => new { item.ID, item.Name }).Json();
            this.Model.IdType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.IDType>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.PayType = EnumUtils.ToDictionary<PayType>().Select(item => new { item.Key, item.Value }).Json();
        }

        /// <summary>
        /// 初始化订单国内交货信息
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["OrderID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders.Where(item => item.MainOrderID == id).FirstOrDefault();
            var orderConsignor = order.OrderConsignor;

            this.Model.OrderData = new
            {
                ID = order.MainOrderID,
                ClientID = order.Client.ID,
                ClientName = order.Client.Company.Name,
                SZDeliveryType = orderConsignor.Type,
                Consignee = orderConsignor.Name,
                Contact = orderConsignor.Contact,
                ContactMobile = orderConsignor.Mobile,
                Address = orderConsignor.Address,
                Picker = orderConsignor.Contact,
                PickerMobile = orderConsignor.Mobile,
                IDType = orderConsignor.IDType,
                IDNumber = orderConsignor.IDNumber,
            }.Json();
        }

        /// <summary>
        /// 初始化分拣信息
        /// </summary>
        protected void data()
        {
            var ids = Request.QueryString["IDs"].Split(',');
            //var sortings = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZSorting.Where(s => ids.Contains(s.ID));
            var sortings = new SZCenterSortingsViewForDisplay().Where(t => ids.Contains(t.ID)).ToList();
            Func<Needs.Ccs.Services.Models.SZSorting, object> convert = sorting => new
            {
                sorting.ID,
                sorting.BoxIndex,
                sorting.DateBoxIndex,
                //sorting.OrderItem.Category.Name,
                sorting.OrderItem.Name,
                sorting.OrderItem.Manufacturer,
                sorting.OrderItem.Model,
                sorting.OrderItem.Origin,
                Quantity = sorting.Quantity - sorting.DeliveriedQuantity
            };

            var sumPacks = 0;
            var alonePack = sortings.Select(item => item.BoxIndex).Distinct().ToList();
            var multi = alonePack.Where(a => a.IndexOf('-') > -1).Select(a => a.ToUpper()).Distinct().ToList();
            var sumRepeat = 0;
            multi.ForEach(a =>
            {
                try
                {
                    var arry = a.Split('-');
                    int startCase = int.Parse(arry[0].Replace("WL", ""));
                    int endCase = int.Parse(arry[1].Replace("WL", ""));
                    var repeat = alonePack.Where(c => !c.Contains("-")).Where(c => int.Parse(c.ToUpper().Replace("WL", "")) >= startCase && int.Parse(c.ToUpper().Replace("WL", "")) <= endCase).Count();
                    sumRepeat += repeat;
                    sumPacks += endCase - startCase + 1;
                }
                catch (Exception ex)
                {

                }
            });
            sumPacks += alonePack.Count() - multi.Count() - sumRepeat;
            var TotalPacks = sumPacks;
            Response.Write(new
            {
                rows = sortings.Select(convert).ToArray(),
                total = sortings.Count(),
                //计算件数

                packNo = TotalPacks
            }.Json());
        }

        /// <summary>
        /// 选择快递公司时，获取快递公司的快递方式
        /// </summary>
        /// <returns></returns>
        protected object GetExpressType()
        {
            string expressCompanyID = Request.Form["ExpressCompanyID"];
            string shipperCode = "";
            switch (expressCompanyID)
            {
                case "顺丰":
                    shipperCode = "SF";
                    break;
                case "跨越速运":
                    shipperCode = "KYSY";
                    break;
                case "EMS":
                    shipperCode = "EMS";
                    break;
            }

            var types = new[] { typeof(SfExpType), typeof(KysyExpType), typeof(EmsExpType) };
            var type = types.SingleOrDefault(item => item.Name.StartsWith(shipperCode,
                StringComparison.OrdinalIgnoreCase));
            var ins = Activator.CreateInstance(type) as CodeType;

            var data = ins.Select(t => new
            {
                ID = t.Value,
                TypeName = t.Name
            });

            return data;

            //string expressCompanyID = Request.Form["ExpressCompanyID"];
            //var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes.Where(item => item.ExpressCompany.ID == expressCompanyID)
            //                                                                                .Select(item => new
            //                                                                                {
            //                                                                                    item.ID,
            //                                                                                    item.TypeName
            //                                                                                });

            //return data;
        }

        /// <summary>
        /// 获取客户的默认收件人
        /// </summary>
        /// <returns></returns>
        protected object GetDefaultConsignee()
        {
            var clientID = Request.Form["ClientID"];
            var defaultConsignee = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientConsignees.Where(item => item.ClientID == clientID &&
                                                                                        item.IsDefault && item.Status == Status.Normal)
                                                                                        .Select(item => new
                                                                                        {
                                                                                            item.ID,
                                                                                            item.Name,
                                                                                            Contact = item.Contact.Name,
                                                                                            item.Contact.Mobile,
                                                                                            item.Address
                                                                                        }).FirstOrDefault();

            return defaultConsignee;
        }

        /// <summary>
        /// 提交送货信息 原来的
        /// </summary>
        protected void SubmitDeliveryInfo1()
        {
            try
            {
                #region 前台数据

                //1)送货方式
                var orderID = Request.Form["OrderID"];
                var szDeliveryType = Request.Form["SZDeliveryType"];
                var pickupDate = Request.Form["PickUpDate"];
                var client = Request.Form["Client"];
                var picker = Request.Form["picker"];
                var pickerMobile = Request.Form["pickerMobile"];
                var idType = Request.Form["IDType"];
                var idNumber = Request.Form["IDNumber"];
                var deliverDate = Request.Form["DeliverDate"];
                var consignee = Request.Form["Consignee"];
                var contact = Request.Form["Contact"];
                var contactMobile = Request.Form["ContactMobile"];
                var address = Request.Form["Address"];
                var packNo = Request.Form["PackNo"];
                var driver = Request.Form["Driver"];
                var vehicle = Request.Form["Vehicle"];
                var expressCompany = Request.Form["ExpressCompany"];
                var expressType = Request.Form["ExpressType"];
                var payType = Request.Form["PayType"];


                //2)送货信息
                var sortings = Request.Form["Sortings"].Replace("&quot;", "'");
                var sortingList = sortings.JsonTo<List<dynamic>>();

                #endregion

                //出库通知
                var exitNotice = new SZExitNotice();
                exitNotice.WarehouseType = WarehouseType.ShenZhen;
                var order = new Needs.Ccs.Services.Models.Order();
                order.ID = orderID;
                exitNotice.Order = order;
                exitNotice.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                //送货信息
                exitNotice.ExitDeliver = new ExitDeliver();
                exitNotice.ExitDeliver.ExitNoticeID = exitNotice.ID;
                exitNotice.ExitDeliver.Code = exitNotice.ID;
                exitNotice.ExitDeliver.PackNo = int.Parse(packNo);

                var deliveryType = (SZDeliveryType)Enum.Parse(typeof(SZDeliveryType), szDeliveryType);
                if (deliveryType == SZDeliveryType.PickUpInStore)
                {
                    exitNotice.ExitType = ExitType.PickUp;
                    exitNotice.ExitDeliver.Name = client;
                    exitNotice.ExitDeliver.DeliverDate = DateTime.Parse(pickupDate);

                    //提货人
                    exitNotice.ExitDeliver.Consignee = new Consignee();
                    exitNotice.ExitDeliver.Consignee.Name = picker;
                    exitNotice.ExitDeliver.Consignee.Mobile = pickerMobile;
                    exitNotice.ExitDeliver.Consignee.IDType = (Needs.Ccs.Services.Enums.IDType)Enum.Parse(typeof(Needs.Ccs.Services.Enums.IDType), idType);
                    exitNotice.ExitDeliver.Consignee.IDNumber = idNumber;
                }
                else if (deliveryType == SZDeliveryType.SentToClient)
                {
                    exitNotice.ExitType = ExitType.Delivery;
                    exitNotice.ExitDeliver.Name = consignee;
                    exitNotice.ExitDeliver.DeliverDate = DateTime.Parse(deliverDate);

                    //送货人
                    exitNotice.ExitDeliver.Deliver = new Deliver();
                    exitNotice.ExitDeliver.Deliver.Driver = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Drivers[driver];
                    exitNotice.ExitDeliver.Deliver.Vehicle = Needs.Wl.Admin.Plat.AdminPlat.Current.Customs.Vehicles[vehicle];
                    exitNotice.ExitDeliver.Deliver.Contact = contact;
                    exitNotice.ExitDeliver.Deliver.Mobile = contactMobile;
                    exitNotice.ExitDeliver.Deliver.Address = address;
                }
                else if (deliveryType == SZDeliveryType.Shipping)
                {
                    exitNotice.ExitType = ExitType.Express;
                    exitNotice.ExitDeliver.Name = consignee;
                    exitNotice.ExitDeliver.DeliverDate = DateTime.Parse(deliverDate);

                    //快递
                    exitNotice.ExitDeliver.Expressage = new Expressage();
                    exitNotice.ExitDeliver.Expressage.Contact = contact;
                    exitNotice.ExitDeliver.Expressage.Mobile = contactMobile;
                    exitNotice.ExitDeliver.Expressage.Address = address;
                    exitNotice.ExitDeliver.Expressage.ExpressCompany = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressCompanies[expressCompany];
                    exitNotice.ExitDeliver.Expressage.ExpressType = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.ExpressTypes[expressType];
                    exitNotice.ExitDeliver.Expressage.PayType = (PayType)Enum.Parse(typeof(PayType), payType);
                }

                //出库通知项
                foreach (var sorting in sortingList)
                {
                    string sortingID = sorting.ID;
                    exitNotice.Items.Add(new ExitNoticeItem
                    {
                        ExitNoticeID = exitNotice.ID,
                        Sorting = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZSorting[sortingID],
                        Quantity = sorting.DeliveryQuantity,
                    });
                }

                exitNotice.EnterSuccess += Notice_EnterSuccess;
                //exitNotice.Enter();
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 提交成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Notice_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "提交成功！", ID = e.Object }).Json());
        }


        protected void SubmitDeliveryInfo()
        {
            #region 前台数据

            //1)送货方式
            var orderID = Request.Form["OrderID"];
            var szDeliveryType = Request.Form["SZDeliveryType"];
            var pickupDate = Request.Form["PickUpDate"];
            var client = Request.Form["Client"];
            var picker = Request.Form["picker"];
            var pickerMobile = Request.Form["pickerMobile"];
            var idType = Request.Form["IDType"];
            var idNumber = Request.Form["IDNumber"];
            var deliverDate = Request.Form["DeliverDate"];
            var consigneeCompanyName = Request.Form["Consignee"];
            var contact = Request.Form["Contact"];
            var contactMobile = Request.Form["ContactMobile"];
            var address = Request.Form["Address"];
            var packNo = Request.Form["PackNo"];
            var driver = Request.Form["Driver"];
            var driverMobile = Request.Form["DriverMobile"];
            var driverName = Request.Form["DriverName"];
            var vehicle = Request.Form["Vehicle"];
            var vehicleName = Request.Form["VehicleName"];
            var expressCompany = Request.Form["ExpressCompany"];
            var ExpressCompanyName = Request.Form["ExpressCompanyName"];
            var expressType = Request.Form["ExpressType"];
            var payType = Request.Form["PayType"];
            var thirdPartyCardNo = Request.Form["ThirdPartyCardNo"];
            var CarrierName = Request.Form["CarrierName"];

            //2)送货信息
            var sortings = Request.Form["Sortings"].Replace("&quot;", "'");
            var sortingList = sortings.JsonTo<List<dynamic>>();

            #endregion

            try
            {
                #region 组装数据
                var waybill = new PickingWaybill();
                waybill.CarrierName = CarrierName;
                waybill.CreateDate = DateTime.Now;
                waybill.ClientName = client;//？
                waybill.WaybillType = (WaybillType)Enum.Parse(typeof(WaybillType), szDeliveryType); ;
                //waybill.CarrierID = voyage.CarrierCode;//?要提供，怎么取
                waybill.CarrierName = ExpressCompanyName;//？
                waybill.InitExType = expressType;
                waybill.InitPayType = payType;
                waybill.Place = "CHN";
                waybill.IsAuto = false;
                waybill.Express = new Express
                {
                    ExType = expressType,
                    ExPayType = payType,
                    ThirdPartyCardNo = thirdPartyCardNo
                };

                Purchaser purchaser = PurchaserContext.Current;
                //收货人
                switch (waybill.WaybillType)
                {
                    case WaybillType.PickUp:
                        waybill.Consignee = new WayParter
                        {
                            Company = client,//？
                            Place = "CHN",
                            Address = purchaser.Address,
                            Contact = picker,
                            Phone = pickerMobile,
                            IDType = IDType.IDCard,
                            IDNumber = idNumber,
                        };

                        waybill.AppointTime = Convert.ToDateTime(pickupDate);
                        waybill.FreightPayer = WaybillPayer.Consignee;
                        break;



                    case WaybillType.DeliveryToWarehouse:
                        waybill.Consignee = new WayParter
                        {
                            Company = consigneeCompanyName,//？
                            Place = "CHN",
                            Address = address,
                            Contact = contact,
                            Phone = contactMobile,
                        };

                        waybill.WayLoading = new WayLoading
                        {
                            TakingDate = Convert.ToDateTime(deliverDate),
                            TakingAddress = purchaser.Address,
                            TakingContact = picker,
                            TakingPhone = driverMobile,
                            Driver = driverName,
                            CarNumber1 = vehicleName
                        };

                        waybill.AppointTime = Convert.ToDateTime(deliverDate);
                        waybill.FreightPayer = WaybillPayer.Consignor;
                        break;

                    case WaybillType.LocalExpress:
                        waybill.Consignee = new WayParter
                        {
                            Company = consigneeCompanyName,//？
                            Place = "CHN",
                            Address = address,
                            Contact = contact,
                            Phone = contactMobile,
                        };

                        waybill.AppointTime = Convert.ToDateTime(deliverDate);
                        waybill.FreightPayer = payType == ((int)PayType.CollectPay).ToString() ? WaybillPayer.Consignor : WaybillPayer.Consignee;
                        break;

                }


                //发货人            
                waybill.Consignor = new WayParter
                {
                    Company = purchaser.CompanyName,
                    Place = "CHN",
                    Address = purchaser.Address,
                    Contact = purchaser.Contact,
                    Phone = purchaser.Tel
                };



                waybill.TotalParts = Convert.ToInt16(packNo);//?总件数
                //waybill.TotalPieces = Convert.ToInt16(packNo);//?总件数

                waybill.Status = GeneralStatus.Normal;
                waybill.OrderID = orderID;
                waybill.Source = CgNoticeSource.AgentBreakCustoms;
                waybill.NoticeType = CgNoticeType.Out;

                waybill.ExcuteStatus = CgPickingExcuteStatus.Picking;

                var noitces = new List<PickingNotice>();

                var whsID = System.Configuration.ConfigurationManager.AppSettings["SZWareHouseID"];
                Dictionary<string, decimal> sortQty = new Dictionary<string, decimal>();
                var sortingId = string.Empty;
                var deliveryQty = 0M;
                foreach (var sorting in sortingList)
                {
                    sortingId = sorting.ID;
                    deliveryQty = sorting.DeliveryQuantity;
                    sortQty.Add(sortingId, deliveryQty);
                }

                var sort = new Needs.Ccs.Services.Views.CgSzStoragesTopView().Where(t => t.WareHouseID == whsID && sortQty.Keys.Contains(t.ID)).ToArray();

                System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;
                List<System.Linq.Expressions.LambdaExpression> lambdas = new List<System.Linq.Expressions.LambdaExpression>();
                System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ID == sort.FirstOrDefault().TinyOrderID;
                lambdas.Add(lambda);
                var order = new Orders1ViewBase<Needs.Ccs.Services.Models.Order>().GetAlls(expression, lambdas.ToArray()).FirstOrDefault();

                //设置业务员为发货人
                var saleman = new ClientAdminsView().Where(t => t.ClientID == order.ClientID && t.Type == ClientAdminType.ServiceManager).FirstOrDefault();
                if (saleman != null)
                {
                    waybill.Consignor.Contact = saleman.Admin.RealName ?? "";
                    waybill.Consignor.Phone = saleman.Admin.Mobile ?? "";
                }

                var dic = MultiEnumUtils.ToDictionaryStr<Needs.Ccs.Services.Models.ApiModels.Warehouse.Currency>();
                var curr = (Needs.Ccs.Services.Models.ApiModels.Warehouse.Currency)int.Parse(dic.First(t => t.Value == order.Currency).Key);

                var declists = new DecOriginListsView().Where(t => t.OrderID.Contains(orderID)).ToList();

                //ERM跟单员ID
                var ermAdminID = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == order.AdminID)?.ID;

                var grossWeight = 0M;
                foreach (var item in sort)
                {
                    grossWeight += (item.Weight == null ? 0 : item.Weight.Value);
                    var notice = new PickingNotice();

                    notice.Product = new Needs.Ccs.Services.Models.CenterProduct
                    {
                        PartNumber = item.PartNumber,
                        Manufacturer = item.Manufacturer
                    };

                    notice.Output = new Output
                    {
                        InputID = item.InputID,
                        OrderID = orderID,
                        TinyOrderID = item.TinyOrderID,
                        ItemID = item.ItemID,
                        StorageID = item.ID,
                        Price = item.UnitPrice,
                        OwnerID = order.AdminID,
                        Currency = curr,
                        TrackerID = ermAdminID
                    };

                    //
                    notice.Quantity = sortQty[item.ID];
                    notice.Type = CgNoticeType.Out;
                    notice.WareHouseID = whsID;

                    notice.InputID = item.InputID;
                    notice.ProductID = item.ProductID;
                    notice.StorageID = item.ID;

                    notice.DateCode = item.DateCode;        //批次号
                                                            //notice.Conditions = item.Conditions;       //条件
                    notice.CreateDate = DateTime.Now;       //创建时间
                    notice.Status = NoticesStatus.Waiting;      //状态：等待Waiting、关闭Closed、（货物）丢失Lost、完成Completed
                    notice.Source = NoticeSource.AgentBreakCustoms;        //来源
                    notice.Target = NoticesTarget.Customs;        //目标
                    notice.BoxCode = item.DateBoxCode;        //箱号
                    notice.Weight = item.Weight;        //重量
                    notice.Volume = item.Volume;       //体积
                                                       //notice.ShelveID = item.StoShelveID;        //货架编号

                    notice.CustomsName = declists.FirstOrDefault(t => t.OrderItemID == item.ItemID).GName;
                    notice.Origin = item.Origin;
                    noitces.Add(notice);
                }

                //waybill.Notices = noitces.ToArray();
                waybill.TotalWeight = grossWeight;
                waybill.EnterCode = order.Client.ClientCode;
                #endregion

                #region 插入发出商品
                //Task.Run(() =>
                //{
                //    IOutGoodsAdd outGoodsAdd = new StockOut();
                //    outGoodsAdd.OrderID = orderID + "-01";
                //    outGoodsAdd.OrderItems = new List<string>();
                //    foreach (var item in noitces)
                //    {
                //        outGoodsAdd.OrderItems.Add(item.Output.ItemID);
                //    }

                //    OutGoodsContext outGoodsContext = new OutGoodsContext(outGoodsAdd);
                //    outGoodsContext.context();
                //});
                #endregion

                #region 接口调用
                var postdata = new { Waybill = waybill, Notices = noitces.ToArray() };
                var apisetting = new Needs.Ccs.Services.ApiSettings.PfWmsApiSetting();
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SzExitNotice;
                var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, postdata);
                var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Needs.Underly.JMessage>(result);
                if (!message.success)
                {
                    Response.Write((new { success = false, message = "提交失败!" + message.data }).Json());
                }
                else
                {
                    Response.Write((new { success = true, message = "提交成功！" }).Json());
                }

                //记录日志
                string batchID = Guid.NewGuid().ToString("N");
                DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    BatchID = batchID,
                    OrderID = waybill.OrderID,
                    //TinyOrderID = decHead.OrderID,
                    Url = apiurl,
                    RequestContent = postdata.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                    Summary = "调用库房深圳出库通知接口"
                };
                apiLog.ResponseContent = message.data;
                apiLog.Enter();

                #endregion
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
        }
    }
}
