using Needs.Wl.Models.Views;
using System.Threading.Tasks;

namespace Needs.Wl.Models
{
    public static partial class ClientExtends
    {
        #region 临时过渡，随时删除

        /// <summary>
        /// 临时用法
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Needs.Ccs.Services.Models.Client ToCssClinet(this Client client)
        {
            Needs.Ccs.Services.Models.Client oldclient = new Ccs.Services.Models.Client()
            {
                ID = client.ID,
                AdminID = client.AdminID,
                ClientCode = client.ClientCode,
                ClientRank = (Needs.Ccs.Services.Enums.ClientRank)(int)client.ClientRank,
                ClientType = (Needs.Ccs.Services.Enums.ClientType)(int)client.ClientType,
                ClientStatus = (Needs.Ccs.Services.Enums.ClientStatus)(int)client.ClientStatus,
                CompanyID = client.Company.ID,
                CreateDate = client.CreateDate,
            };

            return oldclient;
        }

        /// <summary>
        /// 临时用法
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Needs.Ccs.Services.Models.User ToCssUser(this User user)
        {
            Needs.Ccs.Services.Models.User oldUser = new Ccs.Services.Models.User()
            {
                ID = user.ID,
                AdminID = user.AdminID,
                ClientID = user.ClientID,
                Email = user.Email,
                IsMain = user.IsMain,
                OpenID = user.OpenID,
                Name = user.Name,
                RealName = user.RealName,
                Status = (Needs.Ccs.Services.Enums.Status)(int)user.Status,
                Mobile = user.Mobile,
                Password = user.Password,
                UpdateDate = user.UpdateDate,
                CreateDate = user.CreateDate,
            };

            return oldUser;
        }

        /// <summary>
        /// 临时用法
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Needs.Ccs.Services.Models.Admin ToCssAdmin(this Admin admin)
        {
            Needs.Ccs.Services.Models.Admin oldUser = new Ccs.Services.Models.Admin()
            {
                ID = admin.ID,
                Email = admin.Email,
                UserName = admin.UserName,
                Mobile = admin.Mobile,
                RealName = admin.RealName,
                Tel = admin.Tel,
                UpdateDate = admin.UpdateDate,
                CreateDate = admin.CreateDate,
            };

            return oldUser;
        }


        /// <summary>
        /// 临时用法
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Needs.Ccs.Services.Models.ClientSupplier ToCssClientSupplier(this ClientSupplier admin)
        {
            Needs.Ccs.Services.Models.ClientSupplier oldUser = new Ccs.Services.Models.ClientSupplier()
            {
                ID = admin.ID,
                ChineseName = admin.ChineseName,
                ClientID = admin.ClientID,
                Name = admin.Name,
                Status = (Needs.Ccs.Services.Enums.Status)((int)admin.Status),
                Summary = admin.Summary,
                UpdateDate = admin.UpdateDate,
                CreateDate = admin.CreateDate,
            };

            return oldUser;
        }

