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

namespace WebApp.Order.OrderChange
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }


        /// <summary>
        /// 
        /// </summary>
        protected void data()
        {
            string orderID = Request.QueryString["OrderID"];
            string clientCode = Request.QueryString["ClientCode"];
            string startDate = Request.QueryString["StartDate"];
            string endDate = Request.QueryString["EndDate"];


            var productList = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.MyOrderChanges;
            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> expression = t => t.processState == ProcessState.Processing&&t.Status== Status.Normal;         
            if (!string.IsNullOrEmpty(clientCode))
            {
                var orders = Needs.Wl.Admin.Plat.AdminPlat.Current.Order.Orders.Where(item => item.Client.ClientCode == clientCode);                
                if (!string.IsNullOrEmpty(startDate))
                {
                    var from = DateTime.Parse(startDate);
                    orders = orders.Where(item => item.CreateDate >= from);
                }
                if (!string.IsNullOrEmpty(endDate))
                {
                    var to = DateTime.Parse(endDate).AddDays(1);
                    orders = orders.Where(item => item.CreateDate < to);
                }
                var orderids = orders.Select(item => item.ID).ToArray();
                Expression<Func<Needs.Ccs.Services.Models.OrderChangeNotice, bool>> lambda1 = t => orderids.Contains(t.OrderID);
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
                        ID = bill.ID,
                        OrderID = bill.OrderID,
                        ClientCode = bill.ClientCode,
                        ClientName = bill.ClientName,
                        EntryID = bill.DecHead?.EntryId,
                        DDdate = bill.DecHead?.DDate?.ToString("yyyy-MM-dd"),
                        OrderChangeType = bill.Type.GetDescription(),
                        Type = bill.Type,
                        processState = bill.processState.GetDescription(),
                        CreateDate = bill.CreateDate.ToShortDateString()
                    }
                ).ToArray(),
                total = products.Total,
            }.Json());
            #endregion
        }

        protected void ToDeal()
        {
            try
            {
                string OrderId = Request.Form["ID"];
                var orderChange = new Needs.Ccs.Services.Views.OrderChangeView().GetTop(1, x => x.OrderID == OrderId).FirstOrDefault();
                //更新OrderChanges表的状态;
                orderChange.UpdateProcessState();
                orderChange.UpdateApiNoticeStatus();
                Response.Write((new { success = true, message = "处理成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "处理失败：" + ex.Message }).Json());
            }
        }
    }
}