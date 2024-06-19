using Needs.Ccs.Services.Models;
using Needs.Utils.Serializers;
using Needs.Utils.Descriptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Needs.Ccs.Services.Enums;
using Needs.Utils;
using Needs.Ccs.Services.Views;
using Needs.Ccs.Services;
using System.Net;
using Newtonsoft.Json;
using WebApp.App_Utils;
using System.Net.Http;
using Needs.Utils.Converters;
using System.IO;

namespace WebApp.Client.AgreementChange
{
    public partial class AgreementChangeView : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadData();
            }
        }
        protected void LoadData()
        {
            this.Model.AgreementApply = "".Json();
            this.Model.AgreementApplyItem = "".Json();

            string id = Request.QueryString["ID"];
            string From = Request.QueryString["From"];


            var AgreementApply = new Needs.Ccs.Services.Views.AgreementChangeAppliesView().FirstOrDefault(t => t.ID == id);
            if (AgreementApply != null)
            {
                string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
                var serviceFile = new CenterFilesTopView().FirstOrDefault(x => x.ClientID == AgreementApply.ClientID && x.Type == (int)Needs.Ccs.Services.Enums.FileType.ChangeServiceAgreement && x.Status != FileDescriptionStatus.Delete && x.ApplicationID == id);
                if (serviceFile != null)
                {
                    this.Model.ServiceFile = (new { Name = serviceFile.CustomName, Url = FileServerUrl + @"/" + serviceFile.Url.ToUrl() }).Json();
                }
                else
                {
                    this.Model.ServiceFile = null;
                }
                this.Model.AgreementApply = new
                {
                    From = From,
                    ID = AgreementApply.ID,
                    ClientCode = AgreementApply.ClientCode,
                    ClientName = AgreementApply.ClientName,
                    ClientID = AgreementApply.ClientID,
                    ClientRank = AgreementApply.ClientRank,
                    AdminID = AgreementApply.AdminID,
                    Status = AgreementApply.Status,
                }.Json();
            }

        }

        protected void data()
        {
            string id = Request.QueryString["ID"];
            var StartDate = "";
            var OldValue = "";
            var NewValue = "";
            List<AgreementChangeDateil> list = new List<AgreementChangeDateil>();
            var AgreementApplyItem = new Needs.Ccs.Services.Views.AgreementChangeApplyView().Where(t => t.ID == id).ToList();
            if (AgreementApplyItem.Count != 0)
            {
                //生效日期
                var Date = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.StartDate || t.AgreementChangeType == AgreementChangeType.EndDate).ToList();
                foreach (var item in Date)
                {
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.StartDate)
                    {
                        StartDate = "协议生效日期";
                        OldValue = item.OldValue;
                        NewValue = item.NewValue;
                    }
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.EndDate)
                    {
                        if (OldValue == "" && NewValue == "")
                        {
                            StartDate = "协议生效日期";
                            OldValue = item.OldValue;
                            NewValue = item.NewValue;
                        }
                        else
                        {
                            OldValue = OldValue + " - " + item.OldValue;
                            NewValue = NewValue + " - " + item.NewValue;
                        }

                    }

                }
                if (StartDate == "协议生效日期")
                {
                    list.Add(new AgreementChangeDateil
                    {
                        StartDate = StartDate,
                        OldValue = OldValue,
                        NewValue = NewValue
                    });
                }

                //服务费
                var AgencyFee = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.AgencyRate || t.AgreementChangeType == AgreementChangeType.MinAgencyFee || t.AgreementChangeType == AgreementChangeType.PreAgency).ToList();
                StartDate = "";
                OldValue = "";
                NewValue = "";
                foreach (var item in AgencyFee)
                {
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.AgencyRate)
                    {
                        StartDate = "服务费";
                        OldValue = OldValue + "; 服务费率：" + Convert.ToDouble(item.OldValue).ToString();
                        NewValue = NewValue + "; 服务费率：" + Convert.ToDouble(item.NewValue).ToString();
                    }
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.PreAgency)
                    {
                        if (StartDate == "服务费")
                        {
                            if (!string.IsNullOrEmpty(item.OldValue))
                            {
                                OldValue = OldValue + "; 基础服务费：" + Convert.ToDouble(item.OldValue).ToString();
                            }
                            if (!string.IsNullOrEmpty(item.NewValue))
                            {
                                NewValue = NewValue + "; 最低服务费：" + Convert.ToDouble(item.NewValue).ToString();
                            }
                        }
                        else
                        {
                            StartDate = "服务费";
                            OldValue = OldValue + "基础服务费：" + Convert.ToDouble(item.OldValue).ToString();
                            NewValue = NewValue + "基础服务费：" + Convert.ToDouble(item.NewValue).ToString();
                        }

                    }
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.MinAgencyFee)
                    {
                        if (StartDate == "服务费")
                        {
                            if (!string.IsNullOrEmpty(item.OldValue))
                            {
                                OldValue = OldValue + "; 最低服务费：" + Convert.ToDouble(item.OldValue).ToString();
                            }
                            if (!string.IsNullOrEmpty(item.NewValue))
                            {
                                NewValue = NewValue + "; 最低服务费：" + Convert.ToDouble(item.NewValue).ToString();
                            }
                        }
                        else
                        {
                            StartDate = "服务费";
                            OldValue = OldValue + "最低服务费：" + Convert.ToDouble(item.OldValue).ToString();
                            NewValue = NewValue + "最低服务费：" + Convert.ToDouble(item.NewValue).ToString();
                        }

                    }
                }
                if (StartDate == "服务费")
                {
                    list.Add(new AgreementChangeDateil
                    {
                        StartDate = StartDate,
                        OldValue = OldValue,
                        NewValue = NewValue
                    });
                }

                //换汇方式
                var IsPrePayExchange = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.IsPrePayExchange).ToList();
                StartDate = "";
                OldValue = "";
                NewValue = "";
                foreach (var item in IsPrePayExchange)
                {
                    if (item.AgreementChangeType == AgreementChangeType.IsPrePayExchange)
                    {
                        if (item.OldValue == "True")
                        {
                            StartDate = "换汇方式";
                            OldValue = "预换汇";
                            NewValue = "90天内换汇";
                        }
                        else
                        {
                            StartDate = "换汇方式";
                            OldValue = "90天内换汇";
                            NewValue = "预换汇";
                        }
                    }
                }
                if (StartDate == "换汇方式")
                {
                    list.Add(new AgreementChangeDateil
                    {
                        StartDate = StartDate,
                        OldValue = OldValue,
                        NewValue = NewValue
                    });
                }

                //换汇汇率类型
                var IsTenType = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.IsTenType).ToList();
                StartDate = "";
                OldValue = "";
                NewValue = "";
                foreach (var item in IsTenType)
                {
                    if (item.OldValue == "True")
                    {
                        StartDate = "换汇汇率";
                        OldValue = "10:00";
                        NewValue = "09:30";
                    }
                    else
                    {
                        StartDate = "换汇汇率";
                        OldValue = "09:30";
                        NewValue = "10:00";
                    }

                }
                if (StartDate == "换汇汇率")
                {
                    list.Add(new AgreementChangeDateil
                    {
                        StartDate = StartDate,
                        OldValue = OldValue,
                        NewValue = NewValue
                    });
                }
               

                //税款条款
                var Tax = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.TaxPeriodType
                || t.AgreementChangeType == AgreementChangeType.TaxDaysLimit
                || t.AgreementChangeType == AgreementChangeType.TaxMonthlyDay
                || t.AgreementChangeType == AgreementChangeType.TaxUpperLimit
                || t.AgreementChangeType == AgreementChangeType.TaxExchangeRateType
                || t.AgreementChangeType == AgreementChangeType.TaxExchangeRateValue).ToList();
                StartDate = "";
                OldValue = "";
                NewValue = "";
                foreach (var item in Tax)
                {
                    StartDate = "税款条款";
                    if (item.AgreementChangeType == AgreementChangeType.TaxPeriodType)
                    {

                        if (item.OldValue == Needs.Ccs.Services.Enums.PeriodType.PrePaid.ToString())
                        {
                            OldValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            OldValue = item.OldValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            OldValue = "结算方式：" + OldValue + "; ";
                        }
                        if (item.NewValue == PeriodType.PrePaid.ToString())
                        {
                            NewValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            NewValue = item.NewValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            NewValue = "结算方式：" + NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxDaysLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "约定期限(天)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "约定期限(天)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxMonthlyDay)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "结算日期(次月)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "结算日期(次月)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxUpperLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "垫款上限(元)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "垫款上限(元)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxExchangeRateType)
                    {
                        //StartDate = "税款条款";
                        if (item.OldValue == ExchangeRateType.Agreed.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.Custom.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.RealTime.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                        if (item.NewValue == ExchangeRateType.Agreed.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.Custom.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.RealTime.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.TaxExchangeRateValue)
                    {
                        if (OldValue.Contains("约定汇率"))
                        {
                            OldValue = OldValue + ";约定汇率：" + item.OldValue;
                        }
                        if (NewValue.Contains("约定汇率"))
                        {
                            NewValue = NewValue + ";约定汇率：" + item.NewValue;
                        }
                    }
                }
                if (StartDate == "税款条款")
                {
                    list.Add(new AgreementChangeDateil
                    {
                        StartDate = StartDate,
                        OldValue = OldValue,
                        NewValue = NewValue
                    });
                }

                //服务费条款
                var Agency = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.AgencyPeriodType
                || t.AgreementChangeType == AgreementChangeType.AgencyDaysLimit
                || t.AgreementChangeType == AgreementChangeType.AgencyMonthlyDay
                || t.AgreementChangeType == AgreementChangeType.AgencyUpperLimit
                || t.AgreementChangeType == AgreementChangeType.AgencyExchangeRateType
                || t.AgreementChangeType == AgreementChangeType.AgencyExchangeRateValue).ToList();
                StartDate = "";
                OldValue = "";
                NewValue = "";
                foreach (var item in Agency)
                {
                    StartDate = "服务费条款";
                    if (item.AgreementChangeType == AgreementChangeType.AgencyPeriodType)
                    {

                        if (item.OldValue == PeriodType.PrePaid.ToString())
                        {
                            OldValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            OldValue = item.OldValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            OldValue = "结算方式：" + OldValue + "; ";
                        }
                        if (item.NewValue == PeriodType.PrePaid.ToString())
                        {
                            NewValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            NewValue = item.NewValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            NewValue = "结算方式：" + NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyDaysLimit)
                    {

                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "约定期限(天)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "约定期限(天)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyMonthlyDay)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "结算日期(次月)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "结算日期(次月)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyUpperLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "垫款上限(元)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "垫款上限(元)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyExchangeRateType)
                    {
                        StartDate = "服务费条款";
                        if (item.OldValue == ExchangeRateType.Agreed.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.Custom.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.RealTime.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                        if (item.NewValue == ExchangeRateType.Agreed.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.Custom.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.RealTime.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.AgencyExchangeRateValue)
                    {

                        if (OldValue.Contains("约定汇率"))
                        {
                            OldValue = OldValue + ";约定汇率：" + item.OldValue;
                        }
                        if (NewValue.Contains("约定汇率"))
                        {
                            NewValue = NewValue + ";约定汇率：" + item.NewValue;
                        }
                    }
                }
                if (StartDate == "服务费条款")
                {
                    list.Add(new AgreementChangeDateil
                    {
                        StartDate = StartDate,
                        OldValue = OldValue,
                        NewValue = NewValue
                    });
                }

                //杂费条款
                var Other = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.OtherPeriodType
                || t.AgreementChangeType == AgreementChangeType.OtherDaysLimit
                || t.AgreementChangeType == AgreementChangeType.OtherMonthlyDay
                || t.AgreementChangeType == AgreementChangeType.OtherUpperLimit
                || t.AgreementChangeType == AgreementChangeType.OtherExchangeRateType
                || t.AgreementChangeType == AgreementChangeType.OtherExchangeRateValue).ToList();
                StartDate = "";
                OldValue = "";
                NewValue = "";
                foreach (var item in Other)
                {
                    StartDate = "杂费条款";
                    if (item.AgreementChangeType == AgreementChangeType.OtherPeriodType)
                    {

                        if (item.OldValue == PeriodType.PrePaid.ToString())
                        {
                            OldValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            OldValue = item.OldValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            OldValue = "结算方式：" + OldValue + "; ";
                        }
                        if (item.NewValue == PeriodType.PrePaid.ToString())
                        {
                            NewValue = "结算方式：" + PeriodType.PrePaid.GetDescription() + "; ";
                        }
                        else
                        {
                            NewValue = item.NewValue == PeriodType.AgreedPeriod.ToString() ? PeriodType.AgreedPeriod.GetDescription() : Needs.Ccs.Services.Enums.PeriodType.Monthly.GetDescription();
                            NewValue = "结算方式：" + NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherDaysLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "约定期限(天)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "约定期限(天)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherMonthlyDay)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "结算日期(次月)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "结算日期(次月)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherUpperLimit)
                    {
                        if (!string.IsNullOrEmpty(item.OldValue))
                        {
                            OldValue = OldValue + "垫款上限(元)：" + item.OldValue + "; ";
                        }
                        if (!string.IsNullOrEmpty(item.NewValue))
                        {
                            NewValue = NewValue + "垫款上限(元)：" + item.NewValue + "; ";
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherExchangeRateType)
                    {
                        //StartDate = "杂费条款";
                        if (item.OldValue == ExchangeRateType.Agreed.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.Custom.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.OldValue == ExchangeRateType.RealTime.ToString())
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            OldValue = OldValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                        if (item.NewValue == ExchangeRateType.Agreed.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Agreed.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.Custom.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.Custom.GetDescription();
                        }
                        else if (item.NewValue == ExchangeRateType.RealTime.ToString())
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.RealTime.GetDescription();
                        }
                        else
                        {
                            NewValue = NewValue + "汇率类型：" + ExchangeRateType.None.GetDescription();
                        }
                    }
                    if (item.AgreementChangeType == AgreementChangeType.OtherExchangeRateValue)
                    {
                        if (OldValue.Contains("约定汇率"))
                        {
                            OldValue = OldValue + ";约定汇率：" + item.OldValue;
                        }
                        if (NewValue.Contains("约定汇率"))
                        {
                            NewValue = NewValue + ";约定汇率：" + item.NewValue;
                        }
                    }
                }
                if (StartDate == "杂费条款")
                {
                    list.Add(new AgreementChangeDateil
                    {
                        StartDate = StartDate,
                        OldValue = OldValue,
                        NewValue = NewValue
                    });
                }

                //开票类型
                var Invoice = AgreementApplyItem.Where(t => t.AgreementChangeType == AgreementChangeType.InvoiceType || t.AgreementChangeType == AgreementChangeType.InvoiceTaxRate).ToList();
                StartDate = "";
                OldValue = "";
                NewValue = "";
                foreach (var item in Invoice)
                {
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.InvoiceType)
                    {
                        StartDate = "开票类型";
                        if (item.OldValue == InvoiceType.Full.ToString())
                        {
                            OldValue = InvoiceType.Full.GetDescription();
                        }
                        else
                        {
                            OldValue = InvoiceType.Service.GetDescription();
                        }
                        if (item.NewValue == InvoiceType.Full.ToString())
                        {
                            NewValue = InvoiceType.Full.GetDescription();
                        }
                        else
                        {
                            NewValue = InvoiceType.Service.GetDescription();
                        }
                    }
                    if (item.AgreementChangeType == Needs.Ccs.Services.Enums.AgreementChangeType.InvoiceTaxRate)
                    {
                        if (StartDate == "开票类型")
                        {
                            OldValue = OldValue + ";" + (Convert.ToDouble(item.OldValue) * 100).ToString() + "%";
                            NewValue = NewValue + ";" + (Convert.ToDouble(item.NewValue) * 100).ToString() + "%";
                        }
                        else
                        {
                            StartDate = "开票类型";
                            OldValue = (Convert.ToDouble(item.OldValue) * 100).ToString() + "%";
                            NewValue = (Convert.ToDouble(item.NewValue) * 100).ToString() + "%";
                        }

                    }
                }
                if (StartDate == "开票类型")
                {
                    list.Add(new AgreementChangeDateil
                    {
                        StartDate = StartDate,
                        OldValue = OldValue,
                        NewValue = NewValue
                    });
                }
            }
            Func<AgreementChangeDateil, object> convert = item => new
            {
                StartDate = item.StartDate,
                OldValue = item.OldValue,
                NewValue = item.NewValue,
            };
            Response.Write(new
            {
                rows = list.Select(convert).ToArray()
            }.Json());
        }

        /// <summary>
        /// 日志记录
        /// </summary>
        protected void LoadLogs()
        {
            string ApplyID = Request.Form["ApplyID"];
            var applyLogs = new Needs.Ccs.Services.Views.LogsView().Where(t => t.MainID == ApplyID).ToArray();
            Func<Needs.Ccs.Services.Models.Logs, object> convert = item => new
            {
                ID = item.ID,
                CreateDate = item.CreateDate.ToString("yyyy-MM-dd HH:mm:ss"),
                Summary = item.Summary
            };
            Response.Write(new { rows = applyLogs.OrderByDescending(t => t.CreateDate).Select(convert).ToArray() }.Json());
        }

        /// <summary>
        /// 导出协议word
        /// </summary>
        protected void ExportAgreement()
        {
            try
            {
                var applyID = Request.Form["ApplyID"];
                var clientID = Request.Form["ClientID"];

                var clientAgreement = new Needs.Ccs.Services.Views.ClientAgreementsView().FirstOrDefault(t => t.ClientID == clientID && t.Status == Status.Normal);
                if (clientAgreement != null)
                {
                    var agreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements[clientAgreement.ID];
                    //创建文件夹
                    var fileName = agreement.AgreementCode + "补充协议.docx";
                    FileDirectory file = new FileDirectory(fileName);
                    file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                    file.CreateDataDirectory();
                    //保存文件
                    agreement.ChangeSaveAs(file.FilePath, applyID);

                    Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());
                }

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败" + ex.Message }).Json());
            }
        }
        protected void SaveAgreementApplyFile()
        {
            var file = Request.Files["ServiceAgreement"];
            var ClientID = Request.Form["ClientID"];
            var ApplicationID = Request.Form["ApplicationID"];
            //处理附件
            if (file.ContentLength != 0)
            {
                string fileName = file.FileName.ReName();
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                httpFile.CreateDataDirectory();
                string[] result = httpFile.SaveAs(file);

                //上传中心
                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.ChangeServiceAgreement;
                var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                var dic = new { CustomName = fileName, ClientID = ClientID, AdminID = ErmAdminID, ApplicationID = ApplicationID };
                new Needs.Ccs.Services.Models.CenterFilesTopView().NewDeleteFile(ClientID, (int)Needs.Ccs.Services.Enums.FileType.ChangeServiceAgreement,ApplicationID);

                var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + httpFile.VirtualPath, centerType, dic);
                if (!string.IsNullOrEmpty(uploadFile[0].FileID))
                {

                    Response.Write((new { success = true, message = "提交成功" }).Json());
                }
                else
                {

                    Response.Write((new { success = false, message = "提交失败" }).Json());
                }
            }
        }
        protected void Submit()
        {
            try
            {
                string applyID = Request.Form["ApplyID"];
                string summary = Request.Form["Reason"];
                string from = Request.Form["From"];
                string clientID = Request.Form["ClientID"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                if (from == "Audit")
                {
                    if (!string.IsNullOrEmpty(applyID))
                    {
                        var apply = new Needs.Ccs.Services.Models.ClientAgreementChangeApplyModel();
                        apply.ID = applyID;
                        apply.ApplyID = applyID;
                        apply.Status = AgreementChangeApplyStatus.Auditing;
                        apply.AdminID = admin.ID;
                        apply.Summary = summary;
                        apply.RealName = admin.RealName;
                        apply.From = from;
                        apply.Update();
                        //保存 Begin

                        Response.Write((new { success = true, message = "提交成功" }).Json());
                    }
                    else
                    {
                        Response.Write((new { success = false, message = "提交失败" }).Json());
                    }
                }
                else
                {
                    if (!string.IsNullOrEmpty(applyID))
                    {
                        var apply = new Needs.Ccs.Services.Models.ClientAgreementChangeApplyModel();
                        apply.ID = applyID;
                        apply.ApplyID = applyID;
                        apply.Status = AgreementChangeApplyStatus.Effective;
                        apply.AdminID = admin.ID;
                        apply.Summary = summary;
                        apply.RealName = admin.RealName;
                        apply.From = from;
                        apply.Update();

                        //保存 Begin

                        //更新正式客户协议
                        var agreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements.Where(t => t.ClientID == clientID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();
                        var AgreementApplyItem = new Needs.Ccs.Services.Views.AgreementChangeApplyView().Where(t => t.ID == applyID).ToList();
                        if (agreement != null)
                        {
                            var clientAgreement = new Needs.Ccs.Services.Models.ClientAgreement();

                            clientAgreement.ProductFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                            clientAgreement.TaxFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                            clientAgreement.AgencyFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
                            clientAgreement.IncidentalFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();

                            //clientAgreement.ID = "";//agreement.ID;
                            clientAgreement.ClientID = clientID;
                            clientAgreement.AdminID = agreement.AdminID;
                            clientAgreement.Summary = agreement.Summary;

                            #region 协议生效日期

                            //协议生效开始日期
                            var StartDate = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.StartDate);
                            if (StartDate != null)
                            {
                                clientAgreement.StartDate = Convert.ToDateTime((StartDate.NewValue));
                            }
                            else
                            {
                                clientAgreement.StartDate = Convert.ToDateTime(agreement.StartDate);
                            }
                            //协议生效结束日期
                            var EndDate = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.EndDate);
                            if (EndDate != null)
                            {
                                clientAgreement.EndDate = Convert.ToDateTime(EndDate.NewValue);
                            }
                            else
                            {
                                clientAgreement.EndDate = Convert.ToDateTime(agreement.EndDate);
                            }

                            #endregion

                            #region 服务费
                            //预收服务费
                            var preAgency = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.PreAgency);
                            if (preAgency != null)
                            {
                                clientAgreement.PreAgency = Convert.ToDecimal(preAgency.NewValue);
                            }
                            else
                            {
                                clientAgreement.PreAgency = agreement.PreAgency;
                            }

                            //服务费率
                            var agencyRate = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.AgencyRate);
                            if (agencyRate != null)
                            {
                                clientAgreement.AgencyRate = Convert.ToDecimal(agencyRate.NewValue);
                            }
                            else
                            {
                                clientAgreement.AgencyRate = agreement.AgencyRate;
                            }
                            //最低服务费
                            var minAgencyFee = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.MinAgencyFee);
                            if (minAgencyFee != null)
                            {
                                clientAgreement.MinAgencyFee = Convert.ToDecimal(minAgencyFee.NewValue);
                            }
                            else
                            {
                                clientAgreement.MinAgencyFee = agreement.MinAgencyFee;
                            }
                            #endregion

                            #region 换汇方式

                            //是否换汇
                            var isPrePayExchange = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.IsPrePayExchange);
                            if (isPrePayExchange != null)
                            {
                                clientAgreement.IsPrePayExchange = Convert.ToBoolean(isPrePayExchange.NewValue);
                                clientAgreement.IsLimitNinetyDays = Convert.ToBoolean(isPrePayExchange.NewValue) == true ? false : true;
                            }
                            else
                            {
                                clientAgreement.IsPrePayExchange = agreement.IsPrePayExchange;
                                clientAgreement.IsLimitNinetyDays = agreement.IsLimitNinetyDays;
                            }
                            #endregion

                            #region 换汇汇率
                            var IsTenType = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.IsTenType);
                            if (IsTenType != null)
                            {
                                clientAgreement.IsTen = (PEIsTen)Enum.Parse(typeof(PEIsTen), IsTenType.NewValue);
                            }
                            else 
                            {
                                clientAgreement.IsTen = agreement.IsTen;
                            }
                            #endregion

                            #region 开票类型

                            //开票类型
                            var invoiceType = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.InvoiceType);
                            if (invoiceType != null)
                            {
                                clientAgreement.InvoiceType = (InvoiceType)Enum.Parse(typeof(InvoiceType), invoiceType.NewValue);
                            }
                            else
                            {
                                clientAgreement.InvoiceType = agreement.InvoiceType;
                            }
                            //开票税点
                            var invoiceTaxRate = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.InvoiceTaxRate);
                            if (invoiceTaxRate != null)
                            {
                                clientAgreement.InvoiceTaxRate = Convert.ToDecimal(invoiceTaxRate.NewValue);
                            }
                            else
                            {
                                clientAgreement.InvoiceTaxRate = agreement.InvoiceTaxRate;
                            }
                            #endregion

                            //货款
                            clientAgreement.ProductFeeClause.AgreementID = clientAgreement.ID;
                            clientAgreement.ProductFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Product;
                            clientAgreement.ProductFeeClause.PeriodType = (PeriodType)Enum.Parse(typeof(PeriodType), agreement.ProductFeeClause.PeriodType.ToString());
                            clientAgreement.ProductFeeClause.DaysLimit = agreement.ProductFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.ProductFeeClause.DaysLimit;
                            clientAgreement.ProductFeeClause.MonthlyDay = agreement.ProductFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.ProductFeeClause.MonthlyDay;
                            clientAgreement.ProductFeeClause.UpperLimit = agreement.ProductFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.ProductFeeClause.UpperLimit;
                            clientAgreement.ProductFeeClause.ExchangeRateType = agreement.ProductFeeClause.ExchangeRateType;
                            clientAgreement.ProductFeeClause.ExchangeRateValue = agreement.ProductFeeClause.ExchangeRateValue;
                            clientAgreement.ProductFeeClause.AdminID = agreement.ProductFeeClause.AdminID;

                            #region 税款
                            //税款
                            clientAgreement.TaxFeeClause.AgreementID = clientAgreement.ID;
                            clientAgreement.TaxFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Tax;
                            var taxPeriodType = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.TaxPeriodType);
                            if (taxPeriodType != null)
                            {
                                clientAgreement.TaxFeeClause.PeriodType = (PeriodType)Enum.Parse(typeof(PeriodType), taxPeriodType.NewValue);
                            }
                            else
                            {
                                clientAgreement.TaxFeeClause.PeriodType = agreement.TaxFeeClause.PeriodType;
                            }
                            var taxDaysLimit = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.TaxDaysLimit);
                            if (taxDaysLimit != null)
                            {
                                clientAgreement.TaxFeeClause.DaysLimit = taxDaysLimit.NewValue == "" ? (int?)null : Convert.ToInt32(taxDaysLimit.NewValue);
                            }
                            else
                            {
                                clientAgreement.TaxFeeClause.DaysLimit = agreement.TaxFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.TaxFeeClause.DaysLimit;
                            }

                            var taxMonthlyDay = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.TaxMonthlyDay);
                            if (taxMonthlyDay != null)
                            {
                                clientAgreement.TaxFeeClause.MonthlyDay = taxMonthlyDay.NewValue == "" ? (int?)null : Convert.ToInt32(taxMonthlyDay.NewValue);
                            }
                            else
                            {
                                clientAgreement.TaxFeeClause.MonthlyDay = agreement.TaxFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.TaxFeeClause.MonthlyDay;
                            }

                            var taxUpperLimit = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.TaxUpperLimit);
                            if (taxUpperLimit != null)
                            {
                                clientAgreement.TaxFeeClause.UpperLimit = taxUpperLimit.NewValue == "" ? (int?)null : Convert.ToInt32(taxUpperLimit.NewValue);
                            }
                            else
                            {
                                clientAgreement.TaxFeeClause.UpperLimit = agreement.TaxFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.TaxFeeClause.UpperLimit;
                            }

                            var taxExchangeRateType = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.TaxExchangeRateType);
                            if (taxExchangeRateType != null)
                            {
                                clientAgreement.TaxFeeClause.ExchangeRateType = (ExchangeRateType)Enum.Parse(typeof(ExchangeRateType), taxExchangeRateType.NewValue);
                            }
                            else
                            {
                                clientAgreement.TaxFeeClause.ExchangeRateType = agreement.TaxFeeClause.ExchangeRateType;
                            }

                            var taxExchangeRateValue = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.TaxExchangeRateValue);
                            if (taxExchangeRateValue != null)
                            {
                                clientAgreement.TaxFeeClause.ExchangeRateValue = Convert.ToDecimal(taxExchangeRateValue.NewValue);
                            }
                            else
                            {
                                clientAgreement.TaxFeeClause.ExchangeRateValue = agreement.TaxFeeClause.ExchangeRateValue;
                            }
                            clientAgreement.TaxFeeClause.AdminID = agreement.TaxFeeClause.AdminID;
                            #endregion

                            #region 服务费
                            //服务费
                            clientAgreement.AgencyFeeClause.AgreementID = clientAgreement.ID;
                            clientAgreement.AgencyFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.AgencyFee;
                            var agencyPeriodType = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.AgencyPeriodType);
                            if (agencyPeriodType != null)
                            {
                                clientAgreement.AgencyFeeClause.PeriodType = (PeriodType)Enum.Parse(typeof(PeriodType), agencyPeriodType.NewValue);
                            }
                            else
                            {
                                clientAgreement.AgencyFeeClause.PeriodType = agreement.AgencyFeeClause.PeriodType;
                            }

                            var agencyDaysLimit = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.AgencyDaysLimit);
                            if (agencyDaysLimit != null)
                            {
                                clientAgreement.AgencyFeeClause.DaysLimit = agencyDaysLimit.NewValue == "" ? (int?)null : Convert.ToInt32(agencyDaysLimit.NewValue);
                            }
                            else
                            {
                                clientAgreement.AgencyFeeClause.DaysLimit = agreement.AgencyFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.DaysLimit;
                            }

                            var agencyMonthlyDay = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.AgencyMonthlyDay);
                            if (agencyMonthlyDay != null)
                            {
                                clientAgreement.AgencyFeeClause.MonthlyDay = agencyMonthlyDay.NewValue == "" ? (int?)null : Convert.ToInt32(agencyMonthlyDay.NewValue);
                            }
                            else
                            {
                                clientAgreement.AgencyFeeClause.MonthlyDay = agreement.AgencyFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.MonthlyDay;
                            }

                            var agencyUpperLimit = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.AgencyUpperLimit);
                            if (agencyUpperLimit != null)
                            {
                                clientAgreement.AgencyFeeClause.UpperLimit = agencyUpperLimit.NewValue == "" ? (int?)null : Convert.ToInt32(agencyUpperLimit.NewValue);
                            }
                            else
                            {
                                clientAgreement.AgencyFeeClause.UpperLimit = agreement.AgencyFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.AgencyFeeClause.UpperLimit;
                            }

                            var agencyExchangeRateType = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.AgencyExchangeRateType);
                            if (agencyExchangeRateType != null)
                            {
                                clientAgreement.AgencyFeeClause.ExchangeRateType = (ExchangeRateType)Enum.Parse(typeof(ExchangeRateType), agencyExchangeRateType.NewValue);
                            }
                            else
                            {
                                clientAgreement.AgencyFeeClause.ExchangeRateType = agreement.AgencyFeeClause.ExchangeRateType;
                            }

                            var agencyExchangeRateValue = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.AgencyExchangeRateValue);
                            if (agencyExchangeRateValue != null)
                            {
                                clientAgreement.AgencyFeeClause.ExchangeRateValue = Convert.ToDecimal(agencyExchangeRateValue.NewValue);
                            }
                            else
                            {
                                clientAgreement.AgencyFeeClause.ExchangeRateValue = agreement.AgencyFeeClause.ExchangeRateValue;
                            }
                            clientAgreement.AgencyFeeClause.AdminID = agreement.AgencyFeeClause.AdminID;
                            #endregion

                            #region 杂费

                            //杂费
                            clientAgreement.IncidentalFeeClause.AgreementID = clientAgreement.ID;
                            clientAgreement.IncidentalFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Incidental;
                            var otherPeriodType = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.OtherPeriodType);
                            if (otherPeriodType != null)
                            {
                                clientAgreement.IncidentalFeeClause.PeriodType = (PeriodType)Enum.Parse(typeof(PeriodType), otherPeriodType.NewValue);
                            }
                            else
                            {
                                clientAgreement.IncidentalFeeClause.PeriodType = agreement.IncidentalFeeClause.PeriodType;
                            }

                            var otherDaysLimit = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.OtherDaysLimit);
                            if (otherDaysLimit != null)
                            {
                                clientAgreement.IncidentalFeeClause.DaysLimit = otherDaysLimit.NewValue == "" ? (int?)null : Convert.ToInt32(otherDaysLimit.NewValue);
                            }
                            else
                            {
                                clientAgreement.IncidentalFeeClause.DaysLimit = agreement.IncidentalFeeClause.DaysLimit.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.DaysLimit;
                            }

                            var otherMonthlyDay = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.OtherMonthlyDay);
                            if (otherMonthlyDay != null)
                            {
                                clientAgreement.IncidentalFeeClause.MonthlyDay = otherMonthlyDay.NewValue == "" ? (int?)null : Convert.ToInt32(otherMonthlyDay.NewValue);
                            }
                            else
                            {
                                clientAgreement.IncidentalFeeClause.MonthlyDay = agreement.IncidentalFeeClause.MonthlyDay.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.MonthlyDay;
                            }

                            var otherUpperLimit = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.OtherUpperLimit);
                            if (otherUpperLimit != null)
                            {
                                clientAgreement.IncidentalFeeClause.UpperLimit = otherUpperLimit.NewValue == "" ? (int?)null : Convert.ToInt32(otherUpperLimit.NewValue);
                            }
                            else
                            {
                                clientAgreement.IncidentalFeeClause.UpperLimit = agreement.IncidentalFeeClause.UpperLimit.ToString() == "" ? (int?)null : agreement.IncidentalFeeClause.UpperLimit;
                            }

                            var otherExchangeRateType = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.OtherExchangeRateType);
                            if (otherExchangeRateType != null)
                            {
                                clientAgreement.IncidentalFeeClause.ExchangeRateType = (ExchangeRateType)Enum.Parse(typeof(ExchangeRateType), otherExchangeRateType.NewValue);
                            }
                            else
                            {
                                clientAgreement.IncidentalFeeClause.ExchangeRateType = agreement.IncidentalFeeClause.ExchangeRateType;
                            }

                            var otherExchangeRateValue = AgreementApplyItem.FirstOrDefault(t => t.AgreementChangeType == AgreementChangeType.OtherExchangeRateValue);
                            if (otherExchangeRateValue != null)
                            {
                                clientAgreement.IncidentalFeeClause.ExchangeRateValue = Convert.ToDecimal(otherExchangeRateValue.NewValue);
                            }
                            else
                            {
                                clientAgreement.IncidentalFeeClause.ExchangeRateValue = agreement.IncidentalFeeClause.ExchangeRateValue;
                            }

                            clientAgreement.IncidentalFeeClause.AdminID = agreement.IncidentalFeeClause.AdminID;
                            #endregion

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

                                //保存 Begin
                                //Response.Write((new { success = true, message = "提交成功" }).Json());
                            }
                            catch (Exception ex)
                            {
                                Response.Write(new { success = false, message = ex.Message });
                            }

                            #endregion
                        }

                    }
                    else
                    {
                        Response.Write((new { success = false, message = "提交失败" }).Json());
                    }
                }
            }
            catch (Exception ex)
            {
                Response.Write((new
                {
                    success = false,
                    message = "提交失败：" + ex.Message
                }).Json());
            }
        }
        protected void Refuse()
        {
            try
            {
                string applyID = Request.Form["ApplyID"];
                string reason = Request.Form["Reason"];
                string From = Request.Form["From"];
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                var summary = "";
                if (!string.IsNullOrEmpty(applyID))
                {
                    var apply = new Needs.Ccs.Services.Models.ClientAgreementChangeApplyModel();
                    apply.ID = applyID;
                    apply.ApplyID = applyID;
                    apply.ClientID = ClientID;
                    apply.Status = AgreementChangeApplyStatus.Delete;
                    apply.AdminID = admin.ID;
                    apply.Summary = reason;
                    apply.RealName = admin.RealName;
                    apply.From = From;
                    apply.Update();
                    //保存 Begin

                    Response.Write((new { success = true, message = "提交成功" }).Json());
                }
                else
                {
                    Response.Write((new { success = false, message = "提交失败" }).Json());
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