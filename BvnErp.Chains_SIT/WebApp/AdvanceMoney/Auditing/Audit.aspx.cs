using Needs.Ccs.Services.Enums;
using Needs.Utils.Descriptions;
using Needs.Utils.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace WebApp.AdvanceMoney.Auditing
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
            this.Model.AdvanceMoneyStatus = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.AdvanceMoneyStatus>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.ServiceManager = Needs.Wl.Admin.Plat.AdminPlat.Current.Permissions.AdminRoles.Where(manager => manager.Role.Name == "业务员").
              Select(item => new { Key = item.Admin.ID, Value = item.Admin.RealName }).ToArray().Json();

            this.Model.CurrentName = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName;
        }
        protected void data()
        {
            int page, rows;
            int.TryParse(Request.QueryString["page"], out page);
            int.TryParse(Request.QueryString["rows"], out rows);

            string ClientCode = Request.QueryString["ClientCode"];
            string ClientName = Request.QueryString["ClientName"];
            string Status = Request.QueryString["Status"];
            string ServiceManager = Request.QueryString["ServiceManager"];
            using (var query = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView())
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
                    int advanceMoneyStatus = Convert.ToInt32(Status);
                    view = view.SearchByApplyStatus(advanceMoneyStatus);
                }
                if (!string.IsNullOrEmpty(ServiceManager))
                {
                    view = view.SearchByServiceManager(ServiceManager);
                }
                Response.Write(view.ToMyPage(page, rows).Json());
            }
        }


        /// <summary>
        /// 验证额度
        /// </summary>
        protected void CheckIsExcced()
        {
            //垫资申请ID
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


                var advanceMoneyApply = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView1().FirstOrDefault(t => t.ID == ID);

                if (advanceMoneyApply != null)
                {
                    //客户的所有额度
                    var agreementView = new Needs.Ccs.Services.Views.ClientAgreementsView().FirstOrDefault(t => t.ClientID == advanceMoneyApply.ClientID && t.Status == Status.Normal);

                    
                    var TaxFeeAmount = agreementView.TaxFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.TaxFeeClause.UpperLimit : 0;
                    var AgencyFeeAmount = agreementView.AgencyFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.AgencyFeeClause.UpperLimit : 0;
                    var IncidentalFeeAmount = agreementView.IncidentalFeeClause.PeriodType != PeriodType.PrePaid ? agreementView.IncidentalFeeClause.UpperLimit : 0;


                    var amount = advanceMoneyApply.Amount + TaxFeeAmount + AgencyFeeAmount + IncidentalFeeAmount;
                       
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