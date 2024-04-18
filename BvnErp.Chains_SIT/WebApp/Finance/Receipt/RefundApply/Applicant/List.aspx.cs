using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;

namespace WebApp.Finance.Receipt.RefundApply.Applicant
{
    public partial class List : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadComboboxData();
        }

        protected void LoadComboboxData()
        {
            var applyStatus = Needs.Utils.Descriptions.EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.RefundApplyStatus>().Select(item => new { item.Key, item.Value });
            this.Model.RefundApplyStatus = applyStatus.Json();
        }

        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string StartDate = Request.QueryString["StartDate"];
            string EndDate = Request.QueryString["EndDate"];
            string AccountName = Request.QueryString["ClientName"];
            string ApplyStatus = Request.QueryString["ApplyStatus"];

            var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
            List<LambdaExpression> lamdas = new List<LambdaExpression>();

            using (var query = new Needs.Ccs.Services.Views.RefundApplyView())
            {
                var view = query;
                view = view.SearchByAdminID(currentAdmin.ID);

                if (!string.IsNullOrWhiteSpace(StartDate))
                {
                    StartDate = StartDate.Trim();
                    DateTime dtFrom = Convert.ToDateTime(StartDate);
                    view = view.SearchByFrom(dtFrom);
                }

                if (!string.IsNullOrWhiteSpace(EndDate))
                {
                    EndDate = EndDate.Trim();
                    DateTime dtTo = Convert.ToDateTime(EndDate).AddDays(1);
                    view = view.SearchByTo(dtTo);
                }

                if (!string.IsNullOrEmpty(AccountName))
                {
                    AccountName = AccountName.Trim();
                    view = view.SearchByAccountName(AccountName);
                }

                if (!string.IsNullOrEmpty(ApplyStatus))
                {
                    ApplyStatus = ApplyStatus.Trim();
                    view = view.SearchByApplyStatus((RefundApplyStatus)Convert.ToInt16(ApplyStatus));
                }

                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }

        protected void Cancel()
        {
            try
            {
                string applyID = Request.Form["ApplyID"];
                var apply = new Needs.Ccs.Services.Views.RefundApplyView().Where(t => t.ID == applyID).FirstOrDefault();
                apply.Approve(Needs.Ccs.Services.Enums.RefundApplyStatus.Canceled);

                var currentAdmin = Needs.Underly.FkoFactory<Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                Logs log = new Logs();
                log.Name = "退款申请";
                log.MainID = apply.ID;
                log.AdminID = apply.Applicant.ID;
                log.Json = apply.Json();
                log.Summary = "跟单员【" + currentAdmin.RealName + "】取消了退款申请";
                log.Enter();
                Response.Write((new { success = true, message = "保存成功" }).Json());
            }
            catch(Exception ex)
            {
                Response.Write((new { success = false, message = ex.ToString() }).Json());
            }
        }
    }
}