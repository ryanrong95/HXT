using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Models;
using Needs.Utils.Descriptions;

namespace WebApp.Finance.Receipt.Notice
{
    public partial class ReceivedList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        protected void data()
        {
            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string SeqNo = Request.QueryString["SeqNo"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            var notices = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ReceiptNotices.AsQueryable();

            //var notices = notice.Where(s => s.ClearAmount == s.Amount);
            
            if (!string.IsNullOrEmpty(ClientCode))
            {
                notices = notices.Where(item => item.Client.ClientCode == ClientCode.Trim());
            }
            if (!string.IsNullOrEmpty(ClientName))
            {
                notices = notices.Where(item => item.Client.Company.Name == ClientName.Trim());
            }
            if (!string.IsNullOrEmpty(SeqNo))
            {
                notices = notices.Where(item => item.SeqNo== SeqNo.Trim());
            }
            if (!string.IsNullOrEmpty(StartDate))
            {
                notices = notices.Where(item => item.CreateDate >= Convert.ToDateTime(StartDate));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                DateTime end = Convert.ToDateTime(EndDate).AddDays(1);
                notices = notices.Where(item => item.CreateDate < end);
            }

            notices = notices.OrderByDescending(item => item.CreateDate);

            //前台显示
            Func<ReceiptNotice, object> convert = item => new
            {
                item.ID,
                item.SeqNo,
                item.Client.ClientCode,
                ClientName = item.Client.Company.Name,
                item.Amount,
                item.ClearAmount,
                //item.Rate,
                ReceiptDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Vault = item.Vault.Name,
                item.Account.AccountName,
                ReceiptType = item.ReceiptType.GetDescription(),
                //item.Currency
            };

            this.Paging(notices, convert);
        }
    }
}