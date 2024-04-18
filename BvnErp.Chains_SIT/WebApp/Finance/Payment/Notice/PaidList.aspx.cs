using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Finance.Payment.Notice
{
    public partial class PaidList : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Load_Data();
        }

        protected void Load_Data()
        {
            this.Model.FeeType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FinanceFeeType>()
                .Select(item => new { item.Key, item.Value }).Json();
        }

        //protected void data()
        //{
        //    string FeeType = Request.QueryString["FeeType"];
        //    string StartDate = Request.QueryString["StartDate"];
        //    string EndDate = Request.QueryString["EndDate"];

        //    var Notice = Needs.Wl.Admin.Plat.AdminPlat.Current.Finance.MyPaymentNotice.AsQueryable();
        //    Notice = Notice.Where(t => t.Status == Needs.Ccs.Services.Enums.PaymentNoticeStatus.Paid);
        //    Notice = Notice.Where(t => t.PayFeeType == Needs.Ccs.Services.Enums.FinanceFeeType.Product);

        //    if (!string.IsNullOrEmpty(FeeType))
        //    {
        //        var type = int.Parse(FeeType);
        //        Notice = Notice.Where(t => t.PayFeeType == (Needs.Ccs.Services.Enums.FinanceFeeType)type);
        //    }

        //    if (!string.IsNullOrEmpty(StartDate))
        //    {
        //        StartDate = StartDate.Trim();
        //        var from = DateTime.Parse(StartDate);
        //        Notice = Notice.Where(t => t.PayDate >= from);
        //    }
        //    if (!string.IsNullOrEmpty(EndDate))
        //    {
        //        EndDate = EndDate.Trim();
        //        var to = DateTime.Parse(EndDate);
        //        Notice = Notice.Where(t => t.PayDate <= to);
        //    }

        //    Func<Needs.Ccs.Services.Models.PaymentNotice, object> convert = notice => new
        //    {
        //        ID = notice.ID,
        //        ApplyID = notice.PayExchangeApply?.ID,
        //        SeqNo = notice.SeqNo,
        //        FeeType = notice.PayFeeType.GetDescription(),
        //        FeeDesc = notice.FeeDesc,
        //        PayeeName = notice.PayeeName,
        //        Amount = notice.Amount,
        //        Currency = notice.Currency,
        //        PayDate = notice.PayDate.ToShortDateString(),
        //        Summary = notice.Summary,
        //    };
        //    this.Paging(Notice, convert);
        //}

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string FeeType = Request.QueryString["FeeType"];
            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];

            List<LambdaExpression> lamdas = new List<LambdaExpression>();
            lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PaidListViewModel, bool>>)(t => t.PayerID == Needs.Wl.Admin.Plat.AdminPlat.Current.ID));

            if (!string.IsNullOrEmpty(FeeType))
            {
                var type = int.Parse(FeeType);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PaidListViewModel, bool>>)(t => t.FeeTypeInt == type));
            }

            if (!string.IsNullOrEmpty(StartDate))
            {
                StartDate = StartDate.Trim();
                var from = DateTime.Parse(StartDate);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PaidListViewModel, bool>>)(t => t.PayDate >= from));
            }
            if (!string.IsNullOrEmpty(EndDate))
            {
                EndDate = EndDate.Trim();
                var to = DateTime.Parse(EndDate);
                lamdas.Add((Expression<Func<Needs.Ccs.Services.Views.PaidListViewModel, bool>>)(t => t.PayDate <= to));
            }

            Func<Needs.Ccs.Services.Views.PaidListViewModel, object> convert = notice => new
            {
                ID = notice.PaymentNoticeID,
                ApplyID = notice.PayExchangeApplyID,
                SeqNo = notice.SeqNo,
                //FeeType = notice.FeeTypeInt > 10000 ? ((Needs.Ccs.Services.Enums.FeeTypeEnum)notice.FeeTypeInt).ToString()
                //                                    : ((Needs.Ccs.Services.Enums.FeeType)notice.FeeTypeInt).GetDescription(),
                //FeeDesc = notice.FeeTypeInt < 10000 ? notice.FeeDesc 
                //        : (notice.FeeTypeInt == (int)Needs.Ccs.Services.Enums.FeeTypeEnum.其它.GetHashCode() ? notice.FeeDesc 
                //                                                                                             : ((Needs.Ccs.Services.Enums.FeeTypeEnum)notice.FeeTypeInt).ToString()),
                PayeeName = notice.PayeeName,
                Amount = notice.Amount,
                Currency = notice.Currency,
                PayDate = notice.PayDate?.ToShortDateString(),
                CostApplyID = notice.CostApplyID,
                Summary = notice.Summary,
            };

            int totalCount = 0;
            var paidList = new Needs.Ccs.Services.Views.PaidListView().GetResult(out totalCount, page, rows, lamdas.ToArray());

            Response.Write(new
            {
                rows = paidList.Select(convert).ToArray(),
                total = totalCount,
            }.Json());
        }


    }
}