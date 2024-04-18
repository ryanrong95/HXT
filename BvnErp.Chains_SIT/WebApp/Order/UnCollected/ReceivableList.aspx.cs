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
    public partial class ReceivableList : Uc.PageBase
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
            this.Model.Currencies = Needs.Wl.Admin.Plat.AdminPlat.Currencies.Select(item => new { Key = item.Code, Value = item.Code + " " + item.Name }).Json();
        }

        /// <summary>
        /// 初始化订单数据
        /// </summary>
        protected void data()
        {
            
            //string currency = Request.QueryString["Currency"];
            
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];

            var orders = new Needs.Ccs.Services.Views.ReceivablesOriginView();

            Response.Write(new
            {
                rows = orders.Select(
                        order => new
                        {
                            order.ID,
                            order.CreateDate,
                            order.Amount,
                            order.Currency,
                            order.CNYAmount,
                            MatchStatus = order.MatchStatus == 0?"未匹配": order.FinanceReceiptID
                        }
                     ).ToArray(),
                total = orders.Count(),
            }.Json());

            //var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyUnCollectedOrders;
            //List<LambdaExpression> lambdas = new List<LambdaExpression>();
            //Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> expression = item => true;


            //if (!string.IsNullOrEmpty(startDate))
            //{
            //    var from = DateTime.Parse(startDate);
            //    Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> lambda = item => item.CreateDate >= from;
            //    lambdas.Add(lambda);
            //}
            //if (!string.IsNullOrEmpty(endDate))
            //{
            //    var to = DateTime.Parse(endDate);
            //    Expression<Func<Needs.Ccs.Services.Models.UnCollectedOrder, bool>> lambda = item => item.CreateDate < to.AddDays(1);
            //    lambdas.Add(lambda);
            //}


            #region 页面需要数据
            //int page, rows;
            //int.TryParse(Request.QueryString["page"], out page);
            //int.TryParse(Request.QueryString["rows"], out rows);
            //var orderlist = orders.GetPageList(page, rows, expression, lambdas.ToArray());

            //Response.Write(new
            //{
            //    rows = orderlist.Select(
            //            order => new
            //            {
            //                order.ID,
            //                ClientCode = order.Client.ClientCode,
            //                ClientName = order.Client.Company.Name,
            //                DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
            //                Currency = order.Currency,
            //                PaidAmount = order.PaidExchangeAmount.ToRound(2).ToString("0.00"),
            //                OrderStatusValue = order.OrderStatus,
            //                CollectedAmouont = order.CollectedAmount == null ? 0 : order.CollectedAmount.Value,
            //                OrderStatus = order.OrderStatus.GetDescription(),
            //                CreateDate = order.CreateDate.ToShortDateString(),
            //                DeclareDate = order.DeclareDate?.ToShortDateString()
            //            }
            //         ).ToArray(),
            //    total = orderlist.Total,
            //}.Json());
            #endregion
        }


    }
}