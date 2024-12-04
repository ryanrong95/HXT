using Needs.Ccs.Services;
using Needs.Ccs.Services.Enums;
using Needs.Ccs.Services.Models;
using Needs.Utils;
using Needs.Utils.Converters;
using Needs.Utils.Descriptions;
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

namespace WebApp.Client
{
    public partial class Agreement : Uc.PageBase
    {
        private string URL = System.Configuration.ConfigurationManager.AppSettings["CrmUrl"];
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadData();
        }

        /// <summary>
        /// 初始化协议信息
        /// </summary>
        protected void LoadData()
        {
            string id = Request.QueryString["ID"];
            //Source=OrderView 来自综合管理订单协议查看
            this.Model.From = "".Json();
            if (!string.IsNullOrEmpty(Request.QueryString["Source"]))
            {
                this.Model.From = Request.QueryString["Source"].Json();
            }
            this.Model.ID = id;

            //费用类型
            this.Model.FeeType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.FeeType>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.PeriodType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PeriodType>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.InvoiceType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.InvoiceType>().Select(item => new { item.Key, item.Value }).Json();
            //杂费汇率
            this.Model.ExchangeRateTypeOther = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ExchangeRateType>()
                .Where(item => item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.None.GetHashCode().ToString()).Select(item => new { item.Key, item.Value }).Json();
            //税费汇率
            this.Model.ExchangeRateTypeTax = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ExchangeRateType>()
               .Where(item => item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.RealTime.GetHashCode().ToString()
              || item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.Custom.GetHashCode().ToString()
              || item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.Agreed.GetHashCode().ToString()
              ).Select(item => new { item.Key, item.Value }).Json();
            this.Model.ExchangeRateTypeGood = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ExchangeRateType>()
              .Where(item => item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.RealTime.GetHashCode().ToString()
              || item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.Agreed.GetHashCode().ToString()).Select(item => new { item.Key, item.Value }).Json();
            //代理费汇率
            this.Model.ExchangeRateTypeAgree = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ExchangeRateType>()
               .Where(item => item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.RealTime.GetHashCode().ToString()
              || item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.Custom.GetHashCode().ToString()
              || item.Key == Needs.Ccs.Services.Enums.ExchangeRateType.Agreed.GetHashCode().ToString()
              ).Select(item => new { item.Key, item.Value }).Json();

            this.Model.ExchangeRateType = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.ExchangeRateType>().Select(item => new { item.Key, item.Value }).Json();
            this.Model.InvoiceRate = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.InvoiceRate>().Select(item => new { item.Key, item.Value }).Json();

            this.Model.PEIsTen = EnumUtils.ToDictionary<Needs.Ccs.Services.Enums.PEIsTen>().Select(item => new { item.Key, item.Value }).Json();

            if (!string.IsNullOrEmpty(id))
            {
                this.Model.ClientRank = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientsView[id].ClientRank.GetDescription();

                ////查询此客户是否存在已审批生效的垫资申请  by 2020-12-23 yess
                //var advanceMoneyApply = new Needs.Ccs.Services.Views.AdvanceMoneyApplyView().FirstOrDefault(x => x.ClientID == id && x.Status == AdvanceMoneyStatus.Effective);
                //if (advanceMoneyApply != null)
                //{
                //    this.Model.AdvanceMoneyApply = (new { Amount = advanceMoneyApply.Amount, LimitDays = advanceMoneyApply.LimitDays }).Json();
                //}
                //else
                //{
                this.Model.AdvanceMoneyApply = null;
                //}
                //查询是否已申请变更协议  by yeshuangshuang 2021-02-04
                var agreementChangeApply = new Needs.Ccs.Services.Views.ClientAgreementChangeApplyView().FirstOrDefault(x => x.ClientID == id);
                if (agreementChangeApply != null)
                {
                    this.Model.AgreementChangeApply = agreementChangeApply;
                }
                else
                {
                    this.Model.AgreementChangeApply = null;
                }
                var client = new Needs.Ccs.Services.Views.clientView().FirstOrDefault(x => x.ID == id);
                if (client != null)
                {
                    this.Model.ClientStatus = client.ClientStatus.Json();
                }
                else
                {
                    this.Model.ClientStatus = null;
                }
                var agree = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements.Where(t => t.ClientID == id && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();

                if (agree != null)
                {
                    string FileServerUrl = System.Configuration.ConfigurationManager.AppSettings["PvDataFileUrl"];
                    this.Model.AgreementID = agree.ID;
                    this.Model.ClientAgreementData = agree.Json();

                    var serviceFile = new CenterFilesTopView().FirstOrDefault(x => x.ClientID == id && x.Type == (int)Needs.Ccs.Services.Enums.FileType.ServiceAgreement && x.Status != FileDescriptionStatus.Delete);
                    if (serviceFile != null)
                    {
                        this.Model.ServiceFile = (new { Name = serviceFile.CustomName, Url = FileServerUrl + @"/" + serviceFile.Url.ToUrl() }).Json();
                    }
                    else
                    {
                        this.Model.ServiceFile = null;
                    }
                }
                else
                {
                    this.Model.AgreementID = "";
                    this.Model.ServiceFile = null;
                    this.Model.ClientAgreementData = null;
                }
            }
            else
            {
                this.Model.AgreementID = "";
                this.Model.ServiceFile = null;
                this.Model.ClientAgreementData = null;
                this.Model.ClientRank = "";
                this.Model.AdvanceMoneyApply = null;
                this.Model.AgreementChangeApply = null;
                this.Model.clientID = null;
            }
        }

        /// <summary>
        /// 导出协议word
        /// </summary>
        protected void ExportAgreement()
        {
            try
            {
                var AgreementID = Request.Form["AgreementID"];
                var agreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements[AgreementID];
                //创建文件夹
                var fileName = DateTime.Now.Ticks + "服务协议草书.docx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                agreement.SaveAsImport(file.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 导出协议word--新
        /// </summary>
        protected void ExportAgreementNew()
        {
            try
            {
                var AgreementID = Request.Form["AgreementID"];
                var agreement = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements[AgreementID];
                //创建文件夹
                var fileName = agreement.AgreementCode + ".docx";
                FileDirectory file = new FileDirectory(fileName);
                file.SetChildFolder(Needs.Ccs.Services.SysConfig.Dowload);
                file.CreateDataDirectory();
                //保存文件
                agreement.SaveAsImport(file.FilePath);

                Response.Write((new { success = true, message = "导出成功", url = file.FileUrl }).Json());

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "导出失败" + ex.Message }).Json());
            }
        }

