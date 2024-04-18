using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.SZWareHouse.Exit
{
    /// <summary>
    /// 已出库订单展示界面
    /// 深圳库房
    /// </summary>
    public partial class Details : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }
 
        /// <summary>
        /// 加载出库通知详细信息
        /// </summary>
        protected void LoadData()
        {
            this.Model.ExitNotice = "";
            string id = Request["ExitNoticeID"];
            var exitNotices = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNotice
                                          .Where(x => x.ID == id).AsQueryable().FirstOrDefault();

            switch (exitNotices.ExitType)
            {
                case ExitType.PickUp:
                    if (!string.IsNullOrEmpty(id))
                    {
                        this.Model.ExitNotice = new
                        {
                            OrderId = exitNotices.Order.ID,
                            ClientName = exitNotices.ExitDeliver.Name,
                            DeliveryName = exitNotices.ExitDeliver?.Consignee.Name,//提货人
                            DeliveryTel = exitNotices.ExitDeliver?.Consignee.Mobile,
                            IDType = exitNotices.ExitDeliver?.Consignee.IDType,
                            IDCard = exitNotices.ExitDeliver?.Consignee.IDNumber,
                            PackNo = exitNotices.ExitDeliver.PackNo,
                            ExitType = (int)exitNotices.ExitType
                        }.Json();
                    }

                    break;
                case ExitType.Delivery:
                    if (!string.IsNullOrEmpty(id))
                    {
                        this.Model.ExitNotice = new
                        {
                            OrderId = exitNotices.Order.ID,
                            ClientName = exitNotices.ExitDeliver.Name,
                            Contactor = exitNotices.ExitDeliver?.Deliver.Contact,
                            ContactTel = exitNotices.ExitDeliver?.Deliver.Mobile,
                            Address = exitNotices.ExitDeliver?.Deliver.Address,//送货地址
                            DriverName = exitNotices.ExitDeliver?.Deliver?.Driver?.Name,
                            DriverTel = exitNotices.ExitDeliver?.Deliver?.Driver?.Mobile,
                            Velcro = exitNotices.ExitDeliver?.Deliver?.Vehicle?.License,
                            PackNo = exitNotices.ExitDeliver.PackNo,
                            ExitType = (int)exitNotices.ExitType
                        }.Json();
                    }
                    break;
                case ExitType.Express:
                    if (!string.IsNullOrEmpty(id))
                    {
                        this.Model.ExitNotice = new
                        {
                            OrderId = exitNotices.Order.ID,
                            ClientName = exitNotices.ExitDeliver.Name,
                            Contactor = exitNotices.ExitDeliver?.Expressage?.Contact,
                            ContactTel = exitNotices.ExitDeliver?.Expressage.Mobile,
                            Address = exitNotices.ExitDeliver?.Expressage.Address,//送货地址
                            ExpressComp = exitNotices.ExitDeliver?.Expressage?.ExpressCompany.Name,
                            ExpressTy = exitNotices.ExitDeliver?.Expressage?.ExpressType.TypeName,
                            ExpressPayType = exitNotices.ExitDeliver?.Expressage?.PayType.GetDescription(),
                            PackNo = exitNotices.ExitDeliver.PackNo,
                            ExitType = (int)exitNotices.ExitType
                        }.Json();
                    }
                    break;
                default:
                    break;
            }
        }
 
        /// <summary>
        /// 已出库商品信息
        /// </summary>
        protected void data()
        {
            string id = Request["ExitNoticeID"];
            var data = Needs.Wl.Admin.Plat.AdminPlat.Current.Warehouse.SZExitNoticeItem.Where(x => x.ExitNoticeID == id);
            Func<SZExitNoticeItem, object> convert = item => new
            {
                ID = item.ID,
                SortingID = item.Sorting.ID,
                CaseNumber = item.Sorting.BoxIndex,
                NetWeight = item.Sorting.NetWeight,
                GrossWeight = item.Sorting.GrossWeight,
                ProductName = item.Sorting.OrderItem.Category.Name,
                Model = item.Sorting.OrderItem.Model,
                Qty = item.Quantity,
                WrapType = item.Sorting.WrapType,
                Manufacturer = item.Sorting.OrderItem.Manufacturer
            };
            Response.Write(new
            {
                rows = data.Select(convert).ToArray(),
                total = data.Count()
            }.Json());
        }
    }
}