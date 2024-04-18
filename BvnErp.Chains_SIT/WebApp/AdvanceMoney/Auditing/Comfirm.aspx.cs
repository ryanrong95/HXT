using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using WebApp.App_Utils;

namespace WebApp.AdvanceMoney.Auditing
{
    public partial class comfirm : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            string id = Request.QueryString["ID"];
            string From = Request.QueryString["From"];
            string ClientID = Request.QueryString["ClientID"];
            if (!string.IsNullOrEmpty(id))
            {
                this.Model.ApplyID = id;
                this.Model.From = From;
                this.Model.ClientID = ClientID;
            }

        }

        /// <summary>
        /// 垫资申请
        /// </summary>
        protected void Save()
        {
            try
            {
                string applyID = Request.Form["ApplyID"];
                string reason = Request.Form["Reason"];
                string From = Request.Form["From"];
                string ClientID = Request.Form["ClientID"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);

                var advanceMoneyApply = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView1().FirstOrDefault(t => t.ClientID == ClientID && t.Status == Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Auditing);
                var agreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements.Where(t => t.ClientID == ClientID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();
                if (agreement != null)
                {
                    var clientAgreement = new Needs.Ccs.Services.Models.ClientAgreement();

                    clientAgreement.ProductFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                    clientAgreement.TaxFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                    clientAgreement.AgencyFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                    clientAgreement.IncidentalFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();

                    clientAgreement.ID = agreement.ID;
                    clientAgreement.ClientID = ClientID;
                    clientAgreement.AdminID = agreement.AdminID;

                    clientAgreement.StartDate = Convert.ToDateTime(agreement.StartDate);
                    clientAgreement.EndDate = Convert.ToDateTime(agreement.EndDate);
                    clientAgreement.AgencyRate = agreement.AgencyRate;
                    clientAgreement.MinAgencyFee = agreement.MinAgencyFee;
                    clientAgreement.IsPrePayExchange = agreement.IsPrePayExchange;
                    clientAgreement.IsLimitNinetyDays = agreement.IsLimitNinetyDays;
                    clientAgreement.InvoiceType = agreement.InvoiceType;
                    clientAgreement.InvoiceTaxRate = agreement.InvoiceTaxRate;
                    clientAgreement.Summary = agreement.Summary;

                    //货款
                    clientAgreement.ProductFeeClause.AgreementID = agreement.ID;
                    clientAgreement.ProductFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Product;
                    clientAgreement.ProductFeeClause.PeriodType = Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod;
                    clientAgreement.ProductFeeClause.DaysLimit = advanceMoneyApply.LimitDays;
                    clientAgreement.ProductFeeClause.MonthlyDay = agreement.ProductFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.ProductFeeClause.MonthlyDay;
                    clientAgreement.ProductFeeClause.UpperLimit = advanceMoneyApply.Amount;
                    clientAgreement.ProductFeeClause.ExchangeRateType = agreement.ProductFeeClause.ExchangeRateType;
                    clientAgreement.ProductFeeClause.ExchangeRateValue = agreement.ProductFeeClause.ExchangeRateValue;
                    clientAgreement.ProductFeeClause.AdminID = agreement.ProductFeeClause.AdminID;

                    //税款
                    clientAgreement.TaxFeeClause.AgreementID = agreement.ID;
                    clientAgreement.TaxFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Tax;
                    clientAgreement.TaxFeeClause.PeriodType = agreement.TaxFeeClause.PeriodType;
                    clientAgreement.TaxFeeClause.DaysLimit = agreement.TaxFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.TaxFeeClause.DaysLimit;
                    clientAgreement.TaxFeeClause.MonthlyDay = agreement.TaxFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.TaxFeeClause.MonthlyDay;
                    clientAgreement.TaxFeeClause.UpperLimit = agreement.TaxFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.TaxFeeClause.UpperLimit;
                    clientAgreement.TaxFeeClause.ExchangeRateType = agreement.TaxFeeClause.ExchangeRateType;
                    clientAgreement.TaxFeeClause.ExchangeRateValue = agreement.TaxFeeClause.ExchangeRateValue;

                    clientAgreement.TaxFeeClause.AdminID = agreement.TaxFeeClause.AdminID;

                    //代理费
                    clientAgreement.AgencyFeeClause.AgreementID = agreement.ID;
                    clientAgreement.AgencyFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.AgencyFee;
                    clientAgreement.AgencyFeeClause.PeriodType = agreement.AgencyFeeClause.PeriodType;
                    clientAgreement.AgencyFeeClause.DaysLimit = agreement.AgencyFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.DaysLimit;
                    clientAgreement.AgencyFeeClause.MonthlyDay = agreement.AgencyFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.MonthlyDay;
                    clientAgreement.AgencyFeeClause.UpperLimit = agreement.AgencyFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.UpperLimit;
                    clientAgreement.AgencyFeeClause.ExchangeRateType = agreement.AgencyFeeClause.ExchangeRateType;
                    clientAgreement.AgencyFeeClause.ExchangeRateValue = agreement.AgencyFeeClause.ExchangeRateValue;
                    clientAgreement.AgencyFeeClause.AdminID = agreement.AgencyFeeClause.AdminID;

                    //杂费
                    clientAgreement.IncidentalFeeClause.AgreementID = agreement.ID;
                    clientAgreement.IncidentalFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Incidental;
                    clientAgreement.IncidentalFeeClause.PeriodType = agreement.IncidentalFeeClause.PeriodType;
                    clientAgreement.IncidentalFeeClause.DaysLimit = agreement.IncidentalFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.DaysLimit;
                    clientAgreement.IncidentalFeeClause.MonthlyDay = agreement.IncidentalFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.MonthlyDay;
                    clientAgreement.IncidentalFeeClause.UpperLimit = agreement.IncidentalFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.UpperLimit;
                    clientAgreement.IncidentalFeeClause.ExchangeRateType = agreement.IncidentalFeeClause.ExchangeRateType;
                    clientAgreement.IncidentalFeeClause.ExchangeRateValue = agreement.IncidentalFeeClause.ExchangeRateValue;

                    clientAgreement.IncidentalFeeClause.AdminID = agreement.IncidentalFeeClause.AdminID;

                    clientAgreement.EnterError += ClientAgreement_EnterError;
                    clientAgreement.EnterSuccess += ClientAgreement_EnterSuccess;
                    #region  调用之后
                    try
                    {
                        string requestUrl = URL + "/CrmUnify/Contract";
                        HttpResponseMessage response = new HttpResponseMessage();
                        var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[clientAgreement.ClientID];
                        var entity = new Needs.Ccs.Services.Models.ApiModel.ClientAgreement()
                        {
                            Enterprise = new EnterpriseObj
                            {
                                AdminCode = "",
                                District = "",
                                Corporation = client.Company.Corporate,
                                Name = client.Company.Name,
                                RegAddress = client.Company.Address,
                                Uscc = client.Company.Code,
                                Status = 200
                            },
                            Agreement = new ApiModel.Agreement
                            {
                                StartDate = clientAgreement.StartDate,
                                EndDate = clientAgreement.EndDate,
                                AgencyRate = clientAgreement.AgencyRate,
                                MinAgencyFee = clientAgreement.MinAgencyFee,
                                ExchangeMode = clientAgreement.IsPrePayExchange ? 1 : 2,
                                InvoiceType = (int)clientAgreement.InvoiceType,
                                InvoiceTaxRate = clientAgreement.InvoiceTaxRate,
                                Summary = clientAgreement.Summary,
                                CreateDate = DateTime.Now.ToString(),
                                UpdateDate = DateTime.Now.ToString(),
                                AgencyFeeClause = clientAgreement.AgencyFeeClause,
                                ProductFeeClause = clientAgreement.ProductFeeClause,
                                TaxFeeClause = clientAgreement.TaxFeeClause,
                                IncidentalFeeClause = clientAgreement.IncidentalFeeClause


                            },
                            Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                        };

                        string apiAgreement = JsonConvert.SerializeObject(entity);
                        response = new HttpClientHelp().HttpClient("POST", requestUrl, apiAgreement);
                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                            return;
                        }
                        clientAgreement.Enter();

                        //经理修改垫资申请状态
                        Needs.Ccs.Services.Models.AdvanceMoneyApplyModel advanceMoneyApplym = new Needs.Ccs.Services.Models.AdvanceMoneyApplyModel
                        {
                            ApplyID = applyID,
                            Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective,
                            UpdateDate = DateTime.Now,
                            Summary = reason,
                        };
                        Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel advanceMoneyApplyLogs = new Needs.Ccs.Services.Models.AdvanceMoneyApplyLogModel()
                        {
                            ID = Guid.NewGuid().ToString("N"),
                            ApplyID = applyID,
                            Status = Needs.Ccs.Services.Enums.AdvanceMoneyStatus.Effective,
                            AdminID = admin.ID,
                            CreateDate = DateTime.Now,
                            Summary = "经理【" + admin.RealName + "】审批通过了垫资申请；备注：" + reason,
                        };

                        //保存 Begin

                        advanceMoneyApplym.Audit();

                        advanceMoneyApplyLogs.Enter();
                    }
                    catch (Exception ex)
                    {
                        Response.Write(new { success = false, message = ex.Message });
                    }

                    #endregion
                }
            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "保存失败：" + ex.Message }).Json());
            }
        }
        /// <summary>
        /// 保存异常
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientAgreement_EnterError(object sender, Needs.Linq.ErrorEventArgs e)
        {
            Response.Write(new { success = false, message = e.Message });
        }

        /// <summary>
        /// 保存后关闭弹出框
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ClientAgreement_EnterSuccess(object sender, Needs.Linq.SuccessEventArgs e)
        {
            Response.Write((new { success = true, message = "保存成功", ID = e.Object }).Json());
        }
    }
}