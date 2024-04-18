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

namespace WebApp.Classify.ProductChange
{
    public partial class ProcessList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int page, rows;
            int.TryParse(Request.QueryString["PageNumber"], out page);
            int.TryParse(Request.QueryString["PageSize"], out rows);

            var currentSc = new CurrentSc()
            {
                InitUrl = Request.QueryString["InitUrl"] ?? string.Empty,
                PageNumber = page,
                PageSize = rows,
                OrderID = Convert.ToString(Request.QueryString["OrderID"]) ?? string.Empty,
                ClientCode = Convert.ToString(Request.QueryString["ClientCode"]) ?? string.Empty,
                ProductChangeAddTimeBegin = Convert.ToString(Request.QueryString["ProductChangeAddTimeBegin"]) ?? string.Empty,
                ProductChangeAddTimeEnd = Convert.ToString(Request.QueryString["ProductChangeAddTimeEnd"]) ?? string.Empty,
            };

            this.Model.CurrentSc = currentSc.Json();
        }

        protected void data()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string OrderId = Request.QueryString["OrderID"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var productList = new Needs.Ccs.Services.Views.OrderItemChangeNoticesView();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> expression = t => t.ProcessState == ProcessState.Processed;

            if (!string.IsNullOrEmpty(ClientCode))
            {
                var orderids = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.Where(item => item.Client.ClientCode == ClientCode)
                    .Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => orderids.Contains(t.OrderID);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(OrderId))
            {
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.OrderID == OrderId.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                var from = DateTime.Parse(StartDate);
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.CreateDate >= from;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                var to = DateTime.Parse(EndDate).AddDays(1);
                Expression<Func<Needs.Ccs.Services.Models.OrderItemChangeNotice, bool>> lambda1 = t => t.CreateDate < to;
                lamdas.Add(lambda1);
            }

            #region 页面需要数据
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            var products = productList.GetPageList(page, rows, expression, lamdas.ToArray());

            Response.Write(new
            {
                rows = products.Select(
                    item => new
                    {
                        OrderID = item.OrderID,
                        OrderItemID = item.OrderItemID,
                        ClientCode = item.ClientCode,
                        CompanyName = item.CompanyName,
                        ProductName = item.ProductName,
                        ProductModel = item.ProductModel,
                        Type = item.Type.GetDescription(),
                        Date = item.CreateDate.ToString().Replace("T", ""),
                        ProcessState = item.ProcessState.GetDescription()
                    }
                ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }
    }
}