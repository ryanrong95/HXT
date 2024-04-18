using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.Notice
{
    /// <summary>
    /// 收款通知列表界面
    /// </summary>
    public partial class UnMatchedInterestList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            this.Model.ClientName = Server.UrlDecode(Request.QueryString["ClientName"]) ?? string.Empty;
        }

        protected void data()
        {
            string ClientName = Request.QueryString["ClientName"];         
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string OrderMatched = Request.QueryString["OrderMatched"];

            if (!string.IsNullOrEmpty(ClientName))
            {
                ClientName = ClientName.Trim(',');
            }           
            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim(',');
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim(',');
            }
            if (!string.IsNullOrEmpty(OrderMatched))
            {
                OrderMatched = OrderMatched.Trim(',');
            }


            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyFundTransferApply.AsQueryable();

            if (string.IsNullOrEmpty(ClientName) == false)
            {
                notices = notices.Where(item => item.Client.Company.Name.Contains(ClientName));
            }

            if (string.IsNullOrEmpty(StartDate) == false)
            {
                DateTime start;
                if (DateTime.TryParse(StartDate, out start))
                {
                    notices = notices.Where(item => item.CreateDate >= start);
                }
            }
            if (string.IsNullOrEmpty(EndDate) == false)
            {
                DateTime end;
                if (DateTime.TryParse(EndDate, out end))
                {
                    end = end.AddDays(1);
                    notices = notices.Where(item => item.CreateDate < end);
                }
            }
            if (OrderMatched == "0")
            {
                notices = notices.Where(item => item.OrderID == null);
            }
            else if (OrderMatched == "1")
            {
                notices = notices.Where(item => item.OrderID != null);
            }

            notices = notices.OrderByDescending(item => item.CreateDate);

            ////前台显示
            Func< Needs.Ccs.Services.Models.FundTransferApplies, object> convert = item => new
            {
                item.ID,
                OutSeqNo = item.OutSeqNo,
                FromSeqNo = item.FromSeqNo,
                AccountName = item.OutAccount.AccountName,
                Payer = item.Payer,
                DiscountInterest = item.DiscountInterest,
                ReceiptDate = item.CreateDate.ToString("yyyy-MM-dd"),
                OrderID = item.OrderID               
            };

            this.Paging(notices, convert);
        }

        
    }
}