using Needs.Underly;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Receipt.SzStore
{
    public partial class PayeeLeftsList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        protected void LoadData()
        {
            string financeReceiptId = Request.QueryString["FinanceReceiptId"];
            var notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyReceiptNotices[financeReceiptId];
            this.Model.NoticeData = new
            {
                ClienName = notice.Client.Company.Name,
                Amount = notice.AvailableAmount.ToString("f2"),
                ClearAmount = notice.ClearAmount.ToString("f2"),
                RemainAmount = (notice.Amount - notice.ClearAmount).ToString("f2"),
                ReceiptDate = notice.ReceiptDate.ToString("yyyy-MM-dd"),
            }.Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientName = Request.QueryString["ClientName"];
            int CutDateIndex = int.Parse(Request.QueryString["CutDateIndex"]);
            string SeqNo = Request.QueryString["SeqNo"];

            var payeeLefts = new Needs.Ccs.Services.Views.PayeeLeftsTopView().GetIQueryableRoll()
                .Where(t => t.CutDateIndex == CutDateIndex && t.PayerName == ClientName).ToArray();
            int totalCount = payeeLefts.Count();

            Response.Write(new
            {
                rows = payeeLefts.OrderBy(t => t.CreateDate).Skip(rows * (page - 1)).Take(rows).Select(t => new
                {
                    t.ID,
                    CreateDate = t.CreateDate.ToString("yyyy-MM-dd"),
                    t.FormID,
                    t.Subject,
                    PayeeLeftAmount = t.Total,
                    PayeeRightAmount = t.ReceiptTotal ?? 0m,
                    RemianAmount = t.Total - (t.ReceiptTotal ?? 0m),
                }).ToArray(),
                total = totalCount,
            }.Json());
        }

        protected void ConfimSubmit()
        {
            try
            {
                string ID = Request.Form["ID"];
                string FinanceReceiptId = Request.Form["FinanceReceiptId"];
                string SeqNo = Request.Form["SeqNo"];
                //流水可用金额
                decimal RemainAmount = decimal.Parse(Request.Form["RemainAmount"]);//可用金额
                var payeeLeft = new Needs.Ccs.Services.Views.PayeeLeftsTopView().GetIQueryableRoll().Single(t => t.ID == ID);
                //未收金额
                decimal RemianPayeeLeft = payeeLeft.Total - (payeeLeft.ReceiptTotal ?? 0m);
                //判断实收金额
                decimal clearAmount = RemainAmount >= RemianPayeeLeft ? RemianPayeeLeft : RemainAmount;

                //更新PsOrder实收数据
                Needs.Ccs.Services.Views.PayeeRight right = new Needs.Ccs.Services.Views.PayeeRight();
                right.LeftID = ID;
                right.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;
                right.Price = clearAmount;
                right.ReviewerID = right.AdminID;
                right.FlowFormCode = SeqNo;

                var apisetting = new Needs.Ccs.Services.ApiSettings.PsOrderApiSetting();
                var url = ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.SynPayeeRight;
                var result = Needs.Utils.Http.ApiHelper.Current.JPost<JSingle<dynamic>>(url, right);

                //更新流水可用余额
                var storeReceiptHandler = new Needs.Ccs.Services.Models.StoreReceiptHandler(FinanceReceiptId, clearAmount);
                storeReceiptHandler.Execute();

                Response.Write((new { success = "true", message = "提交成功", clearAmount = "0" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = "false", message = "提交失败" + ex.Message }).Json());
            }
        }
    }
}