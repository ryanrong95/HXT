using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Ccs.Services.Views;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using YuanDa_Logistics.Utility;

namespace WebApp.ExitOrder.Exit
{
    /// <summary>
    /// 出库-快递单
    /// 深圳库房
    /// </summary>
    public partial class ExpressBill : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {          
            LoadData();
        }

        private string getExpressType(string ExpressCompany,int EXEValue)
        {
            string ExtName = "";
            switch (ExpressCompany)
            {
                case "SF":
                    Type tSF = typeof(SfExpType);
                    var SFins = Activator.CreateInstance(tSF) as CodeType;
                    ExtName = SFins.Where(t2 => t2.Value == EXEValue).Select(item => item.Name).FirstOrDefault();                   
                    break;

                case "KY":
                    Type tKY = typeof(KysyExpType);
                    var KYins = Activator.CreateInstance(tKY) as CodeType;
                    ExtName = KYins.Where(t2 => t2.Value == EXEValue).Select(item => item.Name).FirstOrDefault();
                    break;
            }

            return ExtName;  
        }

        protected void LoadData()
        {
            this.Model.ExitNotice = "";
            string ID = Request["ExitNoticeID"];
            if (string.IsNullOrEmpty(ID) == true)
            {
                return;
            }
            var exitNotice = new CenterSZExitNoticeItemView().Where(item => item.ID == ID).FirstOrDefault();

            if (exitNotice != null)
            {
                this.Model.ExitNotice = new
                {
                    ExitNoticeID = exitNotice.ID,
                    OrderID = exitNotice.OrderID,
                    ClientName = exitNotice.ConsigneeCompany,
                    Contactor = exitNotice.ConsigneeContact,//提货人
                    ContactTel = exitNotice.ConsigneeContact,
                    Address = exitNotice.ConsigneeAddress,
                    ExpressComp = exitNotice.ConsignorContact,
                   // ExpressTy = exitNotice.ExpressBill.ExpressTy,
                    ExpressPayType = exitNotice.InitExPayType.GetDescription(),                   
                    ExitType = (int)WaybillType.LocalExpress,
                    DeliveryTime = exitNotice.CreateDate.ToString("yyyy-MM-dd"),
                    SZPackingDate = exitNotice.BoxingDate.ToString("yyyy-MM-dd"),//出库的装箱日期
                    Purchaser = PurchaserContext.Current.CompanyName,
                    SealUrl = "../../" + PurchaserContext.Current.BillStamp.ToUrl()
                }.Json();
            }
        }

        protected void ProductData()
        {
            string id = Request["ExitNoticeID"];
            var exitNotice = new CenterSZExitNoticeItemView().Where(item => item.ID == id).OrderBy(x => x.BoxCode);
            Func<CenterWayBill, object> convert = item => new
            {
                ID = item.ID,             
                CaseNumber = item.BoxCode,
                StockCode = item.ShelveID,
                //  PackingDate = item.Sorting.SZPackingDate?.ToString("yyyy-MM-dd") == null ? "" : item.Sorting.SZPackingDate?.ToString("yyyy-MM-dd"),  //出库的装箱日期             
                ProductName = item.DeclareName,
                Model = item.PartNumber,
                Qty = item.Quantity,               
                Manufacturer = item.Manufacturer,

            };
            Response.Write(new
            {
                rows = exitNotice.Select(convert).ToArray(),
                total = exitNotice.Count()
            }.Json());

        }


        /// <summary>
        /// 快递单信息
        /// </summary>
        protected void LoadData1()
        {
            this.Model.ExitNotice = "";
            string ID = Request["ExitNoticeID"];
            if (string.IsNullOrEmpty(ID) == true)
            {
                return;
            }
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.MySZExitNotice[ID];
            var SZPackingDate = exitNotice.SZItems.Select(x => x.Sorting.SZPackingDate).FirstOrDefault();
            if (exitNotice != null)
            {
                this.Model.ExitNotice = new
                {
                    ExitNoticeID = exitNotice.ExpressBill.ExitNoticeID,
                    OrderID = exitNotice.ExpressBill.OrderID,
                    ClientName = exitNotice.ExpressBill.ClientName,
                    Contactor = exitNotice.ExpressBill.Contactor,//提货人
                    ContactTel = exitNotice.ExpressBill.ContactTel,
                    Address = exitNotice.ExpressBill.Address,
                    ExpressComp = exitNotice.ExpressBill.ExpressComp,
                    ExpressTy = exitNotice.ExpressBill.ExpressTy,
                    ExpressPayType = exitNotice.ExpressBill.ExpressPayType.GetDescription(),
                    PackNo = exitNotice.ExpressBill.PackNo,
                    ExitType = (int)ExitType.Express,
                    DeliveryTime = exitNotice.ExpressBill.DeliveryTime.ToString("yyyy-MM-dd"),
                    SZPackingDate = SZPackingDate?.ToString("yyyy-MM-dd") == null ? "" : SZPackingDate?.ToString("yyyy-MM-dd"),//出库的装箱日期
                    Purchaser = PurchaserContext.Current.CompanyName,
                    SealUrl = "../../" + PurchaserContext.Current.BillStamp.ToUrl()
                }.Json();
            }
        }

