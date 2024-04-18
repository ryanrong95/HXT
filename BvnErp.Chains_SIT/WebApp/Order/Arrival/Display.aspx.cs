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

namespace WebApp.Order.Arrival
{
    /// <summary>
    /// 分批到货订单展示界面
    /// 展示订单及库房分拣信息
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
                Name = item.Name,
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
                PickDate = item.Packing.PackingDate.ToString()
            };
            Response.Write(new
            {
                rows = datas.Select(convert).ToArray(),
                total = datas.Count()
            }.Json());
        }

        /// <summary>
        /// 拆分报关单
        /// </summary>
        protected void SplitDeclare()
        {
            try
            {
                string ids = Request.Form["ID"];
                string OrderID = Request.Form["OrderID"];
                string Summary = Request.Form["Summary"];
                var sortingIDs = ids.Split(',').ToList();

                var decnotice = new DeclarationNotice();
                //decnotice.ID = ChainsGuid.NewGuidUp();
                decnotice.OrderID = OrderID;
                decnotice.Admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                decnotice.Summary = Summary;

                var sortings = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.SortingPackings.Where(sort => sortingIDs.Contains(sort.ID));
                foreach (var sort in sortings)
                {
                    decnotice.Items.Add(new DeclarationNoticeItem
                    {
                        ID = ChainsGuid.NewGuidUp(),
                        DeclarationNoticeID = decnotice.ID,
                        Sorting = sort
                    });
                }

                decnotice.EnterError += DeclareNotice_EnterError;
                decnotice.EnterSuccess += DeclareNotice_EnterSuccess;
                decnotice.CreateNotice();
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeclareNotice_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DeclareNotice_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}