using Needs.Utils.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Needs.Ccs.Services.Models
{
    public class Agreement2Crm : ChangeHandler
    {
        public Agreement2Crm(Client client, List<RiskChanges> changes, Admin admin) : base(client, changes, admin)
        {
            this.Client = client;
            this.Changes = changes;
            this.Admin = admin;
        }

        public override void HandleReqeust(string context, string summary)
        {            
            ClientLog log = new ClientLog();
            log.ClientID = Client.ID;
            log.Admin = Client.Admin;
            log.ClientRank = Client.ClientRank;
            log.Summary = context+summary;
            log.Enter();
            
            var change = Changes.Where(t => t.ChangeType != RiskControlChangeType.RankChange).Count();
            if (change != 0)
            {
                Post2Crm();
            }
            this.NextHandler?.HandleReqeust(context,summary);
        }

        private void Post2Crm()
        {
            var clientAgreement = new Needs.Ccs.Services.Models.ClientAgreement();

            clientAgreement.ProductFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.TaxFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.AgencyFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();
            clientAgreement.IncidentalFeeClause = new Needs.Ccs.Services.Models.ClientFeeSettlement();


            clientAgreement.ClientID = Client.ID;
            clientAgreement.AdminID = Admin.ID;

            clientAgreement.StartDate = Client.Agreement.StartDate;
            clientAgreement.EndDate = Client.Agreement.EndDate;
            clientAgreement.AgencyRate = Client.Agreement.AgencyRate;
            clientAgreement.MinAgencyFee = Client.Agreement.MinAgencyFee;
            clientAgreement.IsPrePayExchange = Client.Agreement.IsPrePayExchange;
            clientAgreement.IsLimitNinetyDays = Client.Agreement.IsLimitNinetyDays;
            clientAgreement.InvoiceType = Client.Agreement.InvoiceType;
            clientAgreement.InvoiceTaxRate = Client.Agreement.InvoiceTaxRate; 
            clientAgreement.Summary = Client.Agreement.Summary;
            clientAgreement.IsTen = Client.Agreement.IsTen;

            //货款
            clientAgreement.ProductFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.ProductFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Product;
            clientAgreement.ProductFeeClause.PeriodType = Client.Agreement.ProductFeeClause.PeriodType;
            clientAgreement.ProductFeeClause.DaysLimit = Client.Agreement.ProductFeeClause.DaysLimit;
            clientAgreement.ProductFeeClause.MonthlyDay = Client.Agreement.ProductFeeClause.MonthlyDay;
            clientAgreement.ProductFeeClause.UpperLimit = Client.Agreement.ProductFeeClause.UpperLimit;
            clientAgreement.ProductFeeClause.ExchangeRateType = Client.Agreement.ProductFeeClause.ExchangeRateType;
            clientAgreement.ProductFeeClause.ExchangeRateValue = Client.Agreement.ProductFeeClause.ExchangeRateValue;
            clientAgreement.ProductFeeClause.AdminID = clientAgreement.AdminID;
            var GoodsChange = Changes.Where(t => t.ChangeType == RiskControlChangeType.GoodsUpperLimitChange).FirstOrDefault();
            if (GoodsChange != null)
            {
                clientAgreement.ProductFeeClause.UpperLimit = GoodsChange.NewValue;
            }

            //税款
            clientAgreement.TaxFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.TaxFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Tax;
            clientAgreement.TaxFeeClause.PeriodType = Client.Agreement.TaxFeeClause.PeriodType;
            clientAgreement.TaxFeeClause.DaysLimit = Client.Agreement.TaxFeeClause.DaysLimit;
            clientAgreement.TaxFeeClause.MonthlyDay = Client.Agreement.TaxFeeClause.MonthlyDay;
            clientAgreement.TaxFeeClause.UpperLimit = Client.Agreement.TaxFeeClause.UpperLimit;
            clientAgreement.TaxFeeClause.ExchangeRateType = Client.Agreement.TaxFeeClause.ExchangeRateType;
            clientAgreement.TaxFeeClause.ExchangeRateValue = Client.Agreement.TaxFeeClause.ExchangeRateValue;
            clientAgreement.TaxFeeClause.AdminID = clientAgreement.AdminID;
            var TaxChange = Changes.Where(t => t.ChangeType == RiskControlChangeType.TaxUpperLimitChange).FirstOrDefault();
            if (TaxChange != null)
            {
                clientAgreement.TaxFeeClause.UpperLimit = TaxChange.NewValue;
            }

            //代理费
            clientAgreement.AgencyFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.AgencyFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.AgencyFee;
            clientAgreement.AgencyFeeClause.PeriodType = Client.Agreement.AgencyFeeClause.PeriodType;
            clientAgreement.AgencyFeeClause.DaysLimit = Client.Agreement.AgencyFeeClause.DaysLimit;
            clientAgreement.AgencyFeeClause.MonthlyDay = Client.Agreement.AgencyFeeClause.MonthlyDay;
            clientAgreement.AgencyFeeClause.UpperLimit = Client.Agreement.AgencyFeeClause.UpperLimit;
            clientAgreement.AgencyFeeClause.ExchangeRateType = Client.Agreement.AgencyFeeClause.ExchangeRateType;
            clientAgreement.AgencyFeeClause.ExchangeRateValue = Client.Agreement.AgencyFeeClause.ExchangeRateValue;
            clientAgreement.AgencyFeeClause.AdminID = clientAgreement.AdminID;
            var AgencyChange = Changes.Where(t => t.ChangeType == RiskControlChangeType.AgencyFeeUpperLimitChange).FirstOrDefault();
            if (AgencyChange != null)
            {
                clientAgreement.AgencyFeeClause.UpperLimit = AgencyChange.NewValue;
            }

            //杂费
            clientAgreement.IncidentalFeeClause.AgreementID = clientAgreement.ID;
            clientAgreement.IncidentalFeeClause.FeeType = Needs.Ccs.Services.Enums.FeeType.Incidental;
            clientAgreement.IncidentalFeeClause.PeriodType = Client.Agreement.IncidentalFeeClause.PeriodType;
            clientAgreement.IncidentalFeeClause.DaysLimit = Client.Agreement.IncidentalFeeClause.DaysLimit;
            clientAgreement.IncidentalFeeClause.MonthlyDay = Client.Agreement.IncidentalFeeClause.MonthlyDay;
            clientAgreement.IncidentalFeeClause.UpperLimit = Client.Agreement.IncidentalFeeClause.UpperLimit;
            clientAgreement.IncidentalFeeClause.ExchangeRateType = Client.Agreement.IncidentalFeeClause.ExchangeRateType;
            clientAgreement.IncidentalFeeClause.ExchangeRateValue = Client.Agreement.IncidentalFeeClause.ExchangeRateValue;
            clientAgreement.IncidentalFeeClause.AdminID = clientAgreement.AdminID;
            var IncidentalChange = Changes.Where(t => t.ChangeType == RiskControlChangeType.IncidentalUpperLimitChange).FirstOrDefault();
            if (IncidentalChange != null)
            {
                clientAgreement.IncidentalFeeClause.UpperLimit = IncidentalChange.NewValue;
            }

            #region  调用之后
            try
            {
                var entity = new Needs.Ccs.Services.Models.ApiModel.ClientAgreement()
                {
                    Enterprise = new EnterpriseObj
                    {
                        AdminCode = "",
                        District = "",
                        Corporation = Client.Company.Corporate,
                        Name = Client.Company.Name,
                        RegAddress = Client.Company.Address,
                        Uscc = Client.Company.Code,
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
                    Creator = Admin.RealName,
                };

                var apisetting = new Needs.Ccs.Services.ApiSettings.CrmApiSetting();
                var apiurl = System.Configuration.ConfigurationManager.AppSettings[apisetting.ApiName] + apisetting.UpdataAgreement;

                Needs.Ccs.Services.Models.DeliveryNoticeApiLog apiLog = new Needs.Ccs.Services.Models.DeliveryNoticeApiLog()
                {
                    ID = Guid.NewGuid().ToString("N"),
                    OrderID = Client.ID,
                    TinyOrderID = Client.ID,
                    Url = apiurl,
                    RequestContent = entity.Json(),
                    Status = Needs.Ccs.Services.Enums.Status.Normal,
                    CreateDate = DateTime.Now,
                    UpdateDate = DateTime.Now,
                };
                apiLog.Enter();

                var result = Needs.Utils.Http.ApiHelper.Current.JPost(apiurl, entity);
                apiLog.ResponseContent = result;
                apiLog.Enter();

                
                clientAgreement.Enter();
            }
            catch (Exception ex)
            {
                
            }

            #endregion
        }
    }
}
