using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Declare
{
    public partial class SwapedNotice : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void SwapedNoticeData()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string OrderID = Request.QueryString["OrderID"];
            string ClientCode = Request.QueryString["ClientCode"];
            string OwnerName = Request.QueryString["OwnerName"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            if (!string.IsNullOrEmpty(OrderID))
            {
                OrderID = OrderID.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PayExchangeSwapedNoticeListViewModel, bool>>)(t => t.OrderID.Contains(OrderID)));
            }
            if (!string.IsNullOrEmpty(ClientCode))
            {
                ClientCode = ClientCode.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PayExchangeSwapedNoticeListViewModel, bool>>)(t => t.ClientCode.Contains(ClientCode)));
            }
            if (!string.IsNullOrEmpty(OwnerName))
            {
                OwnerName = OwnerName.Trim();
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PayExchangeSwapedNoticeListViewModel, bool>>)(t => t.OwnerName.Contains(OwnerName)));
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                DateTime start = Convert.ToDateTime(StartDate);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PayExchangeSwapedNoticeListViewModel, bool>>)(t => t.ApplyDate >= start));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PayExchangeSwapedNoticeListViewModel, bool>>)(t => t.ApplyDate < end));
            }

            int totalCount = 0;
            var swapedNoticeList = new Needs.Ccs.Services.Views.PayExchangeSwapedNoticeListView().GetResults(out totalCount, page, rows, lamdas.ToArray());

            Func<Needs.Ccs.Services.Views.PayExchangeSwapedNoticeListViewModel, object> convert = item => new
            {
                PayExchangeSwapedNoticeID = item.PayExchangeSwapedNoticeID,
                DecHeadID = item.DecHeadID,
                ContrNo = item.ContrNo,
                ClientCode = item.ClientCode,
                OwnerName = item.OwnerName,
                OrderID = item.OrderID,
                UnHandleAmount = item.UnHandleAmount,
                ApplyDate = item.ApplyDate.ToString("yyyy-MM-dd HH:mm:ss"),
                HandleStatusInt = item.HandleStatus.GetHashCode(),
                HandleStatusDes = item.HandleStatus.GetDescription(),
            };

            Response.Write(new
            {
                rows = swapedNoticeList.Select(convert).ToArray(),
                total = totalCount,
            }.Json());
        }

        /// <summary>
        /// 单个处理
        /// </summary>
        protected void SingleHandle()
        {
            try
            {
                string PayExchangeSwapedNoticeID = Request.Form["PayExchangeSwapedNoticeID"];

                Needs.Ccs.Services.Models.SwapedNoticeHandler swapedNoticeHandler = new Needs.Ccs.Services.Models.SwapedNoticeHandler(new string[] { PayExchangeSwapedNoticeID, });
                swapedNoticeHandler.Execute();

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 批量处理
        /// </summary>
        protected void BatchHandle()
        {
            try
            {
                string PayExchangeSwapedNoticeIDs = Request.Form["PayExchangeSwapedNoticeIDs"];

                string[] PayExchangeSwapedNoticeIDs_Array = PayExchangeSwapedNoticeIDs.Split(',');

                Needs.Ccs.Services.Models.SwapedNoticeHandler swapedNoticeHandler = new Needs.Ccs.Services.Models.SwapedNoticeHandler(PayExchangeSwapedNoticeIDs_Array);
                swapedNoticeHandler.Execute();

                Response.Write((new { success = true, message = "操作成功" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "发生错误：" + ex.Message }).Json());
            }
        }


    }
}