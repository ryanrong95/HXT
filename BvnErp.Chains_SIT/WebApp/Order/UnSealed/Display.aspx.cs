using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.UnSealed
{
    /// <summary>
    /// 待封箱订单详情界面
    /// </summary>
    public partial class Display : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        public void LoadData()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];
            this.Model.Order = new
            {
                order.ID,
                order.IsHangUp
            }.Json();
        }

        /// <summary>
        /// 订单产品信息
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrders[ID];

            Func<Needs.Ccs.Services.Models.OrderItem, object> convert = item => new
            {
                item.ID,
                Batch = item.Batch,
                Name = item.Category?.Name ?? item.Name,
                Manufacturer = item.Manufacturer,
                Model = item.Model,
                Quantity = item.Quantity,
                DeclaredQuantity = item.DeclaredQuantity ?? 0,
                Origin = item.Origin,
                GrossWeight = item.GrossWeight,
                TotalPrice = item.TotalPrice.ToRound(2),
                ProductDeclareStatus = item.ProductDeclareStatus.GetDescription()
            };

            Response.Write(new
            {
                rows = order.Items.Select(convert).ToArray(),
                total = order.Items.Count()
            }.Json());
        }

        /// <summary>
        /// 装箱信息
        /// </summary>
        protected void dataPackings()
        {
            string OrderID = Request.QueryString["ID"];
            var packingBill = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.GetSortingPacking();
            var datas = packingBill.Where(Item => Item.OrderID == OrderID);
            Func<SortingPacking, object> convert = item => new
            {
                ID = item.ID,
                PackingID = item.Packing.ID,
                SortingID = item.ID,
                BoxIndex = item.BoxIndex,
                NetWeight = item.NetWeight,
                GrossWeight = item.GrossWeight,
                Model = item.OrderItem.Model,//产品型号
                ProductName = item.OrderItem.Name,  //产品名称
                CustomsName = item.OrderItem.Category.Name,  //报关品名
                Quantity = item.Quantity,
                Origin = item.OrderItem.Origin,
                Manufacturer = item.OrderItem.Manufacturer,
                DecStatus = item.DecStatus.GetDescription(),
                Status = item.Packing.PackingStatus.GetDescription(),
                PickDate = item.Packing.PackingDate.ToString("yyyy-MM-dd")
            };
            Response.Write(new
            {
                rows = datas.Select(convert).ToArray(),
                total = datas.Count()
            }.Json());
        }
    }
}