        /// <summary>
        /// 临时用法
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static Needs.Ccs.Services.Models.ClientAgreement ToCssClientAgreement(this ClientAgreement admin)
        {
            Needs.Ccs.Services.Models.ClientAgreement oldUser = new Ccs.Services.Models.ClientAgreement()
            {
                ID = admin.ID,
                AdminID = admin.AdminID,
                AgencyFeeClause = new Ccs.Services.Models.ClientFeeSettlement()
                {
                    ID = admin.AgencyFeeClause.ID,
                    AdminID = admin.AgencyFeeClause.AdminID,
                    AgreementID = admin.AgencyFeeClause.AgreementID,
                    CreateDate = admin.AgencyFeeClause.CreateDate,
                    DaysLimit = admin.AgencyFeeClause.DaysLimit,
                    ExchangeRateType = (Needs.Ccs.Services.Enums.ExchangeRateType)((int)admin.AgencyFeeClause.ExchangeRateType),
                    ExchangeRateValue = admin.AgencyFeeClause.ExchangeRateValue,
                    FeeType = (Needs.Ccs.Services.Enums.FeeType)((int)admin.AgencyFeeClause.FeeType),
                    MonthlyDay = admin.AgencyFeeClause.MonthlyDay,
                    PeriodType = (Needs.Ccs.Services.Enums.PeriodType)((int)admin.AgencyFeeClause.PeriodType),
                    Status = (Needs.Ccs.Services.Enums.Status)((int)admin.AgencyFeeClause.Status),
                    Summary = admin.AgencyFeeClause.Summary,
                    UpdateDate = admin.AgencyFeeClause.UpdateDate,
                    UpperLimit = admin.AgencyFeeClause.UpperLimit
                },
                IncidentalFeeClause = new Ccs.Services.Models.ClientFeeSettlement()
                {
                    ID = admin.IncidentalFeeClause.ID,
                    AdminID = admin.IncidentalFeeClause.AdminID,
                    AgreementID = admin.IncidentalFeeClause.AgreementID,
                    CreateDate = admin.IncidentalFeeClause.CreateDate,
                    DaysLimit = admin.IncidentalFeeClause.DaysLimit,
                    ExchangeRateType = (Needs.Ccs.Services.Enums.ExchangeRateType)((int)admin.IncidentalFeeClause.ExchangeRateType),
                    ExchangeRateValue = admin.IncidentalFeeClause.ExchangeRateValue,
                    FeeType = (Needs.Ccs.Services.Enums.FeeType)((int)admin.IncidentalFeeClause.FeeType),
                    MonthlyDay = admin.IncidentalFeeClause.MonthlyDay,
                    PeriodType = (Needs.Ccs.Services.Enums.PeriodType)((int)admin.IncidentalFeeClause.PeriodType),
                    Status = (Needs.Ccs.Services.Enums.Status)((int)admin.IncidentalFeeClause.Status),
                    Summary = admin.IncidentalFeeClause.Summary,
                    UpdateDate = admin.IncidentalFeeClause.UpdateDate,
                    UpperLimit = admin.IncidentalFeeClause.UpperLimit
                },
                ProductFeeClause = new Ccs.Services.Models.ClientFeeSettlement()
                {
                    ID = admin.ProductFeeClause.ID,
                    AdminID = admin.ProductFeeClause.AdminID,
                    AgreementID = admin.ProductFeeClause.AgreementID,
                    CreateDate = admin.ProductFeeClause.CreateDate,
                    DaysLimit = admin.ProductFeeClause.DaysLimit,
                    ExchangeRateType = (Needs.Ccs.Services.Enums.ExchangeRateType)((int)admin.ProductFeeClause.ExchangeRateType),
                    ExchangeRateValue = admin.ProductFeeClause.ExchangeRateValue,
                    FeeType = (Needs.Ccs.Services.Enums.FeeType)((int)admin.ProductFeeClause.FeeType),
                    MonthlyDay = admin.ProductFeeClause.MonthlyDay,
                    PeriodType = (Needs.Ccs.Services.Enums.PeriodType)((int)admin.ProductFeeClause.PeriodType),
                    Status = (Needs.Ccs.Services.Enums.Status)((int)admin.ProductFeeClause.Status),
                    Summary = admin.ProductFeeClause.Summary,
                    UpdateDate = admin.ProductFeeClause.UpdateDate,
                    UpperLimit = admin.ProductFeeClause.UpperLimit
                },
                TaxFeeClause = new Ccs.Services.Models.ClientFeeSettlement()
                {
                    ID = admin.TaxFeeClause.ID,
                    AdminID = admin.TaxFeeClause.AdminID,
                    AgreementID = admin.TaxFeeClause.AgreementID,
                    CreateDate = admin.TaxFeeClause.CreateDate,
                    DaysLimit = admin.TaxFeeClause.DaysLimit,
                    ExchangeRateType = (Needs.Ccs.Services.Enums.ExchangeRateType)((int)admin.TaxFeeClause.ExchangeRateType),
                    ExchangeRateValue = admin.TaxFeeClause.ExchangeRateValue,
                    FeeType = (Needs.Ccs.Services.Enums.FeeType)((int)admin.TaxFeeClause.FeeType),
                    MonthlyDay = admin.TaxFeeClause.MonthlyDay,
                    PeriodType = (Needs.Ccs.Services.Enums.PeriodType)((int)admin.TaxFeeClause.PeriodType),
                    Status = (Needs.Ccs.Services.Enums.Status)((int)admin.TaxFeeClause.Status),
                    Summary = admin.TaxFeeClause.Summary,
                    UpdateDate = admin.TaxFeeClause.UpdateDate,
                    UpperLimit = admin.TaxFeeClause.UpperLimit
                },
                IsLimitNinetyDays = admin.IsLimitNinetyDays,
                IsPrePayExchange = admin.IsPrePayExchange,
                MinAgencyFee = admin.MinAgencyFee,
                StartDate = admin.StartDate,
                InvoiceType = (Needs.Ccs.Services.Enums.InvoiceType)((int)admin.InvoiceType),
                InvoiceTaxRate = admin.InvoiceTaxRate,
                Status = (Needs.Ccs.Services.Enums.Status)((int)admin.Status),
                AgencyRate = admin.AgencyRate,
                Summary = admin.Summary,
                ClientID = admin.ClientID,
                EndDate = admin.EndDate,
                UpdateDate = admin.UpdateDate,
                CreateDate = admin.CreateDate,
            };

            return oldUser;
        }