        /// <summary>
        /// 待出库商品信息
        /// </summary>
        protected void ProductData1()
        {
            string id = Request["ExitNoticeID"];
            var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNoticeItem.Where(x => x.ExitNoticeID == id).OrderBy(x => x.StoreStorage.BoxIndex); 
            Func<SZExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                SortingID = item.Sorting.ID,
                CaseNumber = item.StoreStorage.BoxIndex,
                StockCode = item.StoreStorage.StockCode,
              //  PackingDate = item.Sorting.SZPackingDate?.ToString("yyyy-MM-dd") == null ? "" : item.Sorting.SZPackingDate?.ToString("yyyy-MM-dd"),  //出库的装箱日期
                NetWeight = item.Sorting.NetWeight,
                GrossWeight = item.Sorting.GrossWeight,
                ProductName = item.Sorting.OrderItem.Category.Name,
                Model = item.Sorting.OrderItem.Model,
                Qty = item.Quantity,
                WrapType = item.Sorting.WrapType,
                Manufacturer = item.Sorting.OrderItem.Manufacturer,

            };
            Response.Write(new
            {
                rows = exitNotice.Select(convert).ToArray(),
                total = exitNotice.Count()
            }.Json());

        }

        /// <summary>
        /// 生成快递面单
        /// </summary>
        /// <returns></returns>
        protected void GenerateExpress()
        {
            try
            {
                string ExitNoticeID = Request["ExitNoticeID"];
                var exitNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice[ExitNoticeID];

                //获取请求数据json
                string requestData = exitNotice.KDDRequestModel.Json();
                //调用快递方法
                var bs = new KdApiEOrder();
                var result = bs.orderTracesSubByJson(requestData);

                //修改请求返回结果
                if (exitNotice.KDDRequestModel.ShipperCode == "KYSY")
                {
                    //跨域默认的发货件数都是1,返回后需修改为真实数量
                    result = result.Replace("件数:1", "件数:" + exitNotice.ExitDeliver.PackNo);
                    result = result.Replace("电子元器件X1", "电子元器件X" + exitNotice.ExitDeliver.PackNo);
                }
                //隐藏月结卡号
                result = result.Replace("月结卡号:7550921123", "月结卡号:" + "075*******23");

                //获取返回结果
                var ResponseResult = result.JsonTo<KDDResultModel>();
                if (ResponseResult.Success)
                {
                    //保存快递面单数据
                    var express = exitNotice.ExitDeliver.Expressage;
                    express.WaybillCode = ResponseResult.Order.LogisticCode;
                    express.SaveKDD();
                    //Response.Write(ResponseResult.PrintTemplate.Json());
                    Response.Write(new
                    {
                        success = ResponseResult.Success,
                        message = ResponseResult.Reason,
                        LogisticCode = ResponseResult.Order?.LogisticCode,
                        PrintTemplate = ResponseResult.PrintTemplate
                    }.Json());
                }
                else
                {
                    Response.Write((new { success = false, message = ResponseResult.Reason }).Json());
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "请求失败" + ex }).Json());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected Dictionary<string, string> HandleAddress(string Address)
        {
            var Province = "";
            var City = "";
            var Area = "";
            var DetailsAddress = "";
            if (Address.Split(' ').Length == 3)
            {
                Province = Address.Split(' ')[0].Trim();
                City = Address.Split(' ')[0].Trim() + "市";
                Area = Address.Split(' ')[1].Trim();
                DetailsAddress = Address.Split(' ')[2].Trim();
            }
            else
            {
                Province = Address.Split(' ')[0].Trim();
                if (Province == "内蒙古" || Province == "西藏")
                    Province = Address.Split(' ')[0] + "自治区";
                if (Province == "新疆")
                    Province = Address.Split(' ')[0] + "维吾尔自治区";
                if (Province == "广西")
                    Province = Address.Split(' ')[0] + "壮族自治区";
                if (Province == "宁夏")
                    Province = Address.Split(' ')[0] + "回族自治区";
                else
                {
                    Province = Address.Split(' ')[0] + "省";
                }
                City = Address.Split(' ')[1].Trim();
                Area = Address.Split(' ')[2].Trim();
                DetailsAddress = Address.Split(' ')[3].Trim();
            }
            var DicAddres = new Dictionary<string, string>();
            DicAddres.Add("Province", Province);
            DicAddres.Add("City", City);
            DicAddres.Add("Area", Area);
            DicAddres.Add("DetailsAddress", DetailsAddress);
            return DicAddres;
        }
    }
}