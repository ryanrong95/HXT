using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Serializers;

namespace Needs.Wl.CustomsTool
{
    public class GenerateExitNotice
    {
        //日志记录
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        private DecHead decHead;

        public GenerateExitNotice(DecHead DecHead)
        {
            decHead = DecHead;
        }


        //public void Excute1()
        //{

        //    try
        //    {
        //        var voyage = new Needs.Wl.Models.Views.VoyagesView().Where(t => t.ID == decHead.VoyNo).FirstOrDefault();

        //        System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;
        //        List<System.Linq.Expressions.LambdaExpression> lambdas = new List<System.Linq.Expressions.LambdaExpression>();
        //        System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ID == decHead.OrderID;
        //        lambdas.Add(lambda);
        //        var order = new Orders1ViewBase<Needs.Ccs.Services.Models.Order>().GetAlls(expression, lambdas.ToArray()).FirstOrDefault();


        //        var waybill = new PickingWaybill();
        //        waybill.CreateDate = DateTime.Now;
        //        waybill.ClientName = decHead.OwnerName;
        //        waybill.WaybillType = WaybillType.DeliveryToWarehouse;
        //        waybill.CarrierID = voyage.CarrierCode;
        //        waybill.CarrierName = voyage.CarrierName;
        //        waybill.Place = "CHN";
        //        waybill.IsAuto = order.Client.ClientType == Ccs.Services.Enums.ClientType.External ? false : true;

        //        //收货人
        //        Ccs.Services.Purchaser purchaser = Ccs.Services.PurchaserContext.Current;
        //        waybill.Consignee = new WayParter
        //        {
        //            Company = purchaser.CompanyName,
        //            Place = "CHN",
        //            Address = purchaser.Address,
        //            Contact = purchaser.Contact,
        //            Phone = purchaser.Tel

        //        };
        //        //发货人
        //        Ccs.Services.Vendor vender = Ccs.Services.VendorContext.Current;
        //        waybill.Consignor = new WayParter
        //        {
        //            Company = vender.OverseasConsignorCname,
        //            Place = "CHN",
        //            Address = vender.Address,
        //            Contact = vender.Contact,
        //            Phone = vender.Tel
        //        };
        //        //运输批次
        //        waybill.WayChcd = new WayChcd
        //        {
        //            LotNumber = decHead.VoyNo,
        //            CarNumber1 = voyage.HKLicense,
        //            CarNumber2 = voyage.VehicleLicence,
        //            Carload = voyage.VehicleType,//库房自行转换
        //            IsOnevehicle = true,
        //            Driver = voyage.DriverName,
        //            PlanDate = voyage.TransportTime,
        //            TotalQuantity = 0//??
        //        };
        //        waybill.TotalParts = decHead.PackNo;
        //        waybill.TotalPieces = decHead.PackNo;
        //        waybill.TotalWeight = decHead.GrossWt;
        //        //waybill.TotalVolume = "";
        //        waybill.VoyageNumber = decHead.VoyNo;
        //        waybill.Status = GeneralStatus.Normal;
        //        waybill.OrderID = order.MainOrderID;
        //        waybill.Source = NoticeSource.AgentBreakCustoms;
        //        //waybill.Condition = new WayCondition().Json();//无特殊要求，可以不填
        //        waybill.ExcuteStatus = PickingExcuteStatus.Waiting;

        //        var noitces = new List<PickingNotice>();

        //        var whsID = System.Configuration.ConfigurationManager.AppSettings["HKWareHouseID"];
        //        var sort = new Needs.Ccs.Services.Views.SortingTopView().Where(t => t.WarehouseID == whsID && t.TinyOrderID == decHead.OrderID).ToArray();

        //        var dic = MultiEnumUtils.ToDictionaryStr<Ccs.Services.Models.ApiModels.Warehouse.Currency>();
        //        var curr = (Ccs.Services.Models.ApiModels.Warehouse.Currency)int.Parse(dic.First(t => t.Value == order.Currency).Key);