        #endregion

        /// <summary>
        /// 会员的供应商
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static MySuppliersView Suppliers(this Client client)
        {
            return new Views.MySuppliersView(client.ID);
        }

        /// <summary>
        /// 会员的附件
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static ClientFilesView Files(this Client client)
        {
            return new Views.ClientFilesView(client.ID);
        }

        /// <summary>
        /// 会员的开票信息
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static ClientInvoice Invoice(this Client client)
        {
            return new Views.ClientInvoicesView(client.ID).FirstOrDefault();
        }

        /// <summary>
        /// 会员的开票信息
        /// 异步
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static async Task<ClientInvoice> InvoiceAsync(this Client client)
        {
            return await new Views.ClientInvoicesView(client.ID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 发票收件地址
        /// </summary>
        public static ClientInvoiceConsignee InvoiceConsignee(this Client client)
        {
            return new Views.ClientInvoiceConsigneesView(client.ID).FirstOrDefault();
        }

        /// <summary>
        /// 发票收件地址
        /// </summary>
        public static async Task<ClientInvoiceConsignee> InvoiceConsigneeAsyn(this Client client)
        {
            return await Task.Run(() => new Views.ClientInvoiceConsigneesView(client.ID).FirstOrDefault());
        }

        /// <summary>
        /// 客户的账号
        /// </summary>
        public static Views.ClientUsersView Users(this Client client)
        {
            return new Views.ClientUsersView(client.ID);
        }

        /// <summary>
        /// 客户的补充协议
        /// </summary>
        public static ClientAgreement Agreement(this Client client)
        {
            return new Views.ClientAgreementsView(client.ID).FirstOrDefault();
        }

        /// <summary>
        /// 客户的补充协议
        /// 异步
        /// </summary>
        public static async Task<ClientAgreement> AgreementAsync(this Client client)
        {
            return await new Views.ClientAgreementsView(client.ID).FirstOrDefaultAsync();
        }

        /// <summary>
        /// 客户的应付账款
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static ClientPayables ClientPayables(this Client client)
        {
            return new Views.ClientPayablesView(client.ID).FirstOrDefault();
        }
    }
}