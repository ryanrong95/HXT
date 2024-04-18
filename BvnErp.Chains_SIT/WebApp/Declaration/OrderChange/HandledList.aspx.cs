using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Declaration.OrderChange
{
    public partial class HandledList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        protected void data()
        {
            string contrNo = Request.QueryString["ContrNo"];
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];


            var productList = new Needs.Ccs.Services.Views.OrderChangeView();
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> expression = t => t.processState != ProcessState.UnProcess && t.Type== OrderChangeType.TaxChange;
            if (!string.IsNullOrEmpty(clientCode))
            {
                var orderids = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.Where(item => item.Client.ClientCode == clientCode)
                    .Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> lambda1 = t => orderids.Contains(t.OrderID);
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(contrNo))
            {
                Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> lambda1 = t => t.DecHead.ContrNo == contrNo.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(orderID))
            {
                Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> lambda1 = t => t.OrderID == orderID.Trim();
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(startDate))
            {
                var from = DateTime.Parse(startDate);
                Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> lambda1 = t => t.DecHead.DDate >= from;
                lamdas.Add(lambda1);
            }
            if (!string.IsNullOrEmpty(endDate))
            {
                var to = DateTime.Parse(endDate).AddDays(1);
                Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> lambda1 = t => t.DecHead.DDate < to;
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
                    bill => new
                    {
                        OrderID = bill.OrderID,
                        ClientCode = bill.ClientCode,
                        ClientName = bill.ClientName,
                        EntryID = bill.DecHead?.EntryId,
                        ContrNo = bill.DecHead?.ContrNo,
                        DDdate = bill.DecHead?.DDate?.ToString("yyyy-MM-dd"),
                        OrderChangeType = bill.Type.GetDescription(),
                        Type = bill.Type,
                        processState ="已处理",
                        CreateDate = bill.CreateDate.ToShortDateString()
                    }
                ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

    }
}