        //        foreach (var item in sort)
        //        {
        //            var notice = new PickingNotice();

        //            notice.Product = new Ccs.Services.Models.CenterProduct
        //            {
        //                PartNumber = item.PartNumber,
        //                Manufacturer = item.Manufacturer
        //            };

        //            notice.Output = new Output
        //            {
        //                InputID = item.StoInputID,
        //                OrderID = order.MainOrderID,
        //                TinyOrderID = order.ID,
        //                ItemID = item.ItemID,
        //                StorageID = item.ID,
        //                Price = item.UnitPrice * item.Quantity,
        //                OwnerID = order.AdminID,
        //                Currency = curr
        //            };

        //            //
        //            notice.Quantity = item.Quantity;
        //            notice.Type = NoticeType.CustomsOut;
        //            notice.WareHouseID = whsID;
        //            //notice.WaybillID = waybill.WaybillID;
        //            notice.InputID = item.StoInputID;
        //            notice.ProductID = item.ProductID;

        //            //notice.Supplier = item.StoSupplier;       //
        //            notice.DateCode = item.DateCode;        //批次号
        //            //notice.Conditions = item.Conditions;       //条件
        //            notice.CreateDate = DateTime.Now;       //创建时间
        //            notice.Status = NoticesStatus.Waiting;      //状态：等待Waiting、关闭Closed、（货物）丢失Lost、完成Completed
        //            notice.Source = NoticeSource.AgentBreakCustoms;        //来源
        //            notice.Target = NoticesTarget.Customs;        //目标
        //            notice.BoxCode = item.BoxCode;        //箱号
        //            notice.Weight = item.Weight;        //重量
        //            notice.Volume = item.Volume;       //体积
        //            //notice.ShelveID = item.StoShelveID;        //货架编号

        //            noitces.Add(notice);
        //        }

        //        waybill.Notices = noitces.ToArray();

        //        #region 接口调用
        //        var apisetting = new Ccs.Services.ApiSettings.PfWmsApiSetting();
        //        var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.HKExitNotice;
        //        var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, waybill);
        //        var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Underly.JMessage>(result);
        //        if (message.code != 200)
        //        {
        //            Logger.Trace("香港出库通知调用库房失败：" + decHead.OrderID + " error:" + message.data);
        //        }
        //        #endregion
        //    }
        //    catch (Exception ex)
        //    {
        //        ex.CcsLog("香港出库通知调用库房");
        //    }
        //}

        //public void Excute()
        //{
        //    try
        //    {
        //        var voyage = new Needs.Wl.Models.Views.VoyagesView().Where(t => t.ID == decHead.VoyNo).FirstOrDefault();

        //        System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;
        //        List<System.Linq.Expressions.LambdaExpression> lambdas = new List<System.Linq.Expressions.LambdaExpression>();
        //        System.Linq.Expressions.Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ID == decHead.OrderID;
        //        lambdas.Add(lambda);
        //        var order = new Orders1ViewBase<Needs.Ccs.Services.Models.Order>().GetAlls(expression, lambdas.ToArray()).FirstOrDefault();


        //        var cgDeclare = new CgDelcare();

        //        #region 组装运单
        //        var waybill = new CgWaybill();

        //        waybill.ClientName = decHead.OwnerName;
        //        waybill.CarrierID = voyage.CarrierCode;
        //        waybill.CarrierName = voyage.CarrierName;
        //        waybill.Type = WaybillType.DeliveryToWarehouse;
        //        waybill.FreightPayer = WaybillPayer.Consignor;
        //        waybill.Source = order.Client.ClientType == Ccs.Services.Enums.ClientType.External ? CgNoticeSource.AgentBreakCustoms : CgNoticeSource.AgentBreakCustomsForIns;
        //        waybill.NoticeType = CgNoticeType.Out;

        //        waybill.Status = GeneralStatus.Normal;
        //        waybill.OrderID = order.MainOrderID;
        //        waybill.CreateDate = DateTime.Now;

