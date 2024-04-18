using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.Client.AgreementChange.Auditing
{
    public partial class Audit : Uc.PageBase
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }

        }
        protected void LoadData()
        {
            this.Model.Status = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.AgreementChangeApplyStatus>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.RealName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string CreateDateFrom = Request.QueryString["CreateDateFrom"];
            string CreateDateTo = Request.QueryString["CreateDateTo"];

            string Status = Request.QueryString["Status"];
            using (var query = new Needs.Ccs.Services.Views.AgreementChangeApplyListView())
            {
                var view = query;

                if (!string.IsNullOrEmpty(ClientCode))
                {
                    view = view.SearchByClientCode(ClientCode);
                }
                if (!string.IsNullOrEmpty(ClientName))
                {
                    view = view.SearchByClientName(ClientName);
                }
                if (!string.IsNullOrEmpty(Status))
                {
                    int agreementChangeApplyStatus = Convert.ToInt32(Status);
                    view = view.SearchByApplyStatus(agreementChangeApplyStatus);
                }
                if (!string.IsNullOrEmpty(CreateDateFrom))
                {
                    var from = DateTime.Parse(CreateDateFrom);
                    view = view.SearchByCreateDateFrom(from);
                }
                if (!string.IsNullOrEmpty(CreateDateTo))
                {
                    var to = DateTime.Parse(CreateDateTo).AddDays(1);
                    view = view.SearchByCreateDateTo(to);
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }


        /// <summary>
        /// 验证额度
        /// </summary>
        protected void CheckIsExcced()
        {
            //协议变更ID
            var ID = Request.Form["ID"];

            try
            {

                //超100W审批权限
                var ApproveExceedID = System.Configuration.ConfigurationManager.AppSettings["ApproveExceedID"];
                //总公司额度
                var ApproveExceedAmountStr = System.Configuration.ConfigurationManager.AppSettings["ApproveExceedAmount"];
                var ApproveExceedAmount = decimal.Parse(ApproveExceedAmountStr);
                //当前登录人
                var AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;


                var agreementChange = new Needs.Ccs.Services.Views.AgreementChangeAppliesView().FirstOrDefault(t => t.ID == ID);
                var AgreementApplyItem = new Needs.Ccs.Services.Views.AgreementChangeApplyView().Where(t => t.ID == ID).ToList();

                if (AgreementApplyItem.Any(t => t.AgreementChangeType == AgreementChangeType.TaxUpperLimit || t.AgreementChangeType == AgreementChangeType.AgencyUpperLimit || t.AgreementChangeType == AgreementChangeType.OtherUpperLimit))
                {
                    //客户的所有额度
                    var agreementView = new Needs.Ccs.Services.Views.ClientAgreementsView().FirstOrDefault(t => t.ClientID == agreementChange.ClientID && t.Status == Status.Normal);

                    var ProductFeeAmount = agreementView.ProductFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.ProductFeeClause.UpperLimit : 0;
                    var TaxFeeAmount = agreementView.TaxFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.TaxFeeClause.UpperLimit : 0;
                    var AgencyFeeAmount = agreementView.AgencyFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.AgencyFeeClause.UpperLimit : 0;
                    var IncidentalFeeAmount = agreementView.IncidentalFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.IncidentalFeeClause.UpperLimit : 0;


                    var amount = ProductFeeAmount
                        + (AgreementApplyItem.Any(t => t.AgreementChangeType == AgreementChangeType.TaxUpperLimit) ? decimal.Parse(AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.TaxUpperLimit).NewValue) : TaxFeeAmount)
                        + (AgreementApplyItem.Any(t => t.AgreementChangeType == AgreementChangeType.AgencyUpperLimit) ? decimal.Parse(AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.AgencyUpperLimit).NewValue) : AgencyFeeAmount)
                        + (AgreementApplyItem.Any(t => t.AgreementChangeType == AgreementChangeType.OtherUpperLimit) ? decimal.Parse(AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.OtherUpperLimit).NewValue) : IncidentalFeeAmount);

                    if (amount <= ApproveExceedAmount)
                    {
                        Response.Write((new { success = true, message = "" }).Json());
                    }
                    else if (ApproveExceedID.Contains(AdminID))
                    {
                        //有权限
                        Response.Write((new { success = true, message = "" }).Json());
                    }
                    else
                    {
                        //无权限
                        Response.Write((new { success = false, message = "" }).Json());
                    }
                }
                else
                {
                    Response.Write((new { success = true, message = "" }).Json());
                }               
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "校验额度权限出错：" + ex.Message }).Json());

            }

        }


    }
}