        /// <summary>
        /// 保存会员协议信息
        /// </summary>
        protected void SaveClientAgreement()
        {
            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();

            var clientAgreement = new Needs.Ccs.Services.Models.ClientAgreement();

            clientAgreement.AgreementCode = Needs.Overall.PKeySigner.Pick(PKeyType.AgreementImportCode);
            clientAgreement.ProductFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.TaxFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.AgencyFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.IncidentalFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();


            clientAgreement.ClientID = model.ID;
            clientAgreement.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            clientAgreement.StartDate = Convert.ToDateTime(model.StartDate);
            clientAgreement.EndDate = Convert.ToDateTime(model.EndDate);
            clientAgreement.PreAgency = (string)model.PreAgency == "" ? 0 : int.Parse((string)model.PreAgency);
            clientAgreement.AgencyRate = model.AgencyRate;
            clientAgreement.MinAgencyFee = model.MinAgencyFee;
            clientAgreement.IsPrePayExchange = model.IsPrePayExchange;
            clientAgreement.IsLimitNinetyDays = model.IsLimitNinetyDays;
            clientAgreement.InvoiceType = model.InvoiceTypeID;
            clientAgreement.InvoiceTaxRate = clientAgreement.InvoiceType == Needs.Ccs.Services.Enums.InvoiceType.Full ? Needs.Ccs.Services.ConstConfig.ValueAddedTaxRate : decimal.Parse((string)model.InvoiceRateID) / 100;
            clientAgreement.Summary = model.Summary;

            //九点半或十点汇率
            clientAgreement.IsTen = model.PEIsTenID;

            //货款
            clientAgreement.ProductFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.ProductFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Product;
            clientAgreement.ProductFeeClause.PeriodType = model.GoodsPeriodTypeID;
            clientAgreement.ProductFeeClause.DaysLimit = (string)model.GoodsDaysLimit == "" ? (int?)null : int.Parse((string)model.GoodsDaysLimit);
            clientAgreement.ProductFeeClause.MonthlyDay = (string)model.GoodsMonthlyDay == "" ? (int?)null : int.Parse((string)model.GoodsMonthlyDay);
            clientAgreement.ProductFeeClause.UpperLimit = (string)model.GoodsUpperLimit == "" ? (int?)null : int.Parse((string)model.GoodsUpperLimit);
            clientAgreement.ProductFeeClause.ExchangeRateType = model.GoodsExchangeRateTypeID;
            clientAgreement.ProductFeeClause.ExchangeRateValue = (string)model.GoodsExchangeRateValue == "" ? (decimal?)null : decimal.Parse((string)model.GoodsExchangeRateValue);
            clientAgreement.ProductFeeClause.AdminID = clientAgreement.AdminID;

            //税款
            clientAgreement.TaxFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.TaxFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Tax;
            clientAgreement.TaxFeeClause.PeriodType = model.TaxPeriodTypeID;
            clientAgreement.TaxFeeClause.DaysLimit = (string)model.TaxDaysLimit == "" ? (int?)null : int.Parse((string)model.TaxDaysLimit);
            clientAgreement.TaxFeeClause.MonthlyDay = (string)model.TaxMonthlyDay == "" ? (int?)null : int.Parse((string)model.TaxMonthlyDay);
            clientAgreement.TaxFeeClause.UpperLimit = (string)model.TaxUpperLimit == "" ? (int?)null : int.Parse((string)model.TaxUpperLimit);
            clientAgreement.TaxFeeClause.ExchangeRateType = model.TaxExchangeRateTypeID;
            clientAgreement.TaxFeeClause.ExchangeRateValue = (string)model.TaxExchangeRateValue == "" ? (decimal?)null : decimal.Parse((string)model.TaxExchangeRateValue);

            clientAgreement.TaxFeeClause.AdminID = clientAgreement.AdminID;

            //代理费
            clientAgreement.AgencyFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.AgencyFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.AgencyFee;
            clientAgreement.AgencyFeeClause.PeriodType = model.AgencyFeePeriodTypeID;
            clientAgreement.AgencyFeeClause.DaysLimit = (string)model.AgencyFeeDaysLimit == "" ? (int?)null : int.Parse((string)model.AgencyFeeDaysLimit);
            clientAgreement.AgencyFeeClause.MonthlyDay = (string)model.AgencyFeeMonthlyDay == "" ? (int?)null : int.Parse((string)model.AgencyFeeMonthlyDay);
            clientAgreement.AgencyFeeClause.UpperLimit = (string)model.AgencyFeeUpperLimit == "" ? (int?)null : int.Parse((string)model.AgencyFeeUpperLimit);
            clientAgreement.AgencyFeeClause.ExchangeRateType = model.AgencyFeeExchangeRateTypeID;
            clientAgreement.AgencyFeeClause.ExchangeRateValue = (string)model.AgencyFeeExchangeRateValue == "" ? (decimal?)null : decimal.Parse((string)model.AgencyFeeExchangeRateValue);
            clientAgreement.AgencyFeeClause.AdminID = clientAgreement.AdminID;

            //杂费
            clientAgreement.IncidentalFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.IncidentalFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Incidental;
            clientAgreement.IncidentalFeeClause.PeriodType = model.IncidentalPeriodTypeID;
            clientAgreement.IncidentalFeeClause.DaysLimit = (string)model.IncidentalDaysLimit == "" ? (int?)null : int.Parse((string)model.IncidentalDaysLimit);
            clientAgreement.IncidentalFeeClause.MonthlyDay = (string)model.IncidentalMonthlyDay == "" ? (int?)null : int.Parse((string)model.IncidentalMonthlyDay);
            clientAgreement.IncidentalFeeClause.UpperLimit = (string)model.IncidentalUpperLimit == "" ? (int?)null : int.Parse((string)model.IncidentalUpperLimit);
            clientAgreement.IncidentalFeeClause.ExchangeRateType = model.IncidentalExchangeRateTypeID;
            clientAgreement.IncidentalFeeClause.ExchangeRateValue = (string)model.IncidentalExchangeRateValue == "" ? (decimal?)null : decimal.Parse((string)model.IncidentalExchangeRateValue);

            clientAgreement.IncidentalFeeClause.AdminID = clientAgreement.AdminID;

            clientAgreement.EnterError += ClientAgreement_EnterError;
            clientAgreement.EnterSuccess += ClientAgreement_EnterSuccess;


            if (string.IsNullOrEmpty(URL))
            {
                #region 没调接口的代码
                clientAgreement.Enter();
                #endregion
            }
            else
            {
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
                        //ServiceAgreement = new ApiModel.AgreementFile
                        //{
                        //    Name = clientfile.Name,
                        //    Type = (int)clientfile.FileType,
                        //    FileFormat = clientfile.FileFormat,
                        //    Url = FileDirectory.Current.FileServerUrl + "/" + result[1].ToUrl(),
                        //    Summary = clientfile.Summary,
                        //    CreateDate = DateTime.Now.ToString()
                        //},
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
                }
                catch (Exception ex)
                {
                    Response.Write(new { success = false, message = ex.Message });
                }

                #endregion

            }

        }

        /// <summary>
        /// 提交协议变更
        /// </summary>
        protected void SaveClientAgreementApply()
        {

            string Model = Request.Form["Model"].Replace("&quot;", "\'");
            dynamic model = Model.JsonTo<dynamic>();

            var clientAgreement = new Needs.Ccs.Services.Models.ClientAgreement();

            clientAgreement.ProductFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.TaxFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.AgencyFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.IncidentalFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();


            clientAgreement.ClientID = model.ID;
            clientAgreement.AdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ID;

            clientAgreement.StartDate = Convert.ToDateTime(model.StartDate);
            clientAgreement.EndDate = Convert.ToDateTime(model.EndDate);
            clientAgreement.PreAgency = (string)model.PreAgency == "" ? (int?)null : int.Parse((string)model.PreAgency);
            clientAgreement.AgencyRate = model.AgencyRate;
            clientAgreement.MinAgencyFee = model.MinAgencyFee;
            clientAgreement.IsPrePayExchange = model.IsPrePayExchange;
            clientAgreement.IsLimitNinetyDays = model.IsLimitNinetyDays;
            clientAgreement.InvoiceType = model.InvoiceTypeID;
            clientAgreement.InvoiceTaxRate = clientAgreement.InvoiceType == Needs.Ccs.Services.Enums.InvoiceType.Full ? Needs.Ccs.Services.ConstConfig.ValueAddedTaxRate : decimal.Parse((string)model.InvoiceRateID) / 100;
            clientAgreement.Summary = model.reason;
            clientAgreement.IsTen = model.PEIsTenID;

            //税款
            clientAgreement.TaxFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.TaxFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Tax;
            clientAgreement.TaxFeeClause.PeriodType = model.TaxPeriodTypeID;
            clientAgreement.TaxFeeClause.DaysLimit = (string)model.TaxDaysLimit == "" ? (int?)null : int.Parse((string)model.TaxDaysLimit);
            clientAgreement.TaxFeeClause.MonthlyDay = (string)model.TaxMonthlyDay == "" ? (int?)null : int.Parse((string)model.TaxMonthlyDay);
            clientAgreement.TaxFeeClause.UpperLimit = (string)model.TaxUpperLimit == "" ? (int?)null : int.Parse((string)model.TaxUpperLimit);
            clientAgreement.TaxFeeClause.ExchangeRateType = model.TaxExchangeRateTypeID;
            clientAgreement.TaxFeeClause.ExchangeRateValue = (string)model.TaxExchangeRateValue == "" ? (decimal?)null : decimal.Parse((string)model.TaxExchangeRateValue);

            clientAgreement.TaxFeeClause.AdminID = clientAgreement.AdminID;

            //代理费
            clientAgreement.AgencyFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.AgencyFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.AgencyFee;
            clientAgreement.AgencyFeeClause.PeriodType = model.AgencyFeePeriodTypeID;
            clientAgreement.AgencyFeeClause.DaysLimit = (string)model.AgencyFeeDaysLimit == "" ? (int?)null : int.Parse((string)model.AgencyFeeDaysLimit);
            clientAgreement.AgencyFeeClause.MonthlyDay = (string)model.AgencyFeeMonthlyDay == "" ? (int?)null : int.Parse((string)model.AgencyFeeMonthlyDay);
            clientAgreement.AgencyFeeClause.UpperLimit = (string)model.AgencyFeeUpperLimit == "" ? (int?)null : int.Parse((string)model.AgencyFeeUpperLimit);
            clientAgreement.AgencyFeeClause.ExchangeRateType = model.AgencyFeeExchangeRateTypeID;
            clientAgreement.AgencyFeeClause.ExchangeRateValue = (string)model.AgencyFeeExchangeRateValue == "" ? (decimal?)null : decimal.Parse((string)model.AgencyFeeExchangeRateValue);
            clientAgreement.AgencyFeeClause.AdminID = clientAgreement.AdminID;

            //杂费
            clientAgreement.IncidentalFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.IncidentalFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Incidental;
            clientAgreement.IncidentalFeeClause.PeriodType = model.IncidentalPeriodTypeID;
            clientAgreement.IncidentalFeeClause.DaysLimit = (string)model.IncidentalDaysLimit == "" ? (int?)null : int.Parse((string)model.IncidentalDaysLimit);
            clientAgreement.IncidentalFeeClause.MonthlyDay = (string)model.IncidentalMonthlyDay == "" ? (int?)null : int.Parse((string)model.IncidentalMonthlyDay);
            clientAgreement.IncidentalFeeClause.UpperLimit = (string)model.IncidentalUpperLimit == "" ? (int?)null : int.Parse((string)model.IncidentalUpperLimit);
            clientAgreement.IncidentalFeeClause.ExchangeRateType = model.IncidentalExchangeRateTypeID;
            clientAgreement.IncidentalFeeClause.ExchangeRateValue = (string)model.IncidentalExchangeRateValue == "" ? (decimal?)null : decimal.Parse((string)model.IncidentalExchangeRateValue);

            clientAgreement.IncidentalFeeClause.AdminID = clientAgreement.AdminID;

            var agree = Needs.Wl.Admin.Plat.AdminPlat.Current.Clients.ClientAgreements.Where(t => t.ClientID == clientAgreement.ClientID && t.Status == Needs.Ccs.Services.Enums.Status.Normal).FirstOrDefault();
            if (agree != null)
            {
                //协议开始时间，结束时间
                int ChangeType = 0;
                var ClientID = clientAgreement.ClientID;
                var AdminID = clientAgreement.AdminID;
                var Summary = clientAgreement.Summary;
                string NewValue;
                string OldValue;
                try
                {
                    if (clientAgreement.StartDate != agree.StartDate)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.StartDate;
                        NewValue = Convert.ToDateTime(clientAgreement.StartDate).ToString("yyyy-MM-dd");
                        OldValue = Convert.ToDateTime(agree.StartDate).ToString("yyyy-MM-dd");
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.EndDate != agree.EndDate)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.EndDate;
                        NewValue = Convert.ToDateTime(clientAgreement.EndDate).ToString("yyyy-MM-dd");
                        OldValue = Convert.ToDateTime(agree.EndDate).ToString("yyyy-MM-dd");
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.PreAgency != agree.PreAgency)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.PreAgency;
                        NewValue = clientAgreement.PreAgency.ToString();
                        OldValue = agree.PreAgency.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.AgencyRate != agree.AgencyRate)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.AgencyRate;
                        NewValue = clientAgreement.AgencyRate.ToString();
                        OldValue = agree.AgencyRate.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.MinAgencyFee != agree.MinAgencyFee)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.MinAgencyFee;
                        NewValue = clientAgreement.MinAgencyFee.ToString();
                        OldValue = agree.MinAgencyFee.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.IsLimitNinetyDays != agree.IsLimitNinetyDays)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.IsPrePayExchange;
                        NewValue = (string)clientAgreement.IsPrePayExchange.ToString();
                        OldValue = agree.IsPrePayExchange.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }

                    //十点汇率
                    if (clientAgreement.IsTen != agree.IsTen)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.IsTenType;
                        NewValue = (string)clientAgreement.IsTen.ToString();
                        OldValue = agree.IsTen.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }

                    if (clientAgreement.InvoiceType != agree.InvoiceType)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.InvoiceType;
                        NewValue = clientAgreement.InvoiceType.ToString();
                        OldValue = agree.InvoiceType.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.InvoiceTaxRate != agree.InvoiceTaxRate)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.InvoiceTaxRate;
                        NewValue = clientAgreement.InvoiceTaxRate.ToString();
                        OldValue = agree.InvoiceTaxRate.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    //税费
                    if (clientAgreement.TaxFeeClause.PeriodType != agree.TaxFeeClause.PeriodType)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.TaxPeriodType;
                        NewValue = clientAgreement.TaxFeeClause.PeriodType.ToString();
                        OldValue = agree.TaxFeeClause.PeriodType.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.TaxFeeClause.DaysLimit != agree.TaxFeeClause.DaysLimit)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.TaxDaysLimit;
                        NewValue = clientAgreement.TaxFeeClause.DaysLimit.ToString();
                        OldValue = clientAgreement.TaxFeeClause.PeriodType == Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod ? agree.TaxFeeClause.DaysLimit.ToString() : agree.TaxFeeClause.MonthlyDay.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.TaxFeeClause.MonthlyDay != agree.TaxFeeClause.MonthlyDay)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.TaxMonthlyDay;
                        NewValue = clientAgreement.TaxFeeClause.MonthlyDay.ToString();
                        OldValue = clientAgreement.TaxFeeClause.PeriodType != Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod ? agree.TaxFeeClause.MonthlyDay.ToString() : agree.TaxFeeClause.DaysLimit.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.TaxFeeClause.UpperLimit != agree.TaxFeeClause.UpperLimit)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.TaxUpperLimit;
                        NewValue = clientAgreement.TaxFeeClause.UpperLimit.ToString();
                        OldValue = agree.TaxFeeClause.UpperLimit.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.TaxFeeClause.ExchangeRateType != agree.TaxFeeClause.ExchangeRateType)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.TaxExchangeRateType;
                        NewValue = clientAgreement.TaxFeeClause.ExchangeRateType.ToString();
                        OldValue = agree.TaxFeeClause.ExchangeRateType.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.TaxFeeClause.ExchangeRateValue != agree.TaxFeeClause.ExchangeRateValue)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.TaxExchangeRateValue;
                        NewValue = clientAgreement.TaxFeeClause.ExchangeRateValue.ToString();
                        OldValue = agree.TaxFeeClause.ExchangeRateValue.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    //代理费
                    if (clientAgreement.AgencyFeeClause.PeriodType != agree.AgencyFeeClause.PeriodType)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.AgencyPeriodType;
                        NewValue = clientAgreement.AgencyFeeClause.PeriodType.ToString();
                        OldValue = agree.AgencyFeeClause.PeriodType.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.AgencyFeeClause.DaysLimit != agree.AgencyFeeClause.DaysLimit)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.AgencyDaysLimit;
                        NewValue = clientAgreement.AgencyFeeClause.DaysLimit.ToString();
                        OldValue = clientAgreement.AgencyFeeClause.PeriodType == Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod ? agree.AgencyFeeClause.DaysLimit.ToString() : agree.AgencyFeeClause.MonthlyDay.ToString();
                        //agree.AgencyFeeClause.DaysLimit.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.AgencyFeeClause.MonthlyDay != agree.AgencyFeeClause.MonthlyDay)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.AgencyMonthlyDay;
                        NewValue = clientAgreement.AgencyFeeClause.MonthlyDay.ToString();
                        OldValue = OldValue = clientAgreement.AgencyFeeClause.PeriodType != Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod ? agree.AgencyFeeClause.MonthlyDay.ToString() : agree.AgencyFeeClause.DaysLimit.ToString();
                        //agree.AgencyFeeClause.MonthlyDay.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.AgencyFeeClause.UpperLimit != agree.AgencyFeeClause.UpperLimit)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.AgencyUpperLimit;
                        NewValue = clientAgreement.AgencyFeeClause.UpperLimit.ToString();
                        OldValue = agree.AgencyFeeClause.UpperLimit.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.AgencyFeeClause.ExchangeRateType != agree.AgencyFeeClause.ExchangeRateType)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.AgencyExchangeRateType;
                        NewValue = clientAgreement.AgencyFeeClause.ExchangeRateType.ToString();
                        OldValue = agree.AgencyFeeClause.ExchangeRateType.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.AgencyFeeClause.ExchangeRateValue != agree.AgencyFeeClause.ExchangeRateValue)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.AgencyExchangeRateValue;
                        NewValue = clientAgreement.AgencyFeeClause.ExchangeRateValue.ToString();
                        OldValue = agree.AgencyFeeClause.ExchangeRateValue.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    //杂费
                    if (clientAgreement.IncidentalFeeClause.PeriodType != agree.IncidentalFeeClause.PeriodType)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.OtherPeriodType;
                        NewValue = clientAgreement.IncidentalFeeClause.PeriodType.ToString();
                        OldValue = agree.IncidentalFeeClause.PeriodType.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.IncidentalFeeClause.DaysLimit != agree.IncidentalFeeClause.DaysLimit)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.OtherDaysLimit;
                        NewValue = clientAgreement.IncidentalFeeClause.DaysLimit.ToString();
                        OldValue = clientAgreement.IncidentalFeeClause.PeriodType == Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod ? agree.IncidentalFeeClause.DaysLimit.ToString() : agree.IncidentalFeeClause.MonthlyDay.ToString();
                        //agree.IncidentalFeeClause.DaysLimit.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.IncidentalFeeClause.MonthlyDay != agree.IncidentalFeeClause.MonthlyDay)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.OtherMonthlyDay;
                        NewValue = clientAgreement.IncidentalFeeClause.MonthlyDay.ToString();
                        OldValue = clientAgreement.IncidentalFeeClause.PeriodType != Needs.Ccs.Services.Enums.PeriodType.AgreedPeriod ? agree.IncidentalFeeClause.MonthlyDay.ToString() : agree.IncidentalFeeClause.DaysLimit.ToString();
                        //agree.IncidentalFeeClause.MonthlyDay.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.IncidentalFeeClause.UpperLimit != agree.IncidentalFeeClause.UpperLimit)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.OtherUpperLimit;
                        NewValue = clientAgreement.IncidentalFeeClause.UpperLimit.ToString();
                        OldValue = agree.IncidentalFeeClause.UpperLimit.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.IncidentalFeeClause.ExchangeRateType != agree.IncidentalFeeClause.ExchangeRateType)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.OtherExchangeRateType;
                        NewValue = clientAgreement.IncidentalFeeClause.ExchangeRateType.ToString();
                        OldValue = agree.IncidentalFeeClause.ExchangeRateType.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    if (clientAgreement.IncidentalFeeClause.ExchangeRateValue != agree.IncidentalFeeClause.ExchangeRateValue)
                    {
                        ChangeType = (int)Needs.Ccs.Services.Enums.AgreementChangeType.OtherExchangeRateValue;
                        NewValue = clientAgreement.IncidentalFeeClause.ExchangeRateValue.ToString();
                        OldValue = agree.IncidentalFeeClause.ExchangeRateValue.ToString();
                        ChangeAgreement(NewValue, OldValue, ChangeType, ClientID, AdminID, Summary);
                    }
                    Response.Write((new { success = true, message = "提交成功" }).Json());
                }
                catch (Exception ex)
                {

                    Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
                }
            }

        }
        void ChangeAgreement(string NewValue, string OldValue, int ChangeType, string ClientID, string AdminID, string Summary)
        {
            try
            {
                var applyId = "";
                var admin = Needs.Underly.FkoFactory<Needs.Ccs.Services.Models.Admin>.Create(Needs.Wl.Admin.Plat.AdminPlat.Current.ID);
                //表里（AgreementChangeApplies）插入一笔协议变更数据，协议变更申请只存在一笔有效的（已生效或作废，可以再次申请变更）
                var clientAgreementChangeApply = new Needs.Ccs.Services.Views.CheckAgreementChangeApplyView().FirstOrDefault(x => x.ClientID == ClientID);
                if (clientAgreementChangeApply == null)
                {
                    applyId = Needs.Overall.PKeySigner.Pick(PKeyType.AgreementChangeApply);
                    var apply = new Needs.Ccs.Services.Models.ClientAgreementChangeApplyModel();
                    apply.ID = applyId;
                    apply.ClientID = ClientID;
                    apply.Status = AgreementChangeApplyStatus.RiskAuditing;
                    apply.AdminID = AdminID;
                    apply.Summary = Summary;
                    apply.RealName = admin.RealName;
                    apply.Enter();

                }
                else
                {
                    applyId = clientAgreementChangeApply.ID;
                }

                //获取界面所有数据，比对变更前数据，如果有存在变更协议项，就保存一条数据（AgreementChangeApplyItems）
                var applyItem = new Needs.Ccs.Services.Models.ClientAgreementChangeApplyItemModel();
                applyItem.ID = Needs.Overall.PKeySigner.Pick(PKeyType.AgreementChangeApplyItem);
                applyItem.ApplyID = applyId;
                applyItem.ChangeType = ChangeType;
                applyItem.Status = Status.Normal;
                applyItem.OldValue = OldValue;
                applyItem.NewValue = NewValue;
                applyItem.Enter();

            }
            catch (Exception ex)
            {
                Response.Write((new { success = false, message = "提交失败：" + ex.Message }).Json());
            }
        }
        protected void SaveServiceAgreement()
        {
            var file = Request.Files["ServiceAgreement"];
            var ClientID = Request.Form["ID"];

            //处理附件
            if (file.ContentLength != 0)
            {
                string fileName = file.FileName.ReName();
                HttpFile httpFile = new HttpFile(fileName);
                httpFile.SetChildFolder(Needs.Ccs.Services.SysConfig.Client);
                httpFile.CreateDataDirectory();
                string[] result = httpFile.SaveAs(file);
                //上传中心
                var centerType = Needs.Ccs.Services.Models.ApiModels.Files.FileType.ServiceAgreement;
                var ErmAdminID = Needs.Wl.Admin.Plat.AdminPlat.Current.ErmAdminID;
                var dic = new { CustomName = fileName, ClientID = ClientID, AdminID = ErmAdminID };
                var uploadFile = Needs.Ccs.Services.Models.CenterFilesTopView.Upload(FileDirectory.Current.FilePath + @"\" + httpFile.VirtualPath, centerType, dic);

                if (string.IsNullOrEmpty(URL))
                {
                    #region 没调接口的代码
                    // clientfile.Enter();
                    #endregion
                }
                else
                {
                    #region  调用之后
                    try
                    {
                        var client = Needs.Wl.Admin.Plat.AdminPlat.Clients[ClientID];
                        string requestUrl = URL + "/CrmUnify/Contract";
                        HttpResponseMessage response = new HttpResponseMessage();
                        string requestClientUrl = requestUrl;//请求地址
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
                            ServiceAgreement = new ApiModel.AgreementFile
                            {
                                Name = fileName,
                                Type = (int)FileType.ServiceAgreement,
                                Url = uploadFile[0].Url,
                                CreateDate = DateTime.Now.ToString(),
                            },

                            Creator = Needs.Wl.Admin.Plat.AdminPlat.Current.RealName,
                        };
                        string agreement = JsonConvert.SerializeObject(entity);
                        response = new HttpClientHelp().HttpClient("POST", requestClientUrl, agreement);
                        if (response == null || response.StatusCode != HttpStatusCode.OK)
                        {
                            Response.Write((new { success = false, message = "请求会员接口失败：" }).Json());
                            return;
                        }
                        // clientfile.Enter();
                    }
                    catch (Exception ex)
                    {
                        Response.Write(new { success = false, message = ex.Message });
                    }

                    #endregion

                }
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