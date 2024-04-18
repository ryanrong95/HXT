using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services;
using System.Linq.Expressions;

namespace WebApp.Order.Match
{
    /// <summary>
    /// 待报价(已归类)订单查询界面
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
            this.Model.BoxStatus = getBoxStatus().Select(item => new { Value = item.Key, Text = item.Value }).OrderBy(item => item.Value).Json(); ;
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
            var BoxStatus = Request.QueryString["BoxStatus"];

            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnMatchedOrders;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;

            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ID == orderID.Trim();
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.CreateDate < to.AddDays(1);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(type))
            {
                int itype = Int32.Parse(type);
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientType == (ClientType)itype).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientID))
            {
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.ClientID == clientID.Trim();
                lambdas.Add(lambda);
            }

            //if (!string.IsNullOrEmpty(BoxStatus))
            //{
            //    switch (BoxStatus)
            //    {
            //        case "1":
            //            Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.IsArrived == false;
            //            lambdas.Add(lambda);
            //            break;

            //        case "2":
            //            Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda2 = item => item.IsArrived == true;
            //            lambdas.Add(lambda2);
            //            Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda3 = item => item.DeclareFlag != DeclareFlagEnums.Done;
            //            lambdas.Add(lambda3);
            //            break;

            //        case "3":
            //            Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda4 = item => item.IsArrived == true;
            //            lambdas.Add(lambda4);
            //            Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda5 = item => item.DeclareFlag == DeclareFlagEnums.Done;
            //            lambdas.Add(lambda5);
            //            break;

            //        default:
            //            break;
            //    }
            //}

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
                            CreateDate = order.CreateDate.ToShortDateString(),
                            order.EntryNoticeStatus,
                            order.IsArrived,
                            order.DeclareFlag,
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion
        }

        protected void IsOrderHangUp()
        {
            string OrderID = Request.Form["OrderID"];
            var controls = Needs.Wl.Admin.Plat.AdminPlat.Current.Control.MyMerchandiserControls.Where(t => t.Order.ID == OrderID&&t.ControlType!=OrderControlType.ExceedLimit).FirstOrDefault();
            if (controls != null)
            {
                Response.Write((new { success = false, message = "该订单已挂起，不能拆分" }).Json());               
            }
            else
            {
                Response.Write((new { success = true, message = "可以拆分" }).Json());
            }
            
        }

        private Dictionary<int, string> getBoxStatus()
        {
            Dictionary<int, string> mark = new Dictionary<int, string>();
            //mark.Add(0, " ");
            mark.Add(1, "待装箱");
            mark.Add(2, "未封箱");
            mark.Add(3, "已封箱");

            return mark;
        }

        protected void ReturnValidate() 
        {
            string clientCode = Request.Form["clientCode"];
            var payexchange = new Needs.Ccs.Services.Views.GendanAuditePayExchangeApplyListView().Where(t => t.ClientCode == clientCode && t.PayExchangeApplyStatus == PayExchangeApplyStatus.Auditing).Count();
            if (payexchange == 0) 
            {
                Response.Write((new { success = true, message = "" }).Json());
                return;
            }
            Response.Write((new { success = false, message = "该客户存在已提交，未审核的付汇申请!" }).Json());
        }
    }
}
