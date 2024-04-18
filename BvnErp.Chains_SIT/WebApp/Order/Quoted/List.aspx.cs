using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Linq;
using Needs.Utils.Serializers;
using Spire.Pdf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.Ccs.Utils;

namespace WebApp.Order.Quoted
{
    /// <summary>
    /// 待客户确认(已报价)订单查询界面
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

            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyQuotedOrders1;
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
        /// 跟单员替客户确认订单
        /// </summary>
        protected void ConfirmOrder()
        {
            try
            {
                var ID = Request.Form["ID"];

                var order = new Needs.Ccs.Services.Views.Orders2View().Where(t => t.ID == ID).FirstOrDefault();

                if (order != null)
                {
                    var apisetting = new Needs.Ccs.Services.ApiSettings.PvWsOrderApiSetting();
                    var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.DeclareConfirm;

                    var msg = new
                    {
                        VastOrderID = order.MainOrderID
                    };

                    var result = Needs.Utils.Http.ApiHelper.Current.PostData(apiurl, msg);
                    var message = Newtonsoft.Json.JsonConvert.DeserializeObject<Needs.Underly.JMessage>(result);

                    var admin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                    if (message.code != 200)
                    {
                        OrderLog log = new OrderLog();
                        log.OrderID = order.ID;
                        log.Admin = admin;
                        log.OrderStatus = order.OrderStatus;
                        log.Summary = "跟单员替客户确认订单失败:" + message.data;
                        log.Enter();

                        //失败消息返回前端
                        Response.Write((new { success = false, message = message.data, }).Json());
                    }

                    Response.Write((new { success = true, message = message.data, }).Json());
                }
            }
            catch (Exception ex)
            {
                ex.CcsLog("跟单员替客户确认失败");
                Response.Write((new { success = false, message = "确认订单失败" + ex.Message }).Json());
            }
        }
    }
}