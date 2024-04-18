using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Hanlders;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Models;
using System.Linq.Expressions;

namespace WebApp.Order.Draft
{
    /// <summary>
    /// 订单草稿查询界面
    /// 用于展示状态为“草稿”的代理订单
    /// </summary>
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboBoxData();
        }

        /// <summary>
        /// 初始化下拉框数据
        /// </summary>
        protected void LoadComboBoxData()
        {
            this.Model.Clients = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView.Where(c => c.ClientType == ClientType.Internal).Select(c => new { c.ID, c.Company.Name }).Json();
            this.Model.ClientData = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Select(item => new { item.ID, item.Company.Name }).Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
            var clientID = Request.QueryString["ClientID"];
            var type = Request.QueryString["ClientType"];

            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyDraftOrders1;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.DraftOrder, bool>> expression = item => true;

            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.DraftOrder, bool>> lambda = item => item.ID == orderID.Trim();
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.DraftOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.DraftOrder, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Models.DraftOrder, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.DraftOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                Expression<Func<Needs.Ccs.Services.Models.DraftOrder, bool>> lambda = item => item.ClientID == clientID.Trim();
                lambdas.Add(lambda);
            }

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);
            var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            Response.Write(new
            {
                rows = orderlist.Select(
                        order => new
                        {
                            order.ID,
                            order.Client.ClientCode,
                            ClientName = order.Client.Company.Name,
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            order.Currency,
                            CreateDate = order.CreateDate.ToShortDateString()
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 删除订单
        /// </summary>
        protected void Delete()
        {
            string id = Request.Form["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyDraftOrders[id];
            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            if (order != null)
            {
                order.SetAdmin(admin);
                order.Deleted += Order_DeleteSuccess;
                order.Delete();
            }
        }
        /// <summary>
        /// 删除成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_DeleteSuccess(object sender, OrderAbandonEventArgs e)
        {
            Alert("删除成功!");
        }

        /// <summary>
        /// 确认下单
        /// </summary>
        protected void Confirm()
        {
            string id = Request.Form["ID"];
            var order = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyDraftOrders[id];
            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            if (order != null)
            {
                order.SetAdmin(admin);
                order.Confirmed += Order_ConfirmSuccess;
                order.Confirm();
            }
        }

        /// <summary>
        /// 下单成功触发事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Order_ConfirmSuccess(object sender, StatusChangedEventArgs e)
        {
            Alert("下单成功!");
        }

        /// <summary>
        /// 批量删除订单
        /// </summary>
        protected void BatchDelete()
        {
            string[] ids = Request.Form["IDs"].Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');
            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyDraftOrders.Where(o => ids.Contains(o.ID));

            foreach (var order in orders)
            {
                order.SetAdmin(admin);
                order.Deleted += Order_DeleteSuccess;
                order.Delete();
            }
        }

        /// <summary>
        /// 批量下单
        /// </summary>
        protected void BatchConfirm()
        {
            string[] ids = Request.Form["IDs"].Replace("[", "").Replace("]", "").Replace("&quot;", "").Split(',');
            var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyDraftOrders.Where(o => ids.Contains(o.ID));

            foreach (var order in orders)
            {
                order.SetAdmin(admin);
                order.Confirmed += Order_ConfirmSuccess;
                order.Confirm();
            }
        }
    }
}