using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Control.Merchandiser
{
    /// <summary>
    /// 管控产品详情界面
    /// 用于跟单员查看尚处于北京总部审核阶段的订单管控信息（3C、禁运）
    /// </summary>
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化管控数据
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            //新增的状态，判断是否是风控审批
            //string listStatus = Request.QueryString["Ststus"];

            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id]; //;: Needs.Wl.Admin.Plat.AdminPlat.Current.Control.RiskControlApprovalNotHangUp1[id];
            if (control == null)
            {
                this.Model.ControlData = null;
            }
            else
            {
                this.Model.ControlData = new
                {
                    control.ID,
                    OrderID = control.Order.ID,
                    ClientName = control.Order.Client.Company.Name,
                    ClientRank = control.Order.Client.ClientRank,
                    DeclarePrice = control.Order.DeclarePrice,
                    Currency = control.Order.Currency,
                    Merchandiser = control.Order.Client.Merchandiser.RealName,
                    ControlType = control.ControlType.GetDescription()
                }.Json();
            }

        }

        /// <summary>
        /// 初始化管控产品列表
        /// </summary>
        protected void data()
        {
            string id = Request.QueryString["ID"];
            //新增的状态，判断是否是风控审批
            //string listStatus = Request.QueryString["Ststus"];

            var control = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id];//listStatus == "Edit" ? Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls[id] : Needs.Wl.Admin.Plat.AdminPlat.Current.Control.RiskControlApprovalNotHangUp1[id];
            //
            if (control != null)
            {
                Func<OrderControlItem, object> convert = item => new
                {
                    item.ID,
                    item.OrderID,
                    OrderItemID = item.OrderItem.ID,
                    ControlType = item.ControlType.GetDescription(),
                    item.OrderItem.Category.Name,
                    item.OrderItem.Model,
                    item.OrderItem.Manufacturer,
                    item.OrderItem.Category.HSCode,
                    item.OrderItem.Quantity,
                    item.OrderItem.UnitPrice,
                    item.OrderItem.TotalPrice,
                    item.OrderItem.Origin,
                    Declarant = item.OrderItem.Category.ClassifySecondOperator.RealName
                };

                Response.Write(new
                {
                    rows = control.Items.Select(convert).ToList(),
                    total = control.Items.Count()
                }.Json());
            }
        }
    }
}