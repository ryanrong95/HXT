using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using Needs.Wl;

namespace WebApp.Finance.Receipt.Notice
{
    public partial class Detail : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化label
        /// </summary>
        protected void LoadData()
        {
            //?ID=21FD3C487CA1733EC58002B88140AF8A
            var ID = Request.QueryString["ID"];
            var receiptNotice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.ReceiptNotices
                .AsQueryable().FirstOrDefault(r => r.ID == ID);
            this.Model.ReceiptNotice = new
            {
                ClientName = receiptNotice?.Client.Company.Name,
                ReceiptType = receiptNotice?.ReceiptType.GetDescription(),
                ReceiptDate = receiptNotice?.CreateDate.ToShortDateString(),
                Amount = receiptNotice?.ClearAmount,
                Currency = receiptNotice?.Currency,
                Rate = receiptNotice?.Rate,
                Vault = receiptNotice?.Vault.Name,
                Account = receiptNotice?.Account.AccountName,
                BankAccount = receiptNotice?.Account.BankAccount,
                Admin = receiptNotice?.Admin.RealName
            }.Json();
        }
        /// <summary>
        /// 
        /// </summary>
        protected void data()
        {
            string ID = Request.QueryString["ID"];

            //实收款记录
            var orderReceiptDetail = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.OrderReceiveds.AsQueryable()
                .Where(r => r.ReceiptNoticeID == ID);

            Func<Needs.Ccs.Services.Models.OrderReceipt, object> convert = orderReceipt => new
            {
                OrderID = orderReceipt.OrderID,
                FeeType = orderReceipt.FeeType.GetDescription(),
                Amount = orderReceipt.Amount,
                Date = orderReceipt.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                OrderStatus = orderReceipt.OrderStatus.GetDescription(),
                Admin = orderReceipt.Admin.RealName
            };

            Response.Write(new
            {
                rows = orderReceiptDetail.Select(convert).ToArray(),
                total = orderReceiptDetail.Count()
            }.Json());

            //Response.Write(new
            //{
            //    rows = orderReceiptDetail.Select(convert).ToList(),
            //    total = orderReceiptDetail.Count()
            //}.Json());
        }
    }
}