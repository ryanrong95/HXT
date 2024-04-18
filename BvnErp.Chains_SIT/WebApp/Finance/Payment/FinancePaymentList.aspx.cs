using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment
{
    public partial class FinancePaymentList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }

        protected void Load_Data()
        {
            this.Model.FeeType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FinanceFeeType>()
                .Select(item => new { item.Key, item.Value }).Json();

            var adminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            var theOperator = new Needs.Ccs.Services.Views.Origins.FinancePaymentOperatorsOrigin()
                .Where(t => t.Status == Needs.Ccs.Services.Enums.Status.Normal
                         && t.Type == Needs.Ccs.Services.Enums.PaymentOperatorType.PaymentOperator
                         && t.AdminID == adminID)
                .FirstOrDefault();

            if (theOperator != null)
            {
                this.Model.IsPaymentOperator = "1";
            }
            else
            {
                this.Model.IsPaymentOperator = "0";
            }
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string FeeType = Request.QueryString["FeeType"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            //var payments = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinancePayment.AsQueryable();

            //if (!string.IsNullOrEmpty(FeeType))
            //{
            //    var type = int.Parse(FeeType);
            //    payments = payments.Where(t => t.PayFeeType == (Needs.Ccs.Services.Enums.FinanceFeeType)type);
            //}

            //if (!string.IsNullOrEmpty(StartDate))
            //{
            //    StartDate = StartDate.Trim();
            //    var from = DateTime.Parse(StartDate);
            //    payments = payments.Where(t => t.PayDate >= from);
            //}
            //if (!string.IsNullOrEmpty(EndDate))
            //{
            //    EndDate = EndDate.Trim();
            //    var to = DateTime.Parse(EndDate);
            //    payments = payments.Where(t => t.PayDate <= to);
            //}

            //payments = payments.OrderByDescending(t => t.CreateDate);

            //Func<Needs.Ccs.Services.Models.FinancePayment, object> convert = item => new
            //{
            //    ID = item.ID,
            //    SeqNo = item.SeqNo,
            //    FinanceVault = item.FinanceVault.Name,
            //    FinanceAccount = item.FinanceAccount.AccountName,
            //    FeeTypeInt = item.FeeTypeInt,
            //    FeeType = item.FeeTypeInt > 10000 ? ((Needs.Ccs.Services.Enums.FeeTypeEnum)item.FeeTypeInt).ToString() 
            //                                      : ((Needs.Ccs.Services.Enums.FinanceFeeType)item.FeeTypeInt).GetDescription(),    //item.PayFeeType.GetDescription(),
            //    PayeeName = item.PayeeName,
            //    Amount = item.Amount,
            //    Currency = item.Currency,
            //    PayDate = item.PayDate.ToShortDateString(),
            //    Payer = item.Payer.RealName,
            //};
            //this.Paging(payments, convert);


            using (var query = new Needs.Ccs.Services.Views.FinancePaymentViewRJ())
            {
                var view = query;

                if (!string.IsNullOrWhiteSpace(FeeType))
                {
                    var type = int.Parse(FeeType);
                    view = view.SearchByFeeType(type);
                }

                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    StartDate = StartDate.Trim();
                    var from = DateTime.Parse(StartDate);
                    view = view.SearchByFrom(from);
                }

                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    EndDate = EndDate.Trim();
                    var to = DateTime.Parse(EndDate);
                    view = view.SearchByTo(to);
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        /// <summary>
        /// 删除
        /// </summary>
        protected void Delete()
        {
            try
            {
                string FinancePaymentID = Request.Form["ID"];
                var finance = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.FinancePayment.AsQueryable()
                    .Where(item => item.ID == FinancePaymentID).FirstOrDefault();
                finance.Abandon();
                Response.Write((new { success = true, message = "取消成功！" }).Json());
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "取消失败：" + ex.Message }).Json());
            }
        }
    }
}