        //        //收货人
        //        Ccs.Services.Purchaser purchaser = Ccs.Services.PurchaserContext.Current;
        //        waybill.Consignee = new WayParter
        //        {
        //            Company = purchaser.CompanyName,
        //            Place = "CHN",
        //            Address = purchaser.Address,
        //            Contact = purchaser.Contact,
        //            Phone = purchaser.Tel

        //        };

        //        //发货人
        //        Ccs.Services.Vendor vender = Ccs.Services.VendorContext.Current;
        //        waybill.Consignor = new WayParter
        //        {
        //            Company = vender.OverseasConsignorCname,
        //            Place = "CHN",
        //            Address = vender.Address,
        //            Contact = vender.Contact,
        //            Phone = vender.Tel
        //        };

        //        //运输批次
        //        waybill.WayChcd = new WayChcd
        //        {
        //            LotNumber = decHead.VoyNo,
        //            CarNumber1 = voyage.HKLicense,
        //            CarNumber2 = voyage.VehicleLicence,
        //            Carload = voyage.VehicleType,//库房自行转换
        //            IsOnevehicle = true,
        //            Driver = voyage.DriverName,
        //            PlanDate = voyage.TransportTime,
        //        };

        //        cgDeclare.HkExitWaybill = waybill;

        //        #endregion

        //        #region 组装notice

        //        var notices = new List<CgNotice>();

        //        var whsID = System.Configuration.ConfigurationManager.AppSettings["HKWareHouseID"];
        //        var sort = new Needs.Ccs.Services.Views.DeclaresTopView().Where(t =>t.TinyOrderID == decHead.OrderID).ToArray();

        //        var dic = MultiEnumUtils.ToDictionaryStr<Ccs.Services.Models.ApiModels.Warehouse.Currency>();
        //        var curr = (Ccs.Services.Models.ApiModels.Warehouse.Currency)int.Parse(dic.First(t => t.Value == order.Currency).Key);

        //        //ERM跟单员ID
        //        var ermAdminID = new Needs.Ccs.Services.Views.AdminsTopView2().FirstOrDefault(x => x.OriginID == order.AdminID)?.ID;

        //        foreach (var item in sort)
        //        {
        //            var notice = new CgNotice();

        //            notice.Type = CgNoticeType.Out;
        //            notice.WareHouseID = whsID;
        //            notice.InputID = item.InputID;
        //            notice.OutputID = item.OutputID;
        //            notice.Quantity = item.Quantity;
        //            notice.Source = NoticeSource.AgentBreakCustoms;
        //            notice.Target = NoticesTarget.Customs;
        //            notice.BoxCode = item.BoxCode;
        //            notice.Weight = item.Weight;
        //            notice.BoxDate = item.BoxingDate;

        //            notice.Product = new Ccs.Services.Models.CenterProduct
        //            {
        //                PartNumber = item.PartNumber,
        //                Manufacturer = item.Manufacturer
        //            };

        //            notice.Output = new Output
        //            {
        //                ID = item.OutputID,
        //                InputID = item.InputID,
        //                OrderID = order.MainOrderID,
        //                TinyOrderID = order.ID,
        //                ItemID = item.ItemID,
        //                Currency = curr,
        //                Price = decHead.Lists.FirstOrDefault(t=>t.OrderItemID == item.ItemID && t.OrderID == order.ID).DeclTotal,
        //                Checker = ermAdminID
        //            };

        //            notices.Add(notice);
        //        }

        //        cgDeclare.Notices = notices;

        //        #endregion

        //        #region 接口调用
        //        var apisetting = new Ccs.Services.ApiSettings.PfWmsApiSetting();
        //        var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.CgHKExitNotice;
        //        var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, cgDeclare);
        //        var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Underly.JMessage>(result);
        //        if (message.code != 200)
        //        {
        //            Logger.Trace("香港出库通知调用库房失败：" + decHead.OrderID + " error:" + message.data);
        //        }
        //        #endregion

        //    }
        //    catch (Exception ex)
        //    {
        //        ex.CcsLog("香港出库通知调用库房");
        //    }
        //}
    }
}
