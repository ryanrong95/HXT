using Needs.Ccs.Services;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using System.Linq.Expressions;

namespace WebApp.Order.Query
{
    /// <summary>
    /// 订单综合查询界面
    /// </summary>
    public partial class AdminList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 数据初始化
        /// </summary>
        public void LoadData()
        {
            var orderStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.OrderStatus>().Select(item => new { item.Key, item.Value });
            var payExchangeStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PayExchangeStatus>().Select(item => new { item.Key, item.Value });
            this.Model.PayExchangeStatus = payExchangeStatus.Json();
            this.Model.OrderStatus = orderStatus.Json();
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
            string orderStatus = Request.QueryString["OrderStatus"];
            string payChangeStatus = Request.QueryString["PayExchangeStatus"];

            #region 待删除
            //var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.OrderByDescending(item => item.CreateDate).AsQueryable();
            //if (!string.IsNullOrEmpty(orderID))
            //{
            //    orders = orders.Where(t => t.ID.Contains(orderID.Trim()));
            //}
            //if (!string.IsNullOrEmpty(clientCode))
            //{
            //    orders = orders.Where(t => t.Client.ClientCode.Contains(clientCode.Trim()));
            //}
            //if (!string.IsNullOrEmpty(startDate))
            //{
            //    var from = DateTime.Parse(startDate);
            //    orders = orders.Where(t => t.CreateDate >= from);
            //}
            //if (!string.IsNullOrEmpty(endDate))
            //{
            //    var to = DateTime.Parse(endDate);
            //    orders = orders.Where(t => t.CreateDate < to.AddDays(1));
            //}
            //if (!string.IsNullOrEmpty(orderStatus))
            //{
            //    int status = Int32.Parse(orderStatus);
            //    orders = orders.Where(t => t.OrderStatus == (Needs.Ccs.Services.Enums.OrderStatus)status);
            //}
            //if (!string.IsNullOrEmpty(payChangeStatus))
            //{
            //    int status = Int32.Parse(payChangeStatus);
            //  if (status == (int) PayExchangeStatus.UnPay)
            //  {
            //      orders = orders.Where(t => t.PaidExchangeAmount == 0);
            //  }
            //  else if (status == (int) PayExchangeStatus.Partial)
            //  {
            //      orders = orders.Where(t => t.PaidExchangeAmount > 0 && t.PaidExchangeAmount < t.DeclarePrice);
            //  }
            //  else
            //  {
            //      orders = orders.Where(t => t.PaidExchangeAmount >= t.DeclarePrice);
            //    }
            //}


            //Func<Needs.Ccs.Services.Models.Order, object> convert = order => new
            //{
            //    order.ID,
            //    order.Client.ClientCode,
            //    ClientName = order.Client.Company.Name,
            //    DeclarePrice = order.DeclarePrice.ToRound(2).ToString("0.00"),
            //    order.Currency,
            //    CreateDate = order.CreateDate.ToShortDateString(),
            //    InvoiceStatus = order.InvoiceStatus.GetDescription(),
            //    PayExchangeStatus = order.PayExchangeStatus.GetDescription(),
            //    OrderStatusValue = order.OrderStatus,
            //    OrderStatus = order.OrderStatus.GetDescription(),
            //};
            //this.Paging(orders, convert);

            #endregion

            var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders1;

            List<LambdaExpression> lambdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.Order, bool>> expression = item => true;

            #region 查询条件
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
            if (!string.IsNullOrEmpty(orderStatus))
            {
                int status = Int32.Parse(orderStatus);
                Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.OrderStatus == (OrderStatus)status;
                lambdas.Add(lambda);
            }
            if (!string.IsNullOrEmpty(payChangeStatus))
            {
                int status = Int32.Parse(payChangeStatus);

                if (status == 1) {
                    Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.PaidExchangeAmount == 0M;
                    lambdas.Add(lambda);
                }
                    
                else if (status == 2)
                {
                    Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => Math.Round(item.PaidExchangeAmount,2,MidpointRounding.AwayFromZero) < Math.Round(item.DeclarePrice, 2, MidpointRounding.AwayFromZero) && item.PaidExchangeAmount > 0M;
                    lambdas.Add(lambda);
                }
                else
                {
                    Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => Math.Round(item.PaidExchangeAmount, 2, MidpointRounding.AwayFromZero) == Math.Round(item.DeclarePrice, 2, MidpointRounding.AwayFromZero);
                    lambdas.Add(lambda);
                }

                //Expression<Func<Needs.Ccs.Services.Models.Order, bool>> lambda = item => item.PayExchangeStatus == (PayExchangeStatus)status;
                //lambdas.Add(lambda);
            }

            #endregion

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
                            InvoiceStatus = order.InvoiceStatus.GetDescription(),
                            PaidAmount = order.PaidExchangeAmount.ToRound(2).ToString("0.00"),
                            UnpaidAmount = (order.DeclarePrice - order.PaidExchangeAmount).ToString("0.00"),
                            PayExchangeStatus = order.PayExchangeStatus.GetDescription(),
                            OrderStatusValue = order.OrderStatus,
                            OrderStatus = order.OrderStatus.GetDescription(),
                            SpecialType = GenerateOrderSpecialType(order.OrderVoyages),
                            MainOrderID = order.MainOrderID
                        }
                     ).ToArray(),
                total = orderlist.Total,
            }.Json());
            #endregion
        }

        /// <summary>
        /// 订单特殊类型
        /// </summary>
        /// <param name="orderVoyages"></param>
        /// <returns></returns>
        private string GenerateOrderSpecialType(List<Needs.Ccs.Services.Models.OrderVoyage> orderVoyages)
        {
            var result = string.Empty;
            orderVoyages.ForEach(t => {
                result += t.Type.GetDescription() + "|";
            });

            result = result.TrimEnd('|');

            return string.IsNullOrEmpty(result) ? "-" : result;
        }
    }
}