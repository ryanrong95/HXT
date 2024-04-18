using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Order.UnCollected
{
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
            this.Model.OrderStatus = EnumUtils.ToDictionary<OrderStatus>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.Currencies = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Key = item.Code, Value = item.Code + " " + item.Name }).Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string currency = Request.QueryString["Currency"];
            string orderStatus = Request.QueryString["OrderStatus"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];
          
            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnCollectedOrders;
            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> expression = item => true;

            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> lambda = item => item.ID == orderID.Trim();
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(clientCode))
            {
                var clientIds = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.MyClients.Where(item => item.ClientCode == clientCode.Trim()).Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> lambda = item => clientIds.Contains(item.ClientID);
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(currency))
            {
                Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> lambda = item => item.Currency.Contains(currency.Trim());
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int status = Int32.Parse(orderStatus);
                Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> lambda = item => item.OrderStatus == (OrderStatus)status;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> lambda = item => item.CreateDate >= from;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate);
                Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> lambda = item => item.CreateDate < to.AddDays(1);
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
                            ClientCode = order.Client.ClientCode,
                            ClientID = order.Client.ID,
                            ClientName = order.Client.Company.Name,                          
                            DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
                            Currency = order.Currency,
                            PaidAmount = order.PaidExchangeAmount.ToRound(2).ToString("0.00"),
                            OrderStatusValue = order.OrderStatus,
                            CollectedAmouont = order.CollectedAmount==null?0: order.CollectedAmount.Value,
                            OrderStatus = order.OrderStatus.GetDescription(),
                            CreateDate = order.CreateDate.ToShortDateString(),
                            DeclareDate = order.DeclareDate?.ToShortDateString()
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion
        }

        protected void ReceiveCheck()
        {            
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            List<CollectData> selectedModel = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CollectData>>(Model);
            CheckContext check = new CheckContext(selectedModel);
            CheckReturnData data = check.Check();

            if (data.Success)
            {
                Receive(selectedModel);
            }
            else
            {               
                Response.Write((new { success = false, message = "收款失败：" + data.Data+"付汇申请未审批" }).Json());
            }
        }

        protected void Receive(List<CollectData> selectedModel)
        {
            try
            {
                CollectContext collectContext = new CollectContext(selectedModel);
                collectContext.Collect();
                Response.Write((new { success = true, message = "收款成功" }).Json());
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = "收款失败:"+ex.ToString() }).Json());
            }
            
        }
    }
}